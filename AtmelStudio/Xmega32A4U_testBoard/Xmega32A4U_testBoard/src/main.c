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
//#include <stdio.h>
//#include <conio.h>
//


//---------------------------------------�����������----------------------------------------------
#define FATAL_ERROR						while(1){showMeByte(255);								\
											delay_ms(50);}
#define FATAL_transmit_ERROR			while(1){transmit(255,254);								\
											delay_ms(50);}
//��
#define version 27
#define birthday 20130828
#define usartCOMP_delay 10
#define usartTIC_delay 1
#define usartRX_delay 2										//�������� ����� ������ ����� �������� ������� �� �����
//��������
#define RTC_Status_ready							0		//������� ����� � ������
#define RTC_Status_stopped							1		//������� ��� ������������� ����������
#define RTC_Status_busy								2		//������� ��� �������
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready	 
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy	
 //delayed status!!! 
		 

//----------------------------------------����������----------------------------------------------
//	���������������
uint8_t MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t MC_status = 0;					/* ��������� ����������������:
*	0 - �����������������, ����������� ���������;
*	1 - ����� ��������;
*	2 - ����������� ����� �� �����������;
*/
uint8_t MC_error = 0;					/* ��� ��������� ������:
*	0 - �����������������, ����������� ���������;
*	1 - ��� ������;
*	2 - ���������� �� ����� �������;
*	3 - ������ MC_lastCommand;
*	4 - ������ ������ SPI ���������� (��� ������)
*/
uint8_t MC_lastCommand = 0;					// ��������� �������, ������� ���������\��� ����������
uint8_t MC_MEM[] = {0,0,0,0,0,0,0,0,0};		//������ ���� ��� ������ �������� (������� SPI) + �������
//		USART
uint8_t USART_rDATA = 0;					//��������� �������� ���� �� USART
uint8_t USART_nextByteIsData_count = 0;
bool	USART_recieving = false;			//����� ������������ ��� �������� ������ � USART_MEM_length
uint8_t USART_MEM[10];						//10 ���� ������ ��� ����� ������ USART
uint8_t USART_MEM_length = 0;
uint8_t CommandStack = 0;
//		SPI
uint8_t SPI_rDATA[] = {0,0};				//������ SPI ��� ����� ������ (��� �����)
//		���������

//uint16_t RTC_MeasureTime = 1000;			//����� ��������� (� �����������)[0.05...30 ���]
//uint8_t  RTC_MeasureDelay = 10;				//�������� � ������������� ����� ����������� [10..50��]
uint8_t RTC_Status = RTC_Status_ready;	//��������� ��������
uint8_t RTC_prescaler = RTC_PRESCALER_OFF_gc; 

//uint8_t	 COA_MeasurementsQuantity = 1;		//���������� ���������
//uint16_t COA_MeasureQuantity = 1;			//���������� ���������� (�������� + ��������)
//uint8_t COA_Results_transmitted = 0;		//���� �� ��������� ���������� ������ 0 - �� ���� ������, 1 - ���� ������, 2 - ���� ��������, 3 - ������!
uint32_t COA_Measurment = 0;				//��������� ��������� ��������
uint8_t	COA_ovf = 0; 
//uint8_t RTC_Status = 0;						//RTC ��������, 1 - ��������� ���������, 2 - ��������

//-----------------------------------------���������----------------------------------------------
static usart_rs232_options_t USART_COMP_OPTIONS = {
	.baudrate = USART_COMP_BAUDRATE,
	.charlength = USART_COMP_CHAR_LENGTH,
	.paritytype = USART_COMP_PARITY,
	.stopbits = USART_COMP_STOP_BIT
};
struct spi_device DAC = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device ADC = {
	.id = IOPORT_CREATE_PIN(PORTC, 2)
};

