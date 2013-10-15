//================================================================================================
//========================ТРЕНИРОВКА В ПРОГРАММИРОВАНИИ МИКРОКОНТРОЛЛЕРА==========================
//================================================================================================
//Режимы выбираеются в conf_board.h
//---------------------------------------ПОЯСНЕНИЯ------------------------------------------------
//Микроконтроллер должен выполнять следующие функции:
//	-
//Сокращения:
//	-МК - Микроконтроллер
//	-ПК - Компьютер
//---------------------------------------ПРИМЕЧАНИЯ-----------------------------------------------
//	-Полезным в освоении оказался youtub'овский канал Atmel Corporation
//		http://www.youtube.com/channel/UC7BvmnfLf-HTRZmMlPXeWwA
//	-Полезным оказался также youtub'овский канал Александра Писанецева
//		http://www.youtube.com/channel/UCczziZl2-kvBUhzX9awdNEA?feature=watch
//----------------------------------------ВКЛЮЧЕНИЯ-----------------------------------------------
#include <asf.h>				//Включаем Atmel Software Framework (ASF), содержащий большинство
//								//необходимых функций.
//#include <avr/pgmspace.h>		//Включаем управление flash-памятью контроллера
#include <spi_master.h>			//Включаем модуль SPI
#include <Decoder.h>
#include <Initializator.h>

//---------------------------------------ОПРЕДЕЛЕНИЯ----------------------------------------------
#define FATAL_transmit_ERROR			while(1){transmit(255,254);								\
											delay_ms(50);}
//МК
#define version 63
#define birthday 20131015
#define usartCOMP_delay 10
#define usartTIC_delay 1
#define usartRX_delay 2										//Задержка приёма данных иначе разобьём команду на части
//Счётчики
#define RTC_Status_ready							0		//Счётчик готов к работе
#define RTC_Status_stopped							1		//Счётчик был принудительно остановлен
#define RTC_Status_busy								2		//Счётчик ещё считает
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready	 
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy	
//----------------------------------------ПЕРЕМЕННЫЕ----------------------------------------------
//	МИКРОКОНТРОЛЛЕР
uint8_t MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t MC_status = 0;					/* Состояние микроконтроллера:
*	0 - Неинициализирован, запрещённое состояние;
*	1 - режим ожидания;
*	2 - отображение байта на светодиодах;
*/
uint8_t MC_error = 0;					/* Код последней ошибки:
*	0 - Неинициализирован, запрещённое состояние;
*	1 - нет ошибок;
*	2 - контроллер не понял команду;
*	3 - ошибка MC_lastCommand;
*	4 - ошибка выбора SPI устройства (нет такого)
*/
uint8_t MC_lastCommand = 0;					// Последняя команда, которую выполняет\нял контроллер
uint8_t MC_MEM[] = {0,0,0,0,0,0,0,0,0};		//Восемь байт для всяких операций (включая SPI) + счётчик
//		USART
uint8_t USART_rDATA = 0;					//Последний принятый байт по USART
uint8_t USART_nextByteIsData_count = 0;
bool	USART_recieving = false;			//Можно пристыковать для экономии памяти к USART_MEM_length
uint8_t USART_MEM[10];						//10 байт памяти для приёма данных USART
uint8_t USART_MEM_length = 0;
uint8_t CommandStack = 0;
//		SPI
uint8_t SPI_rDATA[] = {0,0};				//Память SPI для приёма данных (два байта)
uint8_t SPI_AD5643R_configurated = 0;		//0 - ни один ЦАПов AD5643 не с конфигурирован, 1 - сконфигурирован ЦАП конденсатора, 2 - сконфигурирован ЦАП сканера, 3 - сконфигурированы оба!
//		Измерения
uint8_t RTC_Status = RTC_Status_ready;	//Состояния счётчика
uint8_t RTC_prescaler = RTC_PRESCALER_OFF_gc; 

