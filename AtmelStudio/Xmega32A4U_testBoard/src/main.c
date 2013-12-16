//================================================================================================
//========================���������� � ���������������� ����������������==========================
//================================================================================================
//������ ����������� � conf_board.h
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
#define version										124
#define birthday									20131216
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
#define USART_State_HVEwaiting						4		//USART (TIC) ��� ������ �� TIC'a �� �������������� ������
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
uint8_t TIC_timer_TimeOut = 5;							//10 ������
uint8_t TIC_timer_time = 0;								//������ ������� ����� � ������
uint8_t TIC_MEM[100];									//100 ���� ������ ��� ����� ������ �� TIC
uint8_t TIC_MEM_length = 0;								//����� ����������� � TIC_MEM ������ ������.
uint8_t TIC_buf = 0;									//����� �����. �������� ����� �������� ���� (���� ���)
uint8_t TIC_State = 0;									//��������� ������ USART_TIC
uint8_t TIC_waitForAnswer = 0;							//bool'�� 0 - �� ��� ������ �� TIC'�, 1 - ��� �� HVE
uint8_t TIC_HVE_Message[6] = {63,86,57,49,0,13};		//char'� ��������� �� ������ �������� {?V91<NUL><\r>}
uint8_t TIC_HVE_onGauge = 0;							//��������� char ������ ������� (�������)
uint8_t TIC_HVE_onLevel[4] = {0,0,0,0};					//char'� ������ ���������� (�������)
uint8_t TIC_HVE_offGauge = 0;							//��������� char ������ ������� (���������)
uint8_t TIC_HVE_offLevel[4] = {0,0,0,0};				//char'� ������ ���������� (���������)
//		���������
uint8_t  RTC_Status = RTC_Status_ready;					//��������� ��������
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
	uint8_t setDACs			:1;
	uint8_t noTasks1		:1;
	uint8_t checkHVE		:1;
	uint8_t noTasks3		:1;
	uint8_t noTasks4		:1;
	uint8_t noTasks5		:1;
	uint8_t noTasks6		:1;
	uint8_t noTasks7		:1;
};
struct struct_MC_Tasks MC_Tasks = {0,0,0,0,0,0,0,0};
struct struct_Errors_USART_PC
{
	uint8_t LOCKisLost				:1;
	uint8_t TooShortPacket			:1;
	uint8_t TooFast					:1;
	uint8_t Silence					:1;
	uint8_t Noise					:1;
	uint8_t noError5				:1;
	uint8_t noError6				:1;
	uint8_t noError7				:1;
};
struct struct_Errors_USART_PC Errors_USART_PC = {0,0,0,0,0,0,0,0};
struct struct_Flags
{
	uint8_t SPUMP		:1;
	uint8_t SEMV3		:1;
	uint8_t SEMV2		:1;
	uint8_t SEMV1		:1;
	uint8_t iEDCD		:1;
	uint8_t PRGE		:1;
	uint8_t iHVE		:1;
	uint8_t checkOrSet	:1;	
}Flags;
//USART
static usart_rs232_options_t USART_PC_OPTIONS = {
	.baudrate = USART_PC_BAUDRATE,
	.charlength = USART_PC_CHAR_LENGTH,
	.paritytype = USART_PC_PARITY,
	.stopbits = USART_PC_STOP_BIT
};
static usart_rs232_options_t USART_TIC_OPTIONS = {
	.baudrate = USART_TIC_BAUDRATE,
	.charlength = USART_TIC_CHAR_LENGTH,
	.paritytype = USART_TIC_PARITY,
	.stopbits = USART_TIC_STOP_BIT
};

