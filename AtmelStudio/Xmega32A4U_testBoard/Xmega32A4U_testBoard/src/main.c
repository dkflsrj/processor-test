//================================================================================================
//========================ТРЕНИРОВКА В ПРОГРАММИРОВАНИИ МИКРОКОНТРОЛЛЕРА==========================
//================================================================================================
//
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
//
//---------------------------------------ОПРЕДЕЛЕНИЯ----------------------------------------------
//МК
#define version 10
#define birthday 20130730
#define usart_delay 1
//Commands
#define COMMAND_MC_get_Version			1	//Команда: Запросить версию прошивки
#define COMMAND_MC_get_Birthday			2	//Команда: Запросить дату создания прошивки
#define COMMAND_MC_get_CPUfreq			3	//Команда: Запросить частоту МК

#define COMMAND_MC_wait					5	//Команда: Перевести МК в ожидание
#define COMMAND_showTCD2_CNTh			6	//Команда: Показать младший байт TCD2
#define COMMAND_showTCD2_CNTl			7	//Команда: Показать старший байт TCD2

#define COMMAND_showByte				10	//Команда: Показать байт на светодиодах

#define COMMAND_MC_get_status			20  //Команда: Запросить состояние МК

#define COMMAND_COA_set_timeInterval	30	//Команда: Задать интервал счёта времени
#define COMMAND_COA_start				31	//Команда: Начать счёт импульсов
#define COMMAND_COA_get_count			32	//Команда: Запросить у счётчика результат
#define COMMAND_COA_stop				33	//Команда: Остановить счётчик
#define COMMAND_RTC_set_prescaler		34	//Команда: Задать делитель RTC

#define COMMAND_DAC_set_voltage			40	//Команда: Задать DAC'у напряжение
#define COMMAND_ADC_get_voltage			41	//Команда: Запросить у ADC напряжение
//Счётчики
#define COA_setStatus_ready		COA_state =		7;
#define COA_setStatus_busy		COA_state =		8;
#define COA_setStatus_stunned	COA_state =		9; 
//USART
#define USART_COMP						&USARTE0
#define USART_COMP_BAUDRATE				128000
#define USART_COMP_CHAR_LENGTH			USART_CHSIZE_8BIT_gc
#define USART_COMP_PARITY				USART_PMODE_DISABLED_gc
#define USART_COMP_STOP_BIT				true
#define USART_COMP_init					usart_init_rs232(USART_COMP, &USART_COMP_OPTIONS);		 \
										usart_set_rx_interrupt_level(USART_COMP,USART_INT_LVL_MED)
//SPI
#define SPI_init						spi_master_init(&SPIC);									 \
										spi_enable(&SPIC)			//Включаем ШПИКа
//RTC
#define RTC_init						rtc_init();												 \
										CLK.RTCCTRL = 13//5 // RTC 1.024кГц								
#define RTC_reset						RTC.CNT = 0
//SYSCLK
#define SYSCLK_init						osc_enable(OSC_ID_RC32MHZ);								 \
										osc_wait_ready(OSC_ID_RC32MHZ);							 \
										ccp_write_io((uint8_t *)&CLK.CTRL, CONFIG_SYSCLK_SOURCE);\
										Assert(CLK.CTRL == CONFIG_SYSCLK_SOURCE);				 \
										osc_disable(OSC_ID_RC2MHZ)
