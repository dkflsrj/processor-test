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
//МК
#define version										124
#define birthday									20131216
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
#define USART_State_HVEwaiting						4		//USART (TIC) ждёт ответа от TIC'a на мониторинговый запрос
//Стартовые конфигурации для DAC AD5643R -> двойной референс
#define AD5643R_confHbyte							56
#define AD5643R_confMbyte							0
#define AD5643R_confLbyte							1
//Стартовые напряжения для DAC AD5643R -> 8131 (половина диапазона)
#define AD5643R_startVoltage_Hbyte					24		//Адрес
#define AD5643R_startVoltage_Mbyte					127		//Старший байт напряжения
#define AD5643R_startVoltage_Lbyte					252		//Младший байт напряжения с 2 пустыми младшими битами
//Стартовые конфигурации для DAC AD5328R -> двойной референс
#define AD5328R_confHbyte							128
#define AD5328R_confLbyte							60
//Стартовые напряжение PSIS EC (тока эмиссии)
#define AD5328R_startVoltage_Hbyte_PSIS_EC			0
#define AD5328R_startVoltage_Lbyte_PSIS_EC			82
//Стартовые напряжения DAC PSIS IV,F1,F2 (ионизации, фокусные)
#define AD5328R_startVoltage_Hbyte_PSIS_IV			44
#define AD5328R_startVoltage_Lbyte_PSIS_IV			205

//----------------------------------------ПЕРЕМЕННЫЕ----------------------------------------------
//	МИКРОКОНТРОЛЛЕР
uint8_t  MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t  MC_CommandStack = 0;

