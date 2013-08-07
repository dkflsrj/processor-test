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
//#include <stdio.h>
//#include <conio.h>
//


//---------------------------------------ОПРЕДЕЛЕНИЯ----------------------------------------------
#define FATAL_ERROR						while(1){showMeByte(255);								\
											delay_ms(50);}
#define FATAL_transmit_ERROR			while(1){transmit(255,254);								\
											delay_ms(50);}
//МК
#define version 23
#define birthday 20130807
#define usartCOMP_delay 10
#define usartTIC_delay 1
//Счётчики
#define COUNTER_state_ready						0		//Счётчик готов к работе
#define COUNTER_state_stopped					1		//Счётчик был принудительно остановлен
#define COUNTER_state_busy						2		//Счётчик ещё считает
#define COA_setStatus_ready			COA_Status =	COUNTER_state_ready	 
#define COA_setStatus_stopped		COA_Status =	COUNTER_state_stopped
#define COA_setStatus_busy			COA_Status =	COUNTER_state_busy	
 //delayed status!!! 
#define COA_setStatus_ovflowed		COA_Status++	//Счётчик был переполнен (COA_Status - 2) раз
		 

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
uint8_t USART_MEM[] = {0,0,0,0,0,0,0,0,0,0};
uint8_t USART_MEM_length = 0;
//		SPI
uint8_t SPI_rDATA[] = {0,0};				//Память SPI для приёма данных (два байта)
//		Измерения

uint16_t COA_MeasureTime = 1000;			//Время интервала (в миллисекунд)[0.05...30 сек]
uint8_t  COA_MeasureDelay = 10;				//Задержка в миллисекундах между интервалами [10..50мс]
uint16_t COA_MeasureQuantity = 1;			//Количество интервалов (интервал + задержка)
uint32_t COA_Measurment = 0;				//Последнее измерение счётчика
uint8_t	 COA_MeasurementsQuantity = 1;		//Количество измерений
uint8_t  COA_Status = COUNTER_state_ready;	//Состояния счётчика

uint8_t COA_Results_transmitted = 0;		//Были ли переданны измеренные данные 0 - не было данных, 1 - есть данные, 2 - были переданы, 3 - затёрты!

uint8_t	COA_ovflowed = 0; 
uint8_t RTC_Status = 0;						//RTC выключен, 1 - таймирует измерение, 2 - задержку
uint8_t RTC_prescaler = RTC_PRESCALER_OFF_gc; 
//-----------------------------------------СТРУКТУРЫ----------------------------------------------
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
//------------------------------------ОБЪЯВЛЕНИЯ ФУНКЦИЙ------------------------------------------
void showMeByte(uint8_t LED_BYTE);
bool checkCommand(uint8_t data[], uint8_t data_length);
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length);
void transmit(uint8_t DATA[],uint8_t DATA_length);
void transmit_byte(uint8_t DATA);
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2);
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3);
void MC_transmit_Birthday(void);
void COA_transmit_Result(void);
void MC_transmit_CPUfreq(void);
void SPI_DAC_send(uint8_t data[]);
void SPI_ADC_send(uint8_t data[]);
void RTC_set_Period(uint8_t DATA[]);
/*void RTC_set_Delay(uint8_t DELAY);*/
void COA_set_MeasureQuontity(uint8_t COUNT[]);
void COA_start(void);
void COA_stop(void);
void RTC_setPrescaler(uint8_t DATA[]);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);
void ERROR_ASYNCHR(void);
void MC_reset(void);
void COA_Measure_done(void);
void RTC_delay(void);

void TIC_transmit(uint8_t DATA[]);
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTE0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от компьютера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	USART_rDATA = usart_getchar(USART_COMP);		//Получаем наш байт
	if (USART_recieving)
	{
		//если мы уже получаем байты, то проверяем байт на закрытие передачи
		if (USART_rDATA != COMMAND_LOCK)
		{
			//Передача не закрыта, продолжаем получать байты
			USART_MEM[USART_MEM_length] = USART_rDATA;
			USART_MEM_length++;
		} 
		else
		{
			//showMeByte(127);
			//Пришёл затвор! Закрываем получение байтов, дешифрируем команду
			if (checkCommand(USART_MEM,USART_MEM_length))
			{
				Decode(USART_MEM);
			}
			else
			{
				//Надо послать ошибку
				//FATAL_ERROR;
				uint8_t data[] = {ERROR_Token, ERROR_Decoder, USART_MEM[0]};
				transmit(data,3);
			}
			
			USART_recieving = false;
			USART_MEM_length = 0;
		}
	}
	else
	{
		//если мы не получали байты, то проверяем регистр на ключ
		if (USART_rDATA == COMMAND_KEY)
		{
			//Пришёл ключ! Отпираем получение данных
			USART_recieving = true;
		}
	}
}
ISR(RTC_OVF_vect)
{
	//ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
	//ФУНКЦИЯ: Остановка счётчиков импульсов
// 	if (RTC_Status == 1)
// 	{
		COA_Measure_done();
// 	} 
// 	else if(RTC_Status == 2)
// 	{
// 		TCD0.CNT = 0;
// 		TCD1.CNT = 0;
// 		RTC.CNT = 0;
// 		RTC_Status = 1;
// 		tc_write_clock_source(&TCD1,TC_CLKSEL_EVCH1_gc);
// 		RTC.CTRL = RTC_prescaler;
// 		tc_write_clock_source(&TCD0,TC_CLKSEL_EVCH0_gc);
// 	}
// 	
}
static void ISR_TCD1(void)
 {
	 COA_setStatus_ovflowed;
	 //COA_stop();
	 tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	 tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	 RTC.CTRL = RTC_PRESCALER_OFF_gc;
	 RTC.CNT = 0;
	 TCD0.CNT = 65535;
	 TCD1.CNT = 65535;
	 //COA_ovflowed = 1;
 }
