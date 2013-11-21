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
#define FATAL_transmit_ERROR			while(1){transmit(255,254);								\
											delay_ms(50);}
//��
#define version										86
#define birthday									20131121
//��������
#define RTC_Status_notSet							0		//�������� �� ��������
#define RTC_Status_ready							1		//�������� ����� � ������
#define RTC_Status_stopped							2		//�������� ��� ������������� ����������
#define RTC_Status_busy								3		//�������� ��� �������
#define RTC_Status_delayed							4		//RTC ������� ��������
#define RTC_setStatus_notSet		RTC_Status =	RTC_Status_notSet
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
#define RTC_setStatus_delayed		RTC_Status =	RTC_Status_delayed
//SPI
#define AD5643R_confHbyte							56
#define AD5643R_confMbyte							0
#define AD5643R_confLbyte							1
#define AD5643R_startVoltage_Hbyte					24	
#define AD5643R_startVoltage_Mbyte					127
#define AD5643R_startVoltage_Lbyte					252
//������� ��������
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//��������� ���������� PSIS EC (���� �������)
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			82
//��������� ���������� PSIS IV,F1,F2 (���������, ��������)
#define AD5328R_startVoltage_Hbyte_PSIS_IV			44
#define AD5328R_startVoltage_Lbyte_PSIS_IV			205

//----------------------------------------����������----------------------------------------------
//	���������������
uint8_t MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t CommandStack = 0;

