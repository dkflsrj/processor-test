//================================================================================================
//========================ТРЕНИРОВКА В ПРОГРАММИРОВАНИИ МИКРОКОНТРОЛЛЕРА==========================
//================================================================================================
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
#define byte uint8_t
//								//необходимых функций.
//#include <avr/pgmspace.h>		//Включаем управление flash-памятью контроллера
#include <spi_master.h>			//Включаем модуль SPI
#include <Decoder.h>
#include <Initializator.h>

//---------------------------------------ОПРЕДЕЛЕНИЯ----------------------------------------------
//МК
#define version										173
#define birthday									20140715
//Счётчики
#define RTC_Status_ready							0		//Счётчики готов к работе
#define RTC_Status_stopped							1		//Счётчики был принудительно остановлен
#define RTC_Status_busy								2		//Счётчики ещё считает
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
//Состояния USART
#define	USART_TIC_State_ready						0		//USART ничего не принимает
#define USART_TIC_State_receiving					1		//USART принимает байты
#define USART_TIC_State_ending						2		//USART получил байт затвора, ожидается завершение передачи
#define USART_TIC_State_decoding					3		//USART декодирует команду
#define USART_TIC_State_receiving_TICstatus			4		//USART (TIC) принимает байты TIC'a на запрос HVE
//Стартовые конфигурации для DAC AD5643R -> двойной референс
#define AD5643R_confHbyte							56
#define AD5643R_confMbyte							0
#define AD5643R_confLbyte							1
//Стартовые напряжения для DAC AD5643R MSV Scan (сканирующее) = 2000
#define AD5643R_startVoltage_Hbyte_MSV_S			25	//Адрес
#define AD5643R_startVoltage_Mbyte_MSV_S			31	//Старший байт напряжения
#define AD5643R_startVoltage_Lbyte_MSV_S			64	//Младший байт напряжения с 2 пустыми младшими битами
//Стартовые напряжения для DAC AD5643R MSV Scan (дополнительное) = 1500
#define AD5643R_startVoltage_Hbyte_MSV_PS			24
#define AD5643R_startVoltage_Mbyte_MSV_PS			23
#define AD5643R_startVoltage_Lbyte_MSV_PS			112
//Стартовые напряжения для DAC AD5643R MSV Cap (конденсатор) = 500
#define AD5643R_startVoltage_Hbyte_MSV_C			24
#define AD5643R_startVoltage_Mbyte_MSV_C			7
#define AD5643R_startVoltage_Lbyte_MSV_C			208
//Стартовые конфигурации для DAC AD5328R -> двойной референс
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//Стартовые напряжение PSIS EC (тока эмиссии) - 2,44мкА
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			200
//Стартовые напряжения DAC PSIS IV (ионизации) - 3600 - 80В
#define AD5328R_startVoltage_Hbyte_PSIS_IV			30
#define AD5328R_startVoltage_Lbyte_PSIS_IV			16
//Стартовые напряжения DAC PSIS F1 (фокусное 1) - 4000
#define AD5328R_startVoltage_Hbyte_PSIS_F1			47
#define AD5328R_startVoltage_Lbyte_PSIS_F1			160
//Стартовые напряжения DAC PSIS F2 (фокусное 2) - 4000
#define AD5328R_startVoltage_Hbyte_PSIS_F2			63
#define AD5328R_startVoltage_Lbyte_PSIS_F2			160