//COUNTERS
#define Counters_init					tc_set_overflow_interrupt_level(&TCD0,TC_INT_LVL_LO);	 \
										//tc_set_overflow_interrupt_level(&TCD1,TC_INT_LVL_LO);
										//tc_set_overflow_interrupt_level(&TCC0,TC_INT_LVL_LO)			 

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
uint8_t MC_lastCommand = 0;				// Последняя команда, которую выполняет\нял контроллер
uint8_t MC_MEM[] = {0,0,0,0,0,0,0,0,0}; //Восемь байт для всяких операций (включая SPI) + счётчик
//		USART
uint8_t USART_rDATA = 0;				//Последний принятый байт по USART
uint8_t USART_nextByteIsData_count = 0;
//		SPI
uint8_t SPI_rDATA[] = {0,0};			//Память SPI для приёма данных (два байта)
//		Измерения
uint16_t Interval_length = 1000;		//Время интервала (в миллисекунд)[0.05...30 сек]
uint8_t Interval_delay = 20;			//Задержка в миллисекундах между интервалами [10..50мс]
uint16_t Intervals_count = 100;			//Количество интервалов (интервал + задержка)
uint16_t COA_measurment = 0;			//Последнее измерение счётчика
uint16_t COA_measurment_2 = 0;
uint8_t COA_ovf = 0;					//Количество переполнений счётчика
uint8_t COA_state = 0;					//Состояния счётчика 7 - готов, 8 - считает, 9 - остановлен

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
void USART_COMP_transmit_MCversion(void);
void USART_COMP_transmit_MCbirthday(void);
void USART_COMP_transmit_MCstatus(void);
void USART_COMP_transmit_report(uint8_t ERROR);
void USART_COMP_transmit_COA_count(void);
void USART_COMP_transmit_CPUfreq(void);
void SPI_send(uint8_t DeviceN, uint8_t DATA_1, uint8_t DATA_2);
void COA_set_timeInterval(uint8_t INTERVAL_LENGTH_Hb, uint8_t INTERVAL_LENGTH_Lb);
void COA_set_delayInterval(uint8_t INTERVAL_DELAY);
void COA_set_mesureQuontity(uint8_t INTERVAL_COUNT);
void COA_start(void);
void COA_stop(void);
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel,EVSYS_DIGFILT_t filterCoefficient);
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTE0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от компьютера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	USART_rDATA = usart_getchar(USART_COMP);		//Получаем наш байт
	if(USART_nextByteIsData_count == 0)
	{
		//Если мы не ждали байт данных, то значит, наш байт - команда, её нужно интерпретировать
		switch (USART_rDATA)
		{
			case COMMAND_MC_get_status: USART_COMP_transmit_MCstatus();
				break;
			case COMMAND_showByte:	USART_nextByteIsData_count = 1;	
				break;
			case COMMAND_DAC_set_voltage: USART_nextByteIsData_count = 2;	
				break;
			case COMMAND_ADC_get_voltage: USART_nextByteIsData_count = 2;	
				break;
			case COMMAND_COA_get_count: USART_COMP_transmit_COA_count();	
				break;
			case COMMAND_showTCD2_CNTl: MC_status = 2;
				break;
			case COMMAND_showTCD2_CNTh: MC_status = 3;
				break;
			case COMMAND_COA_set_timeInterval: USART_nextByteIsData_count = 2;
				break;
			case COMMAND_COA_start: COA_start();
				break;
			case COMMAND_COA_stop: COA_stop();
				break;
			case COMMAND_MC_get_CPUfreq: USART_COMP_transmit_CPUfreq();
			break;	
			case COMMAND_MC_get_Version: USART_COMP_transmit_MCversion();
				break;
			case COMMAND_MC_get_Birthday: USART_COMP_transmit_MCbirthday();
				break;
			case COMMAND_MC_wait: MC_status = 1;
				break;
			case COMMAND_RTC_set_prescaler: USART_nextByteIsData_count = 1;
				break;
			default: USART_COMP_transmit_report(2);//Контроллер не понял команду
		}
		MC_lastCommand = USART_rDATA;				//Запопинаем нашу команду
	}
	else if (USART_nextByteIsData_count == 1)
	{
		//Наш байт - данные, с ними нужно что-то сделать в соответствии с предыдущей командой
		//Причём это последний байт, то есть делаем задуманное
		switch (MC_lastCommand)
		{
			case COMMAND_showByte: showMeByte(USART_rDATA);
				break;
			case COMMAND_DAC_set_voltage: SPI_send(1,MC_MEM[1],USART_rDATA);
				break;
			case COMMAND_ADC_get_voltage: SPI_send(2,MC_MEM[1],USART_rDATA);
				break;
			case COMMAND_COA_set_timeInterval: COA_set_timeInterval(MC_MEM[1],USART_rDATA);
				break;
			case COMMAND_RTC_set_prescaler: RTC_prescaler = USART_rDATA;
				break;
			default: USART_COMP_transmit_report(3); //Несоответствие команд
		}
		USART_nextByteIsData_count = 0;			//Сбрасываем счётчик байтов
		MC_MEM[0] = 0;							//И счётчик массива
	}
	else
	{
		//Наших байт данных больше одного - сохраняем в массив MEM
		MC_MEM[MC_MEM[0]+1] = USART_rDATA;
		MC_MEM[0]++;
		USART_nextByteIsData_count--;
	}
}
ISR(RTC_OVF_vect)
{
	//ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
	//ФУНКЦИЯ: Остановка счётчиков импульсов
	//gpio_set_pin_low(A2);	// жёлтый луч
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	//gpio_set_pin_low(A3);	//Зелёный луч
	showMeByte(255);
	COA_measurment = TCD0.CNT;
	COA_measurment_2 = TCD1.CNT;
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	//RTC.CNT = 0;
	COA_setStatus_ready;
	MC_status = 4;
}
ISR(TCD1_OVF_vect)
{
	COA_ovf++;
}
//
//-----------------------------------------ФУНКЦИИ------------------------------------------------
void showMeByte(uint8_t LED_BYTE)
{
	//ФУНКЦИЯ: Показвает на светодиодах байт LED_BYTE. Вводит МК в режим отображения байта
	//ПРИМЕЧАНИЕ: Если срабатывает по прерыванию, возможно некорректное отображение
	//MC_status = 2;									//Меняем статус на "отображение байта"
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
}
//SPI
void SPI_send(uint8_t DeviceN, uint8_t DATA_1, uint8_t DATA_2)
{
	//ФУНКЦИЯ: Определяем устройство, которому надо послать данные по SPI, и посылаем их
	//ПРИМЕЧАНИЕ: Можно доработать на отклик и ответ
	bool Device_Answer = false;					//Булка - будет ли ответ от устройства
	struct spi_device DEVICE = {
		.id = 0									//Абстрактное устройство
	};
	switch (DeviceN)
	{
		case 1:	DEVICE = DAC;						//	1 - DAC
			break;
		case 2: DEVICE = ADC;						//	2 - ADC
				Device_Answer = true;				//Ответ от устройства будет
			break;
		default: MC_error = 4;						//	? - Такого устройства нет!
			break;
	}
	if (MC_error != 4)
	{
		//Если нет ошибки выбора устройства - посылаем данные
		uint8_t d[] = {DATA_1,DATA_2};
		spi_select_device(&SPIC, &DEVICE);
		spi_write_packet(&SPIC, d, 2);
		spi_deselect_device(&SPIC, &DEVICE);
		if (Device_Answer)
		{
			//Если нужно взять ответ - берём
			spi_select_device(&SPIC, &ADC);
			spi_put(&SPIC, 0);
			SPI_rDATA[0] = spi_get(&SPIC);
			spi_put(&SPIC, 0);
			SPI_rDATA[1] = spi_get(&SPIC);
			spi_deselect_device(&SPIC, &ADC);
			//Передём ответ на ПК по USART
			delay_us(usart_delay);
			usart_put(USART_COMP,SPI_rDATA[0]);
			delay_us(usart_delay);
			usart_put(USART_COMP,SPI_rDATA[1]);
		}
	}
	USART_COMP_transmit_report(MC_error);		//Посылаем отчёт ПК-теру (1 - всё путём)
}
//USART
void USART_COMP_transmit_report(uint8_t ERROR)
{
	//ФУНКЦИЯ: Передача по USART компьютеру данных об ошибке
	delay_us(usart_delay);						//Задержка (МК больно шустрый, ПК не успевает)
	usart_put(USART_COMP,2);			//Посылаем отклик - "ошибка"
	delay_us(usart_delay);
	usart_putchar(USART_COMP,ERROR);	//Посылаем код ошибки
}
void USART_COMP_transmit_MCstatus(void)
{
	//ФУНКЦИЯ: Передача по USART компьютеру данных о статусе МК
	delay_us(usart_delay);
	usart_putchar(USART_COMP,1);		//Посылаем отклик - "статус"
	delay_us(usart_delay);
	usart_put(USART_COMP,MC_status);	//Посылаем код статуса
}
void USART_COMP_transmit_CPUfreq(void)
{
	uint32_t freq = sysclk_get_cpu_hz();
	delay_us(usart_delay);
	usart_putchar(USART_COMP,6);
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)freq);
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)(freq >> 8));
	delay_us(usart_delay);
	usart_putchar(USART_COMP ,(uint8_t)(freq >> 16));
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)(freq >> 24));
}
void USART_COMP_transmit_MCversion(void)
{
	delay_us(usart_delay);
	usart_putchar(USART_COMP,4);
	delay_us(usart_delay);
	usart_putchar(USART_COMP,MC_version);
}
void USART_COMP_transmit_MCbirthday(void)
{
	delay_us(usart_delay);
	usart_putchar(USART_COMP,5);
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)MC_birthday);
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)(MC_birthday >> 8));
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)(MC_birthday>>16));
	delay_us(usart_delay);
	usart_putchar(USART_COMP,(uint8_t)(MC_birthday>>24));
}
void USART_COMP_transmit_COA_count(void)
{
	//ФУНКЦИЯ: Вернуть ПК результат измерения
	delay_us(usart_delay);
	usart_putchar(USART_COMP, COA_state);
	delay_us(usart_delay);
	usart_putchar(USART_COMP, (COA_measurment_2 >> 8));
	delay_us(usart_delay);
	usart_putchar(USART_COMP, COA_measurment_2);
	delay_us(usart_delay);
	usart_putchar(USART_COMP, (COA_measurment >> 8));
	delay_us(usart_delay);
	usart_putchar(USART_COMP, COA_measurment);
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

void COA_set_timeInterval(uint8_t INTERVAL_LENGTH_Hb, uint8_t INTERVAL_LENGTH_Lb)
{
	//ФУНКЦИЯ: Задаёт временной интервал во время, которого будет производиться счёт импульсов
	Interval_length = (((uint16_t)INTERVAL_LENGTH_Hb)<<8) + INTERVAL_LENGTH_Lb;	
	RTC.PER = (Interval_length);
}
void COA_set_delayInterval(uint8_t INTERVAL_DELAY)
{
	//ФУНКЦИЯ: Задаёт задержку между интервалами
	Interval_delay = INTERVAL_DELAY;
}
void COA_set_mesureQuontity(uint8_t INTERVAL_COUNT)
{
	//ФУНКЦИЯ: Задаёт количество интервалов
	Intervals_count = INTERVAL_COUNT;
}
void COA_start(void)
{
	//ФУНКЦИЯ: Запускаем счётчик на определённое интервалом время
	TCD0.CNT = 0;
	TCD1.CNT = 0;
	RTC.CNT = 0;
	COA_ovf = 0;
	COA_setStatus_busy;
	tc_write_clock_source(&TCD0,TC_CLKSEL_EVCH0_gc);
	tc_write_clock_source(&TCD1,TC_CLKSEL_EVCH1_gc);
	RTC.CTRL = RTC_prescaler;
}
void COA_stop(void)
{
	//ФУНКЦИЯ: Принудительная остановка счётчика
	tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
	tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
	RTC.CTRL = RTC_PRESCALER_OFF_gc;
	RTC.CNT = 0;
	COA_setStatus_stunned;
}

//-------------------------------------НАЧАЛО ПРОГРАММЫ-------------------------------------------
int main (void)
{
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
	EVSYS_SetEventChannelFilter( 0, EVSYS_DIGFILT_8SAMPLES_gc );
	//Инициировать двойные счётчики
	EVSYS_SetEventSource(1, EVSYS_CHMUX_TCD0_OVF_gc);
	EVSYS_SetEventChannelFilter( 1, EVSYS_DIGFILT_1SAMPLE_gc );
	
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
	
	MC_status = 1;						//Режим ожидания
	MC_error = 1;						//Ошибок нет
	cpu_irq_enable();					//Разрешаем прерывания	
	

	//Инициализация завершена

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
					showMeByte(COA_measurment_2 >> 8);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_measurment_2);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_measurment >> 8);
					delay_ms(2000);
					showMeByte(0);
					delay_ms(500);
					showMeByte(COA_measurment);
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