struct spi_device DAC_IonSource = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device DAC_Detector = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device DAC_Inlet = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device DAC_Scaner = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device DAC_Condensator = {
	.id = IOPORT_CREATE_PIN(PORTC, 1)
};
struct spi_device ADC_IonSource = {
	.id = IOPORT_CREATE_PIN(PORTC, 2)
};
struct spi_device ADC_Detector = {
	.id = IOPORT_CREATE_PIN(PORTC, 2)
};
struct spi_device ADC_Inlet = {
	.id = IOPORT_CREATE_PIN(PORTC, 2)
};
struct spi_device ADC_Scaner = {
	.id = IOPORT_CREATE_PIN(PORTC, 2)
};
/*struct spi_device DAC_IonSource = {
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
struct spi_device ADC_Scaner = {
	.id = pin_iECSV
};*/
//ADC � ������������ ��� �� ��� � � �������
//------------------------------------���������� �������------------------------------------------
void showMeByte(uint8_t LED_BYTE);

void checkCommand(uint8_t data[], uint8_t data_length);
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length);
void transmit(uint8_t DATA[],uint8_t DATA_length);
void transmit_byte(uint8_t DATA);
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2);
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3);

void MC_transmit_Birthday(void);
void MC_transmit_CPUfreq(void);
void MC_reset(void);

void COUNTERS_transmit_Result(void);

void SPI_DAC_send(uint8_t data[]);
void SPI_ADC_send(uint8_t data[]);

void RTC_setPrescaler(uint8_t DATA[]);
void RTC_set_Period(uint8_t DATA[]);
/*void RTC_set_Delay(uint8_t DELAY);*/
//void COA_set_MeasureQuontity(uint8_t COUNT[]);

void COUNTERS_start(void);
void COUNTERS_stop(void);
void COUNTERS_Measure_done(void);

bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);

void ERROR_ASYNCHR(void);

//void RTC_delay(void);
void TIC_transmit(uint8_t DATA[]);

