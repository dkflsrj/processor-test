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
#define version 63
#define birthday 20131015
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
uint8_t SPI_AD5643R_configurated = 0;		//0 - �� ���� ����� AD5643 �� � ��������������, 1 - ��������������� ��� ������������, 2 - ��������������� ��� �������, 3 - ���������������� ���!
//		���������
uint8_t RTC_Status = RTC_Status_ready;	//��������� ��������
uint8_t RTC_prescaler = RTC_PRESCALER_OFF_gc; 

uint32_t COA_Measurment = 0;				//��������� ��������� �������� COA
uint8_t	COA_ovf = 0;						//���������� ������������ �������� ��� � ��������� ���������
uint32_t COB_Measurment = 0;				//��������� ��������� �������� COB
uint8_t	COB_ovf = 0;						//���������� ������������ �������� ��� � ��������� ���������
uint32_t COC_Measurment = 0;				//��������� ��������� �������� COC
uint8_t	COC_ovf = 0;						//���������� ������������ �������� ��� � ��������� ���������
//-----------------------------------------���������----------------------------------------------
//������� ����: �����
struct pin_flags
{
	uint8_t SPUMP :1;
	uint8_t SEMV3 :1;
	uint8_t SEMV2 :1;
	uint8_t SEMV1 :1;
	uint8_t iEDCD :1;
	uint8_t iHVE :1;
	uint8_t dummy :1;
	uint8_t checkOrSet :1;	
}flags;
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
uint8_t *pointer_flags;
//------------------------------------���������� �������------------------------------------------
//���������
void showMeByte(uint8_t LED_BYTE);	
//
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

void COUNTERS_start(void);
void COUNTERS_stop(void);

bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);

void ERROR_ASYNCHR(void);

void TIC_transmit(uint8_t DATA[]);

void SPI_send(uint8_t DEVICE_Number, uint8_t data[]);
void SPI_configurate_DAC_AD5643R(struct spi_device SPI_DEV);
void checkFlags(uint8_t DATA);
void updateFlags();
//------------------------------------������� ����������------------------------------------------
ISR(USARTD0_RXC_vect)
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
	tc_write_clock_source(&TCC0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCE0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCC1, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	COA_Measurment = (((uint32_t)TCC1.CNT) << 16) + TCC0.CNT;
	COB_Measurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
	COC_Measurment = TCE0.CNT;
	
	RTC.CNT = 0;
	RTC_setStatus_ready;
	MC_status = 4;
}
static void ISR_TCC1(void)
 {
	 COA_ovf++;
 }
static void ISR_TCD1(void)
 {
	 COB_ovf++;
 }
static void ISR_TCE0(void)
 {
	 COC_ovf++;
 }
