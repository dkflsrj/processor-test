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
#define byte uint8_t
//								//����������� �������.
//#include <avr/pgmspace.h>		//�������� ���������� flash-������� �����������
#include <spi_master.h>			//�������� ������ SPI
#include <Decoder.h>
#include <Initializator.h>

//---------------------------------------�����������----------------------------------------------
//��
#define version										173
#define birthday									20140715
//��������
#define RTC_Status_ready							0		//�������� ����� � ������
#define RTC_Status_stopped							1		//�������� ��� ������������� ����������
#define RTC_Status_busy								2		//�������� ��� �������
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
//��������� USART
#define	USART_TIC_State_ready						0		//USART ������ �� ���������
#define USART_TIC_State_receiving					1		//USART ��������� �����
#define USART_TIC_State_ending						2		//USART ������� ���� �������, ��������� ���������� ��������
#define USART_TIC_State_decoding					3		//USART ���������� �������
#define USART_TIC_State_receiving_TICstatus			4		//USART (TIC) ��������� ����� TIC'a �� ������ HVE
//��������� ������������ ��� DAC AD5643R -> ������� ��������
#define AD5643R_confHbyte							56
#define AD5643R_confMbyte							0
#define AD5643R_confLbyte							1
//��������� ���������� ��� DAC AD5643R MSV Scan (�����������) = 2000
#define AD5643R_startVoltage_Hbyte_MSV_S			25	//�����
#define AD5643R_startVoltage_Mbyte_MSV_S			31	//������� ���� ����������
#define AD5643R_startVoltage_Lbyte_MSV_S			64	//������� ���� ���������� � 2 ������� �������� ������
//��������� ���������� ��� DAC AD5643R MSV Scan (��������������) = 1500
#define AD5643R_startVoltage_Hbyte_MSV_PS			24
#define AD5643R_startVoltage_Mbyte_MSV_PS			23
#define AD5643R_startVoltage_Lbyte_MSV_PS			112
//��������� ���������� ��� DAC AD5643R MSV Cap (�����������) = 500
#define AD5643R_startVoltage_Hbyte_MSV_C			24
#define AD5643R_startVoltage_Mbyte_MSV_C			7
#define AD5643R_startVoltage_Lbyte_MSV_C			208
//��������� ������������ ��� DAC AD5328R -> ������� ��������
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//��������� ���������� PSIS EC (���� �������) - 2,44���
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			200
//��������� ���������� DAC PSIS IV (���������) - 3600 - 80�
#define AD5328R_startVoltage_Hbyte_PSIS_IV			30
#define AD5328R_startVoltage_Lbyte_PSIS_IV			16
//��������� ���������� DAC PSIS F1 (�������� 1) - 4000
#define AD5328R_startVoltage_Hbyte_PSIS_F1			47
#define AD5328R_startVoltage_Lbyte_PSIS_F1			160
//��������� ���������� DAC PSIS F2 (�������� 2) - 4000
#define AD5328R_startVoltage_Hbyte_PSIS_F2			63
#define AD5328R_startVoltage_Lbyte_PSIS_F2			160

