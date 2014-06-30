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
#define version										164
#define birthday									20140627
//Счётчики
#define RTC_Status_ready							0		//Счётчики готов к работе
#define RTC_Status_stopped							1		//Счётчики был принудительно остановлен
#define RTC_Status_busy								2		//Счётчики ещё считает
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
//Состояния USART
#define	USART_State_ready							0		//USART ничего не принимает
#define USART_State_receiving						1		//USART принимает байты
#define USART_State_ending							2		//USART получил байт затвора, ожидается завершение передачи
#define USART_State_decoding						3		//USART декодирует команду
#define USART_State_HVEreceiving					4		//USART (TIC) принимает байты TIC'a на запрос HVE
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
uint8_t TIC_HVE_Message[6] = {63, 86, 57, 49, 0, 13};	//char'ы сообщения на запрос давления {?V91<NUL><\r>} (1 торр = 133,322368 Па)
uint8_t TIC_HVE_onGauge = 52;							//последний char адреса датчика (турбика). По умолчанию: Gauge_2
float TIC_HVE_onLevel = 1.3332E-02;						//четыре тетрады порога напряжения (турбика). По умолчанию: 2.000V (e-4 торр = 0,0133322368 Па -> 1,3332e-02)
uint8_t TIC_HVE_offGauge = 51;							//последний char адреса датчика (форнасоса). По умолчанию: Gauge_1
float TIC_HVE_offLevel = 9.3326E+02;					//четыре тетрады порога напряжения (форнасоса). По умолчанию: 6.700V (7 торр = 933,256576 Па -> 9,3326e+02)
uint8_t TIC_HVE_offlineCount = 0;						//Количество запросов, которые проигнорировал. 3 раза и считается аварией.
uint8_t TIC_HVE_Error_sent = 0;							//Булка: 0 - ошибка не отправлена компьютеру, 1 - ошибка уже отправлена компьютеру
//		Измерения
uint8_t  RTC_Status = RTC_Status_ready;					//Состояния счётчика
uint16_t RTC_ElapsedTime = 0;
uint8_t  RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//Предделитель RTC
uint16_t RTC_MeasurePeriod = 0;							//Период RTC
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
bool EVSYS_SetEventSource(uint8_t eventChannel, EVSYS_CHMUX_t eventSource);
bool EVSYS_SetEventChannelFilter(uint8_t eventChannel, EVSYS_DIGFILT_t filterCoefficient);
void decode(void);
void TIC_retransmit(void);
void TIC_request_HVE(void);
void TIC_decode_HVE(void);
uint8_t TIC_decode_ASCII(uint8_t ASCII_symbol);
void TIC_set_Gauges(void);
void TIC_send_TIC_MEM(void);
void SPI_send(uint8_t DEVICE_Number);
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
//-----------------------------------------ВКЛЮЧЕНИЯ----------------------------------------------
#include <Radist.h>
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTD0_RXC_vect)
{
    receiving();
}
ISR(USARTE0_RXC_vect)
{
    //ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от TIC контроллера
    //Принимаем символы. Поэтому всё сводится к байтам (есть запрещённые байты).
    //Принимаем байт, что бы там нибыло
	TIC_buf = *USART_TIC.DATA;//->3(95нс)
    cli_TIC;
    //Если МК ожидает байты на ретрансмит
	if((TIC_State == USART_State_receiving) || (TIC_State == USART_State_HVEreceiving))
	{
		TIC_timer.CTRLA = TC_Off;					//Выключаем таймер
		TIC_timer.CNT = 0;							//Обнуляем таймер
		//Если принятый байт равен
		//			   <*>				<=>				 <#>  , то обнуляем принятые данные
		if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35))
		{ 
			if(TIC_State == USART_State_receiving)
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
			if (TIC_State == USART_State_receiving)
			{ 
				transmit(TIC_MEM, TIC_MEM_length); 
			}		//Посылаем всё что накопилось на ПК
			else { TIC_decode_HVE(); }					//При неудачной декодировке HVE уже выключено в декодере
			TIC_MEM_length = 0;
			TIC_timer.CTRLA = TC_125kHz;			//Переходим в режим ожидания
			TIC_State = USART_State_ready;
		}
		else { TIC_timer.CTRLA = TC_500kHz; }//Инче ждём следующий байт
	}
	else { Errors_USART_TIC.Noise = 1; }
	/*
    switch (TIC_State)
    {
        case USART_State_receiving:	//Мы ожидали байты с TIC на ПК
            TIC_timer.CTRLA = TC_Off;					//Выключаем таймер
            TIC_timer.CNT = 0;							//Обнуляем таймер
            //Если принятый байт равен
            //			   <*>				<=>				 <#>  , то обнуляем принятые данные
            if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
            TIC_MEM[TIC_MEM_length] = TIC_buf;			//Сохраняем байт
            TIC_MEM_length++;
            //			   <\r>
            if (TIC_buf == 13)
            {
                //Если этот байт был <\r>
                transmit(TIC_MEM, TIC_MEM_length);		//Посылаем всё что накопилось на ПК
				TIC_MEM_length = 0;
                TIC_timer.CTRLA = TC_125kHz;			//Переходим в режим ожидания
                TIC_State = USART_State_ready;
            }
            else { TIC_timer.CTRLA = TC_500kHz; }//Инче ждём следующий байт
            break;
        case USART_State_HVEreceiving:	//Мы ожидаем данные от TIC'a
            TIC_timer.CTRLA = TC_Off;					//Выключаем таймер
            TIC_timer.CNT = 0;							//Обнуляем таймер
            //Если принятый байт равен
            //			    <*>				   <=>				  <#>  , то обнуляем принятые данные
            if ((TIC_buf == 42) || (TIC_buf == 61) || (TIC_buf == 35)) { TIC_MEM_length = 0; }
            TIC_MEM[TIC_MEM_length] = TIC_buf;			//Сохраняем байт
            TIC_MEM_length++;
            //			   <\r>
            if (TIC_buf == 13)
            {
                //Если декодировка прошла удачно, то отмечаем в журнале
                if (TIC_decode_HVE()) { TIC_HVE_offlineCount = 0; }
                //При неудачной декодировке HVE уже выключено в декодере
				TIC_MEM_length = 0;
				TIC_timer.CTRLA = TC_125kHz;	//запускаем таймер снова
                TIC_State = USART_State_ready;
            }
            else { TIC_timer.CTRLA = TC_500kHz; }//Инче ждём следующий байт
            break;
        default: //Мы не ждали байтов от TIC'a! Игнорируем их, но в книжечку запишем...
            Errors_USART_TIC.Noise = 1;
            break;
    }
	//*/
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
        case USART_State_receiving: //Мы не ожидали завершения передачи! Передача прервана! Время вышло!
			Errors_USART_TIC.Silence = 1;
            TIC_State = USART_State_ready;		//Ждём начала передачи
            TIC_timer.CTRLA = TC_125kHz;		//Переходим в режим тишины
            break;
        case USART_State_ready:	//Время пришло! Пора связаться с TIC'ом!
            TIC_State = USART_State_HVEreceiving;	//Ждём начала передачи
            TIC_request_HVE();
            TIC_timer.CTRLA = TC_500kHz;			//Переходим в режим приёма
            break;
        case USART_State_HVEreceiving:	//TIC не завершил передачу! Или вообще не вышел на связь!
			cli();
			TIC_HVE_offlineCount += 1;
			if(TIC_HVE_offlineCount > 2)
			{
				//TIC не вышел на связь и в третий раз! Что-то нетак! Принимаем меры!
				pin_iHVE_high;						//блокируем HVE
				Flags.iHVE = 1;
				Flags.PRGE = 0;
				if(TIC_HVE_Error_sent == 0)
				{
					transmit_3rytes(TOKEN_ASYNCHRO, CRITICAL_ERROR_TIC_HVE_error_noResponse, TIC_MEM_length);
					TIC_HVE_Error_sent = 1;
				}
			}
			sei();
			Errors_USART_TIC.HVE_TimeOut = 1;		//Отмечаем в журнале
			TIC_State = USART_State_ready;		
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
        case COMMAND_TIC_set_Gauges: 				TIC_set_Gauges();
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
        default: transmit_3rytes(TOKEN_ASYNCHRO, ERROR_DECODER_wrongCommand, PC_MEM[0]);
    }
}
//USART PC
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
//TIC
void TIC_decode_HVE(void)
{
    //ФУНКЦИЯ: Декодируем ответ тика на запрос HVE {?V91<NUL><\r>}
    //ПОЯСНЕНИЯ: Ответ TIC'а должен быть таким: ? - байт от 48 до 57
    /*
    //Байт:   61 86 57 49  ?    32   ?   46 ?    ?    ?   59 54 54 59 49 59 | 48 59 48 13
    //Символ: =  V  9  1 <NUL> <sp> <D1> . <D2> <D3> <D4> ;  6  6  ;  1  ;  | 0  ;  0 <\r>
    //Номер:  0  1  2  3   4    5    6   7  8    9    10  11 12 13 14 15 16 | 17 18 19 20
    if ((TIC_MEM_length == 21) || (TIC_MEM_length == 22))
    {
        if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 49) && (TIC_MEM[5] == 32) && (TIC_MEM[7] == 46) && (TIC_MEM[11] == 59) && (TIC_MEM[12] == 54) && (TIC_MEM[13] == 54) && (TIC_MEM[14] == 59)  && (TIC_MEM[15] == 49) && (TIC_MEM[16] == 59))// &&			(TIC_MEM[17] == 48) && (TIC_MEM[18] == 59) && (TIC_MEM[19] == 48) && (TIC_MEM[20] == 13))
        {
            //Декодируем число, которое пришло вместе с сообщением
            uint8_t Value[4] = {TIC_decode_ASCII(TIC_MEM[6]), TIC_decode_ASCII(TIC_MEM[8]), TIC_decode_ASCII(TIC_MEM[9]), TIC_decode_ASCII(TIC_MEM[10]) };
            if ((Value[0] != 255) && (Value[1] != 255) && (Value[2] != 255) && (Value[3] != 255))
            {
                //значение корректно! формируем суперзначение четырьмя тетрадами
                uint16_t Voltage = (Value[0] << 12) + (Value[1] << 8) + (Value[2] << 4) + Value[3];
                //Смотрим от какого датчика
                if ((Flags.iHVE == 1) && (TIC_MEM[4] == TIC_HVE_onGauge))
                {
                    //На пине iHVE высокий потенциал - он блокирует работу DC-DC 24-12. Высокого напряжения нет.
                    //Контролируем onLevel (турбик), чтобы включить. Присланное значение должно быть равно или ниже порогового
                    if (Voltage <= TIC_HVE_onLevel) { Flags.iHVE = 0; } //Разрешаем высокое!
    				TIC_HVE_offlineCount = 0;//отмечаем в журнале
                    return;                }
                else if ((Flags.iHVE == 0) && (TIC_MEM[4] == TIC_HVE_offGauge))
                {
                    //На пине iHVE низкий потенциал - он разрешает работу DC-DC 24-12. Высокое напряжения есть!
                    //Контролируем offLevel (форик), чтобы выключить. Присланное значение должно быть равно или выше порогового
                    if (Voltage >= TIC_HVE_offLevel)
                    {
                        //Выключаем высокое!
                        pin_iHVE_high;
                        Flags.iHVE = 1;
                        Flags.PRGE = 0;
                    }
                    TIC_HVE_offlineCount = 0;//отмечаем в журнале
                    return;
                }
            }
        }
    }
    //*/
    //Ответ TIC'a  в паскалях
    //*9,3326e+02 и 1,3332e-02
    //Байт:   61 86 57 49  ?    32   ?   46 ?    ?    ?   ?  101 43/45   ?   ?  59 53 57 59 | 48 59 48 59 48 13
    //Символ: =  V  9  1  <G>  <sp> [D]  . [D]  [D]  [D] [D] e   [+/-]  [D] [D] ;  5  9  ;  | 0  ;  0  ;  0 <\r>
    //Номер:  0  1  2  3   4    5    6   7  8    9   10  11  12   13    14  15  16 17 18 19 | 20 21 22 23 24 25
    if ((TIC_MEM[0] == 61) && (TIC_MEM[1] == 86) && (TIC_MEM[2] == 57) && (TIC_MEM[3] == 49) && (TIC_MEM[5] == 32) && (TIC_MEM[7] == 46) && (TIC_MEM[12] == 101) && (TIC_MEM[16] == 59) && (TIC_MEM[17] == 53)  && (TIC_MEM[18] == 57) && (TIC_MEM[19] == 59))
    {
        //Декодируем число, которое пришло вместе с сообщением
        uint8_t Value[5];
        uint8_t Sign;
        uint8_t Power[2];
        Value[0] = TIC_decode_ASCII(TIC_MEM[6]);	//Единицы
        Value[1] = TIC_decode_ASCII(TIC_MEM[8]);	//Десятые
        Value[2] = TIC_decode_ASCII(TIC_MEM[9]);	//Сотые
        Value[3] = TIC_decode_ASCII(TIC_MEM[10]);	//Тысячные
        Value[4] = TIC_decode_ASCII(TIC_MEM[11]);	//Десятитысячные
        Power[0] = TIC_decode_ASCII(TIC_MEM[14]);	//Десятки степени
        Power[1] = TIC_decode_ASCII(TIC_MEM[15]);	//Единицы степени
        switch (TIC_MEM[13])							//Знак степени
        {
            case 43: Sign = 1;					//+
                break;
            case 45: Sign = 0;					//-
                break;
            default: Sign = 255;
                break;
        }
        if ((Value[0] != 255) && (Value[1] != 255) && (Value[2] != 255) && (Value[3] != 255) && (Value[4] != 255) && (Power[1] != 255) && (Power[0] != 255) && (Sign != 255))
        {
            //значение корректно! Составляем число с плавающей точкой (диапазон float от 3.14E-38 до 3.14E+38, но лучше меньше, впрочем давление больше или меньше чем 6-тая степень мы не получим)
			float Pressure = 0;
			Pressure = Value[0] + Value[1] * 0.1 + Value[2] * 0.01 + Value[3] * 0.001 + Value[4] * 0.0001;//очень долгая операция
			uint8_t e = Power[0] * 10 + Power[1];
			if (Sign == 1)
			{
				for (int i = 0; i < e; i++)
				{
					Pressure = Pressure * 10;
				}
			}
			else
			{
				for (int i = 0; i < e; i++)
				{
					Pressure = Pressure * 0.1;
				}
			}
			//HVE разрешено? (метка: 500мкс)
			if(Flags.iHVE == 1)
			{
				//НЕТ! На пине iHVE высокий потенциал - он блокирует работу DC-DC 24-12. Высокого напряжения нет.
				//Контролируем onLevel (турбик), чтобы включить. Ответ прислал турбик?
				if(TIC_MEM[4] == TIC_HVE_onGauge)
				{
					//Присланное значение должно быть ниже порогового onLevel
					if (Pressure < TIC_HVE_onLevel)
					{
						//Разрешаем высокое!
						Flags.iHVE = 0; //Сюда наверное нужно что-то вроде лама - "можно включать"
						transmit_2rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_approve);
					}
					TIC_HVE_Error_sent = 0;	//Всё штатно, будем посылать LAM при ошибке
					TIC_HVE_offlineCount = 0;//обнуляем счётчик ошибок
				}
			}
			else
			{
				//ДА! На пине iHVE низкий потенциал - он разрешает работу DC-DC 24-12. Высокое напряжения есть!
				//Контролируем offLevel (форик), чтобы выключить.
				if(TIC_MEM[4] == TIC_HVE_offGauge)
				{
					//Присланное значение должно быть равно или выше порогового offLevel
					if (Pressure > TIC_HVE_offLevel)
					{
						//Выключаем высокое!
						pin_iHVE_high;
						Flags.iHVE = 1;
						Flags.PRGE = 0;
						transmit_2rytes(TOKEN_ASYNCHRO, LAM_HVE_TIC_disapprove);
					}
					TIC_HVE_Error_sent = 0;	//Система работает штатно, будем посылать LAM при ошибке
					TIC_HVE_offlineCount = 0;//обнуляем счётчик ошибок
				}
			}
            return;
        }
    }
    //*/
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
	//PC_State = USART_State_decoding;
	//transmit_2bytes(COMMAND_TIC_restartMonitoring, TIC_State);
    //while (TIC_State != USART_State_ready) { }	//Ждём
    //TIC_timer.CTRLA = TC_Off;
    //TIC_timer.CNT = 0;
    //TIC_State = USART_State_receiving;	//Переходим в режим приёма на ретрансмит
    //for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//Копируем всё что должны переслать
    //for (uint8_t i = 0; i < TIC_MEM_length; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//Отправляем
    //TIC_timer.CTRLA = TC_500kHz;			//Запускаем таймер на 6мс
}
void TIC_request_HVE(void)
{
    //ФУНКЦИЯ: Запрашиваем у TIC'а давление
    if (Flags.iHVE == 1) { TIC_HVE_Message[4] = TIC_HVE_onGauge; }	//Если HVE запрещено смотрим на onLevel(турбик)
    else { TIC_HVE_Message[4] = TIC_HVE_offGauge; }					//Если HVE разрешено смотрим на offLevel(форик)
    for (uint8_t i = 0; i < 6; i++) { usart_putchar(USART_TIC, TIC_HVE_Message[i]); }	//Отправляем
}
void TIC_set_Gauges(void)
{
    //ФУНКЦИЯ: Задаёт датчики для мониторинга HVE и пороги
    //ПОЯСНЕНИЯ: <Command><onGauge><onLevel_1><onLevel_0><offGauge><offLevel_1><offLevel_0>
    //Состояние на момент выполнения операции должно быть ready, иначе операция отменяется
    //Возвращает состояние TIC
    //transmit_2bytes(COMMAND_TIC_set_Gauges, TIC_State);
    //while (TIC_State != USART_State_ready) { }	//Ждём
    //TIC_HVE_onGauge = PC_MEM[1];
    //TIC_HVE_onLevel = (PC_MEM[2] << 8) + PC_MEM[3];
    //TIC_HVE_offGauge = PC_MEM[4];
    //TIC_HVE_offLevel = (PC_MEM[5] << 8) + PC_MEM[6];
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
    PC_timer.PER = 25000;				//200мс на 125кГц
    PC_timer.CNT = 0;
    PC_timer.CTRLA = TC_Off;			//Вsключаем ПК таймер
    //Tаймер TIC
    TIC_timer.PER = 25000;				//200мс на 125кГц
    TIC_timer.CNT = 0;
	//TIC_timer.CTRLA = TC_125kHz;		//Включаем TIC'овский таймер HVE
    TIC_State = USART_State_ready;		//Переводим USART_PC в режим ожидания
    sei();								//Разрешаем прерывания
    //Инициализация завершена
    while (1)
    {
        if (MC_Tasks.turnOnHVE)
        {
			cli_PC;
            pin_iHVE_low; //Включаем DC-DC 24-12
            cpu_delay_ms(2000, 32000000); //iHVE включает довольно иннерционную цепь, поэтому надо обождать.
            //Высокое напряжение включено - конфигурируем DACи
			uint8_t SPI_DATA[] = {0,0,0};
            //DPS + PSIS DAC'и AD5328R (Детектор и Ионный Источник) - двойной референс
            SPI_DATA[0] = AD5328R_confHbyte;
            SPI_DATA[1] = AD5328R_confLbyte;
            spi_select_device(&SPIC, &DAC_Detector);
            spi_select_device(&SPIC, &DAC_IonSource);
            spi_write_packet(&SPIC, SPI_DATA, 2);
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
		if((MC_Tasks.retransmit)&&(TIC_State != USART_State_HVEreceiving))
		{
			cli_TIC;
			TIC_timer.CTRLA = TC_Off;
			TIC_timer.CNT = 0;
			TIC_State = USART_State_receiving;	//Переходим в режим приёма на ретрансмит
			for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i - 1] = PC_MEM[i]; }	//Копируем всё что должны переслать
			for (uint8_t i = 0; i < PC_MEM_length - 1; i++) { usart_putchar(USART_TIC, TIC_MEM[i]); }	//Отправляем
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