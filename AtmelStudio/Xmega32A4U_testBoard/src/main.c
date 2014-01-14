//================================================================================================
//========================���������� � ���������������� ����������������==========================
//================================================================================================
//---------------------------------------���������------------------------------------------------
//��������������� ������ ��������� ��������� �������:
//	-
//����������:
//	-�� - ���������������
//	-�� - ���������
//---------------------------------------����������-----------------------------------------------
//	-�������� � �������� �������� youtub'������ ����� Atmel Corporation
//		http://www.youtube.com/channel/UC7BvmnfLf-HTRZmMlPXeWwA
//	-�������� �������� ����� youtub'������ ����� ���������� ����������
//		http://www.youtube.com/channel/UCczziZl2-kvBUhzX9awdNEA?feature=watch
//----------------------------------------���������-----------------------------------------------
#include <asf.h>				//�������� Atmel Software Framework (ASF), ���������� �����������
//								//����������� �������.
//#include <avr/pgmspace.h>		//�������� ���������� flash-������� �����������
#include <spi_master.h>			//�������� ������ SPI
#include <Decoder.h>
#include <Initializator.h>

//---------------------------------------�����������----------------------------------------------
//��
#define version										145
#define birthday									20140114
//��������
#define RTC_Status_ready							0		//�������� ����� � ������
#define RTC_Status_stopped							1		//�������� ��� ������������� ����������
#define RTC_Status_busy								2		//�������� ��� �������
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
//��������� USART
#define	USART_State_ready							0		//USART ������ �� ���������
#define USART_State_receiving						1		//USART ��������� �����
#define USART_State_ending							2		//USART ������� ���� �������, ��������� ���������� ��������
#define USART_State_decoding						3		//USART ���������� �������
#define USART_State_HVEreceiving					4		//USART (TIC) ��������� ����� TIC'a �� ������ HVE
//��������� ������������ ��� DAC AD5643R -> ������� ��������
#define AD5643R_confHbyte							56
#define AD5643R_confMbyte							0
#define AD5643R_confLbyte							1
//��������� ���������� ��� DAC AD5643R -> 8131 (�������� ���������)
#define AD5643R_startVoltage_Hbyte					24		//�����
#define AD5643R_startVoltage_Mbyte					127		//������� ���� ����������
#define AD5643R_startVoltage_Lbyte					252		//������� ���� ���������� � 2 ������� �������� ������
//��������� ������������ ��� DAC AD5328R -> ������� ��������
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//��������� ���������� PSIS EC (���� �������)
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			82
//��������� ���������� DAC PSIS IV,F1,F2 (���������, ��������)
#define AD5328R_startVoltage_Hbyte_PSIS_IV			44
#define AD5328R_startVoltage_Lbyte_PSIS_IV			205

//----------------------------------------����������----------------------------------------------
//	���������������
uint8_t  MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t  MC_CommandStack = 0;
uint8_t  MC_Status = 0;
//		USART PC
uint8_t PC_timer_TimeOut = 30;							//60 ������
uint8_t PC_timer_time = 0;								//������ ������� ����� � ������
uint8_t PC_MEM[100];									//100 ���� ������ ��� ����� ������ USART
uint8_t PC_MEM_length = 0;								//����� ����������� � PC_MEM ������ ������.
uint8_t PC_State = 0;									//��������� ������ USART_PC
uint8_t PC_buf = 0;										//����� �����. �������� ����� �������� ���� (���� ���)
uint8_t PC_MEM_CheckSum = 0;							//�������� ����������� ����� (�� ������)
//		USART TIC
uint8_t TIC_timer_time = 0;								//������ ������� ����� � ������
uint8_t TIC_MEM[100];									//100 ���� ������ ��� ����� ������ �� TIC
uint8_t TIC_MEM_length = 0;								//����� ����������� � TIC_MEM ������ ������.
uint8_t TIC_buf = 0;									//����� �����. �������� ����� �������� ���� (���� ���)
uint8_t TIC_State = 0;									//��������� ������ USART_TIC
uint8_t TIC_HVE_Message[6] = {63, 86, 57, 49, 0, 13};	//char'� ��������� �� ������ �������� {?V91<NUL><\r>}
uint8_t TIC_HVE_onGauge = 51;							//��������� char ������ ������� (�������). �� ���������: Gauge_1
uint16_t TIC_HVE_onLevel = 8192;						//������ ������� ������ ���������� (�������). �� ���������: 2.000V
uint8_t TIC_HVE_offGauge = 52;							//��������� char ������ ������� (���������). �� ���������: Gauge_2
uint16_t TIC_HVE_offLevel = 26368;						//������ ������� ������ ���������� (���������). �� ���������: 6.700V
uint8_t TIC_offlineCount = 0;							//���������� ��������, ������� �������������� TIC.+64 - ��� ������ ��� ���� ��� TIC �� �������. �������� ������ 191 �������� ���������.
//		���������
uint8_t  RTC_Status = RTC_Status_ready;					//��������� ��������
uint16_t RTC_ElapsedTime = 0;
uint8_t  RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//������������ RTC
uint16_t RTC_MeasurePeriod = 0;							//������ RTC
uint16_t COA_Measurment = 0;							//��������� ��������� �������� COA
uint16_t COA_OVF = 0;									//���������� ������������ �������� ���
uint16_t COB_Measurment = 0;							//��������� ��������� �������� COB
uint16_t COB_OVF = 0;									//���������� ������������ �������� ���
uint16_t COC_Measurment = 0;							//��������� ��������� �������� COC
uint16_t COC_OVF = 0;									//���������� ������������ �������� ���
//-----------------------------------------���������----------------------------------------------
//������� ����
struct struct_MC_Tasks
{
    uint8_t turnOnHVE				: 1;
    uint8_t retransmit				: 1;
    uint8_t checkHVE				: 1;
    uint8_t noTasks3				: 1;
    uint8_t noTasks4				: 1;
    uint8_t noTasks5				: 1;
    uint8_t noTasks6				: 1;
    uint8_t noTasks7				: 1;
};
struct struct_MC_Tasks MC_Tasks = {0, 0, 0, 0, 0, 0, 0, 0};
struct struct_Errors_USART_PC
{
    uint8_t LOCKisLost				: 1;
    uint8_t TooShortPacket			: 1;
    uint8_t TooFast					: 1;
    uint8_t Silence					: 1;
    uint8_t Noise					: 1;
    uint8_t noError5				: 1;
    uint8_t noError6				: 1;
    uint8_t noError7				: 1;
};
struct struct_Errors_USART_PC Errors_USART_PC = {0, 0, 0, 0, 0, 0, 0, 0};
struct struct_Errors_USART_TIC
{
    uint8_t LOCKisLost				: 1;
    uint8_t TooShortPacket			: 1;
    uint8_t HVE_TimeOut				: 1;
    uint8_t Silence					: 1;
    uint8_t Noise					: 1;
    uint8_t HVE_error			    : 1;
    uint8_t wrongTimerState			: 1;
    uint8_t noError7				: 1;
};
struct struct_Errors_USART_TIC Errors_USART_TIC = {0, 0, 0, 0, 0, 0, 0, 0};
struct struct_Flags
{
    uint8_t SPUMP					: 1;
    uint8_t SEMV3					: 1;
    uint8_t SEMV2					: 1;
    uint8_t SEMV1					: 1;
    uint8_t iEDCD					: 1;
    uint8_t PRGE					: 1;
    uint8_t iHVE					: 1;
    uint8_t checkOrSet				: 1;
} Flags;
//USART
static usart_rs232_options_t USART_PC_OPTIONS =
{
    .baudrate = USART_PC_BAUDRATE,
    .charlength = USART_PC_CHAR_LENGTH,
    .paritytype = USART_PC_PARITY,
    .stopbits = USART_PC_STOP_BIT
};
static usart_rs232_options_t USART_TIC_OPTIONS =
{
    .baudrate = USART_TIC_BAUDRATE,
    .charlength = USART_TIC_CHAR_LENGTH,
    .paritytype = USART_TIC_PARITY,
    .stopbits = USART_TIC_STOP_BIT
};
//SPI
struct spi_device DAC_IonSource =
{
    .id = pin_iWRIS
};
struct spi_device DAC_Detector =
{
    .id = pin_iWRVD
};
struct spi_device DAC_Inlet =
{
    .id = pin_iWINL
};
struct spi_device DAC_Scaner =
{
    .id = pin_iWRSV
};
struct spi_device DAC_Condensator =
{
    .id = pin_iWRCV
};
struct spi_device ADC_IonSource =
{
    .id = pin_iECIS
};
struct spi_device ADC_Detector =
{
    .id = pin_iECVD
};
struct spi_device ADC_Inlet =
{
    .id = pin_iECINL
};
struct spi_device ADC_MSV =
{
    .id = pin_iECSV
};
//ADC � ������������ ��� �� ��� � � �������
//-----------------------------------------���������----------------------------------------------
uint8_t *pointer_MC_Tasks;
uint8_t *pointer_Errors_USART_PC;
uint8_t *pointer_Errors_USART_TIC;
uint8_t *pointer_Flags;
//------------------------------------���������� �������------------------------------------------
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length);
void transmit(uint8_t DATA[], uint8_t DATA_length);
void transmit_byte(uint8_t DATA);
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2);
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2, uint8_t DATA_3);
void MC_transmit_Birthday(void);
void MC_transmit_CPUfreq(void);
void MC_reset(void);
void COUNTERS_start(void);
void COUNTERS_sendResults(void);
void COUNTERS_stop(void);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel, EVSYS_DIGFILT_t filterCoefficient);
void decode(void);
void TIC_retransmit(void);
void TIC_transmit(void);
void TIC_request_HVE(void);
uint8_t TIC_decode_HVE(void);
uint8_t TIC_decode_ASCII(uint8_t ASCII_symbol);
void TIC_set_Gauges(void);
void TIC_send_TIC_MEM(void);
void SPI_send(uint8_t DEVICE_Number);
void checkFlags(void);
void updateFlags(void);
void checkFlag_HVE(void);
void checkFlag_PRGE(void);
void checkFlag_EDCD(void);
void checkFlag_SEMV1(void);
void checkFlag_SEMV2(void);
void checkFlag_SEMV3(void);
void checkFlag_SPUMP(void);