struct spi_device DAC_IonSource = {
	.id = pin_iWRIS
};
struct spi_device DAC_Detector = {
	.id = pin_iWRVD
};
struct spi_device DAC_Inlet = {
	.id = pin_iWINL
};
struct spi_device DAC_Scaner = {
	.id = pin_iWRSV
};
struct spi_device DAC_Condensator = {
	.id = pin_iWRCV
};
struct spi_device ADC_IonSource = {
	.id = pin_iECIS
};
struct spi_device ADC_Detector = {
	.id = pin_iECVD
};
struct spi_device ADC_Inlet = {
	.id = pin_iECINL
};
struct spi_device ADC_MSV = {
	.id = pin_iECSV
};
//ADC � ������������ ��� �� ��� � � �������
//-----------------------------------------���������----------------------------------------------
uint8_t *pointer_MC_Tasks;
uint8_t *pointer_Errors_USART_PC;
uint8_t *pointer_Flags;
//------------------------------------���������� �������------------------------------------------
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length);
void transmit(uint8_t DATA[],uint8_t DATA_length);
void transmit_byte(uint8_t DATA);
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2);
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3);
void MC_transmit_Birthday(void);
void MC_transmit_CPUfreq(void);
void MC_reset(void);
void COUNTERS_start(void);
void COUNTERS_sendResults(void);
void COUNTERS_stop(void);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);
void decode(void);
void TIC_retransmit(void);
void TIC_transmit(void);
void TIC_monitor(void);
void SPI_send(uint8_t DEVICE_Number);
void checkFlags(void);
void updateFlags(void);
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
	if((PC_State == USART_State_receiving)||(PC_State == USART_State_ending))
	{
		PC_timer.CNT = 0;						//�������� ���� ��������
		PC_MEM[PC_MEM_length] = PC_buf;			//��������� ����
		PC_MEM_length++;						//����������� ������� �������� ������
		PC_State = USART_State_receiving;		//������������, ��� ���� ���� �� ������
		if (PC_buf == COMMAND_LOCK){PC_State = USART_State_ending;}	//���� �������� ������, ��������� ��������� ����
	}
	else if(PC_State == USART_State_ready)
	{
		if(PC_buf == COMMAND_KEY)
		{
			//������ ����!
			PC_State = USART_State_receiving;	//��������� � ����� �����
			PC_timer.CNT = 0;					//�������� ������
			PC_timer.CTRLA = TC_32MHz;			//��������� ������ �� 4��.
			PC_MEM_length = 0;					//�������� ������� �������� ������
		}
		else { Errors_USART_PC.Noise = 1; }		//���-�� ��������� �� �����
	}
	else if(PC_State == USART_State_decoding) { Errors_USART_PC.TooFast = 1; } //�� �� �������� ���������� �������
	sei_PC;
}
ISR(USARTE0_RXC_vect)
{
	//����������: ������ ���� ������ �� ����� USART �� TIC �����������
	//�������: �������������� ����� ��� ������� ��� ������, ���������� ������������ ��������
	//��������� ����
	TIC_buf = *USART_TIC.DATA;//->3(95��)
	cli_TIC;
	switch(TIC_State)
	{
		case USART_State_receiving:		//���� � ������ �����
			TIC_timer.CNT = 0;						//�������� ���� ��������
			TIC_MEM[TIC_MEM_length] = TIC_buf;		//��������� ����
			TIC_MEM_length++;						//����������� ������� �������� ������
			break;
		case USART_State_HVEwaiting:	//���� � ������ �������� ������ �� TIC'a �� HVE
			break;
		default:
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
	 //����������: ������� ����� ������ �� TIC � ������ �� ����� (�� �������)
	 switch(TIC_State)
	 {
		case USART_State_receiving:
			break;
		case USART_State_ready:
			break;
		case USART_State_HVEwaiting:
			break;
		default:
			break; 
	 }
 }
 static void ISR_PC_timer(void)
 {
	 //����������: ��� ��� ������������ �� ������� (32��� �� 1024), �� ���������� ����������� ������ 2 �������.
	 //������� ����� ����������� ����������� ���� ������. ����� ���������� ��� �������� ��������, ����� ����������
	 //���� � ������ ����� � ����������� � ��������� ������� ����������.
	 //�� ����� ����� ������ ������ ������ ��������� �����.
	 cli_PC;
	 switch(PC_State)
	 {
		case USART_State_receiving: //�� �� ������� ���������� ��������! �������� ��������! ����� �����!
			PC_timer.CTRLA = TC_31kHz;		//��������� � ����� ������
			PC_timer.CNT = 0;
			PC_timer_time = 0;
			PC_State = USART_State_ready;	//��� ������ ��������
			break;
		case USART_State_ending: //���� ������� ��������! ����� ������������. ��������� ������ �������
			if(PC_MEM_length > 2)
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
				gpio_set_pin_high(pin_iHVE);	//��������� DC-DC 24-12
				Flags.PRGE = 0;					//��������� PRGE �� ���� ���������
				Errors_USART_PC.Silence = 1;	//�������� ������ � �����
				PC_timer.CTRLA = TC_Off;		//��������� ������ ������, �� ���������� ������ �����
				PC_timer.CNT = 0;
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
	switch(PC_MEM[0])																					
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
		case COMMAND_retransmitToTIC:				TIC_transmit();									
		break;																								
		case COMMAND_checkCommandStack:				transmit_2bytes(COMMAND_checkCommandStack,MC_CommandStack);
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
		default: transmit_3bytes(TOCKEN_ERROR, ERROR_Decoder, PC_MEM[0]);
	}
}
//USART PC
void transmit(uint8_t DATA[],uint8_t DATA_length)
{
	//�������: �������� �������� ���������� ������, ������� �� �� ��������� � � ����������� ������
	//���������: �����: ':<response><data><CS>\r' 
	//					   ':' - ������ ������
	//					   '<data>' - ����� ������ <<response><attached_data>>
	//							<response> - ������, ��� �������, �� ������� ��������
	//							<attached_data> - ���� ������. �� ����� �� ���� (������)
	//					   '<CS>' - ����������� �����
	//					   '\r' - ����� ��������
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	for (uint8_t i = 0; i < DATA_length; i++) { usart_putchar(USART_PC,DATA[i]); }	//<data>
	usart_putchar(USART_PC,calcCheckSum(DATA,DATA_length + 1));						//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_byte(uint8_t DATA)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA);													//<data>
	usart_putchar(USART_PC, (uint8_t)(256 - DATA));									//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA_1);
	usart_putchar(USART_PC,DATA_2);													//<data>
	usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2));						//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA_1);
	usart_putchar(USART_PC,DATA_2);													//<data>
	usart_putchar(USART_PC,DATA_3);
	usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));				//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
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
 	uint8_t data[] = {COMMAND_MC_get_CPUfreq,(uint8_t)freq,(uint8_t)(freq >> 8),(uint8_t)(freq >> 16),(uint8_t)(freq >> 24)};
	transmit(data,5);
}
void MC_transmit_Birthday(void)
{
	uint8_t data[] = {COMMAND_MC_get_Birthday, (uint8_t)MC_birthday,(uint8_t)(MC_birthday >> 8),(uint8_t)(MC_birthday>>16),(uint8_t)(MC_birthday>>24)};
	transmit(data,5);
}
void MC_reset(void)
{
	//�������: ������������� ��
	//��������: ����� ������� ������ �������� ��������� ������� ����� �������������, ������� �������
	cpu_irq_disable();
	uint8_t data[] = {COMMAND_MC_reset};
	transmit(data,1);
	RST.CTRL = 1;
}
//COUNTERS
void COUNTERS_start(void)
{
	//�������: ��������� �������� �� ����������� �����
	//������: <Command><RTC_PRE><RTC_PER[1]><RTC_PER[0]>
	cli();
	if((RTC_Status != RTC_Status_busy))
	{
		//����������
		while(RTC.STATUS != 0)
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
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CNT = 0;
		COA.CNT = 0;
		COB.CNT = 0;
		COC.CNT = 0;
		//������
		while(RTC.STATUS != 0)
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
	//������: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><CO�_M[1]><CO�_M[0]><COC_OVF[1]><COC_OVF[0]><CO�_M[1]><CO�_M[0]>
	uint8_t wDATA[15];
	wDATA[0] = COMMAND_COUNTERS_sendResults;
	wDATA[1] = RTC_Status;
	if(RTC_Status == RTC_Status_ready)
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
	}
	transmit(wDATA,14);
}