uint8_t MC_Status = 0;
//		USART
uint8_t USART_MEM[100];						//100 ���� ������ ��� ����� ������ USART
uint8_t USART_MEM_length = 0;
uint16_t MC_receiving_limit = 65535;
uint16_t USART_receiving_time = 0;
uint8_t MC_reciving_error = 0;
uint8_t USART_buf = 0;
bool	USART_recieving = false;			//����� ������������ ��� �������� ������ � USART_MEM_length
uint8_t USART_MEM[100];						//10 ���� ������ ��� ����� ������ USART
uint8_t USART_PACKET_length = 0;
uint8_t USART_MEM_CheckSum = 0;
//		���������
uint8_t RTC_Status = RTC_Status_ready;					//��������� ��������
uint8_t RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//������������ RTC �� ����� ���������
uint8_t RTC_DelayPrescaler = RTC_PRESCALER_OFF_gc;		//������������ RTC �� ����� ��������
uint16_t RTC_MeasurePeriod = 0;							//������ RTC �� ����� ���������� ���������
uint16_t RTC_DelayPeriod = 0;							//������ RTC �� ����� ��������
uint32_t COA_PreviousMeasurment = 0;					//��������� ��������� �������� COA
uint8_t	COA_PreviousOVF = 0;							//���������� ������������ �������� ��� � ��������� ���������
uint8_t	COA_OVF = 0;									//���������� ������������ �������� ��� � ������� ���������
uint32_t COB_PreviousMeasurment = 0;					//��������� ��������� �������� COB
uint8_t	COB_PreviousOVF = 0;							//���������� ������������ �������� ��� � ��������� ���������
uint8_t	COB_OVF = 0;									//���������� ������������ �������� ��� � ������� ���������
uint32_t COC_PreviousMeasurment = 0;					//��������� ��������� �������� COC
uint8_t	COC_PreviousOVF = 0;							//���������� ������������ �������� ��� � ��������� ���������
uint8_t	COC_OVF = 0;									//���������� ������������ �������� ��� � ������� ���������
//-----------------------------------------���������----------------------------------------------
//������� ����
struct _MC_Tasks
{
	uint8_t setDACs			:1;
	uint8_t doNextMeasure	:1;
	uint8_t MeasuredDataWasSended		:1;
	uint8_t NextMeasureSettingsWasChanged		:1;
	uint8_t MeasureDataWasOVERWRITTEN		:1;
	uint8_t Decrypt		:1;
	uint8_t noTasks6		:1;
	uint8_t noTasks7		:1;
};
struct _MC_Tasks MC_Tasks = {0,0,0,0,0,0,0,0};
struct pinFlags
{
	uint8_t SPUMP		:1;
	uint8_t SEMV3		:1;
	uint8_t SEMV2		:1;
	uint8_t SEMV1		:1;
	uint8_t iEDCD		:1;
	uint8_t iHVE		:1;
	uint8_t dummy		:1;
	uint8_t checkOrSet	:1;	
}Flags;
//USART
static usart_rs232_options_t USART_COMP_OPTIONS = {
	.baudrate = USART_COMP_BAUDRATE,
	.charlength = USART_COMP_CHAR_LENGTH,
	.paritytype = USART_COMP_PARITY,
	.stopbits = USART_COMP_STOP_BIT
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
uint8_t *pointer_Flags;
//------------------------------------���������� �������------------------------------------------
//
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length);
void transmit(uint8_t DATA[],uint8_t DATA_length);
void transmit_byte(uint8_t DATA);
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2);
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3);
void setThemAll(void);
void MC_transmit_Birthday(void);
void MC_transmit_CPUfreq(void);
void MC_reset(void);
void COUNTERS_start(void);
void COUNTERS_stop(void);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);
void ERROR_ASYNCHR(void);
void decode(void);
void TIC_transmit(void);
void SPI_send(uint8_t DEVICE_Number);
void checkFlags(void);
void updateFlags(void);
//------------------------------------������� ����������------------------------------------------
ISR(USARTD0_RXC_vect)
{
	//����������:
	//�������: <KEY><PACKET_LENGTH><DATA[...]<CS><LOCK>
	//			PACKET_LENGTH = ������ ����� �������
	cli();
	//25(0,78���) - �������� ���� (�� ���������)
	//42(1,31���) - ������ ���� (������ ���� ������)
	//38(1,19���) - ������! �� �� �������� ���������� �������
	//43(1,34���) - ������ ��Ȩ��! ������ ������ ������ �����������
	//35(1,09���) - ������ ������ ������ (������ ���� ������)
	//59(1,84���) - ���� ����� ������
	//68(2,13���) - ���� ����� ����������� �����
	//74(2,31���) - ������ ��Ȩ��! ����������� ���������! USART_MEM_LENGTH > USART_MAIL_Length!
	//94(2,94���) - ������ ������ (��������� ����) ���� ������� ��������!
	//86(2,69���) - ������! �� ��� ������� ������!

	//��������� ����, ��� �� ��� ������
	USART_buf = *USART_COMP.DATA;//->3(95��)
	//���� � ������ �����
	if(USART_receiving_time > 0)
	{
		if (USART_PACKET_length == 0)
		{
			//��� ���� ����� ������
			USART_PACKET_length = USART_buf;
			if (USART_PACKET_length < 5)
			{
				//������ ��Ȩ��! ������ ������ ������ �����������
				//���� ����� ������� ������ ����� �������� �� ��������� ������� (5 ������), �� ���������� ����, ��������� ���� ������
				MC_reciving_error = 1;
				USART_receiving_time = 0;
			}
		}
		else if(USART_MEM_length < (USART_PACKET_length - 4))
		{
			//��� ����� ������, ��������� � ������
			USART_MEM[USART_MEM_length] = USART_buf;
			//����� �������� �� ������!
			USART_MEM_length++;
		}
		else if(USART_MEM_length == (USART_PACKET_length - 4))
		{
			//���� ���� ����������� ����� ������
			USART_MEM_CheckSum = USART_buf;
			USART_MEM_length++;//��������� ����������,����� ������� �� ������ ��� ���������� ��������� �����
		}
		else if(USART_MEM_length == (USART_PACKET_length - 3))
		{
			//���� ���� ������
			if (USART_buf == COMMAND_LOCK)
			{
				//���� ������� ��������!
				USART_MEM_length--;//���������� �������� �������� ������ �������� ������
				USART_receiving_time = 0;//���������� ����
				MC_Tasks.Decrypt = 1;//������ ������ - ������������
			}
			else
			{
				//������ ��Ȩ��! �������� ������, � �������� �����-���
				MC_reciving_error = 2;
				USART_receiving_time = 0;
			}
		}
		else
		{
			//������ ��Ȩ��! ����������� ���������! USART_MEM_LENGTH > USART_MAIL_Length!
			MC_reciving_error = 4;
			USART_receiving_time = 0;
		}
	}
	else if(USART_buf == COMMAND_KEY)
	{
		//������ ����!
		if(MC_Tasks.Decrypt == 0)
		{
			//���� ���������� ������� ��� �� ���������
			USART_receiving_time = 1;//��������� � ����� �����
			USART_MEM_length = 0;//�������� ������� �������� ������
			USART_PACKET_length = 0;//�������� ������ ������ (��������� � ����� ����� ������ ������)
		}
		else
		{
			//������! �� �� �������� ���������� �������
			MC_reciving_error = 255;
			USART_receiving_time = 0;
		}
	}
	sei();
}
ISR(USARTE0_RXC_vect)
{
	//����������: ������ ���� ������ �� ����� USART �� TIC �����������
	//�������: �������������� ����� ��� ������� ��� ������, ���������� ������������ ��������
	//��������� ����
	
	
}
ISR(RTC_OVF_vect)
{
	//����������: ��������� ��� ��������� ����� ������� ��������
	//�������: ��������� ��������� ���������
	if (RTC_Status == RTC_Status_busy)
	{
		asm(
		"LDI R16, 0x00			\n\t"//���� ��� �������� ���� ��������� (������ � �������� ��������)
		"STS 0x0800, R16		\n\t"//����� TCC0.CTRLA = 0x0800 <- ����
		"STS 0x0900, R16		\n\t"//����� TCD0.CTRLA = 0x0900 <- ����
		"STS 0x0A00, R16		\n\t"//����� TCE0.CTRLA = 0x0A00 <-	����
		"STS 0x0840, R16		\n\t"//����� TCC1.CTRLA = 0x0840 <-	����
		"STS 0x0940, R16		\n\t"//����� TCD1.CTRLA = 0x0940 <-	����
		);
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		if(MC_Tasks.doNextMeasure == 1)
		{
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.CNT = 0;
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.PER = RTC_DelayPeriod;
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.CTRL = RTC_DelayPrescaler;
			RTC_setStatus_delayed;
			MC_Tasks.doNextMeasure = 0;
		}
		else
		{
			RTC_setStatus_ready;
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.CNT = 0;
			transmit_2bytes(COMMAND_COUNTERS_LookAtMe,RTC_Status); //LAM ready
		}
		if (MC_Tasks.MeasuredDataWasSended == 0)
		{
			MC_Tasks.MeasureDataWasOVERWRITTEN = 1;
			RTC_setStatus_stopped;
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.CTRL = RTC_PRESCALER_OFF_gc;
			while(RTC.STATUS != 0)
			{
				//��� ���� ����� ����� ���������� � ��������� RTC
			}
			RTC.CNT = 0;
		}
		else
		{
			//��������� ����������
			COA_PreviousMeasurment = (((uint32_t)TCC1.CNT) << 16) + TCC0.CNT;
			COB_PreviousMeasurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
			COC_PreviousMeasurment = TCE0.CNT;
			COA_PreviousOVF = COA_OVF;
			COB_PreviousOVF = COB_OVF;
			COC_PreviousOVF = COC_OVF;
			//���������� ���������� ���������
			COA_OVF = 0;
			COB_OVF = 0;
			COC_OVF = 0;
			TCC0.CNT = 0;
			TCC1.CNT = 0;
			TCD0.CNT = 0;
			TCD1.CNT = 0;
			TCE0.CNT = 0;
		}
	}
	else if (RTC_Status == RTC_Status_delayed)
	{
		//�������� ����� ���������
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.CNT = 0;
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		asm(
		"LDI R16, 0x08		\n\t"//TCC0:��� ������ ������� 0 = 0x08
		"LDI R17, 0x0A		\n\t"//TCD0:��� ������ ������� 2 = 0x0A
		"LDI R18, 0x0C		\n\t"//TCE0:��� ������ ������� 4 = 0x0C
		//"LDS R19, 0x205F	\n\t"//RTC: ����� RTC_Prescaler  = 0x205F
		"LDI R20, 0x09		\n\t"//TCC1:��� ������ ������� 1 = 0x09
		"LDI R21, 0x0B		\n\t"//TCD1:��� ������ ������� 3 = 0x0B
		"STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
		"STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
		"STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
		//"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_Prescaler(@0x205F)
		"STS 0x0840, R20	\n\t"//����� TCC1.CTRLA = 0x0840 <- ����� ������� 1
		"STS 0x0940, R21	\n\t"//����� TCD1.CTRLA = 0x0940 <- ����� ������� 3
		);
		RTC.CTRL = RTC_MeasurePrescaler;
		RTC_setStatus_busy;
		MC_Tasks.MeasuredDataWasSended = 0;
		transmit_2bytes(COMMAND_COUNTERS_LookAtMe,RTC_Status);//LAM delayed
	}
	else
	{
		transmit_2bytes(ERROR_Token,123);
	}
}
static void ISR_TCC1(void)
 {
	 COA_OVF++;
 }
