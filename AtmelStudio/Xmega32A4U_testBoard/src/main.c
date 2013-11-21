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
#define version										86
#define birthday									20131121
//Счётчики
#define RTC_Status_notSet							0		//Счётчики не настроен
#define RTC_Status_ready							1		//Счётчики готов к работе
#define RTC_Status_stopped							2		//Счётчики был принудительно остановлен
#define RTC_Status_busy								3		//Счётчики ещё считает
#define RTC_Status_delayed							4		//RTC считает задержку
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
//Двойной референс
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//Стартовые напряжение PSIS EC (тока эмиссии)
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			82
//Стартовые напряжения PSIS IV,F1,F2 (ионизации, фокусные)
#define AD5328R_startVoltage_Hbyte_PSIS_IV			44
#define AD5328R_startVoltage_Lbyte_PSIS_IV			205

//----------------------------------------ПЕРЕМЕННЫЕ----------------------------------------------
//	МИКРОКОНТРОЛЛЕР
uint8_t MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t CommandStack = 0;

uint8_t MC_Status = 0;
//		USART
uint8_t USART_MEM[100];						//100 байт памяти для приёма данных USART
uint8_t USART_MEM_length = 0;
uint16_t MC_receiving_limit = 65535;
uint16_t USART_receiving_time = 0;
uint8_t MC_reciving_error = 0;
uint8_t USART_buf = 0;
bool	USART_recieving = false;			//Можно пристыковать для экономии памяти к USART_MEM_length
uint8_t USART_MEM[100];						//10 байт памяти для приёма данных USART
uint8_t USART_PACKET_length = 0;
uint8_t USART_MEM_CheckSum = 0;
//		Измерения
uint8_t RTC_Status = RTC_Status_ready;					//Состояния счётчика
uint8_t RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//Предделитель RTC во время измерения
uint8_t RTC_DelayPrescaler = RTC_PRESCALER_OFF_gc;		//Предделитель RTC во время задержки
uint16_t RTC_MeasurePeriod = 0;							//Период RTC во время следующего измерения
uint16_t RTC_DelayPeriod = 0;							//Период RTC во время задержки
uint32_t COA_PreviousMeasurment = 0;					//Последнее измерение счётчика COA
uint8_t	COA_PreviousOVF = 0;							//Количество переполнений счётчика СОА в последнем измерении
uint8_t	COA_OVF = 0;									//Количество переполнений счётчика СОА в текущем измерении
uint32_t COB_PreviousMeasurment = 0;					//Последнее измерение счётчика COB
uint8_t	COB_PreviousOVF = 0;							//Количество переполнений счётчика СОА в последнем измерении
uint8_t	COB_OVF = 0;									//Количество переполнений счётчика СОВ в текущем измерении
uint32_t COC_PreviousMeasurment = 0;					//Последнее измерение счётчика COC
uint8_t	COC_PreviousOVF = 0;							//Количество переполнений счётчика СОА в последнем измерении
uint8_t	COC_OVF = 0;									//Количество переполнений счётчика СОС в текущем измерении
//-----------------------------------------СТРУКТУРЫ----------------------------------------------
//Битовые поля
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
//ADC у конденсатора тот же что и у сканера
//-----------------------------------------УКАЗАТЕЛИ----------------------------------------------
uint8_t *pointer_MC_Tasks;
uint8_t *pointer_Flags;
//------------------------------------ОБЪЯВЛЕНИЯ ФУНКЦИЙ------------------------------------------
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
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTD0_RXC_vect)
{
	//ПРЕРЫВАНИЕ:
	//ПОСЫЛКА: <KEY><PACKET_LENGTH><DATA[...]<CS><LOCK>
	//			PACKET_LENGTH = полная длина посылки
	cli();
	//25(0,78мкс) - неверный ключ (не реагируем)
	//42(1,31мкс) - верный ключ (первый байт принят)
	//38(1,19мкс) - ОШИБКА! МК не выполнил предыдущую команду
	//43(1,34мкс) - ОШИБКА ПРИЁМА! Длинна пакета меньше минимальной
	//35(1,09мкс) - верная длинна пакета (второй байт принят)
	//59(1,84мкс) - приём байта данных
	//68(2,13мкс) - приём байта контрольной суммы
	//74(2,31мкс) - ОШИБКА ПРИЁМА! Запрещённое состояние! USART_MEM_LENGTH > USART_MAIL_Length!
	//94(2,94мкс) - принят затвор (последний байт) Приём успешно завершён!
	//86(2,69мкс) - ОШИБКА! Не был получен затвор!

	//Принимаем байт, что бы там нибыло
	USART_buf = *USART_COMP.DATA;//->3(95нс)
	//Если в режиме приёма
	if(USART_receiving_time > 0)
	{
		if (USART_PACKET_length == 0)
		{
			//Это байт длины пакета
			USART_PACKET_length = USART_buf;
			if (USART_PACKET_length < 5)
			{
				//ОШИБКА ПРИЁМА! Длинна пакета меньше минимальной
				//Если длина посылки меньше самой короткой из возможных посылок (5 байтов), то прекратить приём, выставить флаг ошибки
				MC_reciving_error = 1;
				USART_receiving_time = 0;
			}
		}
		else if(USART_MEM_length < (USART_PACKET_length - 4))
		{
			//Это байты данных, принимаем в массив
			USART_MEM[USART_MEM_length] = USART_buf;
			//НУЖНА ПРОВЕРКА НА ЗАТВОР!
			USART_MEM_length++;
		}
		else if(USART_MEM_length == (USART_PACKET_length - 4))
		{
			//Этот байт контрольная сумма данных
			USART_MEM_CheckSum = USART_buf;
			USART_MEM_length++;//Временное увеличение,чтобы перейти на затвор для следующего принятого байта
		}
		else if(USART_MEM_length == (USART_PACKET_length - 3))
		{
			//Этот байт затвор
			if (USART_buf == COMMAND_LOCK)
			{
				//Приём успешно завершён!
				USART_MEM_length--;//Возвращаем исиннтое значение длинны принятых данных
				USART_receiving_time = 0;//Прекращаем приём
				MC_Tasks.Decrypt = 1;//Ставим задачу - декодировать
			}
			else
			{
				//ОШИБКА ПРИЁМА! Ожидался затвор, а получено чёрти-что
				MC_reciving_error = 2;
				USART_receiving_time = 0;
			}
		}
		else
		{
			//ОШИБКА ПРИЁМА! Запрещённое состояние! USART_MEM_LENGTH > USART_MAIL_Length!
			MC_reciving_error = 4;
			USART_receiving_time = 0;
		}
	}
	else if(USART_buf == COMMAND_KEY)
	{
		//Пришёл ключ!
		if(MC_Tasks.Decrypt == 0)
		{
			//Если предыдущая команда ещё не выполнена
			USART_receiving_time = 1;//Переходим в режим приёма
			USART_MEM_length = 0;//Обнуляем счётчик принятых байтов
			USART_PACKET_length = 0;//Обнуляем длинну пакета (готовимся к приёму байта длинны пакета)
		}
		else
		{
			//ОШИБКА! МК не выполнил предыдущую команду
			MC_reciving_error = 255;
			USART_receiving_time = 0;
		}
	}
	sei();
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
	if (RTC_Status == RTC_Status_busy)
	{
		asm(
		"LDI R16, 0x00			\n\t"//Ноль для останова всех счётчиков (запись в источник сигналов)
		"STS 0x0800, R16		\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Ноль
		"STS 0x0900, R16		\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Ноль
		"STS 0x0A00, R16		\n\t"//Адрес TCE0.CTRLA = 0x0A00 <-	Ноль
		"STS 0x0840, R16		\n\t"//Адрес TCC1.CTRLA = 0x0840 <-	Ноль
		"STS 0x0940, R16		\n\t"//Адрес TCD1.CTRLA = 0x0940 <-	Ноль
		);
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		if(MC_Tasks.doNextMeasure == 1)
		{
			while(RTC.STATUS != 0)
			{
				//Ждём пока можно будет обратиться к регистрам RTC
			}
			RTC.CNT = 0;
			while(RTC.STATUS != 0)
			{
				//Ждём пока можно будет обратиться к регистрам RTC
			}
			RTC.PER = RTC_DelayPeriod;
			while(RTC.STATUS != 0)
			{
				//Ждём пока можно будет обратиться к регистрам RTC
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
				//Ждём пока можно будет обратиться к регистрам RTC
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
				//Ждём пока можно будет обратиться к регистрам RTC
			}
			RTC.CTRL = RTC_PRESCALER_OFF_gc;
			while(RTC.STATUS != 0)
			{
				//Ждём пока можно будет обратиться к регистрам RTC
			}
			RTC.CNT = 0;
		}
		else
		{
			//сохраняем результаты
			COA_PreviousMeasurment = (((uint32_t)TCC1.CNT) << 16) + TCC0.CNT;
			COB_PreviousMeasurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
			COC_PreviousMeasurment = TCE0.CNT;
			COA_PreviousOVF = COA_OVF;
			COB_PreviousOVF = COB_OVF;
			COC_PreviousOVF = COC_OVF;
			//подготовка следующему измерению
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
		//начинаем новое измерения
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CNT = 0;
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		asm(
		"LDI R16, 0x08		\n\t"//TCC0:Код канала событий 0 = 0x08
		"LDI R17, 0x0A		\n\t"//TCD0:Код канала событий 2 = 0x0A
		"LDI R18, 0x0C		\n\t"//TCE0:Код канала событий 4 = 0x0C
		//"LDS R19, 0x205F	\n\t"//RTC: Адрес RTC_Prescaler  = 0x205F
		"LDI R20, 0x09		\n\t"//TCC1:Код канала событий 1 = 0x09
		"LDI R21, 0x0B		\n\t"//TCD1:Код канала событий 3 = 0x0B
		"STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
		"STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
		"STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
		//"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_Prescaler(@0x205F)
		"STS 0x0840, R20	\n\t"//Адрес TCC1.CTRLA = 0x0840 <- Канал событий 1
		"STS 0x0940, R21	\n\t"//Адрес TCD1.CTRLA = 0x0940 <- Канал событий 3
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
//-----------------------------------------ФУНКЦИИ------------------------------------------------
void decode(void)
{
	//ФУНКЦИЯ: Расшифровываем команду
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
	//СУПЕРФУНКЦИЯ: Были приняты многочисленные данные, все их надо установить
	//ПРИНЯТЫЕ ДАННЫЕ:
	//			[0] - <Command37 - setThemAll
	//			[1] - <Continue>										- флаг "сделать следующее измерение"
	//				<0>		- не делать следующее измерение
	//				<1>		- сделать следующее измерение
	//А НОМЕР ИЗМЕРЕНИЯ?
	//			[2] - <RTC_MeasurePrescaler>							- задаёт предделитель следующего измерения
	//				<0>		- не менять ни предделитель, ни период
	//				<1...7> - возможные значения предделителя
	//			[3..4] - <RTC_MeasurePeriod>							- задать время следующего измерения
	//				<0...65535> - возможные значения периода (0 и 1 лучше не использовать)
	//			[5] - <RTC_DelayPrescaler>								- задаёт предделитель следующей задержки
	//				<0>		- не менять ни предделитель, ни период
	//				<1...7> - возможные значения предделителя
	//			[6..7] - <RTC_DelayPeriod>								- задать время следующей задержки
	//				<1...65535> - возможные значения периода (0 и 1 лучше не использовать)
	cli();
	//Проверка - попли ли мы во время измерения (busy)? Если ready или notSet то всёравно всё установить, но SPI DAC установить сразу
	if (RTC_Status != RTC_Status_delayed)
	{
		//Устанавливаем параметры RTC на следующее измерение
		if (USART_MEM[1] == 1)
		{
			//<Continue> можно сделать многофункциональнее, флаговым (флаг SPI устройств, снимать\устанавливать напряги надо ли)
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
		//Даём задание на SPI DAC (будет выполнено во время задержки)
		//Снимаем показания SPI ADC
		//Передаём предыдущие показания счётчиков и ADC (получается сейчасшные, контроль того что поставили в предыдущий раз)
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
		//МК в задержке!!! Ты либо опоздал, либо слишком рано!
	}
	sei();
	//ПОСЫЛАЕМЫЕ ДАННЫЕ:
	//			[0] - <Response>										- отклик
	//				<37> - COMMAND_MEASURE_set_All
	//А НОМЕР ИЗМЕРЕНИЯ?
	//			[1] - <MC_Status>										- статус МК
	//			[2] - <RTC_Status>										- статус RTC
	//			[3] - <COA_PreciousOVF							- предыдущий результат счёта (СОА_ovf)
	//			[4...5] - <COA_PreviousMeasurment>						- предыдущий результат счёта (СОА_CNT)
	//			2 : <D_PRE><D_PER:2>									- задать время следующей задержки
	//			8 : <IS_EC : 2><IS_IV : 2><IS_F1 : 2><IS_F2 : 2>		- напряжения на DAC PSIS
	//			6 : <D_DV1 : 2><D_DV2 : 2><D_DV3 : 2>					- напряжения на DAC DPS
	//			9 : <S_PV : 3><S_SV : 3><C_V : 3>						- напряжения на DAC MSV
	//			8 : <Inl_Inl : 2><Inl_Heater : 2>						- напряжения на DAC Inlet
	//			8 : <IS_EC : 2><IS_IV : 2><IS_F1 : 2><IS_F2 : 2>		- напряжения на ADC PSIS
	//			6 : <D_DV1 : 2><D_DV2 : 2><D_DV3 : 2>					- напряжения на ADC DPS
	//			8 : <S_PV : 2><S_SV : 2><C_Vp : 2><C_Vn : 2>			- напряжения на ADC MSV
	//			8 : <Inl_Inl : 2><Inl_Heater : 2>						- напряжения на ADC Inlet
}
//USART COMP
void transmit(uint8_t DATA[],uint8_t DATA_length)
{
	//ФУНКЦИЯ: Посылаем заданное количество данных, оформив их по протоколу и с контрольной суммой
	//ПОЯСНЕНИЯ: Пакет: ':<PACKET_LENGTH><response><data><CS>\r' 
	//					   ':' - Начало данных
	//					   '<PACKET_LENGTH>' - Длина пакета
	//					   '<data>' - байты данных <<response><attached_data>>
	//							<response> - отклик, код команды, на которую отвечает
	//							<attached_data> - сами данные. Их может не быть (Приказ)
	//					   '<CS>' - контрольная сумма
	//					   '\r' - конец передачи
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
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	usart_putchar(USART_COMP, 5);									//'<PACKET_LENGTH>'
	usart_putchar(USART_COMP,DATA);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA));				//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);							//'\r'
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	usart_putchar(USART_COMP,COMMAND_KEY);								//':'
	usart_putchar(USART_COMP,6);										//'<PACKET_LENGTH>'
	usart_putchar(USART_COMP,DATA_1);
	usart_putchar(USART_COMP,DATA_2);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2));		//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);								//'\r'
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
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
	//ФУНКЦИЯ: Вычисляет контрольную сумму принятых данных
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
	//ФУНКЦИЯ: Перезагружаем МК
	//ВНИМАНИЕ: Нужно уделить особое внимание состоянию системы перед перезагрузкой, навести порядок
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
	//ФУНКЦИЯ: Запускаем счётчики на определённое интервалом время
	
	//Принимать команду запуска во время busy -> ставить флажок MC_Tasks.doNextMeasure. Если флага нет - значит начинать новую серию измерений
	
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
			"LDI R16, 0x08		\n\t"//TCC0:Код канала событий 0 = 0x08
			"LDI R17, 0x0A		\n\t"//TCD0:Код канала событий 2 = 0x0A
			"LDI R18, 0x0C		\n\t"//TCE0:Код канала событий 4 = 0x0C
			"LDS R19, 0x205F	\n\t"//RTC: Адрес RTC_Prescaler  = 0x205F
			"LDI R20, 0x09		\n\t"//TCC1:Код канала событий 1 = 0x09
			"LDI R21, 0x0B		\n\t"//TCD1:Код канала событий 3 = 0x0B
			"STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
			"STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
			"STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
			"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_Prescaler(@0x205F)	
			"STS 0x0840, R20	\n\t"//Адрес TCC1.CTRLA = 0x0840 <- Канал событий 1
			"STS 0x0940, R21	\n\t"//Адрес TCD1.CTRLA = 0x0940 <- Канал событий 3
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
		//подготовка
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		MC_Tasks.MeasureDataWasOVERWRITTEN = 0;
		MC_Tasks.MeasuredDataWasSended = 1; //якобы были посланы (первый замер)
		COA_OVF = 0;
		COB_OVF = 0;
		COC_OVF = 0;
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
		MC_Tasks.doNextMeasure = 0;
		//начали
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		asm(
		"LDI R16, 0x08		\n\t"//TCC0:Код канала событий 0 = 0x08
		"LDI R17, 0x0A		\n\t"//TCD0:Код канала событий 2 = 0x0A
		"LDI R18, 0x0C		\n\t"//TCE0:Код канала событий 4 = 0x0C
		//"LDS R19, 0x205F	\n\t"//RTC: Адрес RTC_Prescaler  = 0x205F
		"LDI R20, 0x09		\n\t"//TCC1:Код канала событий 1 = 0x09
		"LDI R21, 0x0B		\n\t"//TCD1:Код канала событий 3 = 0x0B
		"STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
		"STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
		"STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
		//"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_Prescaler(@0x205F)
		"STS 0x0840, R20	\n\t"//Адрес TCC1.CTRLA = 0x0840 <- Канал событий 1
		"STS 0x0940, R21	\n\t"//Адрес TCD1.CTRLA = 0x0940 <- Канал событий 3
		);
		RTC.CTRL =  RTC_MeasurePrescaler;
		//отчёт
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
	//ФУНКЦИЯ: Принудительная остановка счётчиков
	if (RTC_Status == RTC_Status_busy)
	{
		tc_write_clock_source(&TCC0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCE0, TC_CLKSEL_OFF_gc);
		RTC.CTRL = RTC_PRESCALER_OFF_gc;
		tc_write_clock_source(&TCC1, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
		//Могут быть траблы, внимательней
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
	//ФУНКЦИЯ: ретранслировать команду TIC насосу
	//delay_us(usartTIC_delay);
	//for (uint8_t i = 2; i < USART_MEM[1]; i++)
	//{
	//	usart_putchar(USART_TIC,USART_MEM[i]);				//USART_TIC
	//	delay_us(usartTIC_delay);
	//}
	//ждём ответа от TIC
	//Пересылаем ответ на ПК
}
//Прочие
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
	//ФУНКЦИЯ: Посылает данные указанному SPI-устройству будь то DAC или ADC
	//	Список устройств:
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
	uint8_t SPI_rDATA[] = {0,0};				//Память SPI для приёма данных (два байта)
	//Если устройство DAC AD5643R то посылаем данные по его протоколу, откликаемся и выходим
	if(DAC_is_AD5643R)
	{
		//Сконфигурированы ли ЦАПы?
		uint8_t sdata[] = {USART_MEM[1], USART_MEM[2], USART_MEM[3]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 3);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		//откликаемся
		uint8_t aswDATA[] = {USART_MEM[0]};
		transmit(aswDATA, 1);
		return;
	}
	//Если SPI-устройство - ЦАП, то посылаем, откликаемся и выходим. 
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
	//Если SPI-устройство - АЦП, то посылаем, получаем ответ, отсылаем ответ.
	uint8_t sdata[] = {USART_MEM[1], USART_MEM[2]};
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
	uint8_t aswDATA[] = {USART_MEM[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//Флаги
void checkFlags(void)
{
	//ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
	//ПОЯСНЕНИЯ: Формат байта: <Проверить\Установить><Операция отменена><iHVE><iEDCD><SEMV1><SEMV2><SEMV3><SPUMP>
	//				Если первый бит <Проверить\Установить> = 0, то МК тут же возвращает текущее состояние флагов
	//				Если первый бит <Проверить\Установить> = 1, то МК устанавливает флаги и возвращает их.
	updateFlags();
	Flags.checkOrSet = USART_MEM[1] >> 7;
	if(Flags.checkOrSet == 0)
	{
		//Проверить. Выслать на ПК свеженькие данные о флагах
		transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
		return;
	}
	//Установить! А надо ли что менять-то?
	if(USART_MEM[1] != *pointer_Flags)
	{
		//Есть что менять!
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
			delay_s(2); //iHVE включает довольно иннерционную цепь, поэтому надо обождать.
			//Высокое напряжение включеноё - конфигурируем DACи
			//MSV DAC'и AD5643R (Конденсатор и сканер) - двойной референс
			uint8_t SPI_DATA[] = {AD5643R_confHbyte, AD5643R_confMbyte, AD5643R_confLbyte};
			spi_select_device(&SPIC, &DAC_Condensator);
			spi_select_device(&SPIC, &DAC_Scaner);
			spi_write_packet(&SPIC, SPI_DATA, 3);
			spi_deselect_device(&SPIC, &DAC_Condensator);
			spi_deselect_device(&SPIC, &DAC_Scaner);
			//MSV DAC'и AD5643R (Конденсатор и сканер) - стартовое напряжение на первых каналах
			SPI_DATA[0] = AD5643R_startVoltage_Hbyte;
			SPI_DATA[1] = AD5643R_startVoltage_Mbyte;
			SPI_DATA[2] = AD5643R_startVoltage_Lbyte;
			spi_select_device(&SPIC, &DAC_Scaner);
			spi_select_device(&SPIC, &DAC_Condensator);
			spi_write_packet(&SPIC, SPI_DATA, 3);
			spi_deselect_device(&SPIC, &DAC_Scaner);
			spi_deselect_device(&SPIC, &DAC_Condensator);
			//MSV DAC AD5643R (Сканер) - стартовое напряжение на первом канале
			SPI_DATA[0] = AD5643R_startVoltage_Hbyte + 1;
			spi_select_device(&SPIC, &DAC_Scaner);
			spi_write_packet(&SPIC, SPI_DATA, 3);
			spi_deselect_device(&SPIC, &DAC_Scaner);
			//DPS + PSIS DAC'и AD5328R (Детектор и Ионный Источник) - двойной референс
			SPI_DATA[0] = AD5328R_confHbyte;
			SPI_DATA[1] = AD5328R_confLbyte;
			spi_select_device(&SPIC,&DAC_Detector);
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, SPI_DATA, 2);
			spi_deselect_device(&SPIC,&DAC_Detector);
			spi_deselect_device(&SPIC,&DAC_IonSource);
			//ОТКЛЮЧЕНО по электротехническим причинам!
			/*delay_s(2);
			//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на втором канале (Ионизации)
			sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_IV;
			sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_IV;
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, sdata, 2);
			spi_deselect_device(&SPIC,&DAC_IonSource);
			//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на первом канале (Ток Эмиссии)
			sdata[0] = AD5328R_startVoltage_Hbyte_PSIS_EC;
			sdata[1] = AD5328R_startVoltage_Lbyte_PSIS_EC;
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, sdata, 2);
			spi_deselect_device(&SPIC,&DAC_IonSource);
			//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на третьем канале (Фокусное 1)
			sdata[0] += 16;//переход на следующий адрес
			//sdata[1] = ;
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, sdata, 2);
			spi_deselect_device(&SPIC,&DAC_IonSource);
			//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на четвёртом канале (Фокусное 2)
			sdata[0] += 16;//переход на следующий адрес
			//sdata[1] = ;
			spi_select_device(&SPIC,&DAC_IonSource);
			spi_write_packet(&SPIC, sdata, 2);
			spi_deselect_device(&SPIC,&DAC_IonSource);*/
			MC_Tasks.setDACs = 0;
		}
		return;
	}
	//Нечего менять. Сообщаем об отмене
	uint8_t data = 64;
	transmit_2bytes(COMMAND_Flags_set, data);
}
void updateFlags(void)
{
	//ФУНКЦИЯ: МК осматривает флаговые пины портов и собирает их в байт Flags
	Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
	Flags.iEDCD = (PORTA.OUT & 128) >> 7;
	Flags.SEMV1 = (PORTD.OUT & 2  ) >> 1;
	Flags.SEMV2 = (PORTD.OUT & 16 ) >> 4;
	Flags.SEMV3 = (PORTD.OUT & 32 ) >> 5;
	Flags.SPUMP = PORTD.OUT & 1;
}
//-------------------------------------НАЧАЛО ПРОГРАММЫ-------------------------------------------
int main(void)
{
	confPORTs;							//Конфигурируем порты (HVE пин в первую очередь)
	SYSCLK_init;						//Инициируем кристалл (32МГц)
	pmic_init();						//Инициируем систему прерываний
	SPIC.CTRL = 87;						//Инициируем систему SPI
	RTC_init;							//Инициируем счётчик реального времени
	Counters_init;						//Инициируем счётчики импульсов
	USART_COMP_init;					//Инициируем USART с компутером
	USART_TIC_init;						//Инициируем USART с насосемъ
	//Инициировать счётчики
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
	//Конечная инициализация
	pointer_Flags = &Flags;
    updateFlags();
	RTC_setStatus_ready;
	cpu_irq_enable();					//Разрешаем прерывания	
	//Инициализация завершена
	while (1) 
	{
		cli();
		if (MC_Tasks.Decrypt == 1)
		{
			//Надо декодировать команду..
			uint8_t CheckSum = 0;
			//Подсчёт контрольной суммы...
			for (uint8_t i = 0; i < USART_MEM_length; i++)
			{
				CheckSum -= USART_MEM[i];
			}
			if (CheckSum == USART_MEM_CheckSum)
			{
				//Контрольная сумма верная можно исполнять команду
				sei();
				decode();
				MC_Tasks.Decrypt = 0;
			}
			else
			{
				//ОШИБКА ПРИЁМА! Неверная контрольная сумма!
				MC_reciving_error = 6;
				USART_receiving_time = 0;
			}
		}
		else
		{
			//Полное время исполнения ~50(1,6мкс)
			if (USART_receiving_time > 0)
			{
				//Отсчитываем время до следующего байта
				USART_receiving_time++;
				if (USART_receiving_time >= MC_receiving_limit)
				{
					USART_receiving_time = 0;
					//+ ОШИБКА ПРИЁМА ДАННЫХ!
					MC_reciving_error = 3;
				}
			}
		}
		sei();
		//Автоустранение нежелательных состояний
		cli();
		if (((RTC_Status == RTC_Status_delayed)||(RTC_Status == RTC_Status_busy))&&(RTC.CTRL == 0))
		{
			//То мы вошли в задержку не включив RTC!
			RTC_Status = RTC_Status_stopped;
		}
		sei();
	}
}
//-----------------------------------------ЗАМЕТКИ------------------------------------------------
/*
*
*/
/*
*/
//-----------------------------------------THE END------------------------------------------------