void SPI_send(uint8_t DEVICE_Number, uint8_t data[]);
//------------------------------------������� ����������------------------------------------------
ISR(USARTE0_RXC_vect)
{
	//����������: ������ ���� ������ �� ����� USART �� ����������
	//�������: �������������� ����� ��� ������� ��� ������, ���������� ������������ ��������
	//��������� ����
	USART_MEM[USART_MEM_length] = usart_get(USART_COMP);
	USART_MEM_length++;
	//��������� ����� ����� ����� ���������������� ���������� ���������� ����� (���� RXIF ������������ ����� �����)
	delay_us(usartRX_delay);
	//���� �� RXIF? ���� �� ������ �� ����?
	if ((*USART_COMP.STATUS >> 7) == 0)
	{
		//CommandStack++;	//��������, ��� ����� ���������� ����������� ������
		//showMeByte(CommandStack);
		//���! ������ �����������! ������ ����� ����������������... �� ������� "����������"...
		//� ������ �� ����?
		if(USART_MEM[0] == COMMAND_KEY)
		{
			//�� � ������� ���� ����, � ���� �� �����?
			if (USART_MEM[USART_MEM_length - 1] == COMMAND_LOCK)
			{
				//�� �k! ����� ����, � ����� �� ����������� �����?
				uint8_t CheckSum = 0;
				for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
				{
					CheckSum -= USART_MEM[i];
				}
				
				if (CheckSum == USART_MEM[USART_MEM_length - 2])
				{
					//�� �����, ������� ��������� �� ���������, ������ � ����� ����������������� � (������ ����, ����� � ��)
					uint8_t command[USART_MEM_length - 2];
					for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
					{
						command[i-1] = USART_MEM[i];
					}
					CommandStack++;	//��������, ��� ����� ���������� ����������� ������
					//showMeByte(CommandStack);
					Decode(command);
				}
				else
				{
					//������! �������� ����������� �����!
					transmit_3bytes(ERROR_Token, ERROR_CheckSum, USART_MEM[USART_MEM_length - 2]);
				}
			}
			else
			{
				//������! ��� �����?
				transmit_3bytes(ERROR_Token, ERROR_WhereIsLOCK,USART_MEM[USART_MEM_length - 1]);
			}
		}
		else
		{
			//������! ��� ����?
			transmit_3bytes(ERROR_Token, ERROR_WhereIsKEY, USART_MEM[0]);
		}
		USART_MEM_length = 0;
	}	
	
	//for (uint8_t i = 0; i < USART_MEM_length; i++)
	//{
	//	usart_putchar(USART_COMP,USART_MEM[i]);							//<data>
	//	delay_us(usartCOMP_delay);
	//}
	
	
	//��������� ������� �� ��������
	/*if(USART_MEM[0] == COMMAND_KEY)
	{
		//�� � ������� ���� ����, � ���� �� �����?
		if (USART_MEM[USART_MEM_length - 1] == COMMAND_LOCK)
		{
			//�� �k! ����� ����, � ����� �� ����������� �����?
			uint8_t CheckSum = 0;
			for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
			{
				CheckSum -= USART_MEM[i];
			}
			if (CheckSum == USART_MEM[USART_MEM_length - 2])
			{
				//�� �����, ������� ��������� �� ���������, ������ � ����� ����������������� � (������ ����, ����� � ��)
				uint8_t command[USART_MEM_length - 3];
				for (uint8_t i = 0; i < USART_MEM_length - 3; i++)
				{
					command[i] = USART_MEM[i+1];
				}
				
				for (uint8_t i = 0; i < USART_MEM_length-3; i++)
				{
					usart_putchar(USART_COMP,command[i]);							//<data>
					delay_us(usartCOMP_delay);
				}
				//Decode(USART_MEM);
			}
			else
			{
				//������! �������� ����������� �����!
				transmit_3bytes(ERROR_Token, ERROR_CheckSum, USART_MEM[USART_MEM_length - 2]);
			}
		}
		else
		{
			//������! ��� �����?
			transmit_3bytes(ERROR_Token, ERROR_WhereIsLOCK,USART_MEM[USART_MEM_length - 1]);
		}
		for (uint8_t i = 0; i < USART_MEM_length-1; i++)
		{
			usart_putchar(USART_COMP,USART_MEM[i]);							//<data>
			delay_us(usartCOMP_delay);
		}
	}
	else
	{
		//������! ��� ����?
		transmit_3bytes(ERROR_Token, ERROR_WhereIsKEY, USART_MEM[0]);
	}*/
	//����������, � ��������� � ����� ��������� ��������
	//USART_MEM_length = 0;
	//usart_set_rx_interrupt_level(USART_COMP,USART_INT_LVL_MED);
	
// 	USART_rDATA = usart_getchar(USART_COMP);		//�������� ��� ����
// 	if (USART_recieving)
// 	{
// 		//���� �� ��� �������� �����, �� ��������� ���� �� �������� �������� (<lock> + ������ ������� �����)
// 		if ((USART_rDATA == COMMAND_LOCK)&&(usart_rx_is_complete(USART_COMP)))
// 		{
// 			//������ ������! ��������� ����������� ����� 
// 			if (checkCommand(USART_MEM,USART_MEM_length))
// 			{
// 				//����������� �������
// 				Decode(USART_MEM);
// 			}
// 			else
// 			{
// 				//���� ������� ������ ����������� �����. ����� ������� �� �� �����
// 				uint8_t data[] = {ERROR_Token, ERROR_CheckSum, USART_MEM[0]};
// 				transmit(data,3);
// 			}
// 			USART_recieving = false;
// 			USART_MEM_length = 0;
// 		} 
// 		else
// 		{
// 			//�������� �� �������, ���������� �������� �����
// 			USART_MEM[USART_MEM_length] = USART_rDATA;
// 			USART_MEM_length++;
// 		}
// 	}
// 	else
// 	{
// 		//���� �� �� �������� �����, �� ��������� ������� �� ����
// 		if (USART_rDATA == COMMAND_KEY)
// 		{
// 			//������ ����! �������� ��������� ������
// 			USART_recieving = true;
// 		}
// 	}
}
ISR(RTC_OVF_vect)
{
	//����������: ��������� ��� ��������� ����� ������� ��������
	//�������: ��������� ��������� ���������
	COUNTERS_Measure_done();
}
static void ISR_TCD1(void)
 {
	 COA_ovf++;
 }