static void ISR_TCD1(void)
 {
	 COB_OVF++;
 }
static void ISR_TCE0(void)
 {
	 COC_OVF++;
 }
//-----------------------------------------�������------------------------------------------------
void decode(void)
{
	//�������: �������������� �������
	switch(USART_MEM[0])																					
	{		
		case COMMAND_COUNTERS_set_All:				setThemAll();
		break;																								
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
		case COMMAND_COUNTERS_stop:					COUNTERS_stop();										
		break;																								
		case COMMAND_MC_reset:						MC_reset();												
		break;																							
		case COMMAND_retransmitToTIC:				TIC_transmit();									
		break;																								
		case COMMAND_checkCommandStack:				transmit_2bytes(COMMAND_checkCommandStack,CommandStack);
		break;																								
		case COMMAND_IonSource_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_IonSource);		
		break;																								
		case COMMAND_Detector_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Detector);			
		break;																								
		case COMMAND_Inlet_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet);			
		break;																								
		case COMMAND_Heater_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet);			
		break;																								
		case COMMAND_Scaner_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Scaner);			
		break;																								
		case COMMAND_Condensator_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Condensator);		
		break;																								
		case COMMAND_IonSource_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource);		
		break;																								
		case COMMAND_Detector_DV1_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector);			
		break;																								
		case COMMAND_Detector_DV2_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector);			
		break;																								
		case COMMAND_Detector_DV3_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector);			
		break;																								
		case COMMAND_Inlet_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet);			
		break;																								
		case COMMAND_Heater_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet);			
		break;																								
		case COMMAND_MSV_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_MSV);				
		break;																								
		case COMMAND_Flags_set: 					checkFlags();									
		break;																								
		default: transmit_3bytes(ERROR_Token, ERROR_Decoder, USART_MEM[0]);
	}
}
void setThemAll(void)
{
	//������������: ���� ������� �������������� ������, ��� �� ���� ����������
	//�������� ������:
	//			[0] - <Command37 - setThemAll
	//			[1] - <Continue>										- ���� "������� ��������� ���������"
	//				<0>		- �� ������ ��������� ���������
	//				<1>		- ������� ��������� ���������
	//� ����� ���������?
	//			[2] - <RTC_MeasurePrescaler>							- ����� ������������ ���������� ���������
	//				<0>		- �� ������ �� ������������, �� ������
	//				<1...7> - ��������� �������� ������������
	//			[3..4] - <RTC_MeasurePeriod>							- ������ ����� ���������� ���������
	//				<0...65535> - ��������� �������� ������� (0 � 1 ����� �� ������������)
	//			[5] - <RTC_DelayPrescaler>								- ����� ������������ ��������� ��������
	//				<0>		- �� ������ �� ������������, �� ������
	//				<1...7> - ��������� �������� ������������
	//			[6..7] - <RTC_DelayPeriod>								- ������ ����� ��������� ��������
	//				<1...65535> - ��������� �������� ������� (0 � 1 ����� �� ������������)
	cli();
	//�������� - ����� �� �� �� ����� ��������� (busy)? ���� ready ��� notSet �� ������� �� ����������, �� SPI DAC ���������� �����
	if (RTC_Status != RTC_Status_delayed)
	{
		//������������� ��������� RTC �� ��������� ���������
		if (USART_MEM[1] == 1)
		{
			//<Continue> ����� ������� �������������������, �������� (���� SPI ���������, �������\������������� ������� ���� ��)
			MC_Tasks.doNextMeasure = 1;
		}
		if (USART_MEM[2] != 0)
		{
			RTC_MeasurePrescaler = USART_MEM[2];
			RTC_MeasurePeriod = (((uint16_t)USART_MEM[3]) << 8) + USART_MEM[4];
		}
		if (USART_MEM[5] != 0)
		{
			RTC_DelayPrescaler = USART_MEM[5];
			RTC_DelayPeriod = (((uint16_t)USART_MEM[6]) << 8) + USART_MEM[7];
		}
		//��� ������� �� SPI DAC (����� ��������� �� ����� ��������)
		//������� ��������� SPI ADC
		//������� ���������� ��������� ��������� � ADC (���������� ����������, �������� ���� ��� ��������� � ���������� ���)
		if (MC_Tasks.MeasureDataWasOVERWRITTEN != 1)
		{
			uint8_t data[] = 
			{
				COMMAND_COUNTERS_set_All, MC_Status, RTC_Status,
				COA_PreviousOVF, (COA_PreviousMeasurment >> 24), (COA_PreviousMeasurment >> 16), (COA_PreviousMeasurment >> 8), COA_PreviousMeasurment,
				COB_PreviousOVF, (COB_PreviousMeasurment >> 24), (COB_PreviousMeasurment >> 16), (COB_PreviousMeasurment >> 8), COB_PreviousMeasurment,
				COC_PreviousOVF, (COC_PreviousMeasurment >> 8), COC_PreviousMeasurment
			};
			switch (RTC_Status)
			{
				case RTC_Status_busy:
				case RTC_Status_ready:
				transmit(data, 16);
				MC_Tasks.MeasuredDataWasSended = 1;
				break;
				default:
				//transmit_2bytes(COMMAND_COUNTERS_get_Count,RTC_Status);
				break;
			}
		}
		else
		{
			//transmit_2bytes(COMMAND_COUNTERS_get_Count,(255 - RTC_Status));
		}
	}
	else
	{
		//�� � ��������!!! �� ���� �������, ���� ������� ����!
	}
	sei();
	//���������� ������:
	//			[0] - <Response>										- ������
	//				<37> - COMMAND_MEASURE_set_All
	//� ����� ���������?
	//			[1] - <MC_Status>										- ������ ��
	//			[2] - <RTC_Status>										- ������ RTC
	//			[3] - <COA_PreciousOVF							- ���������� ��������� ����� (���_ovf)
	//			[4...5] - <COA_PreviousMeasurment>						- ���������� ��������� ����� (���_CNT)
	//			2 : <D_PRE><D_PER:2>									- ������ ����� ��������� ��������
	//			8 : <IS_EC : 2><IS_IV : 2><IS_F1 : 2><IS_F2 : 2>		- ���������� �� DAC PSIS
	//			6 : <D_DV1 : 2><D_DV2 : 2><D_DV3 : 2>					- ���������� �� DAC DPS
	//			9 : <S_PV : 3><S_SV : 3><C_V : 3>						- ���������� �� DAC MSV
	//			8 : <Inl_Inl : 2><Inl_Heater : 2>						- ���������� �� DAC Inlet
	//			8 : <IS_EC : 2><IS_IV : 2><IS_F1 : 2><IS_F2 : 2>		- ���������� �� ADC PSIS
	//			6 : <D_DV1 : 2><D_DV2 : 2><D_DV3 : 2>					- ���������� �� ADC DPS
	//			8 : <S_PV : 2><S_SV : 2><C_Vp : 2><C_Vn : 2>			- ���������� �� ADC MSV
	//			8 : <Inl_Inl : 2><Inl_Heater : 2>						- ���������� �� ADC Inlet
}
//USART COMP
void transmit(uint8_t DATA[],uint8_t DATA_length)
{
	//�������: �������� �������� ���������� ������, ������� �� �� ��������� � � ����������� ������
	//���������: �����: ':<PACKET_LENGTH><response><data><CS>\r' 
	//					   ':' - ������ ������
	//					   '<PACKET_LENGTH>' - ����� ������
	//					   '<data>' - ����� ������ <<response><attached_data>>
	//							<response> - ������, ��� �������, �� ������� ��������
	//							<attached_data> - ���� ������. �� ����� �� ���� (������)
	//					   '<CS>' - ����������� �����
	//					   '\r' - ����� ��������
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	usart_putchar(USART_COMP,DATA_length + 4);						//'<PACKET_LENGTH>'
	for (uint8_t i = 0; i < DATA_length; i++)
	{
		usart_putchar(USART_COMP,DATA[i]);							//<data>
	}
	usart_putchar(USART_COMP,calcCheckSum(DATA,DATA_length + 1));	//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);							//'\r'
}
void transmit_byte(uint8_t DATA)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	usart_putchar(USART_COMP, 5);									//'<PACKET_LENGTH>'
	usart_putchar(USART_COMP,DATA);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA));				//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);							//'\r'
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_COMP,COMMAND_KEY);								//':'
	usart_putchar(USART_COMP,6);										//'<PACKET_LENGTH>'
	usart_putchar(USART_COMP,DATA_1);
	usart_putchar(USART_COMP,DATA_2);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2));		//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);								//'\r'
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//������������: �������� ������ ����� (������)
	usart_putchar(USART_COMP,COMMAND_KEY);									//':'
	usart_putchar(USART_COMP,7);											//'<PACKET_LENGTH>'
	usart_putchar(USART_COMP,DATA_1);
	usart_putchar(USART_COMP,DATA_2);										//<data>
	usart_putchar(USART_COMP,DATA_3);
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));	//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);									//'\r'
}
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length)
{
	//�������: ��������� ����������� ����� �������� ������
	uint8_t CheckSum = 0;
	for (uint8_t i = 0; i < data_length - 1; i++)
	{
		CheckSum -= data[i];
	}
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
void ERROR_ASYNCHR(void)
{
	//showMeByte(255);
	uint8_t ERROR[] = {25,24,15};
	while(1)
	{
		transmit(ERROR,3);
	}
}
//COUNTERS
void COUNTERS_start(void)
{
	//�������: ��������� �������� �� ����������� ���������� �����
	
	//��������� ������� ������� �� ����� busy -> ������� ������ MC_Tasks.doNextMeasure. ���� ����� ��� - ������ �������� ����� ����� ���������
	
	/*if (RTC_Status != RTC_Status_busy)
	{	
		COA_OVF = 0;
		COB_OVF = 0;
		COC_OVF = 0;
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
		RTC.CNT = 0;
		RTC.PER = RTC_MeasurePeriod;
		asm(	
			"LDI R16, 0x08		\n\t"//TCC0:��� ������ ������� 0 = 0x08
			"LDI R17, 0x0A		\n\t"//TCD0:��� ������ ������� 2 = 0x0A
			"LDI R18, 0x0C		\n\t"//TCE0:��� ������ ������� 4 = 0x0C
			"LDS R19, 0x205F	\n\t"//RTC: ����� RTC_Prescaler  = 0x205F
			"LDI R20, 0x09		\n\t"//TCC1:��� ������ ������� 1 = 0x09
			"LDI R21, 0x0B		\n\t"//TCD1:��� ������ ������� 3 = 0x0B
			"STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
			"STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
			"STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
			"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_Prescaler(@0x205F)	
			"STS 0x0840, R20	\n\t"//����� TCC1.CTRLA = 0x0840 <- ����� ������� 1
			"STS 0x0940, R21	\n\t"//����� TCD1.CTRLA = 0x0940 <- ����� ������� 3
		);
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
		RTC_setStatus_busy;
	}
	else
	{
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
	}*/
	if((RTC_Status != RTC_Status_busy)&&(RTC_Status != RTC_Status_delayed))
	{
		//����������
		while(RTC.STATUS != 0)
		{
			//��� ���� ����� ����� ���������� � ��������� RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		MC_Tasks.MeasureDataWasOVERWRITTEN = 0;
		MC_Tasks.MeasuredDataWasSended = 1; //����� ���� ������� (������ �����)
		COA_OVF = 0;
		COB_OVF = 0;
		COC_OVF = 0;
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
		MC_Tasks.doNextMeasure = 0;
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
		"LDI R20, 0x09		\n\t"//TCC1:��� ������ ������� 1 = 0x09
		"LDI R21, 0x0B		\n\t"//TCD1:��� ������ ������� 3 = 0x0B
		"STS 0x0800, R16 	\n\t"//����� TCC0.CTRLA = 0x0800 <- ����� ������� 0
		"STS 0x0900, R17	\n\t"//����� TCD0.CTRLA = 0x0900 <- ����� ������� 2
		"STS 0x0A00, R18	\n\t"//����� TCE0.CTRLA = 0x0A00 <- ����� ������� 4
		//"STS 0x0400, R19	\n\t"//����� RTC.CTRL   = 0x0400 <- ������������ RTC_Prescaler(@0x205F)
		"STS 0x0840, R20	\n\t"//����� TCC1.CTRLA = 0x0840 <- ����� ������� 1
		"STS 0x0940, R21	\n\t"//����� TCD1.CTRLA = 0x0940 <- ����� ������� 3
		);
		RTC.CTRL =  RTC_MeasurePrescaler;
		//�����
		//transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
		RTC_setStatus_busy;
	}
	else
	{
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
	}
}
void COUNTERS_stop(void)
{
	//�������: �������������� ��������� ���������
	if (RTC_Status == RTC_Status_busy)
	{
		tc_write_clock_source(&TCC0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCE0, TC_CLKSEL_OFF_gc);
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		tc_write_clock_source(&TCC1, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
		//����� ���� ������, ������������
		RTC.CNT = 0;
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
		transmit_2bytes(COMMAND_COUNTERS_stop, RTC_Status);
		RTC_setStatus_stopped;
	}
	else
	{
		transmit_2bytes(COMMAND_COUNTERS_stop, RTC_Status);
	}
}
//TIC
void TIC_transmit(void)
{
	//�������: ��������������� ������� TIC ������
	//delay_us(usartTIC_delay);
	//for (uint8_t i = 2; i < USART_MEM[1]; i++)
	//{
	//	usart_putchar(USART_TIC,USART_MEM[i]);				//USART_TIC
	//	delay_us(usartTIC_delay);
	//}
	//��� ������ �� TIC
	//���������� ����� �� ��
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
			transmit_3bytes(ERROR_Token, ERROR_wrong_SPI_DEVICE_Number, DEVICE_Number);
			return;
	}
	uint8_t SPI_rDATA[] = {0,0};				//������ SPI ��� ����� ������ (��� �����)
	//���� ���������� DAC AD5643R �� �������� ������ �� ��� ���������, ����������� � �������
	if(DAC_is_AD5643R)
	{
		//���������������� �� ����?
		uint8_t sdata[] = {USART_MEM[1], USART_MEM[2], USART_MEM[3]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 3);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		//�����������
		uint8_t aswDATA[] = {USART_MEM[0]};
		transmit(aswDATA, 1);
		return;
	}
	//���� SPI-���������� - ���, �� ��������, ����������� � �������. 
	if(DEVICE_is_DAC)
	{	
		uint8_t sdata[] = {USART_MEM[1], USART_MEM[2]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 2);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		uint8_t aswDATA[] = {USART_MEM[0]};
		transmit(aswDATA, 1);
		return;
	}
	//���� SPI-���������� - ���, �� ��������, �������� �����, �������� �����.
	uint8_t sdata[] = {USART_MEM[1], USART_MEM[2]};
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
	uint8_t aswDATA[] = {USART_MEM[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//�����
void checkFlags(void)
{
	//�������: ���������� ����� � ������������ � �������� ������, ���� ������ ���� 1, � ���������� ���������. ����� ������ ���������� �����
	//���������: ������ �����: <���������\����������><�������� ��������><iHVE><iEDCD><SEMV1><SEMV2><SEMV3><SPUMP>
	//				���� ������ ��� <���������\����������> = 0, �� �� ��� �� ���������� ������� ��������� ������
	//				���� ������ ��� <���������\����������> = 1, �� �� ������������� ����� � ���������� ��.
	updateFlags();
	Flags.checkOrSet = USART_MEM[1] >> 7;
	if(Flags.checkOrSet == 0)
	{
		//���������. ������� �� �� ���������� ������ � ������
		transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
		return;
	}
	//����������! � ���� �� ��� ������-��?
	if(USART_MEM[1] != *pointer_Flags)
	{
		//���� ��� ������!
		uint8_t i = ((USART_MEM[1] & 32) >> 5);
		if(Flags.iHVE  != i){if(i == 1){gpio_set_pin_high(pin_iHVE);}else{gpio_set_pin_low(pin_iHVE); MC_Tasks.setDACs = 1;}}
		i = ((USART_MEM[1] & 16) >> 4);
		if(Flags.iEDCD != i){if(i == 1){gpio_set_pin_high(pin_iEDCD);}else{gpio_set_pin_low(pin_iEDCD);}}
		i = ((USART_MEM[1] & 8) >> 3);
		if(Flags.SEMV1 != i){if(i == 1){gpio_set_pin_high(pin_SEMV1);}else{gpio_set_pin_low(pin_SEMV1);}}
		i = ((USART_MEM[1] & 4) >> 2);
		if(Flags.SEMV2 != i){if(i == 1){gpio_set_pin_high(pin_SEMV2);}else{gpio_set_pin_low(pin_SEMV2);}}
		i = ((USART_MEM[1] & 2) >> 1);
		if(Flags.SEMV3 != i){if(i == 1){gpio_set_pin_high(pin_SEMV3);}else{gpio_set_pin_low(pin_SEMV3);}}
		i = USART_MEM[1] & 1;
		if(Flags.SPUMP != i){if(i == 1){gpio_set_pin_high(pin_SPUMP);}else{gpio_set_pin_low(pin_SPUMP);}}
		updateFlags();
		transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
		if(MC_Tasks.setDACs)
		{
			delay_s(2); //iHVE �������� �������� ������������ ����, ������� ���� ��������.
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
			spi_select_device(&SPIC,&DAC_Detector);
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, SPI_DATA, 2);
			spi_deselect_device(&SPIC,&DAC_Detector);
			spi_deselect_device(&SPIC,&DAC_IonSource);
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
	//������ ������. �������� �� ������
	uint8_t data = 64;
	transmit_2bytes(COMMAND_Flags_set, data);
}
void updateFlags(void)
{
	//�������: �� ����������� �������� ���� ������ � �������� �� � ���� Flags
	Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
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
	USART_COMP_init;					//���������� USART � ����������
	USART_TIC_init;						//���������� USART � ��������
	//������������ ��������
	PORTC.PIN0CTRL = PORT_ISC_RISING_gc;
	PORTC.PIN1CTRL = PORT_ISC_RISING_gc;
	PORTC.PIN2CTRL = PORT_ISC_RISING_gc;
	EVSYS_SetEventSource( 0, EVSYS_CHMUX_PORTC_PIN0_gc );
	EVSYS_SetEventChannelFilter( 0, EVSYS_DIGFILT_3SAMPLES_gc );
	EVSYS_SetEventSource( 1, EVSYS_CHMUX_TCC0_OVF_gc );
	EVSYS_SetEventChannelFilter( 1, EVSYS_DIGFILT_1SAMPLE_gc );
	EVSYS_SetEventSource( 2, EVSYS_CHMUX_PORTC_PIN1_gc );
	EVSYS_SetEventChannelFilter( 2, EVSYS_DIGFILT_3SAMPLES_gc );
	EVSYS_SetEventSource( 3, EVSYS_CHMUX_TCD0_OVF_gc );
	EVSYS_SetEventChannelFilter( 3, EVSYS_DIGFILT_1SAMPLE_gc );
	EVSYS_SetEventSource( 4, EVSYS_CHMUX_PORTC_PIN2_gc );
	EVSYS_SetEventChannelFilter( 4, EVSYS_DIGFILT_3SAMPLES_gc );
	//�������� �������������
	pointer_Flags = &Flags;
    updateFlags();
	RTC_setStatus_ready;
	cpu_irq_enable();					//��������� ����������	
	//������������� ���������
	while (1) 
	{
		cli();
		if (MC_Tasks.Decrypt == 1)
		{
			//���� ������������ �������..
			uint8_t CheckSum = 0;
			//������� ����������� �����...
			for (uint8_t i = 0; i < USART_MEM_length; i++)
			{
				CheckSum -= USART_MEM[i];
			}
			if (CheckSum == USART_MEM_CheckSum)
			{
				//����������� ����� ������ ����� ��������� �������
				sei();
				decode();
				MC_Tasks.Decrypt = 0;
			}
			else
			{
				//������ ��Ȩ��! �������� ����������� �����!
				MC_reciving_error = 6;
				USART_receiving_time = 0;
			}
		}
		else
		{
			//������ ����� ���������� ~50(1,6���)
			if (USART_receiving_time > 0)
			{
				//����������� ����� �� ���������� �����
				USART_receiving_time++;
				if (USART_receiving_time >= MC_receiving_limit)
				{
					USART_receiving_time = 0;
					//+ ������ ��Ȩ�� ������!
					MC_reciving_error = 3;
				}
			}
		}
		sei();
		//�������������� ������������� ���������
		cli();
		if (((RTC_Status == RTC_Status_delayed)||(RTC_Status == RTC_Status_busy))&&(RTC.CTRL == 0))
		{
			//�� �� ����� � �������� �� ������� RTC!
			RTC_Status = RTC_Status_stopped;
		}
		sei();
	}
}
//-----------------------------------------�������------------------------------------------------
/*
*
*/
/*
*/
//-----------------------------------------THE END------------------------------------------------