//----------------------------------------ПЕРЕМЕННЫЕ----------------------------------------------
//	МИКРОКОНТРОЛЛЕР
uint8_t  MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t  MC_CommandStack = 0;
uint8_t  MC_Status = 0;
//		USART TIC
uint8_t TIC_timer_time = 0;								//Таймер времени приёма и тишины
uint8_t TIC_MEM[100];									//100 байт памяти для приёма данных от TIC
uint8_t TIC_MEM_length = 0;								//Длина записанного в TIC_MEM пакета байтов.
uint8_t TIC_buf = 0;									//Буфер приёма. Содержит любой принятый байт (даже шум)
uint8_t TIC_State = 0;									//Состояние модуля USART_TIC
uint8_t TIC_HVE_Message[6] = {63, 86, 57, 48, 50, 13};	//char'ы сообщения на запрос статуса TIC'а {"?V902"+'\r'}
uint8_t TIC_HVE_offlineCount = 0;						//Количество запросов, которые проигнорировал. 3 раза и считается аварией.
uint8_t TIC_HVE_Error_sent = 0;							//Булка: 0 - ошибка не отправлена компьютеру, 1 - ошибка уже отправлена компьютеру
uint8_t TIC_Online = 0;									//Булка: 0 - нет связи с TIC'ом, 1 - TIC на связи
byte TIC_Status[30];									//Последнее сообщение статуса от TIC'a 
byte TIC_Status_length = 0;								//Длинна сообщения статуса
byte TIC_disprove_jitter = 0;							//Учёт дребезга контактов (МК вырубает высокое)
byte TIC_disprove_jitter_MAX = 5;						//Максимальное количество дребезга
byte TIC_R1_jitter = 0;									//Учёт дребезга контактов (МК вырубает вентиль)
byte TIC_R1_jitter_MAX = 5;							//Максимальное количество дребезга
//		Измерения
byte RTC_delay = 0;										//Флаг: RTC в задержке
uint8_t  RTC_Status = RTC_Status_ready;					//Состояния счётчика
uint16_t RTC_ElapsedTime = 0;
uint8_t  RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//Предделитель RTC
uint16_t RTC_MeasurePeriod = 0;							//Период RTC
uint8_t  RTC_DelayPrescaler = RTC_PRESCALER_OFF_gc;		//Предделитель RTC
uint16_t RTC_DelayPeriod = 0;							//Период RTC
uint16_t COA_Measurment = 0;							//Последнее измерение счётчика COA
uint16_t COA_OVF = 0;									//Количество переполнений счётчика СОА
uint16_t COB_Measurment = 0;							//Последнее измерение счётчика COB
uint16_t COB_OVF = 0;									//Количество переполнений счётчика СОВ
uint16_t COC_Measurment = 0;							//Последнее измерение счётчика COC
uint16_t COC_OVF = 0;									//Количество переполнений счётчика СОС
//-----------------------------------------СТРУКТУРЫ----------------------------------------------
//Битовые поля
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
//ADC у конденсатора тот же что и у сканера
//-----------------------------------------УКАЗАТЕЛИ----------------------------------------------
uint8_t *pointer_MC_Tasks;
uint8_t *pointer_Errors_USART_PC;
uint8_t *pointer_Errors_USART_TIC;
uint8_t *pointer_Flags;
//------------------------------------ОБЪЯВЛЕНИЯ ФУНКЦИЙ------------------------------------------
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
//-----------------------------------------ВКЛЮЧЕНИЯ----------------------------------------------
#include <Radist.h>
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USART_PC_vect) { receiving(); }
ISR(USART_TIC_vect)
{
    //ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от TIC контроллера
    //Принимаем символы. Поэтому всё сводится к байтам (есть запрещённые байты).
    //Принимаем байт, что бы там нибыло
	TIC_buf = *USART_TIC.DATA;//->3(95нс)
    cli_TIC;
    //Если МК ожидает байты на ретрансмит
	if((TIC_State == USART_TIC_State_receiving) || (TIC_State == USART_TIC_State_receiving_TICstatus))
	{
		TIC_timer.CTRLA = TC_Off;					//Выключаем таймер
		TIC_timer.CNT = 0;							//Обнуляем таймер
		//Если принятый байт равен
		//			   <*>				<=>				 <#>  , то обнуляем принятые данные
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
		TIC_MEM[TIC_MEM_length] = TIC_buf;			//Сохраняем байт
		TIC_MEM_length++;
		//			   <\r>
		if (TIC_buf == 13)
		{
			if (TIC_State == USART_TIC_State_receiving) { transmit(TIC_MEM, TIC_MEM_length); }		//Посылаем всё что накопилось на ПК
			else { TIC_decode(); }	//При неудачной декодировке HVE уже выключено в декодере
			TIC_MEM_length = 0;
			TIC_timer.CTRLA = TC_125kHz;			//Переходим в режим ожидания
			TIC_State = USART_TIC_State_ready;
		}
		else { TIC_timer.CTRLA = TC_500kHz; }//Инче ждём следующий байт
	}
	else { Errors_USART_TIC.Noise = 1; }
    sei_TIC;
}
ISR(RTC_OVF_vect)
{
    //ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
    //ФУНКЦИЯ: Остановка счётчиков импульсов
    cli();
    while (RTC.STATUS != 0)
    {
        //Ждём пока можно будет обратиться к регистрам RTC
    }
	asm(
        "LDI R16, 0x00			\n\t"//Ноль для останова всех счётчиков (запись в источник сигналов)
        "STS 0x0800, R16		\n\t"//COA: Адрес TCC0.CTRLA = 0x0800 <- Ноль
        "STS 0x0900, R16		\n\t"//COB: Адрес TCD0.CTRLA = 0x0900 <- Ноль
        "STS 0x0A00, R16		\n\t"//COC: Адрес TCE0.CTRLA = 0x0A00 <- Ноль
		"STS 0x0400, R16		\n\t"//RTC: Адрес RTC.CTRL   = 0x0400 <- Ноль
    );
	if(RTC_delay)
	{
		RTC_delay = 0;
		while (RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.PER = RTC_MeasurePeriod;
		while (RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CNT = 0;
		//начали
		while (RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CTRL = RTC_MeasurePrescaler;
		asm(
		"LDI R16, 0x08		\n\t"//TCC0:Код канала событий 0 = 0x08
		"LDI R17, 0x0A		\n\t"//TCD0:Код канала событий 2 = 0x0A
		"LDI R18, 0x0C		\n\t"//TCE0:Код канала событий 4 = 0x0C
		//"LDS R19, 0x2078	\n\t"//RTC: Адрес RTC_Prescaler  = 0x2078
		"STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
		"STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
		"STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
		//"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_MeasurePrescaler(@0x2078)
		);
	}
	while (RTC.STATUS != 0)
	{
		//Ждём пока можно будет обратиться к регистрам RTC
	}
	RTC_ElapsedTime = RTC.CNT;
	while (RTC.STATUS != 0)
	{
		//Ждём пока можно будет обратиться к регистрам RTC
	}
	RTC.CNT = 0;
	sei();
	//сохраняем результаты
	COA_Measurment = COA.CNT;
	COB_Measurment = COB.CNT;
	COC_Measurment = COC.CNT;
    RTC_setStatus_ready;
    //Отправляем асинхронное сообщение
	transmit_3rytes(TOKEN_ASYNCHRO, LAM_RTC_end, RTC_Status);
}
static void ISR_COA(void)
{
    if (COA_OVF != 65535)
    {
        //Если COX_OVF не достиг предела, то +1
        COA_OVF++;
    }
    else
    {
        //Если COX_OVF достиг предела, то выключаем счётчик и устанавливаем у него всё на максимум
        COA.CTRLA = 0;
        COA.CNT = 65535;
    }
}
static void ISR_COB(void)
{
    if (COB_OVF != 65535)
    {
        //Если COX_OVF не достиг предела, то +1
        COB_OVF++;
    }
    else
    {
        //Если COX_OVF достиг предела, то выключаем счётчик и устанавливаем у него всё на максимум
        COB.CTRLA = 0;
        COB.CNT = 65535;
    }
}
static void ISR_COC(void)
{
    if (COC_OVF != 65535)
    {
        //Если COX_OVF не достиг предела, то +1
        COC_OVF++;
    }
    else
    {
        //Если COX_OVF достиг предела, то выключаем счётчик и устанавливаем у него всё на максимум
        COC.CTRLA = 0;
        COC.CNT = 65535;
    }
}
static void ISR_TIC_timer(void)
{
    //ПРЕРЫВАНИЕ: Предделитель для таймаута между выходами на связь: 32МГц на 256 = 125кГц на 25000 тиков = 0.2мс
    //Во время приёма байтов от TIC таймер служит таймаутом приёма.
	
    cli_TIC;
    TIC_timer.CTRLA = TC_Off;
    TIC_timer.CNT = 0;
    switch (TIC_State)
    {
        case USART_TIC_State_receiving: //Мы не ожидали завершения передачи! Передача прервана! Время вышло!
			Errors_USART_TIC.Silence = 1;
            TIC_State = USART_TIC_State_ready;		//Ждём начала передачи
            TIC_timer.CTRLA = TC_125kHz;		//Переходим в режим тишины
            break;
        case USART_TIC_State_ready:	//Время пришло! Пора связаться с TIC'ом!
            TIC_State = USART_TIC_State_receiving_TICstatus;	//Ждём начала передачи
            TIC_request_Status();
            TIC_timer.CTRLA = TC_500kHz;			//Переходим в режим приёма
            break;
        case USART_TIC_State_receiving_TICstatus:	//TIC не завершил передачу! Или вообще не вышел на связь!
			cli();
			if (TIC_Online)
			{
				TIC_HVE_offlineCount += 1;
				if(TIC_HVE_offlineCount > 2)
				{
					//TIC не вышел на связь и в третий раз! Что-то нетак! Принимаем меры!
					pin_iHVE_high;						//dвыключаем высокое
					Flags.iHVE = 1;						//Блокируем HVE
					Flags.PRGE = 0;						//Снимаем програмное разрешение оператора
					TIC_Online = 0;						//Нет связи с TIC'ом
					transmit_3rytes(TOKEN_ASYNCHRO, CRITICAL_ERROR_TIC_HVE_error_noResponse, TIC_MEM_length);
				}
			}
			sei();
			Errors_USART_TIC.HVE_TimeOut = 1;		//Отмечаем в журнале
			TIC_State = USART_TIC_State_ready;		
			TIC_timer.CTRLA = TC_125kHz;		//Переходим в режим тишины
            break;
        default: //Внутренняя ошибка! Неверное состояние!
            cli();
            pin_iHVE_high;						//блокируем HVE
            Flags.iHVE = 1;
            Flags.PRGE = 0;
			sei();
            Errors_USART_TIC.HVE_error = 1;		//Отмечаем в журнале
            Errors_USART_TIC.wrongTimerState = 1;
            transmit_3rytes(TOKEN_ASYNCHRO, INTERNAL_ERROR_TIC_State, TIC_State);
            break;
    }
    sei_TIC;
}
static void ISR_PC_timer(void)
{
	PC_receiving = 0;	//Завершаем приём
    PC_timer.CTRLA = TC_Off;
    PC_timer.CNT = 0;
}
//-----------------------------------------ФУНКЦИИ------------------------------------------------
//USART PC
byte receive(void)
{
	return *USART_PC.DATA;
}
void decode(void)
{
    //ФУНКЦИЯ: Расшифровываем команду
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
    //ФУНКЦИЯ: Запускаем счётчики на определённое время
    //ДАННЫЕ: <Command><RTC_PRE><RTC_PER[1]><RTC_PER[0]>
    cli();
    if ((RTC_Status != RTC_Status_busy))
    {
        //подготовка
        while (RTC.STATUS != 0)
        {
            //Ждём пока можно будет обратиться к регистрам RTC
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
            //Ждём пока можно будет обратиться к регистрам RTC
        }
        RTC.CNT = 0;
        COA.CNT = 0;
        COB.CNT = 0;
        COC.CNT = 0;
		RTC_MeasurePrescaler = PC_MEM[1];
        //начали
        while (RTC.STATUS != 0)
        {
            //Ждём пока можно будет обратиться к регистрам RTC
        }
        RTC.CTRL = PC_MEM[1];
        asm(
            "LDI R16, 0x08		\n\t"//TCC0:Код канала событий 0 = 0x08
            "LDI R17, 0x0A		\n\t"//TCD0:Код канала событий 2 = 0x0A
            "LDI R18, 0x0C		\n\t"//TCE0:Код канала событий 4 = 0x0C
            //"LDS R19, 0x2078	\n\t"//RTC: Адрес RTC_Prescaler  = 0x2078
            "STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
            "STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
            "STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
            //"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_MeasurePrescaler(@0x2078)
        );
        //отчёт
        transmit_2rytes(COMMAND_COUNTERS_start, RTC_Status);
        RTC_setStatus_busy;
    }
    else
    {
        //ЗАПРЕЩЕНО! Счётчики считают!
        transmit_2rytes(COMMAND_COUNTERS_start, RTC_Status);
    }
    sei();
}
void COUNTERS_sendResults(void)
{
    //ФУНКЦИЯ: Послать результаты счёта на ПК, если можно
    //ДАННЫЕ: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><COВ_M[1]><COВ_M[0]><COC_OVF[1]><COC_OVF[0]><COС_M[1]><COС_M[0]><RTC_ElapsedTime[1]><RTC_ElapsedTime[0]><RTC_MeasurePrescaler>
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
    //ФУНКЦИЯ: Принудительная остановка счётчиков
    if (RTC_Status == RTC_Status_busy)
    {
        while (RTC.STATUS != 0)
        {
            //Ждём пока можно будет обратиться к регистрам RTC
        }
        RTC.CTRL = RTC_PRESCALER_OFF_gc;
        tc_write_clock_source(&COA, TC_CLKSEL_OFF_gc);
        tc_write_clock_source(&COB, TC_CLKSEL_OFF_gc);
        tc_write_clock_source(&COC, TC_CLKSEL_OFF_gc);
        //Могут быть траблы, внимательней
        COA.CNT = 0;
        COB.CNT = 0;
        COC.CNT = 0;
        while (RTC.STATUS != 0)
        {
            //Ждём пока можно будет обратиться к регистрам RTC
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
	//ФУНКЦИЯ: Задаёт напряжения:
	//			- Сканирующее		(DAC_Scaner - 2 канал (25))
	//			- Дополнительное	(DAC_Scaner - 1 канал (24))
	//			- Конденсатор		(DAC_Condensator - 1 канал (24))
	//		   Запускает задержку
	//		   После задержки производит счёт
	//Принятый пакет:	<33><SV.1><SV.2><PSV.1><PSV.2><C.1><C.2>
	//					<delay_ms.1><delay_ms.2>
	//					<measure_ms.1><measure_ms.2><cs>
	//
	//Ответ:	<33><cs>
	
	//PC_MEM_length = 11;
	//PC_MEM[0] = 33;	/* <cm> */		PC_MEM[4] = 4;	/* <PSV.2> */	PC_MEM[8] = 96; /* <D_ms.2> */	
	//PC_MEM[1] = 56;	/* <SV.1> */	PC_MEM[5] = 65;	/* <C.1> */		PC_MEM[9] = 0; /* <M_ms.1> */	
	//PC_MEM[2] = 4;	/* <SV.2> */	PC_MEM[6] = 4;	/* <C.2> */		PC_MEM[10] = 50; /* <M_ms.2> */
	//PC_MEM[3] = 43;	/* <PSV.1> */	PC_MEM[7] = 1;	/* <D_ms.1> */	
	
	//Сдвигаем биты
	PC_MEM[1] = (PC_MEM[1] << 2) + (PC_MEM[2] >> 6);
	PC_MEM[2] = (PC_MEM[2] << 2);
	PC_MEM[3] = (PC_MEM[3] << 2) + (PC_MEM[4] >> 6);
	PC_MEM[4] = (PC_MEM[4] << 2);
	PC_MEM[5] = (PC_MEM[5] << 2) + (PC_MEM[6] >> 6);
	PC_MEM[6] = (PC_MEM[6] << 2);
	//0:0
	//Выставляем настройки для SPI устройств
	if ((RTC_Status != RTC_Status_busy))
	{
		SPI_confDACs_toDoubleRef();		//Настраиваем референс ЦАПов
		uint8_t spi_data[] = {25, PC_MEM[1], PC_MEM[2]}; //Устанавливаем Сканирующее
		spi_select_device(&SPIC, &DAC_Scaner);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Scaner);
		spi_data[0] = 24;	//Переключаемся на первый канал
		spi_data[1] = PC_MEM[3];//Устанавливаем Дополнительное
		spi_data[2] = PC_MEM[4];
		spi_select_device(&SPIC, &DAC_Scaner);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Scaner);
		spi_data[1] = PC_MEM[5];//Устанавливаем Конденсатор
		spi_data[2] = PC_MEM[6];
		spi_select_device(&SPIC, &DAC_Condensator);
		spi_write_packet(&SPIC, spi_data, 3);
		spi_deselect_device(&SPIC, &DAC_Condensator);
		//12101:378 мс
		//Вычисляем задержку и время экспозиции
		uint16_t delay_time = ((uint16_t)PC_MEM[7] << 8) + PC_MEM[8];//Время в микросекундах(от 1 мс до 65535 с)
		uint32_t dummy = COUNTERS_msToTicks(delay_time);
		if (dummy == 0) { transmit_2rytes(COMMAND_COUNTERS_delayedStart, 0); return; }
		RTC_DelayPrescaler = (byte)(dummy >> 16);
		RTC_DelayPeriod = (uint16_t)dummy;
		uint16_t measure_time = ((uint16_t)PC_MEM[9] << 8) + PC_MEM[10];
		dummy = COUNTERS_msToTicks(measure_time);
		if (dummy == 0) { transmit_2rytes(COMMAND_COUNTERS_delayedStart, 0); return; }
		RTC_MeasurePrescaler = (byte)(dummy >> 16);
		RTC_MeasurePeriod = (uint16_t)dummy;
		//18212:569 мкс
		//Инициируем задержку
		RTC_delay = 1;
		//подготовка
		while (RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
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
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CNT = 0;
		COA.CNT = 0;
		COB.CNT = 0;
		COC.CNT = 0;
		//начали
		while (RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CTRL = RTC_DelayPrescaler;
		transmit_2rytes(COMMAND_COUNTERS_delayedStart, 1);
		RTC_setStatus_busy;
	}
	else
	{
		//ЗАПРЕЩЕНО! Счётчики считают!
		transmit_2rytes(COMMAND_COUNTERS_delayedStart, 2);
	}
}
uint32_t COUNTERS_msToTicks(uint16_t ms)
{
	//ФУНКЦИЯ: Переводит миллисекунды в тики и предделитель
	//ВОЗВРАЩАЕТ: <x><prescaler><ticks_1><ticks_2>
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
	else { return 0; }//Неверное значение
	ticks = ms * frequency;//Исполнение: 2846:88,9 мкс
	answer = (uint32_t)(((uint32_t)prescaler << 16) + ticks);
	//2931:91,6 мкс
	return answer;
}
//TIC
void TIC_decode(void)
{
    //ФУНКЦИЯ: Декодируем ответ тика на запрос HVE {"?V902"+'\r'}
    //ПОЯСНЕНИЯ: Ответ TIC'а должен быть таким: 
    //TIC_MEM_length = 22;
	//TIC_MEM[0] = 61;			TIC_MEM[8] = 48;  /*B*/		TIC_MEM[16] = 52; /*R1*/
	//TIC_MEM[1] = 86;			TIC_MEM[9] = 59;			TIC_MEM[17] = 59;
	//TIC_MEM[2] = 57;			TIC_MEM[10] = 48; /*G1*/	TIC_MEM[18] = 52; /*R2*/
	//TIC_MEM[3] = 48;			TIC_MEM[11] = 59;			TIC_MEM[19] = 59;
	//TIC_MEM[4] = 50;			TIC_MEM[12] = 48; /*G2*/	TIC_MEM[20] = 48; /*R3*/
	//TIC_MEM[5] = 32;			TIC_MEM[13] = 59;			TIC_MEM[21] = 13;
	//TIC_MEM[6] = 52; /*Т*/	TIC_MEM[14] = 48; /*G3*/
	//TIC_MEM[7] = 59;			TIC_MEM[15] = 59;
    //Байт:   61 86 57 48 50  32  ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?  59 ?       59 ?        13
    //Символ: =  V  9  0  2  <sp> T  ;  B  ;  G1 ;  G2 ;  G3 ;  R1 ;  R2 ;  R3 ;  AlertID ;  Priority <\r>
    //Номер:  0  1  2  3  4   5   6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22      23 24       25
    //0:0
	if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 48) && (TIC_MEM[4] == 50) && (TIC_MEM[5] == 32) && (TIC_MEM[TIC_MEM_length - 1] == 13))
    {
		//54:1,69 мкс
		TIC_Online = 1;			//Обёртка пакета корректная, значит TIC на связи
		//Копируем сообщение TIC'a в буфер статуса (только статусы)
		cli();
		TIC_Status[0] = COMMAND_TIC_getStatus;
		TIC_Status_length = TIC_MEM_length - 6;
		for (byte i = 0; i < TIC_Status_length; i++) { TIC_Status[i+1] = TIC_MEM[i+6]; }
		sei();
		//820:25,6 мкс
        //Преобразуем ASCII числа в байты
        byte Turbo, R1, R2, R3;	//Интерисующие нас параметры
		byte semicolon_counter = 0;
		//Ищем ';' и добираемся до R1
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
					case 0: //Начнём попрядку с турбика (Т). Он может иметь состояние 0...7
						Turbo = TIC_decode_ASCII(TIC_MEM[i]);
						break;
					case 5: //Далее подряд три реле. Они могут иметь состояние 0...4
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
		//1645:51,4 мкс
		if ((Turbo <= 7) && (R1 <= 4 )&& (R2 <= 4) && (R3 <= 4))
		{
			//Все статусы корректны. Идём дальше
			byte Turbo_approval = 0;
			byte R2_approval = 0;
			//Статусы турбика:
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
			//Статусы реле:
			//		OffState				0
			//		OffGoingOnState			1
			//		OnGoingOffShutdownState	2
			//		OnGoingOffNormalState	3
			//		OnState					4
			switch(R1)
			{
				case 4: pin_SEMV1_high;	//Открываем вентиль "форик <-> турбик"
					break;
				default: 
					if ((PORTD.OUT & 2) >> 1)
					{
						TIC_R1_jitter++;
						if(TIC_R1_jitter >= TIC_R1_jitter_MAX)
						{
							pin_SEMV1_low;	//Закрываем вентиль "форик <-> турбик"
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
				Мы будем заваливать TIC сообщения каждые 200 мс!!!
				Здесь также надо закрыть вентиль 1
				case 1: //АВАРИЯ! Вырубить всё нафиг!
					pin_iHVE_high;	//Выключаем высокие напряжения
					//Команда TIC'у вырубить всё!
					//					!   C   9   3   3  sp   0  '\r'
					byte Message[8] = {33, 67, 57, 51, 51, 32, 48, 13};
					for (byte i = 0; i < 8; i++) { usart_putchar(USART_TIC, Message[i]); }
					Flags.iHVE = 1;
					Flags.PRGE = 0;
					transmit_2rytes(TOKEN_ASYNCHRO, LAM_TIC_Crash);
					TIC_HVE_Error_sent = 0;	//Всё штатно, будем посылать LAM при ошибке
					TIC_HVE_offlineCount = 0;//обнуляем счётчик ошибок
					return;						//Вылим от седова
				default: 
				break;
			}*/
			//1715:53,6 мкс
			if (Turbo_approval && R2_approval)
			{
				//И турбик и реле дают добро на включение высоких напряжений ну и мы тогда дадим добро
				if(Flags.iHVE == 1)
				{
					//На пине iHVE высокий потенциал - он блокирует работу DC-DC 24-12. Высокого напряжения нет. Не должно быть...
					Flags.iHVE = 0;	//Разрешаем высокое!
					transmit_2rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_approve); //Твитнем компьютеру
				}
			} 
			else
			{
				if(Flags.iHVE == 0)
				{
					TIC_disprove_jitter++;
					if(TIC_disprove_jitter >= TIC_disprove_jitter_MAX)
					{
						//На пине iHVE низкий потенциал - он разрешает работу DC-DC 24-12. Высокое напряжения есть! Должно быть...
						//Выключаем высокое!
						pin_iHVE_high;
						Flags.iHVE = 1;
						Flags.PRGE = 0;
						byte TiR = ((Turbo << 4) & 0xf0) + (R2 & 0x0f); //Пошлём вместе с сигналом состояние турбика и реле 0xTR; T от 0 до 7, R от 0 до 4
						transmit_3rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_disapprove, TiR);
						TIC_disprove_jitter = 0;
					}
				}
			}
			TIC_HVE_Error_sent = 0;	//Всё штатно, будем посылать LAM при ошибке
			TIC_HVE_offlineCount = 0;//обнуляем счётчик ошибок
			return;
			//8218:257 мкс
		}
	}
    //Если в декодироваке пошло что-то не так то спускаемся сюда.
    //Вопервых вырубаем HVE. TIC что-то темнит.
	TIC_HVE_offlineCount++;
	if(TIC_HVE_offlineCount > 2)
	{
		//TIC мелет чушь и в третий раз! Что-то нетак! Принимаем меры!
		pin_iHVE_high;						//блокируем HVE
		Flags.iHVE = 1;
		Flags.PRGE = 0;
		if(TIC_HVE_Error_sent == 0)
		{
			transmit_3rytes(TOKEN_ASYNCHRO, CRITICAL_ERROR_TIC_HVE_error_decode, TIC_MEM_length);
			TIC_HVE_Error_sent = 1;
		}
		TIC_HVE_offlineCount = 0;//обнуляем счётчик ошибок
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
    //ФУНКЦИЯ: Ретрансмитит команду на TIC, если нет опроса HVE, если опрос HVE есть - ждёт ответа от TIC'а на опрос, а потом только ретрансимитит.
	MC_Tasks.retransmit = 1;
}
void TIC_request_Status(void)
{
    //ФУНКЦИЯ: Запрашиваем у TIC'а статус
	//cli();
    for (uint8_t i = 0; i < 6; i++) { usart_putchar(USART_TIC, TIC_HVE_Message[i]); }	//Отправляем
	//sei();
}
void TIC_send_TIC_MEM(void)
{
    //ФУНКЦИЯ: Возвращает память TIC_MEM и TIC_Length
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
	//ФУНКЦИЯ: Отсылает компьютеру последние актуальные данные о TIC'е
	transmit(TIC_Status,TIC_Status_length);
}
void TIC_setJitter(byte Jitter_TIC_disprove, byte Jitter_TIC_R1_off)
{
	//ФУНКЦИЯ: Устанавливает количество допустимых дрожаний
	TIC_R1_jitter = 0;
	TIC_disprove_jitter = 0;
	TIC_R1_jitter_MAX = Jitter_TIC_R1_off;
	TIC_disprove_jitter_MAX = Jitter_TIC_disprove;
	byte aswDATA[1] = {COMMAND_TIC_setJitter};
	transmit(aswDATA, 1);
}
//Прочие
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
	SPI_confDACs_toDoubleRef();					//Вновь настраиваем референс
    uint8_t SPI_rDATA[] = {0, 0};				//Память SPI для приёма данных (два байта)
    //Если устройство DAC AD5643R то посылаем данные по его протоколу, откликаемся и выходим
    if (DAC_is_AD5643R)
    {
        //Сконфигурированы ли ЦАПы?
        uint8_t sdata[] = {PC_MEM[1], PC_MEM[2], PC_MEM[3]};
        spi_select_device(&SPIC, &SPI_DEVICE);
        spi_write_packet(&SPIC, sdata, 3);
        spi_deselect_device(&SPIC, &SPI_DEVICE);
        //откликаемся
        uint8_t aswDATA[] = {PC_MEM[0]};
        transmit(aswDATA, 1);
        return;
    }
    //Если SPI-устройство - ЦАП, то посылаем, откликаемся и выходим.
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
    //Если SPI-устройство - АЦП, то посылаем, получаем ответ, отсылаем ответ.
    uint8_t sdata[] = {PC_MEM[1], PC_MEM[2]};
    gpio_set_pin_low(pin_iRDUN);
    spi_write_packet(&SPIC, sdata, 2);
    gpio_set_pin_high(pin_iRDUN);
    //Читаем два байта
    spi_deselect_device(&SPIC, &SPI_DEVICE);
    gpio_set_pin_low(pin_iRDUN);
    spi_read_packet(&SPIC, SPI_rDATA, 2);
    gpio_set_pin_high(pin_iRDUN);
    spi_select_device(&SPIC, &SPI_DEVICE);
    //Передём ответ на ПК по USART
    uint8_t aswDATA[] = {PC_MEM[0], SPI_rDATA[0], SPI_rDATA[1]};
    transmit(aswDATA, 3);
}
void SPI_confDACs_toDoubleRef(void)
{
	uint8_t SPI_DATA[] = {0,0,0};
	//DPS + PSIS DAC'и AD5328R (Детектор и Ионный Источник) - двойной референс
	SPI_DATA[0] = AD5328R_confHbyte;
	SPI_DATA[1] = AD5328R_confLbyte;
	spi_select_device(&SPIC, &DAC_Inlet);
	spi_select_device(&SPIC, &DAC_Detector);
	spi_select_device(&SPIC, &DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC, &DAC_Inlet);
	spi_deselect_device(&SPIC, &DAC_Detector);
	spi_deselect_device(&SPIC, &DAC_IonSource);
	//MSV DAC'и AD5643R (Конденсатор и сканер) - двойной референс
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
	//ФУНКЦИЯ: Возвращает компьютеру компактный пакет из всех напряжений всех ADC.
	//СПИСОК ОПРОСА:	PSIS	- PortA.1 - 0x600.2		- Ионный источник (4 канала: тока эмиссии, ионизации, фокусное 1, фокусное 2)
	//					DPS		- PortA.5 - 0x600.32		- Детектор (3 канала: детектор 1, детектор 2, детектор 3)
	//					MSV		- PortA.2 - 0x600.4	- Сканирующее (4 канала: сканирующее, дополнительное, конденсатор "+", конденсатор "-")
	//					PSInl	- PortE.0 - 0x680.1		- Натекатель (2 канала: натекатель, нагреватель)
	//				------------------------------
	//					ИТОГО:	-	13 каналов
	//ПОРЯДОК:  - Собрать все напряжения
	//			- Отправить компьютеру
	//ПОЯСНЕНИЕ: Опрос будем производить хитрым образом. Так как по "вине" Юрия Витальевича у нас при любом опросе
	//			любого АЦП отрабатывают все АЦП, то мы можем "сообщение-пустышку" посылать не пустышку, а запрос следующего канала.
	byte answer[28];
	answer[0] = COMMAND_SPI_get_AllVoltages;
	/*Не работает
	//0:0 с
	byte Channels[5] = {16, 131, 135, 139,  143};	//[0] - Последний байт для АЦП (одинаковый для всех);
	//												[1...4] - Первый байт для АЦП содержащий адрес канала (соответствует цифре)
	#define Order_length  14
	//Порядок опроса каналов (Последний - "пустышка")
	//								IS_EC						IS_IV						IS_F1						IS_F2	 					D_1	 						D_2	 						D_3	 						C+		 					C-		 					SV		 					PSV	 						Inl	 						Heat	 					DUMMY
	byte Order[Order_length] =		{1,							2,							3,							4,      					1,      					2,      					3,		 					1,		 					2,		 					3,		 					4,		 					1,		 					2,		 					1					};	
	uint16_t Port[Order_length] =	{(uint16_t)&PORTA.OUTTGL, (uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTA.OUTTGL,	(uint16_t)&PORTE.OUTTGL,	(uint16_t)&PORTE.OUTTGL,	(uint16_t)&PORTA.OUTTGL	};
	byte Pin[Order_length] =		{2,			 				2,							2,							2,	     					32,	 						32,	 						32,	 						4,		 					4,		 					4,		 					4,		 					1,	     					1,		 					1					};
	byte ch = 0;					//указатель канала
	byte a = 1;				//Указатель последнего элемента массива answer
	//Первая запись, ответ не читаем
	//409:12,78 мкс
	for (byte i = 0; i < Order_length; i++)
	{
		pin_iRDUN_low;						//Открытие сеанса
		DWR(Port[ch], Pin[ch]);		//Выбираем на чтение предыдущий АЦП
		SPIC.DATA = Channels[Order[i]];		//Запись (первый байт)
		spi_wait_packet(&SPIC);
		//while (!(SPIC.STATUS & 0x80)) { }	//1032:32,25 мкс//Ожидание ответа
		pin_iRDUN_high;						//Закрытие сеанса
		answer[a++] = SPIC.DATA;			//Чтение
		pin_iRDUN_low;						//Открытие сеанса
		SPIC.DATA = Channels[0];			//Запись (последний байт)
		spi_wait_packet(&SPIC);
		//while (!(SPIC.STATUS & 0x80)) { }	//Ожидание ответа
		pin_iRDUN_high;						//Закрытие сеанса
		DWR(Port[ch], Pin[ch]);				//Снимаем чтение предыдущего АЦП
		answer[a++] = SPIC.DATA;			//Чтение
		ch++;
		if(i == 0) { a = 1; ch = 0; }		//Обнуление, если это был первый раз, чтобы читать предыдущий
		//2676:83,63 мкс (один цикл)
	}
	//*/
	///*Работает
	byte SPI_rDATA[2] = {0,0};
	byte sdata[] = {131, 16}; //IS_EC
	gpio_set_pin_low(pin_iRDUN);
	spi_write_packet(&SPIC, sdata, 2);
	gpio_set_pin_high(pin_iRDUN);
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
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
	//Читаем два байта
	spi_deselect_device(&SPIC, &ADC_Inlet);
	gpio_set_pin_low(pin_iRDUN);
	spi_read_packet(&SPIC, SPI_rDATA, 2);
	gpio_set_pin_high(pin_iRDUN);
	spi_select_device(&SPIC, &ADC_Inlet);
	answer[25] = SPI_rDATA[0];
	answer[26] = SPI_rDATA[1];
	//*/
	//Форимируем байт флагов <HVE_port				|		iHVE		|		PRGE		|		iEDCD				|		SEMV1			|		SEMV2				|		SEMV3			|	SPUMP>
	answer[27] =			((PORTC.OUT & 8) << 4) + (Flags.iHVE << 6) + (Flags.PRGE << 5) + ((PORTA.OUT & 128) >> 3) + ((PORTD.OUT & 2) << 2) + ((PORTD.OUT & 16) >> 2) + ((PORTD.OUT & 32) >> 4) + (PORTD.OUT & 1);
	//32133:1 мс - неверно
	transmit(answer, 28);					//Отправляем ответ компьютеру				
	//113819:3,5 мс - неверно
}
//Флаги
void updateFlags(void)
{
    //ФУНКЦИЯ: МК осматривает флаговые пины портов и собирает их в байт Flags
    //Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
    Flags.iEDCD = (PORTA.OUT & 128) >> 7;
    Flags.SEMV1 = (PORTD.OUT & 2) >> 1;
    Flags.SEMV2 = (PORTD.OUT & 16) >> 4;
    Flags.SEMV3 = (PORTD.OUT & 32) >> 5;
    Flags.SPUMP = PORTD.OUT & 1;
}
void checkFlag_HVE(void)
{
    //ФУНКЦИЯ: Возвращает всё о HVE
    //ПАКЕТ: <Command><pin_iHVE><Flags.HVE><onGauge><onLevel[1]><onLevel[0]><offGauge><offLevel[1]><offLevel[1]><monitoringEnabled>
    //uint8_t DATA[] = {COMMAND_Flags_HVE, ((PORTC.OUT & 8) >> 3), Flags.iHVE, TIC_HVE_onGauge, (TIC_HVE_onLevel >> 8), TIC_HVE_onLevel, TIC_HVE_offGauge, (TIC_HVE_offLevel >> 8), TIC_HVE_offLevel, TIC_timer.CTRLA };
    //transmit(DATA, 10);
	uint8_t DATA[] = {COMMAND_Flags_HVE, ((PORTC.OUT & 8) >> 3), Flags.iHVE};
	transmit(DATA, 3);
}
void checkFlag_PRGE(void)
{
    //ФУНКЦИЯ: Возвращает или устанавливает PRGE
    //ПАКЕТ: <Command><getOrSet>
    //					<0>\<1> - устанавливают
    //					<any_else> - запрашивает
	//*
    switch (PC_MEM[1])
    {
        case 0: //Установка в ноль
            pin_iHVE_high;			//Выключаем DC-DC 24-12
            Flags.PRGE = 0;			//Оператор запрещает
            break;
        case 1://и если iHVE ноль - TIC даёт добро, на высокое напряжение - установка в единицу
            if (Flags.iHVE == 0)
            {
                //То выдать логический ноль на iHVE (низкий потенциал разрешает работу DC-DC 24-12)
                Flags.PRGE = 1;	//Оператор даёт добро
                MC_Tasks.turnOnHVE = 1;//начать всяческие настроки DAC'ов после разбора флагов
                break;
            }
            else
            {
                transmit_2rytes(COMMAND_Flags_PRGE, 254);	//TIC запрещает.
                return;
            }
            return;
        default: //запрос
            break;
    }
    transmit_2rytes(COMMAND_Flags_PRGE, Flags.PRGE);
	//*/
	//Взлом системы безопасности
	/*
		switch (PC_MEM[1])
		{
			case 0: //Установка в ноль
				Flags.PRGE = 0;			//Выключаем безусловно
				PORTC.OUTSET = 8;
				break;
			case 1: //Включаем безусловно
				Flags.PRGE = 1;
				MC_Tasks.turnOnHVE = 1;//начать всяческие настроки DAC'ов после разбора флагов
				break;
			return;
			default: //запрос
			break;
		}
		transmit_2bytes(COMMAND_Flags_PRGE, Flags.PRGE);
	//*/
}
void checkFlag_EDCD(void)
{
    //ФУНКЦИЯ: Включает или выключает дистанционное управление напряжением на детекторах, вроде.
    //ПАКЕТ: <Command><OnOrOff>
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
    //ФУНКЦИЯ: Включает или выключает вентиль
    //ПАКЕТ: <Command><OnOrOff>
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
    //ФУНКЦИЯ: Включает или выключает вентиль
    //ПАКЕТ: <Command><OnOrOff>
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
    //ФУНКЦИЯ: Включает или выключает вентиль
    //ПАКЕТ: <Command><OnOrOff>
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
    //ФУНКЦИЯ: Включает или выключает вентиль
    //ПАКЕТ: <Command><OnOrOff>
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
    pin_iHVE_low; //Включаем DC-DC 24-12
    cpu_delay_ms(2000, 32000000); //iHVE включает довольно иннерционную цепь, поэтому надо обождать.
    //Высокое напряжение включено - конфигурируем DACи
	uint8_t SPI_DATA[] = {0,0,0};
    //DPS + PSIS DAC'и AD5328R (Детектор и Ионный Источник) - двойной референс
    SPI_DATA[0] = AD5328R_confHbyte;
    SPI_DATA[1] = AD5328R_confLbyte;
    spi_select_device(&SPIC, &DAC_Inlet);
    spi_select_device(&SPIC, &DAC_Detector);
    spi_select_device(&SPIC, &DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC, &DAC_Inlet);
    spi_deselect_device(&SPIC, &DAC_Detector);
    spi_deselect_device(&SPIC, &DAC_IonSource);
    //ОТКЛЮЧЕНО по электротехническим причинам!
    //PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на первом канале (Ток Эмиссии)
    SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_EC;
    SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_EC;
    spi_select_device(&SPIC,&DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC,&DAC_IonSource);
    //PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на втором канале (Ионизации)
    SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_IV;
    SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_IV;
    spi_select_device(&SPIC,&DAC_IonSource);
    spi_write_packet(&SPIC, SPI_DATA, 2);
    spi_deselect_device(&SPIC,&DAC_IonSource);
    //MSV DAC'и AD5643R (Конденсатор и сканер) - двойной референс
    SPI_DATA[0] = AD5643R_confHbyte;
	SPI_DATA[1] = AD5643R_confMbyte;
    SPI_DATA[2] = AD5643R_confLbyte;
	spi_select_device(&SPIC, &DAC_Condensator);
    spi_select_device(&SPIC, &DAC_Scaner);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Condensator);
    spi_deselect_device(&SPIC, &DAC_Scaner);
    //MSV DAC AD5643R (Конденсатор) - стартовое напряжение на первом канале
	SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_C;
	SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_C;
	SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_C;
    spi_select_device(&SPIC, &DAC_Condensator);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Condensator);
    //MSV DAC AD5643R (Сканер (сканирующее)) - стартовое напряжение на втором канале
    SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_S;
    SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_S;
    SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_S;
    spi_select_device(&SPIC, &DAC_Scaner);
    spi_write_packet(&SPIC, SPI_DATA, 3);
    spi_deselect_device(&SPIC, &DAC_Scaner);
	//MSV DAC AD5643R (Сканер (дополнительное)) - стартовое напряжение на первом канале
	SPI_DATA[0] = AD5643R_startVoltage_Hbyte_MSV_PS;
	SPI_DATA[1] = AD5643R_startVoltage_Mbyte_MSV_PS;
	SPI_DATA[2] = AD5643R_startVoltage_Lbyte_MSV_PS;
	spi_select_device(&SPIC, &DAC_Scaner);
	spi_write_packet(&SPIC, SPI_DATA, 3);
	spi_deselect_device(&SPIC, &DAC_Scaner);
	//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на втором канале (Фокусное 1)
	SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_F1;
	SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_F1;
	spi_select_device(&SPIC,&DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC,&DAC_IonSource);
	//PSIS DAC AD5328R (Ионный Источник) - стартовое напряжение на втором канале (Фокусное 2)
	SPI_DATA[0] = AD5328R_startVoltage_Hbyte_PSIS_F2;
	SPI_DATA[1] = AD5328R_startVoltage_Lbyte_PSIS_F2;
	spi_select_device(&SPIC,&DAC_IonSource);
	spi_write_packet(&SPIC, SPI_DATA, 2);
	spi_deselect_device(&SPIC,&DAC_IonSource);
	/*
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
    spi_deselect_device(&SPIC,&DAC_IonSource);
	//*/
    MC_Tasks.turnOnHVE = 0;						//Снимаем задачу
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
//-------------------------------------НАЧАЛО ПРОГРАММЫ-------------------------------------------
int main(void)
{
    confPORTs;							//Конфигурируем порты (HVE пин в первую очередь)
    cli();
    SYSCLK_init;						//Инициируем кристалл (32МГц)
    pmic_init();						//Инициируем систему прерываний
    SPIC.CTRL = 87;						//Инициируем систему SPI
    RTC_init;							//Инициируем счётчик реального времени
    Counters_init;						//Инициируем счётчики импульсов
    USART_PC_init;						//Инициируем USART с компутером
    USART_TIC_init;						//Инициируем USART с насосемъ
    //Конечная инициализация
    pointer_Flags = &Flags;
    pointer_Errors_USART_PC = &Errors_USART_PC;
    pointer_Errors_USART_TIC = &Errors_USART_TIC;
    updateFlags();
    RTC_setStatus_ready;
    Flags.iHVE = 1; //Запрещаем высокое напряжение, до тех пор пока от TIC'а на придёт разрешение
    Flags.PRGE = 0;	//Изночально oператор запрещает высокое напряжение (При запрещении от TIC операторская тоже должна запрещаться!)
    //Таймер PC
    PC_timer.PER = 65535;				//524мс на 125кГц
    PC_timer.CNT = 0;
    PC_timer.CTRLA = TC_Off;			//Вsключаем ПК таймер
    //Tаймер TIC
    TIC_State = USART_TIC_State_ready;	//Переводим USART_PC в режим ожидания
    TIC_timer.PER = 25000;				//200мс на 125кГц
    TIC_timer.CNT = 0;
	TIC_timer.CTRLA = TC_125kHz;		//Включаем TIC'овский таймер контроля статуса
    sei();								//Разрешаем прерывания
    //15627:488 мкс//Инициализация завершена
    while (1)
    {
        if (MC_Tasks.turnOnHVE) { turnOn_HV(); }
		if((MC_Tasks.retransmit)&&(TIC_State != USART_TIC_State_receiving_TICstatus))
		{
			//Значит ретрансмитим сообщение на TIC
			cli_TIC;
			TIC_timer.CTRLA = TC_Off;
			TIC_timer.CNT = 0;
			TIC_State = USART_TIC_State_receiving;	//Переходим в режим приёма на ретрансмит
			for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//Копируем всё что должны переслать
			//cli();
			for (uint8_t i = 0; i < PC_MEM_length - 1; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//Отправляем
			//sei();
			TIC_timer.CTRLA = TC_500kHz;			//Запускаем таймер в режиме приёма
			sei_TIC;
			MC_Tasks.retransmit = 0;				//Снимаем задачу
		}
    }
}
//-----------------------------------------ЗАМЕТКИ------------------------------------------------
//Ион = 3600ж - 80В
//Ф1 =4000ж - 
//Ф2 = 4000ж
//Ска - 2000
//Доп - 2000*3/4
//Конденсатор - 500
//-----------------------------------------THE END------------------------------------------------