void COUNTERS_Measure_done(void)
{
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	COA_Measurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	RTC.CNT = 0;
	RTC_setStatus_ready;
	MC_status = 4;								//��������. ����������� ����������� �����
}
//
//-----------------------------------------�������------------------------------------------------
void showMeByte(uint8_t LED_BYTE)
{
	//�������: ��������� �� ����������� ���� LED_BYTE. ������ �� � ����� ����������� �����
	//����������: ���� ����������� �� ����������, �������� ������������ �����������
	//MC_status = LED_BYTE;									//������ ������ �� "����������� �����"
	bool bits[8] = {0,0,0,0,0,0,0,0};				//������ ����� - ��������
	for (int i = 0; i < 8; i++)
	{
		bits[i] = (LED_BYTE  & (1 << (i))) != 0;	//��������� ���� � ����
	}
	//���������� �� �����������
	if (bits[0])
	{
		gpio_set_pin_high(LED_VD1);
	}
	else
	{
		gpio_set_pin_low(LED_VD1);
	}
	if (bits[1])
	{
		gpio_set_pin_high(LED_VD2);
	}
	else
	{
		gpio_set_pin_low(LED_VD2);
	}
	if (bits[2])
	{
		gpio_set_pin_high(LED_VD3);
	}
	else
	{
		gpio_set_pin_low(LED_VD3);
	}
	if (bits[3])
	{
		gpio_set_pin_high(LED_VD4);
	}
	else
	{
		gpio_set_pin_low(LED_VD4);
	}
	if (bits[4])
	{
		gpio_set_pin_high(LED_VD5);
	}
	else
	{
		gpio_set_pin_low(LED_VD5);
	}
	if (bits[5])
	{
		gpio_set_pin_high(LED_VD6);
	}
	else
	{
		gpio_set_pin_low(LED_VD6);
	}
	if (bits[6])
	{
		gpio_set_pin_high(LED_VD7);
	}
	else
	{
		gpio_set_pin_low(LED_VD7);
	}
	if (bits[7])
	{
		gpio_set_pin_high(LED_VD8);
	}
	else
	{
		gpio_set_pin_low(LED_VD8);
	}
	//uint8_t data[] = {COMMAND_showByte};
	//transmit(data,1);
}
//SPI
void SPI_DAC_send(uint8_t DATA[])
{
	//�������: �������� ������ � ���������� (����� � ����������) ���'�
	uint8_t data[] = {DATA[1],DATA[2]};
	spi_select_device(&SPIC, &DAC);
	spi_write_packet(&SPIC, data, 2);
	spi_deselect_device(&SPIC, &DAC);
	uint8_t aswDATA[] = {COMMAND_DAC_set_Voltage};
	transmit(aswDATA, 1);
}
void SPI_ADC_send(uint8_t DATA[])
{
	//�������: ����������� ������ � ���
	uint8_t data[] = {DATA[1],DATA[2]};
	spi_select_device(&SPIC, &ADC);
	spi_write_packet(&SPIC, data, 2);
	spi_deselect_device(&SPIC, &ADC);
	//�������� �����
	spi_select_device(&SPIC, &ADC);
	spi_put(&SPIC, 0);
	SPI_rDATA[0] = spi_get(&SPIC);
	spi_put(&SPIC, 0);
	SPI_rDATA[1] = spi_get(&SPIC);
	spi_deselect_device(&SPIC, &ADC);
	//������ ����� �� �� �� USART
	uint8_t aswDATA[] = {COMMAND_ADC_get_Voltage,SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//USART
void transmit(uint8_t DATA[],uint8_t DATA_length)
{
	//�������: �������� �������� ���������� ������, ������� �� �� ��������� � � ����������� ������
	//���������: ��������: ':<response><data><CS>\r' 
	//					   ':' - ������ ������
	//					   '<data>' - ����� ������ <<response><attached_data>>
	//							<response> - ������, ��� �������, �� ������� ��������
	//							<attached_data> - ���� ������. �� ����� �� ���� (������)
	//					   '<CS>' - ����������� �����
	//					   '\r' - ����� ��������
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	delay_us(usartCOMP_delay);
	for (uint8_t i = 0; i < DATA_length; i++)
	{
		usart_putchar(USART_COMP,DATA[i]);							//<data>
		delay_us(usartCOMP_delay);
	}
	usart_putchar(USART_COMP,calcCheckSum(DATA,DATA_length + 1));	//<CS>
	delay_us(usartCOMP_delay);
	usart_put(USART_COMP,COMMAND_LOCK);								//'\r'
}
void transmit_byte(uint8_t DATA)
{
	//������������: �������� ������ ����� (������)
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA);									//<data>
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA));						//<CS>
	delay_us(usartCOMP_delay);
	usart_put(USART_COMP,COMMAND_LOCK);								//'\r'
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//������������: �������� ������ ����� (������)
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA_1);
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA_2);									//<data>
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2));		//<CS>
	delay_us(usartCOMP_delay);
	usart_put(USART_COMP,COMMAND_LOCK);								//'\r'
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//������������: �������� ������ ����� (������)
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA_1);
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA_2);									//<data>
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP,DATA_3);
	delay_us(usartCOMP_delay);
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));		//<CS>
	delay_us(usartCOMP_delay);
	usart_put(USART_COMP,COMMAND_LOCK);								//'\r'
}
void checkCommand(uint8_t data[], uint8_t data_length)
{
	//�������: ������� ����������� ����� �������� ������
	//��������� ������� �� ���� � �����
	
	
// 	uint8_t CheckSum = 0;
// 	for (uint8_t i = 0; i < data_length - 1; i++)
// 	{
// 		CheckSum -= data[i];
// 	}
// 	if (CheckSum == data[data_length - 1])
// 	{
// 		return true;
// 	}
// 	return false;
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
//COUNTERS
void COUNTERS_transmit_Result(void)
{
	//�������: ������� �� ��������� ���������
	//���������: <key><response_command><RTC_Status><COA_ovf><COA_Measurement_4bytes><COB_ovf><COB_Measurement_4bytes><COC_ovf><COC_Measurement_2bytes><lock>
	uint8_t data[] = {COMMAND_COUNTERS_get_Count,RTC_Status,COA_ovf,0,0,0,0};
	switch(RTC_Status)
	{
		case RTC_Status_ready:
			data[3] = (COA_Measurment >> 24);
			data[4] = (COA_Measurment >> 16);
			data[5] = (COA_Measurment >> 8);
			data[6] = COA_Measurment;
			transmit(data,7);	//15 ����� ��� ��� ���������
			break;
		case RTC_Status_busy:
		case RTC_Status_stopped:
		default:
			transmit_2bytes(COMMAND_COUNTERS_get_Count,RTC_Status);
			break;
	}
}
void RTC_set_Period(uint8_t DATA[])
{
	//�������: ����� ��������� �������� �� �����, �������� ����� ������������� ���� ���������
	
	if (RTC_Status != RTC_Status_busy)
	{
		RTC.PER = (((uint16_t)DATA[1])<<8) + DATA[2];
//  		uint8_t data[] = {COMMAND_RTC_set_Period, 1};
//  		transmit(data, 2);	//�������� �������
		transmit_2bytes(COMMAND_RTC_set_Period, 1);
	}
	else
	{
		transmit_2bytes(COMMAND_RTC_set_Period, 0);	//�������� �� ������� - ������ �������. ���������� ������! ���-������!!!
	}
}
// void RTC_set_Delay(uint8_t DELAY)
// {
// 	//�������: ����� �������� ����� �����������
// 	RTC.PER = (uint16_t)DELAY;
// 	uint8_t data[] = {COMMAND_RTC_set_Delay};
// 	transmit(data,1);
// }
// void COA_set_MeasureQuontity(uint8_t COUNT[])
// {
// 	//�������: ����� ���������� ����������
// 	//COA_MeasureQuantity = (COUNT[1] << 8) + COUNT[2];
// 	uint8_t data[] = {COMMAND_COA_set_Quantity};
// 	transmit(data,1);
// }
void COUNTERS_start(void)
{
	//�������: ��������� �������� �� ����������� ���������� �����
	if (RTC_Status != RTC_Status_busy)
	{	
		COA_ovf = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		RTC.CNT = 0;
		tc_write_clock_source(&TCD1,TC_CLKSEL_EVCH1_gc);	
		RTC.CTRL = RTC_prescaler;
		tc_write_clock_source(&TCD0,TC_CLKSEL_EVCH0_gc);
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
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
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	RTC.CNT = 0;
	TCD0.CNT = 0;
	TCD1.CNT = 0;
	RTC_setStatus_stopped;
	transmit_byte(COMMAND_COUNTERS_stop);
}
//RTC
void RTC_setPrescaler(uint8_t DATA[])
{
	//�������: ����� ������������ ������� ��������� �������
	RTC_prescaler = DATA[1];
	transmit_byte(COMMAND_RTC_set_Prescaler);
}
// void RTC_delay()
// {
// 	//�������: RTC ��������� �������� ����� �����������
// 	RTC.CNT = 0;
// 	RTC_Status = 2;
// 	RTC.CTRL = RTC_PRESCALER_DIV1_gc;
// }
//TIC
void TIC_transmit(uint8_t DATA[])
{
	//�������: ��������������� ������� TIC ������
	delay_us(usartCOMP_delay);
	for (uint8_t i = 2; i < DATA[1]; i++)
	{
		usart_putchar(USART_COMP,DATA[i]);				//USART_TIC
		delay_us(usartCOMP_delay);
	}
	//��� ������ �� TIC
	//���������� ����� �� ��
}
//������
void ERROR_ASYNCHR(void)
{
	showMeByte(255);
	uint8_t ERROR[] = {25,24,15};
	while(1)
	{
		transmit(ERROR,3);
	}
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

void SPI_send(uint8_t DEVICE_Number, uint8_t data[])
{
	//�������: �������� ������ ���������� SPI-���������� ���� �� DAC ��� ADC
	//	������ ���������:
	//		DEVICE_Number		NAME		TYPE
	//			1			 IonSource		DAC
	//			2			 Detector		DAC
	//			3			 Inlet			DAC
	//			4			 Scaner			DAC
	//			5			 Condensator	DAC
	//			6			 IonSource		ADC
	//			7			 Detector		ADC
	//			8			 Inlet			ADC
	//			9			 Scaner			ADC
	//			10			 Condensator	ADC
	//�������� ���������� ����������
	bool DEVICE_is_DAC = true;
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
			break;
		case 5: SPI_DEVICE = DAC_Condensator;
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
		case 9: //������ � ����������� �� ����� ���
		case 10: SPI_DEVICE = ADC_Scaner;
			DEVICE_is_DAC = false;
			break;
		default:
			transmit_3bytes(ERROR_Token, ERROR_wrong_SPI_DEVICE_Number, DEVICE_Number);
			return;
	}
	uint8_t sdata[] = {data[1], data[2]};
	spi_select_device(&SPIC, &SPI_DEVICE);
	spi_write_packet(&SPIC, sdata, 2);
	spi_deselect_device(&SPIC, &SPI_DEVICE);
	//���� SPI-���������� - ���, �� ����������� � �������. 
	if(DEVICE_is_DAC)
	{
		uint8_t aswDATA[] = {data[0]};
		transmit(aswDATA, 1);
		return;
	}
	//���� SPI-���������� - ���, �� �������� �����, �������� �����.
	spi_select_device(&SPIC, &SPI_DEVICE);
	spi_put(&SPIC, 0);
	SPI_rDATA[0] = spi_get(&SPIC);
	spi_put(&SPIC, 0);
	SPI_rDATA[1] = spi_get(&SPIC);
	spi_deselect_device(&SPIC, &SPI_DEVICE);
	//������ ����� �� �� �� USART
	uint8_t aswDATA[] = {data[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//-------------------------------------������ ���������-------------------------------------------
int main(void)
{
	//COA.set_MT(10);
	board_init();						//���������� �����
	SYSCLK_init;						//���������� �������� (32���)
	pmic_init();						//���������� ������� ����������
	SPI_init;							//���������� ������� SPI
	RTC_init;							//���������� ������� ��������� �������
	Counters_init;						//���������� �������� ���������
	USART_COMP_init;					//���������� USART � ����������
	//������������ ������� ���
	PORTD.PIN5CTRL = PORT_ISC_RISING_gc;
	//PORTD.DIRCLR = 0x01;
	EVSYS_SetEventSource( 0, EVSYS_CHMUX_PORTD_PIN5_gc );
	EVSYS_SetEventChannelFilter( 0, EVSYS_DIGFILT_3SAMPLES_gc );
	//������������ ������� ��������
	EVSYS_SetEventSource(1, EVSYS_CHMUX_TCD0_OVF_gc);
	EVSYS_SetEventChannelFilter( 1, EVSYS_DIGFILT_1SAMPLE_gc );
	//COX
	//COUNTER.State.Ready};
	//������������������ ��� ����������� ������������
	for (uint16_t i = 1; i <129 ; i += i)
	{
		delay_ms(50);
		showMeByte(i);
	}
	for (uint16_t i = 128; i >1 ; i -= i/2)
	{
		delay_ms(50);
		showMeByte(i);
	}
	delay_ms(50);
	showMeByte(2);
	delay_ms(50);
	showMeByte(1);
	delay_ms(50);
	showMeByte(0);
	//�������� �������������
	RTC_setStatus_ready;
	MC_status = 1;						//����� ��������
	MC_error = 1;						//������ ���
	cpu_irq_enable();					//��������� ����������	

	//������������� ���������

	//FATAL_transmit_ERROR;
	while (1) 
	{
		switch (MC_status)
		{
			case 1: delay_ms(1000);
					gpio_toggle_pin(LED_VD1);
				break;
			case 2: //showMeByte(duoByte);
				break;
			case 3: //showMeByte(duoByte >> 8);
				break;
			case 4:	showMeByte(0);
					for (Byte i = 0; i < 20; i++)
					{
						gpio_toggle_pin(LED_VD1);
						delay_ms(50);
					}
					showMeByte(COA_Measurment >> 24);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_Measurment >> 16);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_Measurment >> 8);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_Measurment);
					delay_ms(2000);

				break;
		}
	}
}
//-----------------------------------------�������------------------------------------------------
/*
*
*/
/*SPI_rDATA[0] = 255;
SPI_rDATA[1] = 129;
ADC_word = 131 + 4*ADC_channel;

spi_select_device(&SPIC, &ADC);
//�������� ������
spi_put(&SPIC, ADC_word);
spi_put(&SPIC, 16);
spi_deselect_device(&SPIC, &ADC);

spi_select_device(&SPIC, &ADC);
//�������� �������� � ������
spi_put(&SPIC, 0);
SPI_rDATA[0] = spi_get(&SPIC);
spi_put(&SPIC, 0);
SPI_rDATA[1] = spi_get(&SPIC);
spi_deselect_device(&SPIC, &ADC);

//���������� �� �������
for (uint8_t i = 0; i < 10; i++)
{
showMeByte(LED_channel);
delay_ms(50);
showMeByte(0);
delay_ms(50);
}
showMeByte(SPI_rDATA[0]);
delay_ms(2000);
showMeByte(0);
delay_ms(500);
showMeByte(SPI_rDATA[1]);
delay_ms(2000);
LED_channel = LED_channel*2;
if (LED_channel == 0)
{
LED_channel = 1;
}
ADC_channel++;
if (ADC_channel == 8)
{
ADC_channel = 0;
}
*/
//-----------------------------------------THE END------------------------------------------------