//-----------------------------------------�������------------------------------------------------
void showMeByte(uint8_t LED_BYTE)
{
	//�������: ��������� �� ����������� ���� LED_BYTE. ������ �� � ����� ����������� �����
	//����������: ���� ����������� �� ����������, �������� ������������ �����������
	//MC_status = LED_BYTE;									//������ ������ �� "����������� �����"
// 	bool bits[8] = {0,0,0,0,0,0,0,0};				//������ ����� - ��������
// 	for (int i = 0; i < 8; i++)
// 	{
// 		bits[i] = (LED_BYTE  & (1 << (i))) != 0;	//��������� ���� � ����
// 	}
// 	//���������� �� �����������
// 	if (bits[0])
// 	{
// 		gpio_set_pin_high(LED_VD1);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD1);
// 	}
// 	if (bits[1])
// 	{
// 		gpio_set_pin_high(LED_VD2);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD2);
// 	}
// 	if (bits[2])
// 	{
// 		gpio_set_pin_high(LED_VD3);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD3);
// 	}
// 	if (bits[3])
// 	{
// 		gpio_set_pin_high(LED_VD4);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD4);
// 	}
// 	if (bits[4])
// 	{
// 		gpio_set_pin_high(LED_VD5);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD5);
// 	}
// 	if (bits[5])
// 	{
// 		gpio_set_pin_high(LED_VD6);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD6);
// 	}
// 	if (bits[6])
// 	{
// 		gpio_set_pin_high(LED_VD7);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD7);
// 	}
// 	if (bits[7])
// 	{
// 		gpio_set_pin_high(LED_VD8);
// 	}
// 	else
// 	{
// 		gpio_set_pin_low(LED_VD8);
// 	}
	//uint8_t data[] = {COMMAND_showByte};
	//transmit(data,1);
}
//SPI
void SPI_DAC_send(uint8_t DATA[])
{
	//�������: �������� ������ � ���������� (����� � ����������) ���'�
// 	uint8_t data[] = {DATA[1],DATA[2]};
// 	spi_select_device(&SPIC, &DAC);
// 	spi_write_packet(&SPIC, data, 2);
// 	spi_deselect_device(&SPIC, &DAC);
// 	uint8_t aswDATA[] = {COMMAND_DAC_set_Voltage};
// 	transmit(aswDATA, 1);
}
void SPI_ADC_send(uint8_t DATA[])
{
	//�������: ����������� ������ � ���
// 	uint8_t data[] = {DATA[1],DATA[2]};
// 	spi_select_device(&SPIC, &ADC);
// 	spi_write_packet(&SPIC, data, 2);
// 	spi_deselect_device(&SPIC, &ADC);
// 	//�������� �����
// 	spi_select_device(&SPIC, &ADC);
// 	spi_put(&SPIC, 0);
// 	SPI_rDATA[0] = spi_get(&SPIC);
// 	spi_put(&SPIC, 0);
// 	SPI_rDATA[1] = spi_get(&SPIC);
// 	spi_deselect_device(&SPIC, &ADC);
// 	//������ ����� �� �� �� USART
// 	uint8_t aswDATA[] = {COMMAND_ADC_get_Voltage,SPI_rDATA[0],SPI_rDATA[1]};
// 	transmit(aswDATA, 3);
}
//USART COMP
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
	//���������: <key><response_command><RTC_Status><COA_ovf><COA_Measurement_4bytes><COB_ovf><COB_Measurement_4bytes><COC_ovf><COC_Measurement_2bytes><checkSum><lock>
	uint8_t data[] = {COMMAND_COUNTERS_get_Count,RTC_Status,COA_ovf,0,0,0,0,COB_ovf,0,0,0,0,COC_ovf,0,0};
	switch(RTC_Status)
	{
		case RTC_Status_ready:
			data[3] = (COA_Measurment >> 24);
			data[4] = (COA_Measurment >> 16);
			data[5] = (COA_Measurment >> 8);
			data[6] = COA_Measurment;
			data[8] = (COB_Measurment >> 24);
			data[9] = (COB_Measurment >> 16);
			data[10] = (COB_Measurment >> 8);
			data[11] = COB_Measurment;
			data[13] = (COC_Measurment >> 8);
			data[14] = COC_Measurment;
			transmit(data,15);	//15 ����� ��� ��� ���������
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
void COUNTERS_start(void)
{
	//�������: ��������� �������� �� ����������� ���������� �����
	if (RTC_Status != RTC_Status_busy)
	{	
		COA_ovf = 0;
		COB_ovf = 0;
		COC_ovf = 0;
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
		RTC.CNT = 0;
		tc_write_clock_source(&TCC0,TC_CLKSEL_EVCH0_gc);
		tc_write_clock_source(&TCD0,TC_CLKSEL_EVCH2_gc);
		tc_write_clock_source(&TCE0,TC_CLKSEL_EVCH4_gc);	
		RTC.CTRL = RTC_prescaler;
		tc_write_clock_source(&TCC1,TC_CLKSEL_EVCH1_gc);
		tc_write_clock_source(&TCD1,TC_CLKSEL_EVCH3_gc);
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
	if (RTC_Status == RTC_Status_busy)
	{
		tc_write_clock_source(&TCC0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCE0, TC_CLKSEL_OFF_gc);
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		tc_write_clock_source(&TCC1, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
		
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
//RTC
void RTC_setPrescaler(uint8_t DATA[])
{
	//�������: ����� ������������ ������� ��������� �������
	RTC_prescaler = DATA[1];
	transmit_byte(COMMAND_RTC_set_Prescaler);
}
//TIC
void TIC_transmit(uint8_t DATA[])
{
	//�������: ��������������� ������� TIC ������
	delay_us(usartTIC_delay);
	for (uint8_t i = 2; i < DATA[1]; i++)
	{
		usart_putchar(USART_TIC,DATA[i]);				//USART_TIC
		delay_us(usartTIC_delay);
	}
	//��� ������ �� TIC
	//���������� ����� �� ��
}
//������
void ERROR_ASYNCHR(void)
{
	//showMeByte(255);
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
			switch(SPI_AD5643R_configurated)
			{
				case 0: SPI_AD5643R_configurated = 2;
						SPI_configurate_DAC_AD5643R(DAC_Scaner);
					break;
				case 1:	SPI_AD5643R_configurated = 3;
						SPI_configurate_DAC_AD5643R(DAC_Scaner);
					break;
				default:
					break;
			}
			DAC_is_AD5643R = true;
			break;
		case 5: SPI_DEVICE = DAC_Condensator;
			switch(SPI_AD5643R_configurated)
			{
				case 0: SPI_AD5643R_configurated = 1;
						SPI_configurate_DAC_AD5643R(DAC_Condensator);
					break;
				case 2:	SPI_AD5643R_configurated = 3;
						SPI_configurate_DAC_AD5643R(DAC_Condensator);
					break;
				default:
					break;
			}
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
	//���� ���������� DAC AD5643R �� �������� ������ �� ��� ���������, ����������� � �������
	if(DAC_is_AD5643R)
	{
		//���������������� �� ����?
		uint8_t sdata[] = {data[1], data[2], data[3]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 3);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		//�����������
		uint8_t aswDATA[] = {data[0]};
		transmit(aswDATA, 1);
		return;
	}
	//���� SPI-���������� - ���, �� ��������, ����������� � �������. 
	if(DEVICE_is_DAC)
	{	
		uint8_t sdata[] = {data[1], data[2]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 2);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		uint8_t aswDATA[] = {data[0]};
		transmit(aswDATA, 1);
		return;
	}
	//���� SPI-���������� - ���, �� ��������, �������� �����, �������� �����.
	uint8_t sdata[] = {data[1], data[2]};
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
	uint8_t aswDATA[] = {data[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
void SPI_configurate_DAC_AD5643R(struct spi_device SPI_DEV)
{
	//�������: ������������� ��� AD5643R
	uint8_t sdata[] = {56, 0, 1};	//����������� ����� ��� ���� AD5643R
	spi_select_device(&SPIC, &SPI_DEV);
	spi_write_packet(&SPIC, sdata, 3);
	spi_deselect_device(&SPIC, &SPI_DEV);
}
void checkFlags(uint8_t DATA)
{
	//�������: ���������� ����� � ������������ � �������� ������, ���� ������ ���� 1, � ���������� ���������. ����� ������ ���������� �����
	//���������: ������ �����: <���������\����������><�������� ��������><iHVE><iEDCD><SEMV1><SEMV2><SEMV3><SPUMP>
	//				���� ������ ��� <���������\����������> = 0, �� �� ��� �� ���������� ������� ��������� ������
	//				���� ������ ��� <���������\����������> = 1, �� �� ������������� ����� � ���������� ��.
	updateFlags();
	flags.checkOrSet = DATA >> 7;
	if(flags.checkOrSet == 0)
	{
		//���������. ������� �� �� ���������� ������ � ������
		transmit_2bytes(COMMAND_Flags_set, *pointer_flags);
		return;
	}
	//����������! � ���� �� ��� ������-��?
	if(DATA != *pointer_flags)
	{
		//���� ��� ������!
		uint8_t i = ((DATA & 32) >> 5);
		if(flags.iHVE  != i){if(i == 1){gpio_set_pin_high(pin_iHVE);}else{gpio_set_pin_low(pin_iHVE); SPI_AD5643R_configurated = 0;}}
		i = ((DATA & 16) >> 4);
		if(flags.iEDCD != i){if(i == 1){gpio_set_pin_high(pin_iEDCD);}else{gpio_set_pin_low(pin_iEDCD);}}
		i = ((DATA & 8) >> 3);
		if(flags.SEMV1 != i){if(i == 1){gpio_set_pin_high(pin_SEMV1);}else{gpio_set_pin_low(pin_SEMV1);}}
		i = ((DATA & 4) >> 2);
		if(flags.SEMV2 != i){if(i == 1){gpio_set_pin_high(pin_SEMV2);}else{gpio_set_pin_low(pin_SEMV2);}}
		i = ((DATA & 2) >> 1);
		if(flags.SEMV3 != i){if(i == 1){gpio_set_pin_high(pin_SEMV3);}else{gpio_set_pin_low(pin_SEMV3);}}
		i = DATA & 1;
		if(flags.SPUMP != i){if(i == 1){gpio_set_pin_high(pin_SPUMP);}else{gpio_set_pin_low(pin_SPUMP);}}
		updateFlags();
		transmit_2bytes(COMMAND_Flags_set, *pointer_flags);
		return;
	}
	//������ ������. �������� �� ������
	uint8_t data = 64;
	transmit_2bytes(COMMAND_Flags_set, data);
}
void updateFlags()
{
	//�������: �� ����������� �������� ���� ������ � �������� �� � ���� flags
	flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
	flags.iEDCD = (PORTA.OUT & 128) >> 7;
	flags.SEMV1 = (PORTD.OUT & 2  ) >> 1;
	flags.SEMV2 = (PORTD.OUT & 16 ) >> 4;
	flags.SEMV3 = (PORTD.OUT & 32 ) >> 5;
	flags.SPUMP = PORTD.OUT & 1;
}
//-------------------------------------������ ���������-------------------------------------------
int main(void)
{
	board_init();						//���������� �����
	SYSCLK_init;						//���������� �������� (32���)
	pmic_init();						//���������� ������� ����������
	SPIC.CTRL = 83;						//���������� ������� SPI
	RTC_init;							//���������� ������� ��������� �������
	Counters_init;						//���������� �������� ���������
	USART_COMP_init;					//���������� USART � ����������
	USART_TIC_init;					//���������� USART � ��������
	//������������ ������� ���
	PORTC.PIN0CTRL = PORT_ISC_RISING_gc;
	PORTC.PIN1CTRL = PORT_ISC_RISING_gc;
	PORTC.PIN2CTRL = PORT_ISC_RISING_gc;
	//PORTD.DIRCLR = 0x01;
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
	pointer_flags = &flags;
    updateFlags();
	RTC_setStatus_ready;
	MC_status = 1;						//����� ��������
	MC_error = 1;						//������ ���
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