uint8_t  MC_Status = 0;
//		USART PC
uint8_t PC_timer_TimeOut = 30;							//60 секунд
uint8_t PC_timer_time = 0;								//Таймер времени приёма и тишины
uint8_t PC_MEM[100];									//100 байт памяти для приёма данных USART
uint8_t PC_MEM_length = 0;								//Длина записанного в PC_MEM пакета байтов.
uint8_t PC_State = 0;									//Состояние модуля USART_PC
uint8_t PC_buf = 0;										//Буфер приёма. Содержит любой принятый байт (даже шум)
uint8_t PC_MEM_CheckSum = 0;							//Принятая контрольная сумма (из пакета)
//		USART TIC
uint8_t TIC_timer_TimeOut = 5;							//10 секунд
uint8_t TIC_timer_time = 0;								//Таймер времени приёма и тишины
uint8_t TIC_MEM[100];									//100 байт памяти для приёма данных от TIC
uint8_t TIC_MEM_length = 0;								//Длина записанного в TIC_MEM пакета байтов.
uint8_t TIC_buf = 0;									//Буфер приёма. Содержит любой принятый байт (даже шум)
uint8_t TIC_State = 0;									//Состояние модуля USART_TIC
uint8_t TIC_waitForAnswer = 0;							//bool'ка 0 - не ждём ответа от TIC'а, 1 - ждём на HVE
uint8_t TIC_HVE_Message[6] = {63,86,57,49,0,13};		//char'ы сообщения на запрос давления {?V91<NUL><\r>}
uint8_t TIC_HVE_onGauge = 0;							//последний char адреса датчика (турбика)
uint8_t TIC_HVE_onLevel[4] = {0,0,0,0};					//char'ы порога напряжения (турбика)
uint8_t TIC_HVE_offGauge = 0;							//последний char адреса датчика (форнасоса)
uint8_t TIC_HVE_offLevel[4] = {0,0,0,0};				//char'ы порога напряжения (форнасоса)
//		Измерения
uint8_t  RTC_Status = RTC_Status_ready;					//Состояния счётчика
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
//ADC у конденсатора тот же что и у сканера
//-----------------------------------------УКАЗАТЕЛИ----------------------------------------------
uint8_t *pointer_MC_Tasks;
uint8_t *pointer_Errors_USART_PC;
uint8_t *pointer_Flags;
//------------------------------------ОБЪЯВЛЕНИЯ ФУНКЦИЙ------------------------------------------
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
//------------------------------------ФУНКЦИИ ПРЕРЫВАНИЯ------------------------------------------
ISR(USARTD0_RXC_vect)
{
	//ПРЕРЫВАНИЕ:
	//ПОСЫЛКА: <KEY><DATA[...]<CS><LOCK>
	//~1..3мкс
	//Принимаем байт, что бы там нибыло
	PC_buf = *USART_PC.DATA;//->3(95нс)
	cli_PC;
	//Если в режиме приёма
	if((PC_State == USART_State_receiving)||(PC_State == USART_State_ending))
	{
		PC_timer.CNT = 0;						//Обнуляем счёт счётчика
		PC_MEM[PC_MEM_length] = PC_buf;			//Сохраняем байт
		PC_MEM_length++;						//Увеличиваем счётчик принятых байтов
		PC_State = USART_State_receiving;		//Предполагаем, что этот байт не затвор
		if (PC_buf == COMMAND_LOCK){PC_State = USART_State_ending;}	//Если получили затвор, готовимся завершить приём
	}
	else if(PC_State == USART_State_ready)
	{
		if(PC_buf == COMMAND_KEY)
		{
			//Пришёл ключ!
			PC_State = USART_State_receiving;	//Переходим в режим приёма
			PC_timer.CNT = 0;					//Обнуляем таймер
			PC_timer.CTRLA = TC_32MHz;			//Запускаем таймер на 4мс.
			PC_MEM_length = 0;					//Обнуляем счётчик принятых байтов
		}
		else { Errors_USART_PC.Noise = 1; }		//Что-то твориться на линии
	}
	else if(PC_State == USART_State_decoding) { Errors_USART_PC.TooFast = 1; } //МК не выполнил предыдущую команду
	sei_PC;
}
ISR(USARTE0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от TIC контроллера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	//Принимаем байт
	TIC_buf = *USART_TIC.DATA;//->3(95нс)
	cli_TIC;
	switch(TIC_State)
	{
		case USART_State_receiving:		//Если в режиме приёма
			TIC_timer.CNT = 0;						//Обнуляем счёт счётчика
			TIC_MEM[TIC_MEM_length] = TIC_buf;		//Сохраняем байт
			TIC_MEM_length++;						//Увеличиваем счётчик принятых байтов
			break;
		case USART_State_HVEwaiting:	//Если в режиме ожидания ответа от TIC'a на HVE
			break;
		default:
			break;
	}
	sei_TIC;
}
ISR(RTC_OVF_vect)
{
    //ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
    //ФУНКЦИЯ: Остановка счётчиков импульсов
    cli();
    asm(
        "LDI R16, 0x00			\n\t"//Ноль для останова всех счётчиков (запись в источник сигналов)
        "STS 0x0800, R16		\n\t"//COA: Адрес TCC0.CTRLA = 0x0800 <- Ноль
        "STS 0x0900, R16		\n\t"//COB: Адрес TCD0.CTRLA = 0x0900 <- Ноль
        "STS 0x0A00, R16		\n\t"//COC: Адрес TCE0.CTRLA = 0x0A00 <- Ноль
    );
    while (RTC.STATUS != 0)
    {
        //Ждём пока можно будет обратиться к регистрам RTC
    }
    RTC.CTRL = RTC_PRESCALER_OFF_gc;
	sei();
    //сохраняем результаты
    COA_Measurment = COA.CNT;
    COB_Measurment = COB.CNT;
    COC_Measurment = COC.CNT;
	RTC_setStatus_ready;
	//Отправляем асинхронное сообщение
	transmit_3bytes(TOCKEN_LookAtMe, LAM_RTC_end, RTC_Status);
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
	 //ПРЕРЫВАНИЕ: Таймаут приёма байтов от TIC и тишины на линии (МК ведущий)
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
	 //ПРЕРЫВАНИЕ: Так как предделителя не хватает (32МГц на 1024), то прерывание срабатывает каждые 2 секунды.
	 //Поэтому будем отсчитывать процессором пары секунд. Когда количество пар превысит заданное, можно выставлять
	 //флаг о потере связи с компьютером и выключать высокое напряжение.
	 //Во время приёма байтов таймер служит таймаутом приёма.
	 cli_PC;
	 switch(PC_State)
	 {
		case USART_State_receiving: //Мы не ожидали завершения передачи! Передача прервана! Время вышло!
			PC_timer.CTRLA = TC_31kHz;		//Переходим в режим тишины
			PC_timer.CNT = 0;
			PC_timer_time = 0;
			PC_State = USART_State_ready;	//Ждём начала передачи
			break;
		case USART_State_ending: //Приём успешно завершён! Можно декодировать. Проверяем длинну команды
			if(PC_MEM_length > 2)
			{
				PC_MEM_length--;				//Отсекаем последний байт (то байт затвора)
				uint8_t CheckSum = 0;			//Подсчёт контрольной суммы...
				for (uint8_t i = 0; i < PC_MEM_length; i++) { CheckSum -= PC_MEM[i]; }
				PC_MEM_length--;				//Отсекаем контрольную сумму
				if (CheckSum == PC_MEM_CheckSum) { decode();  }
				else { transmit_3bytes(TOCKEN_ERROR, ERROR_CheckSum, CheckSum); }	//Неверная контрольная сумма!
			}
			else { Errors_USART_PC.TooShortPacket = 1; }
			PC_State = USART_State_ready;		//Приём и декодирование завершено
			PC_timer.CTRLA = TC_31kHz;			//Переходим в режим тишины
			PC_timer.CNT = 0;
			PC_timer_time = 0;
			break;
		case USART_State_ready:					//Мы в режиме тишины.
			if (PC_timer_time >= PC_timer_TimeOut)
			{
				//Время тишины вышло, PC не выходит на связь, выключаем PRGE (и iHVE)
				cli();
				gpio_set_pin_high(pin_iHVE);	//Выключаем DC-DC 24-12
				Flags.PRGE = 0;					//Выключаем PRGE от лица оператора
				Errors_USART_PC.Silence = 1;	//Отмечаем тишину в эфире
				PC_timer.CTRLA = TC_Off;		//Выключаем таймер тишины, до следующего сеанса связи
				PC_timer.CNT = 0;
				sei();
			}
			else { PC_timer_time++; }
			break;
		 default: //В любом другом случае ничего не делаем (декодирование в самом разгаре например)
			break;
	 }
	 sei_PC;
 }