//------------------------------------������� ����������------------------------------------------
ISR(USARTD0_RXC_vect)
{
    //����������:
    //�������: <KEY><DATA[...]<CS><LOCK>
    //~1..3���
    //��������� ����, ��� �� ��� ������
    PC_buf = *USART_PC.DATA;//->3(95��)
    cli_PC;
    //���� � ������ �����
    if ((PC_State == USART_State_receiving) || (PC_State == USART_State_ending))
    {
        PC_timer.CNT = 0;						//�������� ���� ��������
        PC_MEM[PC_MEM_length] = PC_buf;			//��������� ����
        PC_MEM_length++;						//����������� ������� �������� ������
        PC_State = USART_State_receiving;		//������������, ��� ���� ���� �� ������
        if (PC_buf == COMMAND_LOCK) {PC_State = USART_State_ending;}	//���� �������� ������, ��������� ��������� ����
    }
    else if (PC_State == USART_State_ready)
    {
        if (PC_buf == COMMAND_KEY)
        {
            //������ ����!
            PC_State = USART_State_receiving;	//��������� � ����� �����
            PC_timer.CNT = 0;					//�������� ������
            PC_timer.CTRLA = TC_32MHz;			//��������� ������ �� 4��.
            PC_MEM_length = 0;					//�������� ������� �������� ������
        }
        else { Errors_USART_PC.Noise = 1; }		//���-�� ��������� �� �����
    }
    else if (PC_State == USART_State_decoding) { Errors_USART_PC.TooFast = 1; } //�� �� �������� ���������� �������
    sei_PC;
}
ISR(USARTE0_RXC_vect)
{
    //����������: ������ ���� ������ �� ����� USART �� TIC �����������
    //��������� �������. ������� �� �������� � ������ (���� ����������� �����).
    //��������� ����, ��� �� ��� ������
	TIC_buf = *USART_TIC.DATA;//->3(95��)
    cli_TIC;
    //���� �� ������� ����� �� ����������
    switch (TIC_State)
    {
        case USART_State_receiving:	//�� ������� ����� � TIC �� ��
            TIC_timer.CTRLA = TC_Off;					//��������� ������
            TIC_timer.CNT = 0;							//�������� ������
            //���� �������� ���� �����
            //			   <*>				<=>				 <#>  , �� �������� �������� ������
            if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
            TIC_MEM[TIC_MEM_length] = TIC_buf;			//��������� ����
            TIC_MEM_length++;
            //			   <\r>
            if (TIC_buf == 13)
            {
                //���� ���� ���� ��� <\r>
                transmit(TIC_MEM, TIC_MEM_length);		//�������� �� ��� ���������� �� ��
				TIC_MEM_length = 0;
                TIC_timer.CTRLA = TC_125kHz;			//��������� � ����� ��������
                TIC_State = USART_State_ready;
            }
            else { TIC_timer.CTRLA = TC_500kHz; }//���� ��� ��������� ����
            break;
        case USART_State_HVEreceiving:	//�� ������� ������ �� TIC'a
            TIC_timer.CTRLA = TC_Off;					//��������� ������
            TIC_timer.CNT = 0;							//�������� ������
            //���� �������� ���� �����
            //			    <*>				   <=>				  <#>  , �� �������� �������� ������
            if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
            TIC_MEM[TIC_MEM_length] = TIC_buf;			//��������� ����
            TIC_MEM_length++;
            //			   <\r>
            if (TIC_buf == 13)
            {
                //���� ����������� ������ ������, �� �������� � �������
                if (TIC_decode_HVE()) { TIC_offlineCount &= 0b00111111; }
                //��� ��������� ����������� HVE ��� ��������� � ��������
				TIC_MEM_length = 0;
				TIC_timer.CTRLA = TC_125kHz;	//��������� ������ �����
                TIC_State = USART_State_ready;
            }
            else { TIC_timer.CTRLA = TC_500kHz; }//���� ��� ��������� ����
            break;
        default: //�� �� ����� ������ �� TIC'a! ���������� ��, �� � �������� �������...
            Errors_USART_TIC.Noise = 1;
            break;
    }
    sei_TIC;
}
ISR(RTC_OVF_vect)
{
    //����������: ��������� ��� ��������� ����� ������� ��������
    //�������: ��������� ��������� ���������
    cli();
    asm(
        "LDI R16, 0x00			\n\t"//���� ��� �������� ���� ��������� (������ � �������� ��������)
        "STS 0x0800, R16		\n\t"//COA: ����� TCC0.CTRLA = 0x0800 <- ����
        "STS 0x0900, R16		\n\t"//COB: ����� TCD0.CTRLA = 0x0900 <- ����
        "STS 0x0A00, R16		\n\t"//COC: ����� TCE0.CTRLA = 0x0A00 <- ����
    );
    while (RTC.STATUS != 0)
    {
        //��� ���� ����� ����� ���������� � ��������� RTC
    }
    RTC.CTRL = RTC_PRESCALER_OFF_gc;
	while (RTC.STATUS != 0)
	{
		//��� ���� ����� ����� ���������� � ��������� RTC
	}
	RTC_ElapsedTime = RTC.CNT;
    sei();
    //��������� ����������
    COA_Measurment = COA.CNT;
    COB_Measurment = COB.CNT;
    COC_Measurment = COC.CNT;
    RTC_setStatus_ready;
    //���������� ����������� ���������
    transmit_3bytes(TOCKEN_LookAtMe, LAM_RTC_end, RTC_Status);
}
static void ISR_COA(void)
{
    if (COA_OVF != 65535)
    {
        //���� COX_OVF �� ������ �������, �� +1
        COA_OVF++;
    }
    else
    {
        //���� COX_OVF ������ �������, �� ��������� ������� � ������������� � ���� �� �� ��������
        COA.CTRLA = 0;
        COA.CNT = 65535;
    }
}
static void ISR_COB(void)
{
    if (COB_OVF != 65535)
    {
        //���� COX_OVF �� ������ �������, �� +1
        COB_OVF++;
    }
    else
    {
        //���� COX_OVF ������ �������, �� ��������� ������� � ������������� � ���� �� �� ��������
        COB.CTRLA = 0;
        COB.CNT = 65535;
    }
}
static void ISR_COC(void)
{
    if (COC_OVF != 65535)
    {
        //���� COX_OVF �� ������ �������, �� +1
        COC_OVF++;
    }
    else
    {
        //���� COX_OVF ������ �������, �� ��������� ������� � ������������� � ���� �� �� ��������
        COC.CTRLA = 0;
        COC.CNT = 65535;
    }
}
static void ISR_TIC_timer(void)
{
    //����������: ������������ ��� �������� ����� �������� �� �����: 32��� �� 256 = 125��� �� 25000 ����� = 0.2��
    //�� ����� ����� ������ �� TIC ������ ������ ��������� �����.
    cli_TIC;
    TIC_timer.CTRLA = TC_Off;
    TIC_timer.CNT = 0;
    switch (TIC_State)
    {
        case USART_State_receiving: //�� �� ������� ���������� ��������! �������� ��������! ����� �����!
			Errors_USART_TIC.Silence = 1;
            TIC_State = USART_State_ready;		//��� ������ ��������
            TIC_timer.CTRLA = TC_125kHz;		//��������� � ����� ������
            break;
        case USART_State_ready:	//����� ������! ���� ��������� � TIC'��!
            TIC_State = USART_State_HVEreceiving;	//��� ������ ��������
            TIC_request_HVE();
            TIC_timer.CTRLA = TC_500kHz;			//��������� � ����� �����
            break;
        case USART_State_HVEreceiving:	//TIC �� �������� ��������! ��� ������ �� ����� �� �����!
			cli();
			TIC_offlineCount += 64; // 01������ = 1; 10������ = 2; 11������ = 3;
			if(TIC_offlineCount > 191)
			{
				//TIC �� ����� �� ����� � � ������ ���! ���-�� �����! ��������� ����!
				pin_iHVE_high;						//��������� HVE
				Flags.iHVE = 1;
				Flags.PRGE = 0;
				sei();
				transmit_3bytes(TOCKEN_CRITICAL_ERROR, CRITICAL_ERROR_TIC_HVE_error_noResponse, TIC_MEM_length);
			}
			sei();
			Errors_USART_TIC.HVE_TimeOut = 1;		//�������� � �������
			TIC_State = USART_State_ready;		
			TIC_timer.CTRLA = TC_125kHz;		//��������� � ����� ������
            break;
        default: //���������� ������! �������� ���������!
            cli();
            pin_iHVE_high;						//��������� HVE
            Flags.iHVE = 1;
            Flags.PRGE = 0;
			sei();
            Errors_USART_TIC.HVE_error = 1;		//�������� � �������
            Errors_USART_TIC.wrongTimerState = 1;
            transmit_3bytes(TOCKEN_INTERNAL_ERROR, INTERNAL_ERROR_TIC_State, TIC_State);
            break;
    }
    sei_TIC;
}
static void ISR_PC_timer(void)
{
    //����������: ��� ��� ������������ �� ������� (32��� �� 1024), �� ���������� ����������� ������ 2 �������.
    //������� ����� ����������� ����������� ���� ������. ����� ���������� ��� �������� ��������, ����� ����������
    //���� � ������ ����� � ����������� � ��������� ������� ����������.
    //�� ����� ����� ������ ������ ������ ��������� �����.
    cli_PC;
    PC_timer.CTRLA = TC_Off;
    PC_timer.CNT = 0;
    switch (PC_State)
    {
        case USART_State_receiving: //�� �� ������� ���������� ��������! �������� ��������! ����� �����!
            PC_timer.CTRLA = TC_31kHz;		//��������� � ����� ������
            PC_timer.CNT = 0;
            PC_timer_time = 0;
            PC_State = USART_State_ready;	//��� ������ ��������
            break;
        case USART_State_ending: //���� ������� ��������! ����� ������������. ��������� ������ �������
            if (PC_MEM_length > 2)
            {
                PC_MEM_length--;				//�������� ��������� ���� (�� ���� �������)
                uint8_t CheckSum = 0;			//������� ����������� �����...
                for (uint8_t i = 0; i < PC_MEM_length; i++) { CheckSum -= PC_MEM[i]; }
                PC_MEM_length--;				//�������� ����������� �����
                if (CheckSum == PC_MEM_CheckSum) { decode();  }
                else { transmit_3bytes(TOCKEN_ERROR, ERROR_CheckSum, CheckSum); }	//�������� ����������� �����!
            }
            else { Errors_USART_PC.TooShortPacket = 1; }
            PC_State = USART_State_ready;		//���� � ������������� ���������
            PC_timer.CTRLA = TC_31kHz;			//��������� � ����� ������
            PC_timer.CNT = 0;
            PC_timer_time = 0;
            break;
        case USART_State_ready:					//�� � ������ ������.
            if (PC_timer_time >= PC_timer_TimeOut)
            {
                //����� ������ �����, PC �� ������� �� �����, ��������� PRGE (� iHVE)
                cli();
                //gpio_set_pin_high(pin_iHVE);	//��������� DC-DC 24-12
                //Flags.PRGE = 0;				//��������� PRGE �� ���� ���������
                Errors_USART_PC.Silence = 1;	//�������� ������ � �����
                sei();
            }
            else { PC_timer_time++; }
            break;
        default: //� ����� ������ ������ ������ �� ������ (������������� � ����� ������� ��������)
            break;
    }
    sei_PC;
}
//-----------------------------------------�������------------------------------------------------
void decode(void)
{
    //�������: �������������� �������
    switch (PC_MEM[0])
    {
        case COMMAND_MC_get_Status:					MC_transmit_Status;
            break;
        case COMMAND_MC_get_CPUfreq:				MC_transmit_CPUfreq();
            break;
        case COMMAND_MC_get_Version:				MC_transmit_Version;
            break;
        case COMMAND_MC_get_Birthday:				MC_transmit_Birthday();
            break;
        case COMMAND_COUNTERS_start:				COUNTERS_start();
            break;
        case COMMAND_COUNTERS_sendResults:			COUNTERS_sendResults();
            break;
        case COMMAND_COUNTERS_stop:					COUNTERS_stop();
            break;
        case COMMAND_MC_reset:						MC_reset();
            break;
        case COMMAND_TIC_retransmit:				TIC_retransmit();
            break;
        case COMMAND_checkCommandStack:				transmit_2bytes(COMMAND_checkCommandStack, MC_CommandStack);
            break;
        case COMMAND_PSIS_set_Voltage: 				SPI_send(SPI_DEVICE_Number_DAC_PSIS);
            break;
        case COMMAND_DPS_set_Voltage: 				SPI_send(SPI_DEVICE_Number_DAC_DPS);
            break;
        case COMMAND_PSInl_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_PSInl);
            break;
        case COMMAND_Scaner_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Scaner);
            break;
        case COMMAND_Condensator_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Condensator);
            break;
        case COMMAND_PSIS_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_PSIS);
            break;
        case COMMAND_DPS_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_DPS);
            break;
        case COMMAND_PSInl_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_PSInl);
            break;
        case COMMAND_MSV_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_MSV);
            break;
        case COMMAND_Flags_set: 					checkFlags();
            break;
        case COMMAND_TIC_set_Gauges: 				TIC_set_Gauges();
            break;
        case COMMAND_TIC_send_TIC_MEM: 				TIC_send_TIC_MEM();
            break;
        case COMMAND_Flags_HVE:						checkFlag_HVE();
            break;
        case COMMAND_Flags_PRGE:					checkFlag_PRGE();
            break;
        case COMMAND_Flags_EDCD:					checkFlag_EDCD();
            break;
        case COMMAND_Flags_SEMV1:					checkFlag_SEMV1();
            break;
        case COMMAND_Flags_SEMV2:					checkFlag_SEMV2();
            break;
        case COMMAND_Flags_SEMV3:					checkFlag_SEMV3();
            break;
        case COMMAND_Flags_SPUMP:					checkFlag_SPUMP();
            break;
        default: transmit_3bytes(TOCKEN_ERROR, ERROR_Decoder, PC_MEM[0]);
    }
}
//USART PC
void transmit(uint8_t DATA[], uint8_t DATA_length)
{
    //�������: �������� �������� ���������� ������, ������� �� �� ��������� � � ����������� ������
    //���������: �����: ':<response><data><CS>\r'
    //					   ':' - ������ ������
    //					   '<data>' - ����� ������ <<response><attached_data>>
    //							<response> - ������, ��� �������, �� ������� ��������
    //							<attached_data> - ���� ������. �� ����� �� ���� (������)
    //					   '<CS>' - ����������� �����
    //					   '\r' - ����� ��������
    cli();
    usart_putchar(USART_PC, COMMAND_KEY);											//':'
    for (uint8_t i = 0; i < DATA_length; i++) { usart_putchar(USART_PC, DATA[i]); }	//<data>
    usart_putchar(USART_PC, calcCheckSum(DATA, DATA_length + 1));						//<CS>
    usart_putchar(USART_PC, COMMAND_LOCK);											//'\r'
    sei();
}
void transmit_byte(uint8_t DATA)
{
    //������������: �������� ������ ����� (������)
    cli();
    usart_putchar(USART_PC, COMMAND_KEY);											//':'
    usart_putchar(USART_PC, DATA);													//<data>
    usart_putchar(USART_PC, (uint8_t)(256 - DATA));									//<CS>
    usart_putchar(USART_PC, COMMAND_LOCK);											//'\r'
    sei();
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
    //������������: �������� ������ ����� (������)
    cli();
    usart_putchar(USART_PC, COMMAND_KEY);											//':'
    usart_putchar(USART_PC, DATA_1);
    usart_putchar(USART_PC, DATA_2);													//<data>
    usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2));						//<CS>
    usart_putchar(USART_PC, COMMAND_LOCK);											//'\r'
    sei();
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2, uint8_t DATA_3)
{
    //������������: �������� ������ ����� (������)
    cli();
    usart_putchar(USART_PC, COMMAND_KEY);											//':'
    usart_putchar(USART_PC, DATA_1);
    usart_putchar(USART_PC, DATA_2);													//<data>
    usart_putchar(USART_PC, DATA_3);
    usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));				//<CS>
    usart_putchar(USART_PC, COMMAND_LOCK);											//'\r'
    sei();
}
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length)
{
    //�������: ��������� ����������� ����� �������� ������
    uint8_t CheckSum = 0;
    for (uint8_t i = 0; i < data_length - 1; i++) { CheckSum -= data[i]; }
    return CheckSum;
}
//MC
void MC_transmit_CPUfreq(void)
{
    uint32_t freq = sysclk_get_cpu_hz();
    uint8_t data[] = {COMMAND_MC_get_CPUfreq, (uint8_t)freq, (uint8_t)(freq >> 8), (uint8_t)(freq >> 16), (uint8_t)(freq >> 24)};
    transmit(data, 5);
}
void MC_transmit_Birthday(void)
{
    uint8_t data[] = {COMMAND_MC_get_Birthday, (uint8_t)MC_birthday, (uint8_t)(MC_birthday >> 8), (uint8_t)(MC_birthday >> 16), (uint8_t)(MC_birthday >> 24)};
    transmit(data, 5);
}
void MC_reset(void)
{
    //�������: ������������� ��
    //��������: ����� ������� ������ �������� ��������� ������� ����� �������������, ������� �������
    cpu_irq_disable();
    uint8_t data[] = {COMMAND_MC_reset};
    transmit(data, 1);
    RST.CTRL = 1;
}
//COUNTERS
void COUNTERS_start(void)
{
    //�������: ��������� �������� �� ����������� �����
    //������: <Command><RTC_PRE><RTC_PER[1]><RTC_PER[0]>
    cli();
    if ((RTC_Status != RTC_Status_busy))
    {
        //����������
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        RTC.PER = (PC_MEM[2] << 8) + PC_MEM[3];
        COA_Measurment = 0;
        COB_Measurment = 0;
        COC_Measurment = 0;
        COA_OVF = 0;
        COB_OVF = 0;
        COC_OVF = 0;
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        RTC.CNT = 0;
        COA.CNT = 0;
        COB.CNT = 0;
        COC.CNT = 0;
        //������
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        asm(
            "LDI R16, 0x08		\n\t"//TCC0:��� ������ ������� 0 = 0x08
            "LDI R17, 0x0A		\n\t"//TCD0:��� ������ ������� 2 = 0x0A
            "LDI R18, 0x0C		\n\t"//TCE0:��� ������ ������� 4 = 0x0C
            //"LDS R19, 0x205F	\n\t"//RTC: ����� RTC_Prescaler  = 0x205F
            "STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
            "STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
            "STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
            //"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_Prescaler(@0x205F)
        );
        RTC.CTRL =  PC_MEM[1];
        //�����
        transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
        RTC_setStatus_busy;
    }
    else
    {
        //���������! �������� �������!
        transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
    }
    sei();
}
void COUNTERS_sendResults(void)
{
    //�������: ������� ���������� ����� �� ��, ���� �����
    //������: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><CO�_M[1]><CO�_M[0]><COC_OVF[1]><COC_OVF[0]><CO�_M[1]><CO�_M[0]><RTC_ElapsedTime[1]><RTC_ElapsedTime[0]><RTC_MeasurePrescaler>
    uint8_t wDATA[14];
    wDATA[0] = COMMAND_COUNTERS_sendResults;
    wDATA[1] = RTC_Status;
    if (RTC_Status == RTC_Status_ready)
    {
        wDATA[2] = (COA_OVF >> 8);
        wDATA[3] =	COA_OVF;
        wDATA[4] = (COA_Measurment >> 8);
        wDATA[5] =	COA_Measurment;
        wDATA[6] = (COB_OVF >> 8);
        wDATA[7] =	COB_OVF;
        wDATA[8] = (COB_Measurment >> 8);
        wDATA[9] =	COB_Measurment;
        wDATA[10] = (COC_OVF >> 8);
        wDATA[11] =	 COC_OVF;
        wDATA[12] = (COC_Measurment >> 8);
        wDATA[13] =  COC_Measurment;
		wDATA[14] = (RTC_ElapsedTime >> 8);
		wDATA[15] = RTC_ElapsedTime;
		wDATA[16] = RTC_MeasurePrescaler;
    }
    transmit(wDATA, 17);
}
void COUNTERS_stop(void)
{
    //�������: �������������� ��������� ���������
    if (RTC_Status == RTC_Status_busy)
    {
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        RTC.CTRL = RTC_PRESCALER_OFF_gc;
        tc_write_clock_source(&COA, TC_CLKSEL_OFF_gc);
        tc_write_clock_source(&COB, TC_CLKSEL_OFF_gc);
        tc_write_clock_source(&COC, TC_CLKSEL_OFF_gc);
        //����� ���� ������, ������������
        COA.CNT = 0;
        COB.CNT = 0;
        COC.CNT = 0;
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        RTC.CNT = 0;
        transmit_2bytes(COMMAND_COUNTERS_stop, RTC_Status);
        RTC_setStatus_stopped;
    }
    else
    {
        transmit_2bytes(COMMAND_COUNTERS_stop, RTC_Status);
    }
}
//TIC
uint8_t TIC_decode_HVE(void)
{
    //�������: ���������� ����� ���� �� ������ HVE {?V91<NUL><\r>}
    //���������: ����� TIC'� ������ ���� �����: ? - ���� �� 48 �� 57
	//*
    //����:   61 86 57 49  ?    32   ?   46 ?    ?    ?   59 54 54 59 49 59 | 48 59 48 13
    //������: =  V  9  1 <NUL> <sp> <D1> . <D2> <D3> <D4> ;  6  6  ;  1  ;  | 0  ;  0 <\r>
    //�����:  0  1  2  3   4    5    6   7  8    9    10  11 12 13 14 15 16 | 17 18 19 20
    if ((TIC_MEM_length == 21) || (TIC_MEM_length == 22))
    {
        if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 49) && (TIC_MEM[5] == 32) && (TIC_MEM[7] == 46) && (TIC_MEM[11] == 59) && (TIC_MEM[12] == 54) && (TIC_MEM[13] == 54) && (TIC_MEM[14] == 59)  && (TIC_MEM[15] == 49) && (TIC_MEM[16] == 59))// &&			(TIC_MEM[17] == 48) && (TIC_MEM[18] == 59) && (TIC_MEM[19] == 48) && (TIC_MEM[20] == 13))
        {
            //���������� �����, ������� ������ ������ � ����������
            uint8_t Value[4] = {TIC_decode_ASCII(TIC_MEM[6]), TIC_decode_ASCII(TIC_MEM[8]), TIC_decode_ASCII(TIC_MEM[9]), TIC_decode_ASCII(TIC_MEM[10]) };
            if ((Value[0] != 255) && (Value[1] != 255) && (Value[2] != 255) && (Value[3] != 255))
            {
                //�������� ���������! ��������� ������������� �������� ���������
                uint16_t Voltage = (Value[0] << 12) + (Value[1] << 8) + (Value[2] << 4) + Value[3];
                //������� �� ������ �������
                if ((Flags.iHVE == 1) && (TIC_MEM[4] == TIC_HVE_onGauge))
                {
                    //�� ���� iHVE ������� ��������� - �� ��������� ������ DC-DC 24-12. �������� ���������� ���.
                    //������������ onLevel (������), ����� ��������. ���������� �������� ������ ���� ����� ��� ���� ����������
                    if (Voltage <= TIC_HVE_onLevel) { Flags.iHVE = 0; } //��������� �������!
                    return 1;
                }
                else if ((Flags.iHVE == 0) && (TIC_MEM[4] == TIC_HVE_offGauge))
                {
                    //�� ���� iHVE ������ ��������� - �� ��������� ������ DC-DC 24-12. ������� ���������� ����!
                    //������������ offLevel (�����), ����� ���������. ���������� �������� ������ ���� ����� ��� ���� ����������
                    if (Voltage >= TIC_HVE_offLevel)
                    {
                        //��������� �������!
                        pin_iHVE_high;
                        Flags.iHVE = 1;
                        Flags.PRGE = 0;
                    }
                    return 1;
                }
            }
        }
    }
	//*/
	//� ������ ������ TIC �� ��������� ��������� � �������, ������� ������� �������� ���������� ��������� ���������
	/*
	//����:   61 86 57 49  ?    32   ?   46 ?    ?    ?   ?  101 43  ?   ?  59 53 57 59 48 59 48 59 48 13
	//������: =  V  9  1  <G>  <sp> [D]  . [D]  [D]  [D] [D] e   +  [D] [D] ;  5  9  ;  0  ;  0  ;  0 <\r>
	//�����:  0  1  2  3   4    5    6   7  8    9   10  11  12  13 14  15  16 17 18 19 20 21 22 23 24 25
	//if (TIC_MEM_length == 26)
	//{
	//	if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 49) && 
	//		(TIC_MEM[5] == 32) && (TIC_MEM[7] == 46) && (TIC_MEM[12] == 101) && (TIC_MEM[13] == 43) && 
	//		(TIC_MEM[16] == 59) && (TIC_MEM[17] == 53)  && (TIC_MEM[18] == 57) && (TIC_MEM[19] == 59) &&
	//		(TIC_MEM[20] == 48) && (TIC_MEM[21] == 59) && (TIC_MEM[22] == 48) && (TIC_MEM[23] == 59) && 
	//		(TIC_MEM[24] == 48) && (TIC_MEM[25] == 13))
	//	{
			//���������� �����, ������� ������ ������ � ����������
			uint8_t Value[8];
			Value[0] = 9;//TIC_decode_ASCII(TIC_MEM[6]);	//�������
			Value[1] = 1;//TIC_decode_ASCII(TIC_MEM[8]);	//�������
			Value[2] = 2;//TIC_decode_ASCII(TIC_MEM[9]);	//�����
			Value[3] = 3;//TIC_decode_ASCII(TIC_MEM[10]);	//��������
			Value[4] = 4;//TIC_decode_ASCII(TIC_MEM[11]);	//��������������
			Value[5] = 5;//TIC_decode_ASCII(TIC_MEM[14]);	//������� �������
			Value[6] = 6;//TIC_decode_ASCII(TIC_MEM[15]);	//������� �������
			//switch(TIC_MEM[13])							//���� �������
			//{
			//	case 43: Value[7] = 1;					//+
			//		break;
			//	case 45: Value[7] = 0;					//-
			//		break;
			//	default: Value[7] = 255;				//������
			//		break;
			//}
			if ((Value[0] != 255) && (Value[1] != 255) && (Value[2] != 255) && (Value[3] != 255) && 
			    (Value[4] != 255) && (Value[5] != 255) && (Value[6] != 255) && (Value[7] != 255))
			{ 
				//�������� ���������! ��������� ������������� �������� ���������
				int32_t Pressure = 0;
				//Pressure =  Value[4];
				//Pressure += (Value[3] << 4);
				//Pressure += (Value[2] << 8);
				//Pressure += (Value[1] << 12);
				//Pressure += (Value[0] << 16);
				//Pressure += (Value[6] << 20);
				//Pressure += (Value[5] << 24);
				Pressure = Value[5];
				Pressure = (Pressure << 4) + Value[6];
				Pressure = (Pressure << 4) + Value[0];
				Pressure = (Pressure << 4) + Value[1];
				Pressure = (Pressure << 4) + Value[2];
				Pressure = (Pressure << 4) + Value[3];
				Pressure = (Pressure << 4) + Value[4];
				//������� �� ������ �������
				if (Value[7] == 0) { Pressure = -Pressure; }
				if ((Flags.iHVE == 1) && (TIC_MEM[4] == TIC_HVE_onGauge))
				{
					//�� ���� iHVE ������� ��������� - �� ��������� ������ DC-DC 24-12. �������� ���������� ���.
					//������������ onLevel (������), ����� ��������. ���������� �������� ������ ���� ����� ��� ���� ����������
					int32_t valve = 16777216;
					if (Pressure <= valve) 
					{ 
						Flags.iHVE = 0; 
					} //��������� �������!
					//if (Pressure <= TIC_HVE_onLevel) { Flags.iHVE = 0; } //��������� �������!
					return 1;
				}
				else if ((Flags.iHVE == 0) && (TIC_MEM[4] == TIC_HVE_offGauge))
				{
					//�� ���� iHVE ������ ��������� - �� ��������� ������ DC-DC 24-12. ������� ���������� ����!
					//������������ offLevel (�����), ����� ���������. ���������� �������� ������ ���� ����� ��� ���� ����������
					int32_t valve = -10048576;
					if (Pressure >= valve)
					//if (Pressure >= TIC_HVE_offLevel)
					{
						//��������� �������!
						pin_iHVE_high;
						Flags.iHVE = 1;
						Flags.PRGE = 0;
					}
					return 1;
				}
			}
			//return 1;
	//	}
	//}
	//*/
    //���� � ������������ ����� ���-�� �� ��� �� ���������� ����.
    //�������� �������� HVE. TIC ���-�� ������
    pin_iHVE_high;
    Flags.iHVE = 1;
    Flags.PRGE = 0;
    Errors_USART_TIC.HVE_error = 1;
    transmit_3bytes(TOCKEN_CRITICAL_ERROR, CRITICAL_ERROR_TIC_HVE_error_decode, TIC_MEM_length);
    return 0;
}
uint8_t TIC_decode_ASCII(uint8_t ASCII_symbol)
{
    switch (ASCII_symbol)
    {
        case 48: return 0;
        case 49: return 1;
        case 50: return 2;
        case 51: return 3;
        case 52: return 4;
        case 53: return 5;
        case 54: return 6;
        case 55: return 7;
        case 56: return 8;
        case 57: return 9;
        default: return 255;
    }
}
void TIC_retransmit(void)
{
    //�������: ������������ ������� �� TIC, ���� ��� ������ HVE, ���� ����� HVE ���� - ��� ������ �� TIC'� �� �����, � ����� ������ �������������.
	MC_Tasks.retransmit = 1;
	PC_State = USART_State_decoding;
	//transmit_2bytes(COMMAND_TIC_restartMonitoring, TIC_State);
    //while (TIC_State != USART_State_ready) { }	//���
    //TIC_timer.CTRLA = TC_Off;
    //TIC_timer.CNT = 0;
    //TIC_State = USART_State_receiving;	//��������� � ����� ����� �� ����������
    //for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//�������� �� ��� ������ ���������
    //for (uint8_t i = 0; i < TIC_MEM_length; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//����������
    //TIC_timer.CTRLA = TC_500kHz;			//��������� ������ �� 6��
}
void TIC_transmit(void)
{
    //�������:
    cli_TIC;
    usart_putchar(USART_TIC, 33);
    usart_putchar(USART_TIC, 83);
    usart_putchar(USART_TIC, 57);
    usart_putchar(USART_TIC, 50);
    usart_putchar(USART_TIC, 53);
    usart_putchar(USART_TIC, 32);
    usart_putchar(USART_TIC, 49);
    usart_putchar(USART_TIC, 53);
    usart_putchar(USART_TIC, 13);
    sei_TIC;
    //��� ������ �� TIC
    //���������� ����� �� ��
}
void TIC_request_HVE(void)
{
    //�������: ����������� � TIC'� ��������
    if (Flags.iHVE == 1) { TIC_HVE_Message[4] = TIC_HVE_onGauge; }	//���� HVE ��������� ������� �� onLevel(������)
    else { TIC_HVE_Message[4] = TIC_HVE_offGauge; }					//���� HVE ��������� ������� �� offLevel(�����)
    for (uint8_t i = 0; i < 6; i++) { usart_putchar(USART_TIC, TIC_HVE_Message[i]); }	//����������
}
void TIC_set_Gauges(void)
{
    //�������: ����� ������� ��� ����������� HVE � ������
    //���������: <Command><onGauge><onLevel_1><onLevel_0><offGauge><offLevel_1><offLevel_0>
    //��������� �� ������ ���������� �������� ������ ���� ready, ����� �������� ����������
    //���������� ��������� TIC
    transmit_2bytes(COMMAND_TIC_set_Gauges, TIC_State);
    while (TIC_State != USART_State_ready) { }	//���
    TIC_HVE_onGauge = PC_MEM[1];
    TIC_HVE_onLevel = (PC_MEM[2] << 8) + PC_MEM[3];
    TIC_HVE_offGauge = PC_MEM[4];
    TIC_HVE_offLevel = (PC_MEM[5] << 8) + PC_MEM[6];
}
void TIC_send_TIC_MEM(void)
{
    //�������: ���������� ������ TIC_MEM � TIC_Length
    uint8_t data[TIC_MEM_length + 10];
    data[0] = COMMAND_TIC_send_TIC_MEM;
    data[1] = TIC_MEM_length;
    for (uint8_t i = 0; i < TIC_MEM_length + 8; i++)
    {
        data[i + 2] = TIC_MEM[i];
    }
    transmit(data, TIC_MEM_length + 10);
}
//������
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource)
{
    volatile uint8_t *chMux;
    /*  Check if channel is valid and set the pointer offset for the selected
     *  channel and assign the eventSource value.
     */
    if (eventChannel < 8)
    {
        chMux = &EVSYS.CH0MUX + eventChannel;
        *chMux = eventSource;
        return true;
    }
    else
    {
        return false;
    }
}
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel, EVSYS_DIGFILT_t filterCoefficient)
{
    /*  Check if channel is valid and set the pointer offset for the selected
     *  channel and assign the configuration value.
     */
    if (eventChannel < 8)
    {
        volatile uint8_t *chCtrl;
        chCtrl = &EVSYS.CH0CTRL + eventChannel;
        *chCtrl = filterCoefficient;
        return true;
    }
    else
    {
        return false;
    }
}
//SPI
void SPI_send(uint8_t DEVICE_Number)
{
    //�������: �������� ������ ���������� SPI-���������� ���� �� DAC ��� ADC
    //	������ ���������:
    //		DEVICE_Number		NAME		TYPE
    //			1			 IonSource		DAC AD5328
    //			2			 Detector		DAC AD5328
    //			3			 Inlet			DAC AD5328
    //			4			 Scaner			DAC AD5643R
    //			5			 Condensator	DAC AD5643R
    //			6			 IonSource		ADC
    //			7			 Detector		ADC
    //			8			 Inlet			ADC
    //			9			 MSV			ADC (Scaner and Condensator)
    //�������� ���������� ����������
    bool DEVICE_is_DAC = true;
    bool DAC_is_AD5643R = false;
    struct spi_device SPI_DEVICE =
    {
        .id = 0
    };
    switch (DEVICE_Number)
    {
        case 1: SPI_DEVICE = DAC_IonSource;
            break;
        case 2: SPI_DEVICE = DAC_Detector;
            break;
        case 3:	SPI_DEVICE = DAC_Inlet;
            break;
        case 4: SPI_DEVICE = DAC_Scaner;
            DAC_is_AD5643R = true;
            break;
        case 5: SPI_DEVICE = DAC_Condensator;
            DAC_is_AD5643R = true;
            break;
        case 6:	SPI_DEVICE = ADC_IonSource;
            DEVICE_is_DAC = false;
            break;
        case 7:	SPI_DEVICE = ADC_Detector;
            DEVICE_is_DAC = false;
            break;
        case 8: SPI_DEVICE = ADC_Inlet;
            DEVICE_is_DAC = false;
            break;
        case 9:  SPI_DEVICE = ADC_MSV;
            DEVICE_is_DAC = false;
            break;
        default:
            transmit_3bytes(TOCKEN_ERROR, INTERNAL_ERROR_SPI, DEVICE_Number);
            return;
    }
    uint8_t SPI_rDATA[] = {0, 0};				//������ SPI ��� ����� ������ (��� �����)
    //���� ���������� DAC AD5643R �� �������� ������ �� ��� ���������, ����������� � �������
    if (DAC_is_AD5643R)
    {
        //���������������� �� ����?
        uint8_t sdata[] = {PC_MEM[1], PC_MEM[2], PC_MEM[3]};
        spi_select_device(&SPIC, &SPI_DEVICE);
        spi_write_packet(&SPIC, sdata, 3);
        spi_deselect_device(&SPIC, &SPI_DEVICE);
        //�����������
        uint8_t aswDATA[] = {PC_MEM[0]};
        transmit(aswDATA, 1);
        return;
    }
    //���� SPI-���������� - ���, �� ��������, ����������� � �������.
    if (DEVICE_is_DAC)
    {
        uint8_t sdata[] = {PC_MEM[1], PC_MEM[2]};
        spi_select_device(&SPIC, &SPI_DEVICE);
        spi_write_packet(&SPIC, sdata, 2);
        spi_deselect_device(&SPIC, &SPI_DEVICE);
        uint8_t aswDATA[] = {PC_MEM[0]};
        transmit(aswDATA, 1);
        return;
    }
    //���� SPI-���������� - ���, �� ��������, �������� �����, �������� �����.
    uint8_t sdata[] = {PC_MEM[1], PC_MEM[2]};
    gpio_set_pin_low(pin_iRDUN);
    spi_write_packet(&SPIC, sdata, 2);
    gpio_set_pin_high(pin_iRDUN);
    //������ ��� �����
    spi_deselect_device(&SPIC, &SPI_DEVICE);
    gpio_set_pin_low(pin_iRDUN);
    spi_read_packet(&SPIC, SPI_rDATA, 2);
    gpio_set_pin_high(pin_iRDUN);
    spi_select_device(&SPIC, &SPI_DEVICE);
    //������ ����� �� �� �� USART
    uint8_t aswDATA[] = {PC_MEM[0], SPI_rDATA[0], SPI_rDATA[1]};
    transmit(aswDATA, 3);
}
//�����
void checkFlags(void)
{
    //�������: ���������� ����� � ������������ � �������� ������, ���� ������ ���� 1, � ���������� ���������. ����� ������ ���������� �����
    //������: <Command><[���������\����������][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]>
    //				���� ������ ��� <���������\����������> = 0, �� �� ��� �� ���������� ������� ��������� ������
    //				���� ������ ��� <���������\����������> = 1, �� �� ������������� ����� (����� iHVE) � ���������� ��.
    //				iHVE - ������ ������
    //				����� �� ����� [���������\����������] -> 1 - ��������� ���� ��������, 0 - ������ ������
    updateFlags();
    Flags.checkOrSet = 0; //�� ���� �������� �� ��� ������
    if ((PC_MEM[1] >> 7) == 0)
    {
        //���������. ������� �� �� ���������� ������ � ������
        transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
        return;
    }
    //����������!
    uint8_t receivedFlag = ((PC_MEM[1] & 32) >> 5);	//�������� ��� PRGE
    //���� ���������� PRGE(receivedFlag) �� ����� ��� ����������...
    if (Flags.PRGE  != receivedFlag)
    {
        //��, ���� �������� �������...
        if (receivedFlag == 1)
        {
            //� ���� iHVE ���� - TIC ��� �����, �� ������� ����������
            if (Flags.iHVE == 0)
            {
                //�� ������ ���������� ���� �� iHVE (������ ��������� ��������� ������ DC-DC 24-12)
                Flags.PRGE = 1;	//�������� ��� �����
                MC_Tasks.turnOnHVE = 1;//������ ��������� �������� DAC'�� ����� ������� ������
                Flags.checkOrSet = 1; //��� ������ ��������
            }
        }
        else
        {
            pin_iHVE_high;			//��������� DC-DC 24-12
            Flags.PRGE = 0;			//�������� ���������
            Flags.checkOrSet = 1;	//��� ������ ��������
        }
    }
    receivedFlag = ((PC_MEM[1] & 16) >> 4);
    if (Flags.iEDCD != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_iEDCD); Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_iEDCD); Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 8) >> 3);
    if (Flags.SEMV1 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV1); Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV1); Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 4) >> 2);
    if (Flags.SEMV2 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV2); Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV2); Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 2) >> 1);
    if (Flags.SEMV3 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV3); Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV3); Flags.checkOrSet = 1;}}
    receivedFlag = PC_MEM[1] & 1;
    if (Flags.SPUMP != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SPUMP); Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SPUMP); Flags.checkOrSet = 1;}}
    updateFlags();
    transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
    if (MC_Tasks.turnOnHVE)
    {
        pin_iHVE_low; //�������� DC-DC 24-12
        cpu_delay_ms(2000, 32000000); //iHVE �������� �������� ������������ ����, ������� ���� ��������.
        //������� ���������� �������� - ������������� DAC�
        //MSV DAC'� AD5643R (����������� � ������) - ������� ��������
        uint8_t SPI_DATA[] = {AD5643R_confHbyte, AD5643R_confMbyte, AD5643R_confLbyte};
        spi_select_device(&SPIC, &DAC_Condensator);
        spi_select_device(&SPIC, &DAC_Scaner);
        spi_write_packet(&SPIC, SPI_DATA, 3);
        spi_deselect_device(&SPIC, &DAC_Condensator);
        spi_deselect_device(&SPIC, &DAC_Scaner);
        //MSV DAC'� AD5643R (����������� � ������) - ��������� ���������� �� ������ �������
        SPI_DATA[0] = AD5643R_startVoltage_Hbyte;
        SPI_DATA[1] = AD5643R_startVoltage_Mbyte;
        SPI_DATA[2] = AD5643R_startVoltage_Lbyte;
        spi_select_device(&SPIC, &DAC_Scaner);
        spi_select_device(&SPIC, &DAC_Condensator);
        spi_write_packet(&SPIC, SPI_DATA, 3);
        spi_deselect_device(&SPIC, &DAC_Scaner);
        spi_deselect_device(&SPIC, &DAC_Condensator);
        //MSV DAC AD5643R (������) - ��������� ���������� �� ������ ������
        SPI_DATA[0] = AD5643R_startVoltage_Hbyte + 1;
        spi_select_device(&SPIC, &DAC_Scaner);
        spi_write_packet(&SPIC, SPI_DATA, 3);
        spi_deselect_device(&SPIC, &DAC_Scaner);
        //DPS + PSIS DAC'� AD5328R (�������� � ������ ��������) - ������� ��������
        SPI_DATA[0] = AD5328R_confHbyte;
        SPI_DATA[1] = AD5328R_confLbyte;
        spi_select_device(&SPIC, &DAC_Detector);
        spi_select_device(&SPIC, &DAC_IonSource);
        spi_write_packet(&SPIC, SPI_DATA, 2);
        spi_deselect_device(&SPIC, &DAC_Detector);
        spi_deselect_device(&SPIC, &DAC_IonSource);
        //��������� �� ������������������ ��������!
        /*delay_s(2);
        //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (���������)
        sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_IV;
        sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_IV;
        spi_select_device(&SPIC,&DAC_IonSource);
        spi_write_packet(&SPIC, sdata, 2);
        spi_deselect_device(&SPIC,&DAC_IonSource);
        //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (��� �������)
        sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_EC;
        sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_EC;
        spi_select_device(&SPIC,&DAC_IonSource);
        spi_write_packet(&SPIC, sdata, 2);
        spi_deselect_device(&SPIC,&DAC_IonSource);
        //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������� ������ (�������� 1)
        sdata[0] += 16;//������� �� ��������� �����
        //sdata[1] = ;
        spi_select_device(&SPIC,&DAC_IonSource);
        spi_write_packet(&SPIC, sdata, 2);
        spi_deselect_device(&SPIC,&DAC_IonSource);
        //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� �������� ������ (�������� 2)
        sdata[0] += 16;//������� �� ��������� �����
        //sdata[1] = ;
        spi_select_device(&SPIC,&DAC_IonSource);
        spi_write_packet(&SPIC, sdata, 2);
        spi_deselect_device(&SPIC,&DAC_IonSource);*/
        MC_Tasks.turnOnHVE = 0;
    }
    return;
}
void updateFlags(void)
{
    //�������: �� ����������� �������� ���� ������ � �������� �� � ���� Flags
    //Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
    Flags.iEDCD = (PORTA.OUT & 128) >> 7;
    Flags.SEMV1 = (PORTD.OUT & 2) >> 1;
    Flags.SEMV2 = (PORTD.OUT & 16) >> 4;
    Flags.SEMV3 = (PORTD.OUT & 32) >> 5;
    Flags.SPUMP = PORTD.OUT & 1;
}
void checkFlag_HVE(void)
{
    //�������: ���������� �� � HVE
    //�����: <Command><pin_iHVE><Flags.HVE><onGauge><onLevel[1]><onLevel[0]><offGauge><offLevel[1]><offLevel[1]><monitoringEnabled>
    uint8_t DATA[] = {COMMAND_Flags_HVE, ((PORTC.OUT & 8) >> 3), Flags.iHVE, TIC_HVE_onGauge, (TIC_HVE_onLevel >> 8), TIC_HVE_onLevel, TIC_HVE_offGauge, (TIC_HVE_offLevel >> 8), TIC_HVE_offLevel, TIC_timer.CTRLA };
    transmit(DATA, 10);
}
void checkFlag_PRGE(void)
{
    //�������: ���������� ��� ������������� PRGE
    //�����: <Command><getOrSet>
    //					<0>\<1> - �������������
    //					<any_else> - �����������
    switch (PC_MEM[1])
    {
        case 0: //��������� � ����
            pin_iHVE_high;			//��������� DC-DC 24-12
            Flags.PRGE = 0;			//�������� ���������
            break;
        case 1://� ���� iHVE ���� - TIC ��� �����, �� ������� ���������� - ��������� � �������
            if (Flags.iHVE == 0)
            {
                //�� ������ ���������� ���� �� iHVE (������ ��������� ��������� ������ DC-DC 24-12)
                Flags.PRGE = 1;	//�������� ��� �����
                MC_Tasks.turnOnHVE = 1;//������ ��������� �������� DAC'�� ����� ������� ������
                break;
            }
            else
            {
                transmit_2bytes(COMMAND_Flags_PRGE, 254);	//TIC ���������.
                return;
            }
            return;
        default: //������
            break;
    }
    transmit_2bytes(COMMAND_Flags_PRGE, Flags.PRGE);
}
void checkFlag_EDCD(void)
{
    //�������: �������� ��� ��������� ������������� ���������� ����������� �� ����������, �����.
    //�����: <Command><OnOrOff>
    switch (PC_MEM[1])
    {
        case 0: pin_iEDCD_low;
            break;
        case 1: pin_iEDCD_high;
            break;
        default:
            break;
    }
    transmit_2bytes(COMMAND_Flags_EDCD, ((PORTA.OUT & 128) >> 7));
}
void checkFlag_SEMV1(void)
{
    //�������: �������� ��� ��������� �������
    //�����: <Command><OnOrOff>
    switch (PC_MEM[1])
    {
        case 0: pin_SEMV1_low;
            break;
        case 1: pin_SEMV1_high;
            break;
        default:
            break;
    }
    transmit_2bytes(COMMAND_Flags_SEMV1, ((PORTD.OUT & 2) >> 1));
}
void checkFlag_SEMV2(void)
{
    //�������: �������� ��� ��������� �������
    //�����: <Command><OnOrOff>
    switch (PC_MEM[1])
    {
        case 0: pin_SEMV2_low;
            break;
        case 1: pin_SEMV2_high;
            break;
        default:
            break;
    }
    transmit_2bytes(COMMAND_Flags_SEMV2, ((PORTD.OUT & 16) >> 4));
}
void checkFlag_SEMV3(void)
{
    //�������: �������� ��� ��������� �������
    //�����: <Command><OnOrOff>
    switch (PC_MEM[1])
    {
        case 0: pin_SEMV3_low;
            break;
        case 1: pin_SEMV3_high;
            break;
        default:
            break;
    }
    transmit_2bytes(COMMAND_Flags_SEMV3, ((PORTD.OUT & 32) >> 5));
}
void checkFlag_SPUMP(void)
{
    //�������: �������� ��� ��������� �������
    //�����: <Command><OnOrOff>
    switch (PC_MEM[1])
    {
        case 0: pin_SPUMP_low;
            break;
        case 1: pin_SPUMP_high;
            break;
        default:
            break;
    }
    transmit_2bytes(COMMAND_Flags_SPUMP, (PORTD.OUT & 1));
}
/*
void fun(void)
{
	//����������: ������ ���� ������ �� ����� USART �� TIC �����������
	//��������� �������. ������� �� �������� � ������ (���� ����������� �����).
	//��������� ����, ��� �� ��� ������
	//TIC_buf = *USART_TIC.DATA;//->3(95��)
	cli_TIC;
	//���� �� ������� ����� �� ����������
	switch (TIC_State)
	{
		case USART_State_receiving:	//�� ������� ����� � TIC �� ��
		TIC_timer.CTRLA = TC_Off;					//��������� ������
		TIC_timer.CNT = 0;							//�������� ������
		//���� �������� ���� �����
		//			   <*>				<=>				 <#>  , �� �������� �������� ������
		if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
		TIC_MEM[TIC_MEM_length] = TIC_buf;			//��������� ����
		TIC_MEM_length++;
		//			   <\r>
		if (TIC_buf == 13)
		{
			//���� ���� ���� ��� <\r>
			TIC_timer.CTRLA = TC_125kHz;			//��������� � ����� ��������
			TIC_timer.CNT = 0;
			TIC_State = USART_State_ready;
			transmit(TIC_MEM, TIC_MEM_length);		//�������� �� ��� ���������� �� ��
		}
		else { TIC_timer.CTRLA = TC_500kHz; }//���� ��� ��������� ����
		break;
		case USART_State_HVEreceiving:	//�� ������� ������ �� TIC'a
		TIC_timer.CTRLA = TC_Off;					//��������� ������
		TIC_timer.CNT = 0;							//�������� ������
		//���� �������� ���� �����
		//			    <*>				   <=>				  <#>  , �� �������� �������� ������
		if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
		TIC_MEM[TIC_MEM_length] = TIC_buf;			//��������� ����
		TIC_MEM_length++;
		//			   <\r>
		if (TIC_buf == 13)
		{
			//���� ����������� ������ ������, ��������� ������ �����
			if(TIC_decode_HVE()) { TIC_timer.CTRLA = TC_125kHz; }
			//��� ��������� ����������� HVE ��� ��������� � ��������
			TIC_State = USART_State_ready;
		}
		else { TIC_timer.CTRLA = TC_500kHz; }//���� ��� ��������� ����
		break;
		default: //�� �� ����� ������ �� TIC'a! ���������� ��, �� � �������� �������...
		Errors_USART_TIC.Noise = 1;
		break;
	}
	sei_TIC;
}
void PC_fun(void)
{
	//����������:
	//�������: <KEY><DATA[...]<CS><LOCK>
	//~1..3���
	//��������� ����, ��� �� ��� ������
	//PC_buf = *USART_PC.DATA;//->3(95��)
	cli_PC;
	//���� � ������ �����
	if ((PC_State == USART_State_receiving) || (PC_State == USART_State_ending))
	{
		PC_timer.CNT = 0;						//�������� ���� ��������
		PC_MEM[PC_MEM_length] = PC_buf;			//��������� ����
		PC_MEM_length++;						//����������� ������� �������� ������
		PC_State = USART_State_receiving;		//������������, ��� ���� ���� �� ������
		if (PC_buf == COMMAND_LOCK) {PC_State = USART_State_ending;}	//���� �������� ������, ��������� ��������� ����
	}
	else if (PC_State == USART_State_ready)
	{
		if (PC_buf == COMMAND_KEY)
		{
			//������ ����!
			PC_State = USART_State_receiving;	//��������� � ����� �����
			PC_timer.CNT = 0;					//�������� ������
			PC_timer.CTRLA = TC_32MHz;			//��������� ������ �� 4��.
			PC_MEM_length = 0;					//�������� ������� �������� ������
		}
		else { Errors_USART_PC.Noise = 1; }		//���-�� ��������� �� �����
	}
	else if (PC_State == USART_State_decoding) { Errors_USART_PC.TooFast = 1; } //�� �� �������� ���������� �������
	sei_PC;
}
//*/
//-------------------------------------������ ���������-------------------------------------------
int main(void)
{
    confPORTs;							//������������� ����� (HVE ��� � ������ �������)
    cli();
    SYSCLK_init;						//���������� �������� (32���)
    pmic_init();						//���������� ������� ����������
    SPIC.CTRL = 87;						//���������� ������� SPI
    RTC_init;							//���������� ������� ��������� �������
    Counters_init;						//���������� �������� ���������
    USART_PC_init;					//���������� USART � ����������
    USART_TIC_init;						//���������� USART � ��������
    //USARTD0.CTRLA = 32;
    //USARTD0.CTRLB = 24;
    //USARTD0.CTRLC = 3;
    //USARTD0.BAUDCTRLA = 234;
    //USARTD0.BAUDCTRLB = 192;
    //�������� �������������
    pointer_Flags = &Flags;
    pointer_Errors_USART_PC = &Errors_USART_PC;
    pointer_Errors_USART_TIC = &Errors_USART_TIC;
    updateFlags();
    RTC_setStatus_ready;
    Flags.iHVE = 1; //��������� ������� ����������, �� ��� ��� ���� �� TIC'� �� ����� ����������
    Flags.PRGE = 0;	//���������� o������� ��������� ������� ���������� (��� ���������� �� TIC ������������ ���� ������ �����������!)
    //������ PC
    PC_timer.PER = 65535;				//60 ��� �� 31���
    PC_timer.CNT = 0;
    PC_timer.CTRLA = TC_31kHz;			//�������� �� ������ �� ����� ������
    PC_State = USART_State_ready;		//��������� USART_PC � ����� ������
    //T����� TIC
    TIC_timer.PER = 25000;				//200�� �� 125���
    TIC_timer.CNT = 0;
	//TIC_timer.CTRLA = TC_125kHz;		//�������� TIC'������ ������ HVE
    TIC_State = USART_State_ready;		//��������� USART_PC � ����� �����.
    sei();								//��������� ����������
    //������������� ���������
	TIC_decode_HVE();
	TIC_decode_HVE();
	TIC_decode_HVE();
	TIC_decode_HVE();
    while (1)
    {
        if (MC_Tasks.turnOnHVE)
        {
            pin_iHVE_low; //�������� DC-DC 24-12
            cpu_delay_ms(2000, 32000000); //iHVE �������� �������� ������������ ����, ������� ���� ��������.
            //������� ���������� �������� - ������������� DAC�
            //MSV DAC'� AD5643R (����������� � ������) - ������� ��������
            uint8_t SPI_DATA[] = {AD5643R_confHbyte, AD5643R_confMbyte, AD5643R_confLbyte};
            spi_select_device(&SPIC, &DAC_Condensator);
            spi_select_device(&SPIC, &DAC_Scaner);
            spi_write_packet(&SPIC, SPI_DATA, 3);
            spi_deselect_device(&SPIC, &DAC_Condensator);
            spi_deselect_device(&SPIC, &DAC_Scaner);
            //MSV DAC'� AD5643R (����������� � ������) - ��������� ���������� �� ������ �������
            SPI_DATA[0] = AD5643R_startVoltage_Hbyte;
            SPI_DATA[1] = AD5643R_startVoltage_Mbyte;
            SPI_DATA[2] = AD5643R_startVoltage_Lbyte;
            spi_select_device(&SPIC, &DAC_Scaner);
            spi_select_device(&SPIC, &DAC_Condensator);
            spi_write_packet(&SPIC, SPI_DATA, 3);
            spi_deselect_device(&SPIC, &DAC_Scaner);
            spi_deselect_device(&SPIC, &DAC_Condensator);
            //MSV DAC AD5643R (������) - ��������� ���������� �� ������ ������
            SPI_DATA[0] = AD5643R_startVoltage_Hbyte + 1;
            spi_select_device(&SPIC, &DAC_Scaner);
            spi_write_packet(&SPIC, SPI_DATA, 3);
            spi_deselect_device(&SPIC, &DAC_Scaner);
            //DPS + PSIS DAC'� AD5328R (�������� � ������ ��������) - ������� ��������
            SPI_DATA[0] = AD5328R_confHbyte;
            SPI_DATA[1] = AD5328R_confLbyte;
            spi_select_device(&SPIC, &DAC_Detector);
            spi_select_device(&SPIC, &DAC_IonSource);
            spi_write_packet(&SPIC, SPI_DATA, 2);
            spi_deselect_device(&SPIC, &DAC_Detector);
            spi_deselect_device(&SPIC, &DAC_IonSource);
            //��������� �� ������������������ ��������!
            /*delay_s(2);
            //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (���������)
            sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_IV;
            sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_IV;
            spi_select_device(&SPIC,&DAC_IonSource);
            spi_write_packet(&SPIC, sdata, 2);
            spi_deselect_device(&SPIC,&DAC_IonSource);
            //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (��� �������)
            sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_EC;
            sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_EC;
            spi_select_device(&SPIC,&DAC_IonSource);
            spi_write_packet(&SPIC, sdata, 2);
            spi_deselect_device(&SPIC,&DAC_IonSource);
            //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������� ������ (�������� 1)
            sdata[0] += 16;//������� �� ��������� �����
            //sdata[1] = ;
            spi_select_device(&SPIC,&DAC_IonSource);
            spi_write_packet(&SPIC, sdata, 2);
            spi_deselect_device(&SPIC,&DAC_IonSource);
            //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� �������� ������ (�������� 2)
            sdata[0] += 16;//������� �� ��������� �����
            //sdata[1] = ;
            spi_select_device(&SPIC,&DAC_IonSource);
            spi_write_packet(&SPIC, sdata, 2);
            spi_deselect_device(&SPIC,&DAC_IonSource);*/
            MC_Tasks.turnOnHVE = 0;
			transmit_2bytes(TOCKEN_LookAtMe,LAM_SPI_conf_done);
        }
		if((MC_Tasks.retransmit)&&(TIC_State != USART_State_HVEreceiving))
		{
			cli_TIC;
			TIC_timer.CTRLA = TC_Off;
			TIC_timer.CNT = 0;
			TIC_State = USART_State_receiving;	//��������� � ����� ����� �� ����������
			for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//�������� �� ��� ������ ���������
			//TIC_MEM_length = PC_MEM_length - 1;
			for (uint8_t i = 0; i < PC_MEM_length - 1; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//����������
			//TIC_MEM_length = 0;
			//usart_putchar(USART_PC,TIC_MEM_length);
			TIC_timer.CTRLA = TC_500kHz;			//��������� ������ �� 6��
			sei_TIC;
			MC_Tasks.retransmit = 0;
			PC_timer.CTRLA = TC_31kHz;		//��������� � ����� ������
			PC_timer.CNT = 0;
			PC_timer_time = 0;
			PC_State = USART_State_ready;	//��� ������ ��������
		}
    }
}
//-----------------------------------------�������------------------------------------------------
/*
*
*/
/*
*/
//-----------------------------------------THE END------------------------------------------------