uint32_t COA_Measurment = 0;				//Последнее измерение счётчика COA
uint8_t	COA_ovf = 0;						//Количество переполнений счётчика СОА в последнем измерении
uint32_t COB_Measurment = 0;				//Последнее измерение счётчика COB
uint8_t	COB_ovf = 0;						//Количество переполнений счётчика СОВ в последнем измерении
uint32_t COC_Measurment = 0;				//Последнее измерение счётчика COC
uint8_t	COC_ovf = 0;						//Количество переполнений счётчика СОС в последнем измерении
//-----------------------------------------СТРУКТУРЫ----------------------------------------------
//Битовое поле: флаги
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
//ADC у конденсатора тот же что и у сканера
//-----------------------------------------УКАЗАТЕЛИ----------------------------------------------
uint8_t *pointer_flags;
//------------------------------------ОБЪЯВЛЕНИЯ ФУНКЦИЙ------------------------------------------
//Рудименты
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
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTD0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от компьютера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	//Принимаем байт
	USART_MEM[USART_MEM_length] = usart_get(USART_COMP);
	USART_MEM_length++;
	//задержимя иначе сразу начнём интерпретировать недослушав оставшиеся байты (флаг RXIF сбрасывается после приёма)
	delay_us(usartRX_delay);
	//Есть ли RXIF? Есть ли данные на приём?
	if ((*USART_COMP.STATUS >> 7) == 0)
	{
		//CommandStack++;	//тестовое, для учёта количества выполненных команд
		//showMeByte(CommandStack);
		//НЕТ! Данные закончились! Значит можно интерпретировать... Но сначала "орфография"...
		//А пришёл ли ключ?
		if(USART_MEM[0] == COMMAND_KEY)
		{
			//Всё в порядке ключ есть, а есть ли замок?
			if (USART_MEM[USART_MEM_length - 1] == COMMAND_LOCK)
			{
				//всё оk! Замок есть, а верна ли контрольная сумма?
				uint8_t CheckSum = 0;
				for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
				{
					CheckSum -= USART_MEM[i];
				}
				
				if (CheckSum == USART_MEM[USART_MEM_length - 2])
				{
					//всё супер, команда оформлена по протоколу, теперь её нужно переформатировать её (убрать ключ, замок и кс)
					uint8_t command[USART_MEM_length - 2];
					for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
					{
						command[i-1] = USART_MEM[i];
					}
					CommandStack++;	//тестовое, для учёта количества выполненных команд
					//showMeByte(CommandStack);
					Decode(command);
				}
				else
				{
					//ОШИБКА! НЕВЕРНАЯ КОНТРОЛЬНАЯ СУММА!
					transmit_3bytes(ERROR_Token, ERROR_CheckSum, USART_MEM[USART_MEM_length - 2]);
				}
			}
			else
			{
				//ОШИБКА! ГДЕ ЗАМОК?
				transmit_3bytes(ERROR_Token, ERROR_WhereIsLOCK,USART_MEM[USART_MEM_length - 1]);
			}
		}
		else
		{
			//ОШИБКА! ГДЕ КЛЮЧ?
			transmit_3bytes(ERROR_Token, ERROR_WhereIsKEY, USART_MEM[0]);
		}
		USART_MEM_length = 0;
	}	
	
	//for (uint8_t i = 0; i < USART_MEM_length; i++)
	//{
	//	usart_putchar(USART_COMP,USART_MEM[i]);							//<data>
	//	delay_us(usartCOMP_delay);
	//}
	
	
	//Проверяем команду на вшивость
	/*if(USART_MEM[0] == COMMAND_KEY)
	{
		//Всё в порядке ключ есть, а есть ли замок?
		if (USART_MEM[USART_MEM_length - 1] == COMMAND_LOCK)
		{
			//всё оk! Замок есть, а верна ли контрольная сумма?
			uint8_t CheckSum = 0;
			for (uint8_t i = 1; i < USART_MEM_length - 2; i++)
			{
				CheckSum -= USART_MEM[i];
			}
			if (CheckSum == USART_MEM[USART_MEM_length - 2])
			{
				//всё супер, команда оформлена по протоколу, теперь её нужно переформатировать её (убрать ключ, замок и кс)
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
				//ОШИБКА! НЕВЕРНАЯ КОНТРОЛЬНАЯ СУММА!
				transmit_3bytes(ERROR_Token, ERROR_CheckSum, USART_MEM[USART_MEM_length - 2]);
			}
		}
		else
		{
			//ОШИБКА! ГДЕ ЗАМОК?
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
		//ОШИБКА! ГДЕ КЛЮЧ?
		transmit_3bytes(ERROR_Token, ERROR_WhereIsKEY, USART_MEM[0]);
	}*/
	//обнуляемся, и готовимся к приёму следующей передачи
	//USART_MEM_length = 0;
	//usart_set_rx_interrupt_level(USART_COMP,USART_INT_LVL_MED);
	