void COUNTERS_stop(void)
{
	//�������: �������������� ��������� ���������
	if (RTC_Status == RTC_Status_busy)
	{
		while(RTC.STATUS != 0)
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
		while(RTC.STATUS != 0)
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
void TIC_retransmit(void)
{
	//�������: ������������ ������� �� TIC, ���� ��� ������ HVE, ���� ����� HVE ���� - ��� ������ �� TIC'� �� �����, � ����� ������ �������������.
	cli_PC;
	for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i-1] = PC_MEM[i]; }	//�������� �� ��� ������ ���������
	for (uint8_t i = 0; i < TIC_MEM_length; i++) { usart_putchar(USART_TIC,TIC_MEM[i]); }	//����������
	sei_PC;
}
void TIC_transmit(void)
{
	//�������: ��������������� ������� TIC ������
	//PC_MEM[0] - ������� �� ��, ��������� � PC_MEM[0] ��, ��� ���������� ������� TIC'�
	//�������� �� ��� ������ ���������
	//MC_transmit_Version;
	//cli();
	//for (uint8_t i = 1; i < PC_MEM_length; i++)
	//{
	//	TIC_MEM[i-1] = PC_MEM[i];
	//}
	//TIC_MEM_Length = PC_MEM_length - 1;
	//sei();
	//������������� �� ���� ����������
	//for (uint8_t i = 0; i < TIC_MEM_Length; i++)
	//{
	//	usart_putchar(USART_TIC,TIC_MEM[i]);
	//}
	cli_TIC;
	usart_putchar(USART_TIC,33);
	usart_putchar(USART_TIC,83);
	usart_putchar(USART_TIC,57);
	usart_putchar(USART_TIC,50);
	usart_putchar(USART_TIC,53);
	usart_putchar(USART_TIC,32);
	usart_putchar(USART_TIC,49);
	usart_putchar(USART_TIC,53);
	usart_putchar(USART_TIC,13);
	sei_TIC;
	//��� ������ �� TIC
	//���������� ����� �� ��
}
void TIC_monitor(void)
{
	//�������: ��������� TIC �� ������� �������� �������
	//���� HVE ���������, �� ���������� � ���������������� �������
	if(Flags.iHVE = 1)
	{
		//���� HVE ���������
		TIC_HVE_Message[4] = TIC_HVE_onGauge;
		TIC_transmit();//TIC_HVE_Message, 6);
		//�������� ������
	}
	else
	{
		TIC_HVE_Message[4] = TIC_HVE_offGauge;
		TIC_transmit();//TIC_HVE_Message, 6);
		//�������� ������
	}
}
//������
bool EVSYS_SetEventSource( uint8_t eventChannel, EVSYS_CHMUX_t eventSource )
{
	volatile uint8_t * chMux;

	/*  Check if channel is valid and set the pointer offset for the selected
	 *  channel and assign the eventSource value.
	 */
	if (eventChannel < 8) {
		chMux = &EVSYS.CH0MUX + eventChannel;
		*chMux = eventSource;

		return true;
	} else {
		return false;
	}
}
bool EVSYS_SetEventChannelFilter( uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient )
{
	/*  Check if channel is valid and set the pointer offset for the selected
	 *  channel and assign the configuration value.
	 */
	if (eventChannel < 8) {

		volatile uint8_t * chCtrl;
		chCtrl = &EVSYS.CH0CTRL + eventChannel;
		*chCtrl = filterCoefficient;

		return true;
	} else {
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
	struct spi_device SPI_DEVICE = {
		.id = 0
	};
	switch(DEVICE_Number)
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
	uint8_t SPI_rDATA[] = {0,0};				//������ SPI ��� ����� ������ (��� �����)
	//���� ���������� DAC AD5643R �� �������� ������ �� ��� ���������, ����������� � �������
	if(DAC_is_AD5643R)
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
	if(DEVICE_is_DAC)
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
	spi_read_packet(&SPIC,SPI_rDATA,2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &SPI_DEVICE);
	//������ ����� �� �� �� USART
	uint8_t aswDATA[] = {PC_MEM[0],SPI_rDATA[0],SPI_rDATA[1]};
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
			if(Flags.iHVE == 0)
			{
				//�� ������ ���������� ���� �� iHVE (������ ��������� ��������� ������ DC-DC 24-12)
				Flags.PRGE = 1;	//�������� ��� �����
				MC_Tasks.setDACs = 1;//������ ��������� �������� DAC'�� ����� ������� ������
				Flags.checkOrSet = 1; //��� ������ ��������
			}
		}
		else 
		{
			gpio_set_pin_high(pin_iHVE);	//��������� DC-DC 24-12
			Flags.PRGE = 0;			//�������� ���������
			Flags.checkOrSet = 1;	//��� ������ ��������
		}
	}
    receivedFlag = ((PC_MEM[1] & 16) >> 4);
    if (Flags.iEDCD != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_iEDCD);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_iEDCD);Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 8) >> 3);
    if (Flags.SEMV1 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV1);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV1);Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 4) >> 2);
    if (Flags.SEMV2 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV2);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV2);Flags.checkOrSet = 1;}}
    receivedFlag = ((PC_MEM[1] & 2) >> 1);
    if (Flags.SEMV3 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV3);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV3);Flags.checkOrSet = 1;}}
    receivedFlag = PC_MEM[1] & 1;
    if (Flags.SPUMP != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SPUMP);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SPUMP);Flags.checkOrSet = 1;}}
    updateFlags();
	transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
    if (MC_Tasks.setDACs)
    {
		gpio_set_pin_low(pin_iHVE); //�������� DC-DC 24-12
        cpu_delay_ms(2000,32000000); //iHVE �������� �������� ������������ ����, ������� ���� ��������.
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
        MC_Tasks.setDACs = 0;
    }
    return;
}
void updateFlags(void)
{
	//�������: �� ����������� �������� ���� ������ � �������� �� � ���� Flags
	//Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
	//���������� ��������� � TIC'� ��������� iHVE
	Flags.iEDCD = (PORTA.OUT & 128) >> 7;
	Flags.SEMV1 = (PORTD.OUT & 2  ) >> 1;
	Flags.SEMV2 = (PORTD.OUT & 16 ) >> 4;
	Flags.SEMV3 = (PORTD.OUT & 32 ) >> 5;
	Flags.SPUMP = PORTD.OUT & 1;
}
//-------------------------------------������ ���������-------------------------------------------
int main(void)
{
	confPORTs;							//������������� ����� (HVE ��� � ������ �������)
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
    updateFlags();
	RTC_setStatus_ready;
	Flags.iHVE = 1; //��������� ������� ����������, �� ��� ��� ���� �� TIC �� ����� ����������
	Flags.PRGE = 0;	//���������� ������� ��������� ������� ���������� (��� ���������� �� TIC ������������ ���� ������ �����������!)
	//�������				
	PC_timer.PER = 65535;
	PC_timer.CNT = 0;
	PC_timer.CTRLA = TC_31kHz;			//�������� �� ������ �� ����� ������
	PC_State = USART_State_ready;		//��������� USART_PC � ����� ������
	cpu_irq_enable();					//��������� ����������	
	
	//������������� ���������
	while (1) 
	{
		
	}
}
//-----------------------------------------�������------------------------------------------------
/*
*
*/
/*
*/
//-----------------------------------------THE END------------------------------------------------