//----------------------------------------����������----------------------------------------------
//	���������������
uint8_t  MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t  MC_CommandStack = 0;
uint8_t  MC_Status = 0;
//		USART TIC
uint8_t TIC_timer_time = 0;								//������ ������� ����� � ������
uint8_t TIC_MEM[100];									//100 ���� ������ ��� ����� ������ �� TIC
uint8_t TIC_MEM_length = 0;								//����� ����������� � TIC_MEM ������ ������.
uint8_t TIC_buf = 0;									//����� �����. �������� ����� �������� ���� (���� ���)
uint8_t TIC_State = 0;									//��������� ������ USART_TIC
uint8_t TIC_HVE_Message[6] = {63, 86, 57, 48, 50, 13};	//char'� ��������� �� ������ ������� TIC'� {"?V902"+'\r'}
uint8_t TIC_HVE_offlineCount = 0;						//���������� ��������, ������� ��������������. 3 ���� � ��������� �������.
uint8_t TIC_HVE_Error_sent = 0;							//�����: 0 - ������ �� ���������� ����������, 1 - ������ ��� ���������� ����������
uint8_t TIC_Online = 0;									//�����: 0 - ��� ����� � TIC'��, 1 - TIC �� �����
byte TIC_Status[30];									//��������� ��������� ������� �� TIC'a 
byte TIC_Status_length = 0;								//������ ��������� �������
byte TIC_disprove_jitter = 0;							//���� �������� ��������� (�� �������� �������)
byte TIC_disprove_jitter_MAX = 5;						//������������ ���������� ��������
byte TIC_R1_jitter = 0;									//���� �������� ��������� (�� �������� �������)
byte TIC_R1_jitter_MAX = 5;							//������������ ���������� ��������
//		���������
byte RTC_delay = 0;										//����: RTC � ��������
uint8_t  RTC_Status = RTC_Status_ready;					//��������� ��������
uint16_t RTC_ElapsedTime = 0;
uint8_t  RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//������������ RTC
uint16_t RTC_MeasurePeriod = 0;							//������ RTC
uint8_t  RTC_DelayPrescaler = RTC_PRESCALER_OFF_gc;		//������������ RTC
uint16_t RTC_DelayPeriod = 0;							//������ RTC
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
    uint8_t noError3				: 1;
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
void MC_transmit_Birthday(void);
void MC_transmit_CPUfreq(void);
void COUNTERS_start(void);
void COUNTERS_sendResults(void);
void COUNTERS_stop(void);
void COUNTERS_delayedStart(void);
uint32_t COUNTERS_msToTicks(uint16_t ms);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel, EVSYS_DIGFILT_t filterCoefficient);
void decode(void);
void TIC_retransmit(void);
void TIC_request_Status(void);
void TIC_decode(void);
uint8_t TIC_decode_ASCII(uint8_t ASCII_symbol);
void TIC_send_TIC_MEM(void);
void TIC_getStatus(void);
void TIC_setJitter(byte Jitter_TIC_disprove, byte Jitter_TIC_R1_off);
void SPI_send(uint8_t DEVICE_Number);
void SPI_get_AllVoltages(void);
void SPI_confDACs_toDoubleRef(void);
void updateFlags(void);
void checkFlag_HVE(void);
void checkFlag_PRGE(void);
void checkFlag_EDCD(void);
void checkFlag_SEMV1(void);
void checkFlag_SEMV2(void);
void checkFlag_SEMV3(void);
void checkFlag_SPUMP(void);
//void fun(void);
byte receive(void);
void turnOn_HV(void);
//-----------------------------------------���������----------------------------------------------
#include <Radist.h>
//------------------------------------������� ����������------------------------------------------
ISR(USART_PC_vect) { receiving(); }
ISR(USART_TIC_vect)
{
    //����������: ������ ���� ������ �� ����� USART �� TIC �����������
    //��������� �������. ������� �� �������� � ������ (���� ����������� �����).
    //��������� ����, ��� �� ��� ������
	TIC_buf = *USART_TIC.DATA;//->3(95��)
    cli_TIC;
    //���� �� ������� ����� �� ����������
	if((TIC_State == USART_TIC_State_receiving) || (TIC_State == USART_TIC_State_receiving_TICstatus))
	{
		TIC_timer.CTRLA = TC_Off;					//��������� ������
		TIC_timer.CNT = 0;							//�������� ������
		//���� �������� ���� �����
		//			   <*>				<=>				 <#>  , �� �������� �������� ������
		if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35))
		{ 
			if(TIC_State == USART_TIC_State_receiving)
			{
				TIC_MEM[0] = COMMAND_TIC_retransmit;
				TIC_MEM_length = 1;
			}
			else
			{
				TIC_MEM_length = 0;
			}
		}
		TIC_MEM[TIC_MEM_length] = TIC_buf;			//��������� ����
		TIC_MEM_length++;
		//			   <\r>
		if (TIC_buf == 13)
		{
			if (TIC_State == USART_TIC_State_receiving) { transmit(TIC_MEM, TIC_MEM_length); }		//�������� �� ��� ���������� �� ��
			else { TIC_decode(); }	//��� ��������� ����������� HVE ��� ��������� � ��������
			TIC_MEM_length = 0;
			TIC_timer.CTRLA = TC_125kHz;			//��������� � ����� ��������
			TIC_State = USART_TIC_State_ready;
		}
		else { TIC_timer.CTRLA = TC_500kHz; }//���� ��� ��������� ����
	}
	else { Errors_USART_TIC.Noise = 1; }
    sei_TIC;
}
ISR(RTC_OVF_vect)
{
    //����������: ��������� ��� ��������� ����� ������� ��������
    //�������: ��������� ��������� ���������
    cli();
    while (RTC.STATUS != 0)
    {
        //��� ���� ����� ����� ���������� � ��������� RTC
    }
	asm(
        "LDI R16, 0x00			\n\t"//���� ��� �������� ���� ��������� (������ � �������� ��������)
        "STS 0x0800, R16		\n\t"//COA: ����� TCC0.CTRLA = 0x0800 <- ����
        "STS 0x0900, R16		\n\t"//COB: ����� TCD0.CTRLA = 0x0900 <- ����
        "STS 0x0A00, R16		\n\t"//COC: ����� TCE0.CTRLA = 0x0A00 <- ����
		"STS 0x0400, R16		\n\t"//RTC: ����� RTC.CTRL   = 0x0400 <- ����
    );
	if(RTC_delay)
	{
		RTC_delay = 0;
		while (RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		while (RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CNT = 0;
		//������
		while (RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CTRL = RTC_MeasurePrescaler;
		asm(
		"LDI R16, 0x08		\n\t"//TCC0:��� ������ ������� 0 = 0x08
		"LDI R17, 0x0A		\n\t"//TCD0:��� ������ ������� 2 = 0x0A
		"LDI R18, 0x0C		\n\t"//TCE0:��� ������ ������� 4 = 0x0C
		//"LDS R19, 0x2078	\n\t"//RTC: ����� RTC_Prescaler  = 0x2078
		"STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
		"STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
		"STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
		//"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_MeasurePrescaler(@0x2078)
		);
	}
	while (RTC.STATUS != 0)
	{
		//��� ���� ����� ����� ���������� � ��������� RTC
	}
	RTC_ElapsedTime = RTC.CNT;
	while (RTC.STATUS != 0)
	{
		//��� ���� ����� ����� ���������� � ��������� RTC
	}
	RTC.CNT = 0;
	sei();
	//��������� ����������
	COA_Measurment = COA.CNT;
	COB_Measurment = COB.CNT;
	COC_Measurment = COC.CNT;
    RTC_setStatus_ready;
    //���������� ����������� ���������
	transmit_3rytes(TOKEN_ASYNCHRO, LAM_RTC_end, RTC_Status);
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
        case USART_TIC_State_receiving: //�� �� ������� ���������� ��������! �������� ��������! ����� �����!
			Errors_USART_TIC.Silence = 1;
            TIC_State = USART_TIC_State_ready;		//��� ������ ��������
            TIC_timer.CTRLA = TC_125kHz;		//��������� � ����� ������
            break;
        case USART_TIC_State_ready:	//����� ������! ���� ��������� � TIC'��!
            TIC_State = USART_TIC_State_receiving_TICstatus;	//��� ������ ��������
            TIC_request_Status();
            TIC_timer.CTRLA = TC_500kHz;			//��������� � ����� �����
            break;
        case USART_TIC_State_receiving_TICstatus:	//TIC �� �������� ��������! ��� ������ �� ����� �� �����!
			cli();
			if (TIC_Online)
			{
				TIC_HVE_offlineCount += 1;
				if(TIC_HVE_offlineCount > 2)
				{
					//TIC �� ����� �� ����� � � ������ ���! ���-�� �����! ��������� ����!
					pin_iHVE_high;						//d��������� �������
					Flags.iHVE = 1;						//��������� HVE
					Flags.PRGE = 0;						//������� ���������� ���������� ���������
					TIC_Online = 0;						//��� ����� � TIC'��
					transmit_3rytes(TOKEN_ASYNCHRO, CRITICAL_ERROR_TIC_HVE_error_noResponse, TIC_MEM_length);
				}
			}
			sei();
			Errors_USART_TIC.HVE_TimeOut = 1;		//�������� � �������
			TIC_State = USART_TIC_State_ready;		
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
            transmit_3rytes(TOKEN_ASYNCHRO, INTERNAL_ERROR_TIC_State, TIC_State);
            break;
    }
    sei_TIC;
}
static void ISR_PC_timer(void)
{
	PC_receiving = 0;	//��������� ����
    PC_timer.CTRLA = TC_Off;
    PC_timer.CNT = 0;
}
//-----------------------------------------�������------------------------------------------------
//USART PC
byte receive(void)
{
	return *USART_PC.DATA;
}
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
        case COMMAND_TIC_retransmit:				TIC_retransmit();
            break;
        case COMMAND_checkCommandStack:				transmit_2rytes(COMMAND_checkCommandStack, MC_CommandStack);
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
		case COMMAND_SPI_get_AllVoltages:			SPI_get_AllVoltages();
			break;
		case COMMAND_TIC_getStatus:					TIC_getStatus();
			break;
		case COMMAND_COUNTERS_delayedStart:			COUNTERS_delayedStart();
			break;
		case COMMAND_TIC_setJitter:					TIC_setJitter(PC_MEM[1],PC_MEM[2]);
			break;
        default: transmit_3rytes(TOKEN_ASYNCHRO, ERROR_DECODER_wrongCommand, PC_MEM[0]);
    }
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
		//RTC.PER = 3277;
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
		RTC_MeasurePrescaler = PC_MEM[1];
        //������
        while (RTC.STATUS != 0)
        {
            //��� ���� ����� ����� ���������� � ��������� RTC
        }
        RTC.CTRL = PC_MEM[1];
        asm(
            "LDI R16, 0x08		\n\t"//TCC0:��� ������ ������� 0 = 0x08
            "LDI R17, 0x0A		\n\t"//TCD0:��� ������ ������� 2 = 0x0A
            "LDI R18, 0x0C		\n\t"//TCE0:��� ������ ������� 4 = 0x0C
            //"LDS R19, 0x2078	\n\t"//RTC: ����� RTC_Prescaler  = 0x2078
            "STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
            "STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
            "STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
            //"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_MeasurePrescaler(@0x2078)
        );
        //�����
        transmit_2rytes(COMMAND_COUNTERS_start, RTC_Status);
        RTC_setStatus_busy;
    }
    else
    {
        //���������! �������� �������!
        transmit_2rytes(COMMAND_COUNTERS_start, RTC_Status);
    }
    sei();
}
void COUNTERS_sendResults(void)
{
    //�������: ������� ���������� ����� �� ��, ���� �����
    //������: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><CO�_M[1]><CO�_M[0]><COC_OVF[1]><COC_OVF[0]><CO�_M[1]><CO�_M[0]><RTC_ElapsedTime[1]><RTC_ElapsedTime[0]><RTC_MeasurePrescaler>
    uint8_t wDATA[17];
    wDATA[0] = COMMAND_COUNTERS_sendResults;
    wDATA[1] = RTC_Status;
    if (RTC_Status == RTC_Status_ready)
    {
	    wDATA[2] = (COA_OVF >> 8);
	    wDATA[3] = COA_OVF;
	    wDATA[4] = (COA_Measurment >> 8);
	    wDATA[5] = COA_Measurment;
	    wDATA[6] = (COB_OVF >> 8);
	    wDATA[7] = COB_OVF;
	    wDATA[8] = (COB_Measurment >> 8);
	    wDATA[9] = COB_Measurment;
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
        transmit_2rytes(COMMAND_COUNTERS_stop, RTC_Status);
        RTC_setStatus_stopped;
    }
    else
    {
        transmit_2rytes(COMMAND_COUNTERS_stop, RTC_Status);
    }
}
void COUNTERS_delayedStart(void)
{
	//�������: ����� ����������:
	//			- �����������		(DAC_Scaner - 2 ����� (25))
	//			- ��������������	(DAC_Scaner - 1 ����� (24))
	//			- �����������		(DAC_Condensator - 1 ����� (24))
	//		   ��������� ��������
	//		   ����� �������� ���������� ����
	//�������� �����:	<33><SV.1><SV.2><PSV.1><PSV.2><C.1><C.2>
	//					<delay_ms.1><delay_ms.2>
	//					<measure_ms.1><measure_ms.2><cs>
	//
	//�����:	<33><cs>
	
	//PC_MEM_length = 11;
	//PC_MEM[0] = 33;	/* <cm> */		PC_MEM[4] = 4;	/* <PSV.2> */	PC_MEM[8] = 96; /* <D_ms.2> */	
	//PC_MEM[1] = 56;	/* <SV.1> */	PC_MEM[5] = 65;	/* <C.1> */		PC_MEM[9] = 0; /* <M_ms.1> */	
	//PC_MEM[2] = 4;	/* <SV.2> */	PC_MEM[6] = 4;	/* <C.2> */		PC_MEM[10] = 50; /* <M_ms.2> */
	//PC_MEM[3] = 43;	/* <PSV.1> */	PC_MEM[7] = 1;	/* <D_ms.1> */	
	
	//�������� ����
	PC_MEM[1] = (PC_MEM[1] << 2) + (PC_MEM[2] >> 6);
	PC_MEM[2] = (PC_MEM[2] << 2);
	PC_MEM[3] = (PC_MEM[3] << 2) + (PC_MEM[4] >> 6);
	PC_MEM[4] = (PC_MEM[4] << 2);
	PC_MEM[5] = (PC_MEM[5] << 2) + (PC_MEM[6] >> 6);
	PC_MEM[6] = (PC_MEM[6] << 2);
	//0:0
	//���������� ��������� ��� SPI ���������
	if ((RTC_Status != RTC_Status_busy))
	{
		SPI_confDACs_toDoubleRef();		//����������� �������� �����
		uint8_t spi_data[] = {25, PC_MEM[1], PC_MEM[2]}; //������������� �����������
		spi_select_device(&SPIC, &DAC_Scaner);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Scaner);
		spi_data[0] = 24;	//������������� �� ������ �����
		spi_data[1] = PC_MEM[3];//������������� ��������������
		spi_data[2] = PC_MEM[4];
		spi_select_device(&SPIC, &DAC_Scaner);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Scaner);
		spi_data[1] = PC_MEM[5];//������������� �����������
		spi_data[2] = PC_MEM[6];
		spi_select_device(&SPIC, &DAC_Condensator);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Condensator);
		//12101:378 ��
		//��������� �������� � ����� ����������
		uint16_t delay_time = ((uint16_t)PC_MEM[7] << 8) + PC_MEM[8];//����� � �������������(�� 1 �� �� 65535 �)
		uint32_t dummy = COUNTERS_msToTicks(delay_time);
		if (dummy == 0) { transmit_2rytes(COMMAND_COUNTERS_delayedStart, 0); return; }
		RTC_DelayPrescaler = (byte)(dummy >> 16);
		RTC_DelayPeriod = (uint16_t)dummy;
		uint16_t measure_time = ((uint16_t)PC_MEM[9] << 8) + PC_MEM[10];
		dummy = COUNTERS_msToTicks(measure_time);
		if (dummy == 0) { transmit_2rytes(COMMAND_COUNTERS_delayedStart, 0); return; }
		RTC_MeasurePrescaler = (byte)(dummy >> 16);
		RTC_MeasurePeriod = (uint16_t)dummy;
		//18212:569 ���
		//���������� ��������
		RTC_delay = 1;
		//����������
		while (RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.PER = RTC_DelayPeriod;
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
		RTC.CTRL = RTC_DelayPrescaler;
		transmit_2rytes(COMMAND_COUNTERS_delayedStart, 1);
		RTC_setStatus_busy;
	}
	else
	{
		//���������! �������� �������!
		transmit_2rytes(COMMAND_COUNTERS_delayedStart, 2);
	}
}
uint32_t COUNTERS_msToTicks(uint16_t ms)
{
	//�������: ��������� ������������ � ���� � ������������
	//����������: <x><prescaler><ticks_1><ticks_2>
	//0:0
	uint32_t answer = 0;
	byte prescaler = 0;
	float frequency = 0;
	uint16_t ticks = 0;
	if		((ms > 0)		&&(ms < 2000))	{ prescaler = 1; frequency = 32.768; }
	else if ((ms >= 2000)	&&(ms < 4000))	{ prescaler = 2; frequency = 16.384; }
	else if ((ms >= 4000)	&&(ms < 16000))	{ prescaler = 3; frequency = 4.096; }
	else if ((ms >= 16000)	&&(ms < 32000))	{ prescaler = 4; frequency = 2.048; }
	else if ((ms >= 32000)	&&(ms <= 65535)){ prescaler = 5; frequency = 0.512; }
	else { return 0; }//�������� ��������
	ticks = ms * frequency;//����������: 2846:88,9 ���
	answer = (uint32_t)(((uint32_t)prescaler << 16) + ticks);
	//2931:91,6 ���
	return answer;
}
//TIC
void TIC_decode(void)
{
    //�������: ���������� ����� ���� �� ������ HVE {"?V902"+'\r'}
    //���������: ����� TIC'� ������ ���� �����: 
    //TIC_MEM_length = 22;
	//TIC_MEM[0] = 61;			TIC_MEM[8] = 48;  /*B*/		TIC_MEM[16] = 52; /*R1*/
	//TIC_MEM[1] = 86;			TIC_MEM[9] = 59;			TIC_MEM[17] = 59;
	//TIC_MEM[2] = 57;			TIC_MEM[10] = 48; /*G1*/	TIC_MEM[18] = 52; /*R2*/
	//TIC_MEM[3] = 48;			TIC_MEM[11] = 59;			TIC_MEM[19] = 59;
	//TIC_MEM[4] = 50;			TIC_MEM[12] = 48; /*G2*/	TIC_MEM[20] = 48; /*R3*/
	//TIC_MEM[5] = 32;			TIC_MEM[13] = 59;			TIC_MEM[21] = 13;
	//TIC_MEM[6] = 52; /*�*/	TIC_MEM[14] = 48; /*G3*/
	//TIC_MEM[7] = 59;			TIC_MEM[15] = 59;
    //����:   61 86 57 48 50  32  ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?       59 ?        13
    //������: =  V  9  0  2  <sp> T  ;  B  ;  G1 ;  G2 ;  G3 ;  R1 ;  R2 ;  R3 ;  AlertID ;  Priority <\r>
    //�����:  0  1  2  3  4   5   6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22      23 24       25
    //0:0
	if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 48) && (TIC_MEM[4] == 50) && (TIC_MEM[5] == 32) && (TIC_MEM[TIC_MEM_length - 1] == 13))
    {
		//54:1,69 ���
		TIC_Online = 1;			//������ ������ ����������, ������ TIC �� �����
		//�������� ��������� TIC'a � ����� ������� (������ �������)
		cli();
		TIC_Status[0] = COMMAND_TIC_getStatus;
		TIC_Status_length = TIC_MEM_length - 6;
		for (byte i = 0; i < TIC_Status_length; i++) { TIC_Status[i+1] = TIC_MEM[i+6]; }
		sei();
		//820:25,6 ���
        //����������� ASCII ����� � �����
        byte Turbo, R1, R2, R3;	//������������ ��� ���������
		byte semicolon_counter = 0;
		//���� ';' � ���������� �� R1
		for (byte i = 6; i < TIC_MEM_length; i++)
		{
			if (TIC_MEM[i] == 59)
			{
				semicolon_counter++;
			}
			else
			{
				switch(semicolon_counter)
				{
					case 0: //����� �������� � ������� (�). �� ����� ����� ��������� 0...7
						Turbo = TIC_decode_ASCII(TIC_MEM[i]);
						break;
					case 5: //����� ������ ��� ����. ��� ����� ����� ��������� 0...4
						R1 = TIC_decode_ASCII(TIC_MEM[i]);
						break;
					case 6: R2 = TIC_decode_ASCII(TIC_MEM[i]);
						break;
					case 7: R3 = TIC_decode_ASCII(TIC_MEM[i]);
						i = TIC_MEM_length;
						break;
					default: break;
				}
			}
		}
		//1645:51,4 ���
		if ((Turbo <= 7) && (R1 <= 4 )&& (R2 <= 4) && (R3 <= 4))
		{
			//��� ������� ���������. ��� ������
			byte Turbo_approval = 0;
			byte R2_approval = 0;
			//������� �������:
			//		Stopped					0
			//		Starting Delay			1
			//		Accelerating			5
			//		Runnging				4
			//		StorringShortDelay		2
			//		StoppingNormalDelay		3
			//		FaultBraking			6
			//		Braking					7
			switch(Turbo)
			{
				case 4: Turbo_approval = 1;
					break;
				default: Turbo_approval = 0;
					break;
			}
			//������� ����:
			//		OffState				0
			//		OffGoingOnState			1
			//		OnGoingOffShutdownState	2
			//		OnGoingOffNormalState	3
			//		OnState					4
			switch(R1)
			{
				case 4: pin_SEMV1_high;	//��������� ������� "����� <-> ������"
					break;
				default: 
					if ((PORTD.OUT & 2) >> 1)
					{
						TIC_R1_jitter++;
						if(TIC_R1_jitter >= TIC_R1_jitter_MAX)
						{
							pin_SEMV1_low;	//��������� ������� "����� <-> ������"
							transmit_3rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_R1_off, R1);
							TIC_R1_jitter = 0;
						}
					}
					break;
			}
			switch(R2)
			{
				case 4: R2_approval = 1;
					break;
				default: R2_approval = 0;
					break;
			}
			/*switch(R3)
			{
				�� ����� ���������� TIC ��������� ������ 200 ��!!!
				����� ����� ���� ������� ������� 1
				case 1: //������! �������� �� �����!
					pin_iHVE_high;	//��������� ������� ����������
					//������� TIC'� �������� ��!
					//					!   C   9   3   3  sp   0  '\r'
					byte Message[8] = {33, 67, 57, 51, 51, 32, 48, 13};
					for (byte i = 0; i < 8; i++) { usart_putchar(USART_TIC, Message[i]); }
					Flags.iHVE = 1;
					Flags.PRGE = 0;
					transmit_2rytes(TOKEN_ASYNCHRO, LAM_TIC_Crash);
					TIC_HVE_Error_sent = 0;	//�� ������, ����� �������� LAM ��� ������
					TIC_HVE_offlineCount = 0;//�������� ������� ������
					return;						//����� �� ������
				default: 
				break;
			}*/
			//1715:53,6 ���
			if (Turbo_approval && R2_approval)
			{
				//� ������ � ���� ���� ����� �� ��������� ������� ���������� �� � �� ����� ����� �����
				if(Flags.iHVE == 1)
				{
					//�� ���� iHVE ������� ��������� - �� ��������� ������ DC-DC 24-12. �������� ���������� ���. �� ������ ����...
					Flags.iHVE = 0;	//��������� �������!
					transmit_2rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_approve); //������� ����������
				}
			} 
			else
			{
				if(Flags.iHVE == 0)
				{
					TIC_disprove_jitter++;
					if(TIC_disprove_jitter >= TIC_disprove_jitter_MAX)
					{
						//�� ���� iHVE ������ ��������� - �� ��������� ������ DC-DC 24-12. ������� ���������� ����! ������ ����...
						//��������� �������!
						pin_iHVE_high;
						Flags.iHVE = 1;
						Flags.PRGE = 0;
						byte TiR = ((Turbo << 4) & 0xf0) + (R2 & 0x0f); //����� ������ � �������� ��������� ������� � ���� 0xTR; T �� 0 �� 7, R �� 0 �� 4
						transmit_3rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_disapprove, TiR);
						TIC_disprove_jitter = 0;
					}
				}
			}
			TIC_HVE_Error_sent = 0;	//�� ������, ����� �������� LAM ��� ������
			TIC_HVE_offlineCount = 0;//�������� ������� ������
			return;
			//8218:257 ���
		}
	}
    //���� � ������������ ����� ���-�� �� ��� �� ���������� ����.
    //�������� �������� HVE. TIC ���-�� ������.
	TIC_HVE_offlineCount++;
	if(TIC_HVE_offlineCount > 2)
	{
		//TIC ����� ���� � � ������ ���! ���-�� �����! ��������� ����!
		pin_iHVE_high;						//��������� HVE
		Flags.iHVE = 1;
		Flags.PRGE = 0;
		if(TIC_HVE_Error_sent == 0)
		{
			transmit_3rytes(TOKEN_ASYNCHRO, CRITICAL_ERROR_TIC_HVE_error_decode, TIC_MEM_length);
			TIC_HVE_Error_sent = 1;
		}
		TIC_HVE_offlineCount = 0;//�������� ������� ������
	}
    Errors_USART_TIC.HVE_error = 1;
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
}
void TIC_request_Status(void)
{
    //�������: ����������� � TIC'� ������
	//cli();
    for (uint8_t i = 0; i < 6; i++) { usart_putchar(USART_TIC, TIC_HVE_Message[i]); }	//����������
	//sei();
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
void TIC_getStatus(void)
{
	//�������: �������� ���������� ��������� ���������� ������ � TIC'�
	transmit(TIC_Status,TIC_Status_length);
}
void TIC_setJitter(byte Jitter_TIC_disprove, byte Jitter_TIC_R1_off)
{
	//�������: ������������� ���������� ���������� ��������
	TIC_R1_jitter = 0;
	TIC_disprove_jitter = 0;
	TIC_R1_jitter_MAX = Jitter_TIC_R1_off;
	TIC_disprove_jitter_MAX = Jitter_TIC_disprove;
	byte aswDATA[1] = {COMMAND_TIC_setJitter};
	transmit(aswDATA, 1);
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
            transmit_3rytes(TOKEN_ASYNCHRO, INTERNAL_ERROR_SPI, DEVICE_Number);
            return;
    }
	SPI_confDACs_toDoubleRef();					//����� ����������� ��������
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
void SPI_confDACs_toDoubleRef(void)
{
	uint8_t SPI_DATA[] = {0,0,0};
	//DPS + PSIS DAC'� AD5328R (�������� � ������ ��������) - ������� ��������
	SPI_DATA[0] = AD5328R_confHbyte;
	SPI_DATA[1] = AD5328R_confLbyte;
	spi_select_device(&SPIC, &DAC_Inlet);
	spi_select_device(&SPIC, &DAC_Detector);
	spi_select_device(&SPIC, &DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC, &DAC_Inlet);
	spi_deselect_device(&SPIC, &DAC_Detector);
	spi_deselect_device(&SPIC, &DAC_IonSource);
	//MSV DAC'� AD5643R (����������� � ������) - ������� ��������
	SPI_DATA[0] = AD5643R_confHbyte;
	SPI_DATA[1] = AD5643R_confMbyte;
	SPI_DATA[2] = AD5643R_confLbyte;
	spi_select_device(&SPIC, &DAC_Condensator);
	spi_select_device(&SPIC, &DAC_Scaner);
	spi_write_packet(&SPIC, SPI_DATA, 3);
	spi_deselect_device(&SPIC, &DAC_Condensator);
	spi_deselect_device(&SPIC, &DAC_Scaner);
}
void SPI_get_AllVoltages(void)
{
	//�������: ���������� ���������� ���������� ����� �� ���� ���������� ���� ADC.
	//������ ������:	PSIS	- PortA.1 - 0x600.2		- ������ �������� (4 ������: ���� �������, ���������, �������� 1, �������� 2)
	//					DPS		- PortA.5 - 0x600.32		- �������� (3 ������: �������� 1, �������� 2, �������� 3)
	//					MSV		- PortA.2 - 0x600.4	- ����������� (4 ������: �����������, ��������������, ����������� "+", ����������� "-")
	//					PSInl	- PortE.0 - 0x680.1		- ���������� (2 ������: ����������, �����������)
	//				------------------------------
	//					�����:	-	13 �������
	//�������:  - ������� ��� ����������
	//			- ��������� ����������
	//���������: ����� ����� ����������� ������ �������. ��� ��� �� "����" ���� ����������� � ��� ��� ����� ������
	//			������ ��� ������������ ��� ���, �� �� ����� "���������-��������" �������� �� ��������, � ������ ���������� ������.
	byte answer[28];
	answer[0] = COMMAND_SPI_get_AllVoltages;
	/*�� ��������
	//0:0 �
	byte Channels[5] = {16, 131, 135, 139,  143};	//[0] - ��������� ���� ��� ��� (���������� ��� ����);
	//												[1...4] - ������ ���� ��� ��� ���������� ����� ������ (������������� �����)
	#define Order_length  14
	//������� ������ ������� (��������� - "��������")
	//								IS_EC						IS_IV						IS_F1						IS_F2	 					D_1	 						D_2	 						D_3	 						C+		 					C-		 					SV		 					PSV	 						Inl	 						Heat	 					DUMMY
	byte Order[Order_length] =		{1,							2,							3,							4,      					1,      					2,      					3,		 					1,		 					2,		 					3,		 					4,		 					1,		 					2,		 					1					};	
	uint16_t Port[Order_length] =	{(uint16_t)&PORTA.OUTTGL, (uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTE.OUTTGL,	(uint16_t)&PORTE.OUTTGL,	(uint16_t)&PORTA.OUTTGL	};
	byte Pin[Order_length] =		{2,			 				2,							2,							2,	     					32,	 						32,	 						32,	 						4,		 					4,		 					4,		 					4,		 					1,	     					1,		 					1					};
	byte ch = 0;					//��������� ������
	byte a = 1;				//��������� ���������� �������� ������� answer
	//������ ������, ����� �� ������
	//409:12,78 ���
	for (byte i = 0; i < Order_length; i++)
	{
		pin_iRDUN_low;						//�������� ������
		DWR(Port[ch], Pin[ch]);		//�������� �� ������ ���������� ���
		SPIC.DATA = Channels[Order[i]];		//������ (������ ����)
		spi_wait_packet(&SPIC);
		//while (!(SPIC.STATUS & 0x80)) { }	//1032:32,25 ���//�������� ������
		pin_iRDUN_high;						//�������� ������
		answer[a++] = SPIC.DATA;			//������
		pin_iRDUN_low;						//�������� ������
		SPIC.DATA = Channels[0];			//������ (��������� ����)
		spi_wait_packet(&SPIC);
		//while (!(SPIC.STATUS & 0x80)) { }	//�������� ������
		pin_iRDUN_high;						//�������� ������
		DWR(Port[ch], Pin[ch]);				//������� ������ ����������� ���
		answer[a++] = SPIC.DATA;			//������
		ch++;
		if(i == 0) { a = 1; ch = 0; }		//���������, ���� ��� ��� ������ ���, ����� ������ ����������
		//2676:83,63 ��� (���� ����)
	}
	//*/
	///*��������
	byte SPI_rDATA[2] = {0,0};
	byte sdata[] = {131, 16}; //IS_EC
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_IonSource);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_IonSource);
	answer[1] = SPI_rDATA[0];
	answer[2] = SPI_rDATA[1];
	//IS_IV
	sdata[0] = 135;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_IonSource);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_IonSource);
	answer[3] = SPI_rDATA[0];
	answer[4] = SPI_rDATA[1];
	//IS_F1
	sdata[0] = 139;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_IonSource);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_IonSource);
	answer[5] = SPI_rDATA[0];
	answer[6] = SPI_rDATA[1];
	//IS_F2
	sdata[0] = 143;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_IonSource);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_IonSource);
	answer[7] = SPI_rDATA[0];
	answer[8] = SPI_rDATA[1];
	//DPS_D1
	sdata[0] = 131;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_Detector);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Detector);
	answer[9] = SPI_rDATA[0];
	answer[10] = SPI_rDATA[1];
	//DPS_D2
	sdata[0] = 135;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_Detector);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Detector);
	answer[11] = SPI_rDATA[0];
	answer[12] = SPI_rDATA[1];
	//DPS_D3
	sdata[0] = 139;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_Detector);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Detector);
	answer[13] = SPI_rDATA[0];
	answer[14] = SPI_rDATA[1];
	//MSV_C+
	sdata[0] = 131;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_MSV);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_MSV);
	answer[15] = SPI_rDATA[0];
	answer[16] = SPI_rDATA[1];
	//MSV_C-
	sdata[0] = 135;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_MSV);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_MSV);
	answer[17] = SPI_rDATA[0];
	answer[18] = SPI_rDATA[1];
	//MSV_SV
	sdata[0] = 139;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_MSV);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_MSV);
	answer[19] = SPI_rDATA[0];
	answer[20] = SPI_rDATA[1];
	//MSV_PSV
	sdata[0] = 143;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_MSV);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_MSV);
	answer[21] = SPI_rDATA[0];
	answer[22] = SPI_rDATA[1];
	//PSInl_Inl
	sdata[0] = 131;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_Inlet);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Inlet);
	answer[23] = SPI_rDATA[0];
	answer[24] = SPI_rDATA[1];
	//PSInl_Heat
	sdata[0] = 135;
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//������ ��� �����
	spi_deselect_device(&SPIC, &ADC_Inlet);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Inlet);
	answer[25] = SPI_rDATA[0];
	answer[26] = SPI_rDATA[1];
	//*/
	//���������� ���� ������ <HVE_port				|		iHVE		|		PRGE		|		iEDCD				|		SEMV1			|		SEMV2				|		SEMV3			|	SPUMP>
	answer[27] =			((PORTC.OUT & 8) << 4) + (Flags.iHVE << 6) + (Flags.PRGE << 5) + ((PORTA.OUT & 128) >> 3) + ((PORTD.OUT & 2) << 2) + ((PORTD.OUT & 16) >> 2) + ((PORTD.OUT & 32) >> 4) + (PORTD.OUT & 1);
	//32133:1 �� - �������
	transmit(answer, 28);					//���������� ����� ����������				
	//113819:3,5 �� - �������
}
//�����
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
    //uint8_t DATA[] = {COMMAND_Flags_HVE, ((PORTC.OUT & 8) >> 3), Flags.iHVE, TIC_HVE_onGauge, (TIC_HVE_onLevel >> 8), TIC_HVE_onLevel, TIC_HVE_offGauge, (TIC_HVE_offLevel >> 8), TIC_HVE_offLevel, TIC_timer.CTRLA };
    //transmit(DATA, 10);
	uint8_t DATA[] = {COMMAND_Flags_HVE, ((PORTC.OUT & 8) >> 3), Flags.iHVE};
	transmit(DATA, 3);
}
void checkFlag_PRGE(void)
{
    //�������: ���������� ��� ������������� PRGE
    //�����: <Command><getOrSet>
    //					<0>\<1> - �������������
    //					<any_else> - �����������
	//*
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
                transmit_2rytes(COMMAND_Flags_PRGE, 254);	//TIC ���������.
                return;
            }
            return;
        default: //������
            break;
    }
    transmit_2rytes(COMMAND_Flags_PRGE, Flags.PRGE);
	//*/
	//����� ������� ������������
	/*
		switch (PC_MEM[1])
		{
			case 0: //��������� � ����
				Flags.PRGE = 0;			//��������� ����������
				PORTC.OUTSET = 8;
				break;
			case 1: //�������� ����������
				Flags.PRGE = 1;
				MC_Tasks.turnOnHVE = 1;//������ ��������� �������� DAC'�� ����� ������� ������
				break;
			return;
			default: //������
			break;
		}
		transmit_2bytes(COMMAND_Flags_PRGE, Flags.PRGE);
	//*/
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
    transmit_2rytes(COMMAND_Flags_EDCD, ((PORTA.OUT & 128) >> 7));
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
    transmit_2rytes(COMMAND_Flags_SEMV1, ((PORTD.OUT & 2) >> 1));
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
    transmit_2rytes(COMMAND_Flags_SEMV2, ((PORTD.OUT & 16) >> 4));
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
    transmit_2rytes(COMMAND_Flags_SEMV3, ((PORTD.OUT & 32) >> 5));
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
    transmit_2rytes(COMMAND_Flags_SPUMP, (PORTD.OUT & 1));
}
void turnOn_HV(void)
{
	cli_PC;
    pin_iHVE_low; //�������� DC-DC 24-12
    cpu_delay_ms(2000, 32000000); //iHVE �������� �������� ������������ ����, ������� ���� ��������.
    //������� ���������� �������� - ������������� DAC�
	uint8_t SPI_DATA[] = {0,0,0};
    //DPS + PSIS DAC'� AD5328R (�������� � ������ ��������) - ������� ��������
    SPI_DATA[0] = AD5328R_confHbyte;
    SPI_DATA[1] = AD5328R_confLbyte;
    spi_select_device(&SPIC, &DAC_Inlet);
    spi_select_device(&SPIC, &DAC_Detector);
    spi_select_device(&SPIC, &DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC, &DAC_Inlet);
    spi_deselect_device(&SPIC, &DAC_Detector);
    spi_deselect_device(&SPIC, &DAC_IonSource);
    //��������� �� ������������������ ��������!
    //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (��� �������)
    SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_EC;
    SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_EC;
    spi_select_device(&SPIC,&DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC,&DAC_IonSource);
    //PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (���������)
    SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_IV;
    SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_IV;
    spi_select_device(&SPIC,&DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC,&DAC_IonSource);
    //MSV DAC'� AD5643R (����������� � ������) - ������� ��������
    SPI_DATA[0] = AD5643R_confHbyte;
	SPI_DATA[1] = AD5643R_confMbyte;
    SPI_DATA[2] = AD5643R_confLbyte;
	spi_select_device(&SPIC, &DAC_Condensator);
    spi_select_device(&SPIC, &DAC_Scaner);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Condensator);
    spi_deselect_device(&SPIC, &DAC_Scaner);
    //MSV DAC AD5643R (�����������) - ��������� ���������� �� ������ ������
	SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_C;
	SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_C;
	SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_C;
    spi_select_device(&SPIC, &DAC_Condensator);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Condensator);
    //MSV DAC AD5643R (������ (�����������)) - ��������� ���������� �� ������ ������
    SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_S;
    SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_S;
    SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_S;
    spi_select_device(&SPIC, &DAC_Scaner);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Scaner);
	//MSV DAC AD5643R (������ (��������������)) - ��������� ���������� �� ������ ������
	SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_PS;
	SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_PS;
	SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_PS;
	spi_select_device(&SPIC, &DAC_Scaner);
	spi_write_packet(&SPIC, SPI_DATA, 3);
	spi_deselect_device(&SPIC, &DAC_Scaner);
	//PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (�������� 1)
	SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_F1;
	SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_F1;
	spi_select_device(&SPIC,&DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC,&DAC_IonSource);
	//PSIS DAC AD5328R (������ ��������) - ��������� ���������� �� ������ ������ (�������� 2)
	SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_F2;
	SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_F2;
	spi_select_device(&SPIC,&DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC,&DAC_IonSource);
	/*
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
    spi_deselect_device(&SPIC,&DAC_IonSource);
	//*/
    MC_Tasks.turnOnHVE = 0;						//������� ������
    cpu_delay_ms(2000, 32000000);
	transmit_2rytes(TOKEN_ASYNCHRO,LAM_SPI_conf_done);
	sei_PC;
}
/*
void fun(void)
{
	byte incoming[10];
	incoming[0] = COMMAND_TIC_retransmit;
	incoming[1] = 20;
	incoming[2] = 0;
	incoming[3] = 58;
	incoming[4] = 40;
	incoming[5] = 23;
	incoming[6] = 56;
	
	incoming[7] = 74;
	incoming[8] = 234;
	incoming[9] = 125;
	incoming[10] = 148;
	incoming[11] = 4;
	incoming[12] = 29;
	incoming[13] = 249;
	
	incoming[14] = 1;
	incoming[15] = 42;
	incoming[16] = 2;
	incoming[17] = 5;
	incoming[18] = door;
	incoming[19] = 4;
	incoming[20] = 16;
	incoming[21] = 13;
	byte incoming_length = 22;
	incoming[incoming_length] = calcCheckSum(incoming,incoming_length);
	incoming_length++;
	PC_buffer = key;
	receiving();
	for (byte i = 0; i < incoming_length; i++)
	{
		PC_buffer = incoming[i];
		receiving();
	}
	PC_buffer = lock;
	receiving();
}*/
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
    USART_PC_init;						//���������� USART � ����������
    USART_TIC_init;						//���������� USART � ��������
    //�������� �������������
    pointer_Flags = &Flags;
    pointer_Errors_USART_PC = &Errors_USART_PC;
    pointer_Errors_USART_TIC = &Errors_USART_TIC;
    updateFlags();
    RTC_setStatus_ready;
    Flags.iHVE = 1; //��������� ������� ����������, �� ��� ��� ���� �� TIC'� �� ����� ����������
    Flags.PRGE = 0;	//���������� o������� ��������� ������� ���������� (��� ���������� �� TIC ������������ ���� ������ �����������!)
    //������ PC
    PC_timer.PER = 65535;				//524�� �� 125���
    PC_timer.CNT = 0;
    PC_timer.CTRLA = TC_Off;			//�s������� �� ������
    //T����� TIC
    TIC_State = USART_TIC_State_ready;	//��������� USART_PC � ����� ��������
    TIC_timer.PER = 25000;				//200�� �� 125���
    TIC_timer.CNT = 0;
	TIC_timer.CTRLA = TC_125kHz;		//�������� TIC'������ ������ �������� �������
    sei();								//��������� ����������
    //15627:488 ���//������������� ���������
    while (1)
    {
        if (MC_Tasks.turnOnHVE) { turnOn_HV(); }
		if((MC_Tasks.retransmit)&&(TIC_State != USART_TIC_State_receiving_TICstatus))
		{
			//������ ������������ ��������� �� TIC
			cli_TIC;
			TIC_timer.CTRLA = TC_Off;
			TIC_timer.CNT = 0;
			TIC_State = USART_TIC_State_receiving;	//��������� � ����� ����� �� ����������
			for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//�������� �� ��� ������ ���������
			//cli();
			for (uint8_t i = 0; i < PC_MEM_length - 1; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//����������
			//sei();
			TIC_timer.CTRLA = TC_500kHz;			//��������� ������ � ������ �����
			sei_TIC;
			MC_Tasks.retransmit = 0;				//������� ������
		}
    }
}
//-----------------------------------------�������------------------------------------------------
//��� = 3600� - 80�
//�1 =4000� - 
//�2 = 4000�
//��� - 2000
//��� - 2000*3/4
//����������� - 500
//-----------------------------------------THE END------------------------------------------------