// 	USART_rDATA = usart_getchar(USART_COMP);		//Получаем наш байт
// 	if (USART_recieving)
// 	{
// 		//если мы уже получаем байты, то проверяем байт на закрытие передачи (<lock> + чистый регистр приёма)
// 		if ((USART_rDATA == COMMAND_LOCK)&&(usart_rx_is_complete(USART_COMP)))
// 		{
// 			//Пришёл затвор! Проверяем контрольную сумму 
// 			if (checkCommand(USART_MEM,USART_MEM_length))
// 			{
// 				//дешифрируем команду
// 				Decode(USART_MEM);
// 			}
// 			else
// 			{
// 				//Надо послать ошибку контрольной суммы. Такой команды мы не знаем
// 				uint8_t data[] = {ERROR_Token, ERROR_CheckSum, USART_MEM[0]};
// 				transmit(data,3);
// 			}
// 			USART_recieving = false;
// 			USART_MEM_length = 0;
// 		} 
// 		else
// 		{
// 			//Передача не закрыта, продолжаем получать байты
// 			USART_MEM[USART_MEM_length] = USART_rDATA;
// 			USART_MEM_length++;
// 		}
// 	}
// 	else
// 	{
// 		//если мы не получали байты, то проверяем регистр на ключ
// 		if (USART_rDATA == COMMAND_KEY)
// 		{
// 			//Пришёл ключ! Отпираем получение данных
// 			USART_recieving = true;
// 		}
// 	}
}
ISR(USARTE0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от TIC контроллера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	//Принимаем байт
	
	
}
ISR(RTC_OVF_vect)
{
	//ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
	//ФУНКЦИЯ: Остановка счётчиков импульсов
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
//-----------------------------------------ФУНКЦИИ------------------------------------------------
void showMeByte(uint8_t LED_BYTE)
{
	//ФУНКЦИЯ: Показвает на светодиодах байт LED_BYTE. Вводит МК в режим отображения байта
	//ПРИМЕЧАНИЕ: Если срабатывает по прерыванию, возможно некорректное отображение
	//MC_status = LED_BYTE;									//Меняем статус на "отображение байта"
// 	bool bits[8] = {0,0,0,0,0,0,0,0};				//Массив битов - лампочек
// 	for (int i = 0; i < 8; i++)
// 	{
// 		bits[i] = (LED_BYTE  & (1 << (i))) != 0;	//Переводим байт в биты
// 	}
// 	//Отображаем на светодиодах
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
	//ФУНКЦИЯ: Посылаем данные о напряжении (адрес и напряжение) ЦАП'у
// 	uint8_t data[] = {DATA[1],DATA[2]};
// 	spi_select_device(&SPIC, &DAC);
// 	spi_write_packet(&SPIC, data, 2);
// 	spi_deselect_device(&SPIC, &DAC);
// 	uint8_t aswDATA[] = {COMMAND_DAC_set_Voltage};
// 	transmit(aswDATA, 1);
}
void SPI_ADC_send(uint8_t DATA[])
{
	//ФУНКЦИЯ: Запрашиваем данные у АЦП
// 	uint8_t data[] = {DATA[1],DATA[2]};
// 	spi_select_device(&SPIC, &ADC);
// 	spi_write_packet(&SPIC, data, 2);
// 	spi_deselect_device(&SPIC, &ADC);
// 	//Получаем ответ
// 	spi_select_device(&SPIC, &ADC);
// 	spi_put(&SPIC, 0);
// 	SPI_rDATA[0] = spi_get(&SPIC);
// 	spi_put(&SPIC, 0);
// 	SPI_rDATA[1] = spi_get(&SPIC);
// 	spi_deselect_device(&SPIC, &ADC);
// 	//Передём ответ на ПК по USART
// 	uint8_t aswDATA[] = {COMMAND_ADC_get_Voltage,SPI_rDATA[0],SPI_rDATA[1]};
// 	transmit(aswDATA, 3);
}
//USART COMP
void transmit(uint8_t DATA[],uint8_t DATA_length)
{
	//ФУНКЦИЯ: Посылаем заданное количество данных, оформив их по протоколу и с контрольной суммой
	//ПОЯСНЕНИЯ: Протокол: ':<response><data><CS>\r' 
	//					   ':' - Начало данных
	//					   '<data>' - байты данных <<response><attached_data>>
	//							<response> - отклик, код команды, на которую отвечает
	//							<attached_data> - сами данные. Их может не быть (Приказ)
	//					   '<CS>' - контрольная сумма
	//					   '\r' - конец передачи
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
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
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
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
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
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
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
	//ФУНКЦИЯ: Вычисляет контрольную сумму принятых данных
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
	//ФУНКЦИЯ: Вернуть ПК результат измерения
	//ПОЯСНЕНИЯ: <key><response_command><RTC_Status><COA_ovf><COA_Measurement_4bytes><COB_ovf><COB_Measurement_4bytes><COC_ovf><COC_Measurement_2bytes><checkSum><lock>
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
			transmit(data,15);	//15 будет для трёх счётчиков
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
	//ФУНКЦИЯ: Задаёт временной интервал во время, которого будет производиться счёт импульсов
	
	if (RTC_Status != RTC_Status_busy)
	{
		RTC.PER = (((uint16_t)DATA[1])<<8) + DATA[2];
//  		uint8_t data[] = {COMMAND_RTC_set_Period, 1};
//  		transmit(data, 2);	//Операция удалась
		transmit_2bytes(COMMAND_RTC_set_Period, 1);
	}
	else
	{
		transmit_2bytes(COMMAND_RTC_set_Period, 0);	//Операция не удалась - таймер считает. Остановите таймер! Кто-нибудь!!!
	}
}
void COUNTERS_start(void)
{
	//ФУНКЦИЯ: Запускаем счётчики на определённое интервалом время
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
	//ФУНКЦИЯ: Принудительная остановка счётчиков
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
	//ФУНКЦИЯ: Задаёт предделитель таймера реального времени
	RTC_prescaler = DATA[1];
	transmit_byte(COMMAND_RTC_set_Prescaler);
}
//TIC
void TIC_transmit(uint8_t DATA[])
{
	//ФУНКЦИЯ: ретранслировать команду TIC насосу
	delay_us(usartTIC_delay);
	for (uint8_t i = 2; i < DATA[1]; i++)
	{
		usart_putchar(USART_TIC,DATA[i]);				//USART_TIC
		delay_us(usartTIC_delay);
	}
	//ждём ответа от TIC
	//Пересылаем ответ на ПК
}
//Прочие
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
	//ФУНКЦИЯ: Перезагружаем МК
	//ВНИМАНИЕ: Нужно уделить особое внимание состоянию системы перед перезагрузкой, навести порядок
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
	//ФУНКЦИЯ: Посылает данные указанному SPI-устройству будь то DAC или ADC
	//	Список устройств:
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
	//Создадим виртульное устройство
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
	//Если устройство DAC AD5643R то посылаем данные по его протоколу, откликаемся и выходим
	if(DAC_is_AD5643R)
	{
		//Сконфигурированы ли ЦАПы?
		uint8_t sdata[] = {data[1], data[2], data[3]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 3);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		//откликаемся
		uint8_t aswDATA[] = {data[0]};
		transmit(aswDATA, 1);
		return;
	}
	//Если SPI-устройство - ЦАП, то посылаем, откликаемся и выходим. 
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
	//Если SPI-устройство - АЦП, то посылаем, получаем ответ, отсылаем ответ.
	uint8_t sdata[] = {data[1], data[2]};
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//Читаем два байта
	spi_deselect_device(&SPIC, &SPI_DEVICE);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC,SPI_rDATA,2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &SPI_DEVICE);
	//Передём ответ на ПК по USART
	uint8_t aswDATA[] = {data[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
void SPI_configurate_DAC_AD5643R(struct spi_device SPI_DEV)
{
	//ФУНКЦИЯ: Конфигурирует ЦАП AD5643R
	uint8_t sdata[] = {56, 0, 1};	//настроечные байты для ЦАПА AD5643R
	spi_select_device(&SPIC, &SPI_DEV);
	spi_write_packet(&SPIC, sdata, 3);
	spi_deselect_device(&SPIC, &SPI_DEV);
}
void checkFlags(uint8_t DATA)
{
	//ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
	//ПОЯСНЕНИЯ: Формат байта: <Проверить\Установить><Операция отменена><iHVE><iEDCD><SEMV1><SEMV2><SEMV3><SPUMP>
	//				Если первый бит <Проверить\Установить> = 0, то МК тут же возвращает текущее состояние флагов
	//				Если первый бит <Проверить\Установить> = 1, то МК устанавливает флаги и возвращает их.
	updateFlags();
	flags.checkOrSet = DATA >> 7;
	if(flags.checkOrSet == 0)
	{
		//Проверить. Выслать на ПК свеженькие данные о флагах
		transmit_2bytes(COMMAND_Flags_set, *pointer_flags);
		return;
	}
	//Установить! А надо ли что менять-то?
	if(DATA != *pointer_flags)
	{
		//Есть что менять!
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
	//Нечего менять. Сообщаем об отмене
	uint8_t data = 64;
	transmit_2bytes(COMMAND_Flags_set, data);
}
void updateFlags()
{
	//ФУНКЦИЯ: МК осматривает флаговые пины портов и собирает их в байт flags
	flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
	flags.iEDCD = (PORTA.OUT & 128) >> 7;
	flags.SEMV1 = (PORTD.OUT & 2  ) >> 1;
	flags.SEMV2 = (PORTD.OUT & 16 ) >> 4;
	flags.SEMV3 = (PORTD.OUT & 32 ) >> 5;
	flags.SPUMP = PORTD.OUT & 1;
}
//-------------------------------------НАЧАЛО ПРОГРАММЫ-------------------------------------------
int main(void)
{
	board_init();						//Инициируем карту
	SYSCLK_init;						//Инициируем кристалл (32МГц)
	pmic_init();						//Инициируем систему прерываний
	SPIC.CTRL = 83;						//Инициируем систему SPI
	RTC_init;							//Инициируем счётчик реального времени
	Counters_init;						//Инициируем счётчики импульсов
	USART_COMP_init;					//Инициируем USART с компутером
	USART_TIC_init;					//Инициируем USART с насосемъ
	//Инициировать счётчик СОА
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
	//Конечная инициализация
	pointer_flags = &flags;
    updateFlags();
	RTC_setStatus_ready;
	MC_status = 1;						//Режим ожидания
	MC_error = 1;						//Ошибок нет
	cpu_irq_enable();					//Разрешаем прерывания	
	//Инициализация завершена
	while (1) 
	{
		
	}
}
//-----------------------------------------ЗАМЕТКИ------------------------------------------------
/*
*
*/
/*
*/
//-----------------------------------------THE END------------------------------------------------