//-----------------------------------------ФУНКЦИИ------------------------------------------------
void decode(void)
{
	//ФУНКЦИЯ: Расшифровываем команду
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
	//ФУНКЦИЯ: Посылаем заданное количество данных, оформив их по протоколу и с контрольной суммой
	//ПОЯСНЕНИЯ: Пакет: ':<response><data><CS>\r' 
	//					   ':' - Начало данных
	//					   '<data>' - байты данных <<response><attached_data>>
	//							<response> - отклик, код команды, на которую отвечает
	//							<attached_data> - сами данные. Их может не быть (Приказ)
	//					   '<CS>' - контрольная сумма
	//					   '\r' - конец передачи
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	for (uint8_t i = 0; i < DATA_length; i++) { usart_putchar(USART_PC,DATA[i]); }	//<data>
	usart_putchar(USART_PC,calcCheckSum(DATA,DATA_length + 1));						//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_byte(uint8_t DATA)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA);													//<data>
	usart_putchar(USART_PC, (uint8_t)(256 - DATA));									//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA_1);
	usart_putchar(USART_PC,DATA_2);													//<data>
	usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2));						//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	usart_putchar(USART_PC,COMMAND_KEY);											//':'
	usart_putchar(USART_PC,DATA_1);
	usart_putchar(USART_PC,DATA_2);													//<data>
	usart_putchar(USART_PC,DATA_3);
	usart_putchar(USART_PC, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));				//<CS>
	usart_putchar(USART_PC,COMMAND_LOCK);											//'\r'
}
uint8_t calcCheckSum(uint8_t data[], uint8_t data_length)
{
	//ФУНКЦИЯ: Вычисляет контрольную сумму принятых данных
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
	//ФУНКЦИЯ: Перезагружаем МК
	//ВНИМАНИЕ: Нужно уделить особое внимание состоянию системы перед перезагрузкой, навести порядок
	cpu_irq_disable();
	uint8_t data[] = {COMMAND_MC_reset};
	transmit(data,1);
	RST.CTRL = 1;
}
//COUNTERS
void COUNTERS_start(void)
{
	//ФУНКЦИЯ: Запускаем счётчики на определённое время
	//ДАННЫЕ: <Command><RTC_PRE><RTC_PER[1]><RTC_PER[0]>
	cli();
	if((RTC_Status != RTC_Status_busy))
	{
		//подготовка
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
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
			//Ждём пока можно будет обратиться к регистрам RTC
		}
		RTC.CNT = 0;
		COA.CNT = 0;
		COB.CNT = 0;
		COC.CNT = 0;
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
		"STS 0x0800, R16 	\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Канал событий 0
		"STS 0x0900, R17	\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Канал событий 2
		"STS 0x0A00, R18	\n\t"//Адрес TCE0.CTRLA = 0x0A00 <- Канал событий 4
		//"STS 0x0400, R19	\n\t"//Адрес RTC.CTRL   = 0x0400 <- Предделитель RTC_Prescaler(@0x205F)
		);
		RTC.CTRL =  PC_MEM[1];
		//отчёт
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
		RTC_setStatus_busy;
	}
	else
	{
		//ЗАПРЕЩЕНО! Счётчики считают!
		transmit_2bytes(COMMAND_COUNTERS_start, RTC_Status);
	}
	sei();
}
void COUNTERS_sendResults(void)
{
	//ФУНКЦИЯ: Послать результаты счёта на ПК, если можно
	//ДАННЫЕ: <Command><RTC_Status><COA_OVF[1]><COA_OVF[0]><COA_M[1]><COA_M[0]><COB_OVF[1]><COB_OVF[0]><COВ_M[1]><COВ_M[0]><COC_OVF[1]><COC_OVF[0]><COС_M[1]><COС_M[0]>
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
	//ФУНКЦИЯ: Принудительная остановка счётчиков
	if (RTC_Status == RTC_Status_busy)
	{
		while(RTC.STATUS != 0)
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
		while(RTC.STATUS != 0)
		{
			//Ждём пока можно будет обратиться к регистрам RTC
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
	//ФУНКЦИЯ: Ретрансмитит команду на TIC, если нет опроса HVE, если опрос HVE есть - ждёт ответа от TIC'а на опрос, а потом только ретрансимитит.
	cli_PC;
	for (uint8_t i = 1; i < PC_MEM_length; i++) { TIC_MEM[i-1] = PC_MEM[i]; }	//Копируем всё что должны переслать
	for (uint8_t i = 0; i < TIC_MEM_length; i++) { usart_putchar(USART_TIC,TIC_MEM[i]); }	//Отправляем
	sei_PC;
}
void TIC_transmit(void)
{
	//ФУНКЦИЯ: ретранслировать команду TIC насосу
	//PC_MEM[0] - команда от ПК, остальное в PC_MEM[0] то, что необходимо послать TIC'у
	//Копируем всё что должны переслать
	//MC_transmit_Version;
	//cli();
	//for (uint8_t i = 1; i < PC_MEM_length; i++)
	//{
	//	TIC_MEM[i-1] = PC_MEM[i];
	//}
	//TIC_MEM_Length = PC_MEM_length - 1;
	//sei();
	//Осматриваемся на счёт прерываний
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
	//ждём ответа от TIC
	//Пересылаем ответ на ПК
}
void TIC_monitor(void)
{
	//ФУНКЦИЯ: Мониторим TIC на предмет рабочего вакуума
	//Если HVE выключено, то обращаемся к высоковакуумному датчику
	if(Flags.iHVE = 1)
	{
		//Если HVE запрещено
		TIC_HVE_Message[4] = TIC_HVE_onGauge;
		TIC_transmit();//TIC_HVE_Message, 6);
		//включаем таймер
	}
	else
	{
		TIC_HVE_Message[4] = TIC_HVE_offGauge;
		TIC_transmit();//TIC_HVE_Message, 6);
		//включаем таймер
	}
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
			transmit_3bytes(TOCKEN_ERROR, INTERNAL_ERROR_SPI, DEVICE_Number);
			return;
	}
	uint8_t SPI_rDATA[] = {0,0};				//Память SPI для приёма данных (два байта)
	//Если устройство DAC AD5643R то посылаем данные по его протоколу, откликаемся и выходим
	if(DAC_is_AD5643R)
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
	//Если SPI-устройство - АЦП, то посылаем, получаем ответ, отсылаем ответ.
	uint8_t sdata[] = {PC_MEM[1], PC_MEM[2]};
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
	uint8_t aswDATA[] = {PC_MEM[0],SPI_rDATA[0],SPI_rDATA[1]};
	transmit(aswDATA, 3);
}
//Флаги
void checkFlags(void)
{
    //ФУНКЦИЯ: Выставляет флаги в соответствии с принятым байтом, если первый байт 1, и возвращает результат. Иначе просто возвращает флаги
    //ДАННЫЕ: <Command><[Проверить\Установить][iHVE][PRGE][iEDCD][SEMV1][SEMV2][SEMV3][SPUMP]>
    //				Если первый бит <Проверить\Установить> = 0, то МК тут же возвращает текущее состояние флагов
    //				Если первый бит <Проверить\Установить> = 1, то МК устанавливает флаги (кроме iHVE) и возвращает их.
    //				iHVE - только чтение
	//				Ответ МК битом [Проверить\Установить] -> 1 - параметры были изменены, 0 - нечего менять
    updateFlags();
	Flags.checkOrSet = 0; //Ни один параметр не был изменён
    if ((PC_MEM[1] >> 7) == 0)
    {
        //Проверить. Выслать на ПК свеженькие данные о флагах
        transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
        return;
    }
    //Установить!
    uint8_t receivedFlag = ((PC_MEM[1] & 32) >> 5);	//Выделяем бит PRGE
	//Если присланный PRGE(receivedFlag) не равен уже имеющемуся...
    if (Flags.PRGE  != receivedFlag) 
	{
		//то, если прислана единица...
		if (receivedFlag == 1)
		{
			//и если iHVE ноль - TIC даёт добро, на высокое напряжение
			if(Flags.iHVE == 0)
			{
				//То выдаль логический ноль на iHVE (низкий потенциал разрешает работу DC-DC 24-12)
				Flags.PRGE = 1;	//Оператор даёт добро
				MC_Tasks.setDACs = 1;//начать всяческие настроки DAC'ов после разбора флагов
				Flags.checkOrSet = 1; //был изменён параметр
			}
		}
		else 
		{
			gpio_set_pin_high(pin_iHVE);	//Выключаем DC-DC 24-12
			Flags.PRGE = 0;			//Оператор запрещает
			Flags.checkOrSet = 1;	//был изменён параметр
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
		gpio_set_pin_low(pin_iHVE); //Включаем DC-DC 24-12
        cpu_delay_ms(2000,32000000); //iHVE включает довольно иннерционную цепь, поэтому надо обождать.
        //Высокое напряжение включено - конфигурируем DACи
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
        spi_select_device(&SPIC, &DAC_Detector);
        spi_select_device(&SPIC, &DAC_IonSource);
        spi_write_packet(&SPIC, SPI_DATA, 2);
        spi_deselect_device(&SPIC, &DAC_Detector);
        spi_deselect_device(&SPIC, &DAC_IonSource);
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
void updateFlags(void)
{
	//ФУНКЦИЯ: МК осматривает флаговые пины портов и собирает их в байт Flags
	//Flags.iHVE =  (PORTC.OUT & 8  ) >> 3;
	//Необходимо запросить у TIC'а параметры iHVE
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
	USART_PC_init;						//Инициируем USART с компутером
	USART_TIC_init;						//Инициируем USART с насосемъ
	//Конечная инициализация
	pointer_Flags = &Flags;
	pointer_Errors_USART_PC = &Errors_USART_PC;
    updateFlags();
	RTC_setStatus_ready;
	Flags.iHVE = 1; //Запрещаем высокое напряжение, до тех пор пока от TIC на придёт разрешение
	Flags.PRGE = 0;	//Изночально ператор запрещает высокое напряжение (При запрещении от TIC операторская тоже должна запрещаться!)
	//Таймера				
	PC_timer.PER = 65535;
	PC_timer.CNT = 0;
	PC_timer.CTRLA = TC_31kHz;			//Включаем ПК таймер на режим тишины
	PC_State = USART_State_ready;		//Переводим USART_PC в режим тишины
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