void COA_Measure_done(void)
{
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	//showMeByte(255);
	COA_Measurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	RTC.CNT = 0;
	RTC_Status = 0;
	if (COA_Status == COUNTER_state_busy)
	{
		COA_setStatus_ready;
	}
	MC_status = 4;
// 	//Проверяем, надо ли ещё проводить измерения
// 	if (COA_MeasureQuantity > 1)
// 	{
// 		//COA_start();
// 		COA_MeasureQuantity--;
// 		//Выполнить задержку
// 		RTC_delay();
// 	}
}
//
//-----------------------------------------ФУНКЦИИ------------------------------------------------
void showMeByte(uint8_t LED_BYTE)
{
	//ФУНКЦИЯ: Показвает на светодиодах байт LED_BYTE. Вводит МК в режим отображения байта
	//ПРИМЕЧАНИЕ: Если срабатывает по прерыванию, возможно некорректное отображение
	//MC_status = LED_BYTE;									//Меняем статус на "отображение байта"
	bool bits[8] = {0,0,0,0,0,0,0,0};				//Массив битов - лампочек
	for (int i = 0; i < 8; i++)
	{
		bits[i] = (LED_BYTE  & (1 << (i))) != 0;	//Переводим байт в биты
	}
	//Отображаем на светодиодах
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
	//ФУНКЦИЯ: Посылаем данные о напряжении (адрес и напряжение) ЦАП'у
	uint8_t data[] = {DATA[1],DATA[2]};
	spi_select_device(&SPIC, &DAC);
	spi_write_packet(&SPIC, data, 2);
	spi_deselect_device(&SPIC, &DAC);
	uint8_t aswDATA[] = {COMMAND_DAC_set_voltage};
	transmit(aswDATA, 1);
}
void SPI_ADC_send(uint8_t DATA[])
{
	//ФУНКЦИЯ: Запрашиваем данные у АЦП
	uint8_t data[] = {DATA[1],DATA[2]};
	spi_select_device(&SPIC, &ADC);
	spi_write_packet(&SPIC, data, 2);
	spi_deselect_device(&SPIC, &ADC);
	//Получаем ответ
	spi_select_device(&SPIC, &ADC);
	spi_put(&SPIC, 0);
	SPI_rDATA[0] = spi_get(&SPIC);
	spi_put(&SPIC, 0);
	SPI_rDATA[1] = spi_get(&SPIC);
	spi_deselect_device(&SPIC, &ADC);
	//Передём ответ на ПК по USART
	uint8_t aswDATA[] = {COMMAND_ADC_get_voltage,SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//USART
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
bool checkCommand(uint8_t data[], uint8_t data_length)
{
	//ФУНКЦИЯ: Сверяет контрольную сумму принятых данных
	uint8_t CheckSum = 0;
	for (uint8_t i = 0; i < data_length - 1; i++)
	{
		CheckSum -= data[i];
	}
	if (CheckSum == data[data_length - 1])
	{
		return true;
	}
	return false;
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
void COA_transmit_Result(void)
{
	//ФУНКЦИЯ: Вернуть ПК результат измерения
	uint8_t data[] = {COMMAND_COA_get_Count,COA_Status,0,0,0,0};
	switch(COA_Status)
	{
		case 1:
		case 2:
			transmit(data,2);
			break;
		case 0:
		default:
			data[2] = (COA_Measurment >> 24);
			data[3] = (COA_Measurment >> 16);
			data[4] = (COA_Measurment >> 8);
			data[5] = COA_Measurment;
			transmit(data,6);	
			break;
	}
}
void RTC_set_Period(uint8_t DATA[])
{
	//ФУНКЦИЯ: Задаёт временной интервал во время, которого будет производиться счёт импульсов
	
	if (RTC_Status == 0)
	{
		RTC.PER = (((uint16_t)DATA[1])<<8) + DATA[2];
//  		uint8_t data[] = {COMMAND_RTC_set_Period, 1};
//  		transmit(data, 2);	//Операция удалась
		transmit_2bytes(COMMAND_RTC_set_Period,1);
	}
	else
	{
		transmit_2bytes(COMMAND_RTC_set_Period, 0);	//Операция не удалась - таймер считает. Остановите таймер! Кто-нибудь!!!
	}
}
// void RTC_set_Delay(uint8_t DELAY)
// {
// 	//ФУНКЦИЯ: Задаёт задержку между интервалами
// 	RTC.PER = (uint16_t)DELAY;
// 	uint8_t data[] = {COMMAND_RTC_set_Delay};
// 	transmit(data,1);
// }
void COA_set_MeasureQuontity(uint8_t COUNT[])
{
	//ФУНКЦИЯ: Задаёт количество интервалов
	//COA_MeasureQuantity = (COUNT[1] << 8) + COUNT[2];
	uint8_t data[] = {COMMAND_COA_set_Quantity};
	transmit(data,1);
}
void COA_start(void)
{
	//ФУНКЦИЯ: Запускаем счётчик на определённое интервалом время
	if (COA_Status != COUNTER_state_busy)
	{	
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		RTC.CNT = 0;
		RTC_Status = 1;
		tc_write_clock_source(&TCD1,TC_CLKSEL_EVCH1_gc);
		RTC.CTRL = RTC_prescaler;
		tc_write_clock_source(&TCD0,TC_CLKSEL_EVCH0_gc);
	}
	uint8_t data[] = {COMMAND_COA_start, COA_Status};
	transmit(data,2);
	COA_setStatus_busy;
}
void COA_stop(void)
{
	//ФУНКЦИЯ: Принудительная остановка счётчика
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	RTC.CNT = 0;
	TCD0.CNT = 0;
	TCD1.CNT = 0;
	RTC_Status = 0;
	COA_setStatus_stopped;
	uint8_t data[] = {COMMAND_COA_stop};
	transmit(data,1);
}
//RTC
void RTC_setPrescaler(uint8_t DATA[])
{
	//ФУНКЦИЯ: Задаёт предделитель таймера реального времени
	RTC_prescaler = DATA[1];
	uint8_t data[] = {COMMAND_RTC_set_Prescaler};
	transmit(data, 1);
}
void RTC_delay()
{
	//ФУНКЦИЯ: RTC выполняет задержку между измерениями
	RTC.CNT = 0;
	RTC_Status = 2;
	RTC.CTRL = RTC_PRESCALER_DIV1_gc;
}
//TIC
void TIC_transmit(uint8_t DATA[])
{
	//ФУНКЦИЯ: ретранслировать команду TIC насосу
	delay_us(usartCOMP_delay);
	for (uint8_t i = 2; i < DATA[1]; i++)
	{
		usart_putchar(USART_COMP,DATA[i]);				//USART_TIC
		delay_us(usartCOMP_delay);
	}
	//ждём ответа от TIC
	//Пересылаем ответ на ПК
}
//Прочие
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
//-------------------------------------НАЧАЛО ПРОГРАММЫ-------------------------------------------
int main(void)
{
	//COA.set_MT(10);
	board_init();						//Инициируем карту
	SYSCLK_init;						//Инициируем кристалл (32МГц)
	pmic_init();						//Инициируем систему прерываний
	SPI_init;							//Инициируем систему SPI
	RTC_init;							//Инициируем счётчик реального времени
	Counters_init;						//Инициируем счётчики импульсов
	USART_COMP_init;					//Инициируем USART с компутером
	//Инициировать счётчик СОА
	PORTD.PIN5CTRL = PORT_ISC_RISING_gc;
	//PORTD.DIRCLR = 0x01;
	EVSYS_SetEventSource( 0, EVSYS_CHMUX_PORTD_PIN5_gc );
	EVSYS_SetEventChannelFilter( 0, EVSYS_DIGFILT_3SAMPLES_gc );
	//Инициировать двойные счётчики
	EVSYS_SetEventSource(1, EVSYS_CHMUX_TCD0_OVF_gc);
	EVSYS_SetEventChannelFilter( 1, EVSYS_DIGFILT_1SAMPLE_gc );
	//COX
	//COUNTER.State.Ready};
	//Светопредставление для определения перезагрузки
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
	//Конечная инициализация
	COA_setStatus_ready;
	MC_status = 1;						//Режим ожидания
	MC_error = 1;						//Ошибок нет
	cpu_irq_enable();					//Разрешаем прерывания	

	//Инициализация завершена

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
//-----------------------------------------ЗАМЕТКИ------------------------------------------------
/*
*
*/
/*SPI_rDATA[0] = 255;
SPI_rDATA[1] = 129;
ADC_word = 131 + 4*ADC_channel;

spi_select_device(&SPIC, &ADC);
//посылаем запрос
spi_put(&SPIC, ADC_word);
spi_put(&SPIC, 16);
spi_deselect_device(&SPIC, &ADC);

spi_select_device(&SPIC, &ADC);
//посылаем болванов и читаем
spi_put(&SPIC, 0);
SPI_rDATA[0] = spi_get(&SPIC);
spi_put(&SPIC, 0);
SPI_rDATA[1] = spi_get(&SPIC);
spi_deselect_device(&SPIC, &ADC);

//Показываем на идиодах
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
