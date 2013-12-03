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
#define version										120
#define birthday									20131203
//Счётчики
#define RTC_Status_ready							0		//Счётчики готов к работе
#define RTC_Status_stopped							1		//Счётчики был принудительно остановлен
#define RTC_Status_busy								2		//Счётчики ещё считает
#define RTC_setStatus_ready			RTC_Status =	RTC_Status_ready
#define RTC_setStatus_stopped		RTC_Status =	RTC_Status_stopped
#define RTC_setStatus_busy			RTC_Status =	RTC_Status_busy
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
uint8_t MC_version = version;
uint32_t MC_birthday = birthday;
uint8_t MC_CommandStack = 0;

uint8_t MC_Status = 0;
//		USART COMP
uint8_t  COMP_MEM[100];									//100 байт памяти для приёма данных USART
uint8_t  COMP_MEM_length = 0;							//Длина записанного в USART_MEM пакета байтов.
uint16_t COMP_receiving_limit = 800;					//~1,2мс ожидания следующего байта. Если не придёт, то анулируем.
uint16_t COMP_receiving_time = 0;						//Счётчик времени приёма и знак того что находимся в режиме приёма
uint8_t  COMP_LOCK_received = 0;						//Булка - принят байт похожий на затвор!
uint8_t  COMP_buf = 0;									//Буфер приёма. Содержит любой принятый байт (даже шум)
uint8_t  COMP_MEM[100];									//10 байт памяти для приёма данных USART
uint8_t  COMP_MEM_CheckSum = 0;							//Принятая контрольная сумма (из пакета)
//		USART TIC
uint8_t TIC_MEM[100];									//100 байт памяти для приёма данных от TIC
uint8_t TIC_MEM_Length = 0;								//Длина записанного в TIC_MEM пакета байтов.
uint8_t TIC_buf = 0;									//Буфер приёма. Содержит любой принятый байт (даже шум)
//		Измерения
uint8_t  RTC_Status = RTC_Status_ready;					//Состояния счётчика
uint8_t  RTC_MeasurePrescaler = RTC_PRESCALER_OFF_gc;	//Предделитель RTC во время измерения
uint16_t RTC_MeasurePeriod = 0;							//Период RTC во время следующего измерения
uint32_t COA_Measurment = 0;							//Последнее измерение счётчика COA
uint8_t	 COA_OVF = 0;									//Количество переполнений счётчика СОА в текущем измерении
uint32_t COB_Measurment = 0;							//Последнее измерение счётчика COB
uint8_t	 COB_OVF = 0;									//Количество переполнений счётчика СОВ в текущем измерении
uint32_t COC_Measurment = 0;							//Последнее измерение счётчика COC
uint8_t	 COC_OVF = 0;									//Количество переполнений счётчика СОС в текущем измерении
//-----------------------------------------СТРУКТУРЫ----------------------------------------------
//Битовые поля
struct struct_MC_Tasks
{
	uint8_t setDACs			:1;
	uint8_t Decrypt			:1;
	uint8_t noTasks2		:1;
	uint8_t noTasks3		:1;
	uint8_t noTasks4		:1;
	uint8_t noTasks5		:1;
	uint8_t noTasks6		:1;
	uint8_t noTasks7		:1;
};
struct struct_MC_Tasks MC_Tasks = {0,0,0,0,0,0,0,0};
struct struct_Errors_USART_COMP
{
	uint8_t noError1				:1;
	uint8_t LOCKisLost				:1;
	uint8_t TooShortPacket			:1;
	uint8_t TooFast					:1;
	uint8_t noError4				:1;
	uint8_t noError5				:1;
	uint8_t noError6				:1;
	uint8_t noError7				:1;
};
struct struct_Errors_USART_COMP Errors_USART_COMP = {0,0,0,0,0,0,0,0};
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
uint8_t *pointer_Errors_USART_COMP;
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
	COMP_buf = *USART_COMP.DATA;//->3(95нс)
	//Если в режиме приёма
	if(COMP_receiving_time > 0)
	{
		COMP_receiving_time = 1;				//Готовимся принять следующий байт
		COMP_MEM[COMP_MEM_length] = COMP_buf;	//Сохраняем байт
		COMP_MEM_length++;						//Увеличиваем счётчик принятых байтов
		COMP_LOCK_received = 0;					//Предполагаем, что этот байт не затвор
		if (COMP_buf == COMMAND_LOCK){COMP_LOCK_received = 1;}
	}
	else if(COMP_buf == COMMAND_KEY)
	{
		//Пришёл ключ!
		if(MC_Tasks.Decrypt == 0)
		{
			COMP_receiving_time = 1;	//Переходим в режим приёма
			COMP_MEM_length = 0;		//Обнуляем счётчик принятых байтов
			COMP_LOCK_received = 0;		//Ожидаем затвор
		}
		else
		{
			//ОШИБКА! МК не выполнил предыдущую команду
			Errors_USART_COMP.TooFast = 1;
		}
	}
	sei();
}
ISR(USARTE0_RXC_vect)
{
	//ПРЕРЫВАНИЕ: Пришёл байт данных по порту USART от TIC контроллера
	//ФУНКЦИЯ: Дешифрирование байта как команду или данные, выполнение предписанных действий
	//Принимаем байт
	TIC_buf = *USART_TIC.DATA;//->3(95нс)
	
}
ISR(RTC_OVF_vect)
{
    //ПРЕРЫВАНИЕ: Возникает при окончании счёта времени таймером
    //ФУНКЦИЯ: Остановка счётчиков импульсов
    cli();
    asm(
        "LDI R16, 0x00			\n\t"//Ноль для останова всех счётчиков (запись в источник сигналов)
        "STS 0x0800, R16		\n\t"//Адрес TCC0.CTRLA = 0x0800 <- Ноль
        "STS 0x0900, R16		\n\t"//Адрес TCD0.CTRLA = 0x0900 <- Ноль
        "STS 0x0A00, R16		\n\t"//Адрес TCE0.CTRLA = 0x0A00 <-	Ноль
        "STS 0x0840, R16		\n\t"//Адрес TCC1.CTRLA = 0x0840 <-	Ноль
        "STS 0x0940, R16		\n\t"//Адрес TCD1.CTRLA = 0x0940 <-	Ноль
    );
    while (RTC.STATUS != 0)
    {
        //Ждём пока можно будет обратиться к регистрам RTC
    }
    RTC.CTRL = RTC_PRESCALER_OFF_gc;
    //сохраняем результаты
    COA_Measurment = (((uint32_t)TCC1.CNT) << 16) + TCC0.CNT;
    COB_Measurment = (((uint32_t)TCD1.CNT) << 16) + TCD0.CNT;
    COC_Measurment = TCE0.CNT;
	RTC_setStatus_ready;
	//Отправляем асинхронное сообщение
	transmit_3bytes(TOCKEN_LookAtMe, LAM_RTC_end, RTC_Status);
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
	switch(COMP_MEM[0])																					
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
		default: transmit_3bytes(TOCKEN_ERROR, ERROR_Decoder, COMP_MEM[0]);
	}
}
//USART COMP
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
	cli();
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	for (uint8_t i = 0; i < DATA_length; i++)
	{
		usart_putchar(USART_COMP,DATA[i]);							//<data>
	}
	usart_putchar(USART_COMP,calcCheckSum(DATA,DATA_length + 1));	//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);							//'\r'
	sei();
}
void transmit_byte(uint8_t DATA)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	cli();
	usart_putchar(USART_COMP,COMMAND_KEY);							//':'
	usart_putchar(USART_COMP,DATA);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA));				//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);							//'\r'
	sei();
}
void transmit_2bytes(uint8_t DATA_1, uint8_t DATA_2)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	cli();
	usart_putchar(USART_COMP,COMMAND_KEY);								//':'
	usart_putchar(USART_COMP,DATA_1);
	usart_putchar(USART_COMP,DATA_2);									//<data>
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2));		//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);								//'\r'
	sei();
}
void transmit_3bytes(uint8_t DATA_1, uint8_t DATA_2,uint8_t DATA_3)
{
	//ПЕРЕЗАГРУЗКА: Передача одного байта (отклик)
	cli();
	usart_putchar(USART_COMP,COMMAND_KEY);									//':'
	usart_putchar(USART_COMP,DATA_1);
	usart_putchar(USART_COMP,DATA_2);										//<data>
	usart_putchar(USART_COMP,DATA_3);
	usart_putchar(USART_COMP, (uint8_t)(256 - DATA_1 - DATA_2 - DATA_3));	//<CS>
	usart_putchar(USART_COMP,COMMAND_LOCK);									//'\r'
	sei();
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
		RTC.PER = (COMP_MEM[2] << 8) + COMP_MEM[3];
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
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
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
		RTC.CTRL =  COMP_MEM[1];
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
	//ДАННЫЕ: <Command><RTC_Status><COA_OVF><COA_M[3]><COA_M[2]><COA_M[1]><COA_M[0]><COВ_OVF><COВ_M[3]><COВ_M[2]><COВ_M[1]><COВ_M[0]><COС_OVF><COС_M[1]><COС_M[0]>
	uint8_t wDATA[15];
	wDATA[0] = COMMAND_COUNTERS_sendResults;
	wDATA[1] = RTC_Status;
	if(RTC_Status == RTC_Status_ready)
	{
		wDATA[2] = COA_OVF;
		wDATA[3] = (COA_Measurment >> 24);
		wDATA[4] = (COA_Measurment >> 16);
		wDATA[5] = (COA_Measurment >> 8);
		wDATA[6] = COA_Measurment;
		wDATA[7] = COB_OVF;
		wDATA[8] = (COB_Measurment >> 24);
		wDATA[9] = (COB_Measurment >> 16);
		wDATA[10] = (COB_Measurment >> 8);
		wDATA[11] = COB_Measurment;
		wDATA[12] = COC_OVF;
		wDATA[13] = (COC_Measurment >> 8);
		wDATA[14] = COC_Measurment;
	}
	transmit(wDATA,15);
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
		tc_write_clock_source(&TCC0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCE0, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCC1, TC_CLKSEL_OFF_gc);
		tc_write_clock_source(&TCD1, TC_CLKSEL_OFF_gc);
		//Могут быть траблы, внимательней
		TCC0.CNT = 0;
		TCC1.CNT = 0;
		TCD0.CNT = 0;
		TCD1.CNT = 0;
		TCE0.CNT = 0;
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
			transmit_3bytes(TOCKEN_ERROR, INTERNAL_ERROR_SPI, DEVICE_Number);
			return;
	}
	uint8_t SPI_rDATA[] = {0,0};				//Память SPI для приёма данных (два байта)
	//Если устройство DAC AD5643R то посылаем данные по его протоколу, откликаемся и выходим
	if(DAC_is_AD5643R)
	{
		//Сконфигурированы ли ЦАПы?
		uint8_t sdata[] = {COMP_MEM[1], COMP_MEM[2], COMP_MEM[3]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 3);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		//откликаемся
		uint8_t aswDATA[] = {COMP_MEM[0]};
		transmit(aswDATA, 1);
		return;
	}
	//Если SPI-устройство - ЦАП, то посылаем, откликаемся и выходим. 
	if(DEVICE_is_DAC)
	{	
		uint8_t sdata[] = {COMP_MEM[1], COMP_MEM[2]};
		spi_select_device(&SPIC, &SPI_DEVICE);
		spi_write_packet(&SPIC, sdata, 2);
		spi_deselect_device(&SPIC, &SPI_DEVICE);
		uint8_t aswDATA[] = {COMP_MEM[0]};
		transmit(aswDATA, 1);
		return;
	}
	//Если SPI-устройство - АЦП, то посылаем, получаем ответ, отсылаем ответ.
	uint8_t sdata[] = {COMP_MEM[1], COMP_MEM[2]};
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
	uint8_t aswDATA[] = {COMP_MEM[0],SPI_rDATA[0],SPI_rDATA[1]};
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
    if ((COMP_MEM[1] >> 7) == 0)
    {
        //Проверить. Выслать на ПК свеженькие данные о флагах
        transmit_2bytes(COMMAND_Flags_set, *pointer_Flags);
        return;
    }
    //Установить!
    uint8_t receivedFlag = ((COMP_MEM[1] & 32) >> 5);	//Выделяем бит PRGE
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
    receivedFlag = ((COMP_MEM[1] & 16) >> 4);
    if (Flags.iEDCD != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_iEDCD);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_iEDCD);Flags.checkOrSet = 1;}}
    receivedFlag = ((COMP_MEM[1] & 8) >> 3);
    if (Flags.SEMV1 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV1);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV1);Flags.checkOrSet = 1;}}
    receivedFlag = ((COMP_MEM[1] & 4) >> 2);
    if (Flags.SEMV2 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV2);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV2);Flags.checkOrSet = 1;}}
    receivedFlag = ((COMP_MEM[1] & 2) >> 1);
    if (Flags.SEMV3 != receivedFlag) {if (receivedFlag == 1) {gpio_set_pin_high(pin_SEMV3);Flags.checkOrSet = 1;} else {gpio_set_pin_low(pin_SEMV3);Flags.checkOrSet = 1;}}
    receivedFlag = COMP_MEM[1] & 1;
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
	pointer_Errors_USART_COMP = &Errors_USART_COMP;
    updateFlags();
	RTC_setStatus_ready;
	Flags.iHVE = 1; //Запрещаем высокое напряжение, до тех пор пока от TIC на придёт разрешение
	Flags.PRGE = 0;	//Изночально ператор запрещает высокое напряжение (При запрещении от TIC операторская тоже должна запрещаться!)
	cpu_irq_enable();					//Разрешаем прерывания	

	//Инициализация завершена
	while (1) 
	{
		//21 (0,66мкс) - холостой ход
		cli();
		if (MC_Tasks.Decrypt == 1)
		{
			//Проверяем длинну команды
			if(COMP_MEM_length > 2)
			{
				COMP_MEM_length--;	//Отсекаем последний байт (то байт затвора)
				//Надо декодировать команду..
				uint8_t CheckSum = 0;
				//Подсчёт контрольной суммы...
				for (uint8_t i = 0; i < COMP_MEM_length; i++)
				{
					CheckSum -= COMP_MEM[i];
				}
				if (CheckSum == COMP_MEM_CheckSum)
				{
					//Контрольная сумма верная можно исполнять команду
					sei();
					decode();
					MC_Tasks.Decrypt = 0;
				}
				else
				{
					//ОШИБКА ПРИЁМА! Неверная контрольная сумма!
					transmit_3bytes(TOCKEN_ERROR, ERROR_CheckSum, CheckSum);
					COMP_receiving_time = 0;
					MC_Tasks.Decrypt = 0;
				}
			}
			else
			{
				Errors_USART_COMP.TooShortPacket = 1;
				MC_Tasks.Decrypt = 0;
			}
		}
		else
		{
			//Полное время исполнения ~50(1,6мкс)
			if (COMP_receiving_time > 0)
			{
				//Отсчитываем время до следующего байта
				COMP_receiving_time++;
				//48 (1,5 мкс) - ход ожидания байта
				if (COMP_receiving_time > COMP_receiving_limit)
				{
					COMP_receiving_time = 0;	//Завершаем приём
					//Если последний полученый байт был затвор, то дешифруем, иначе происшествие
					if(COMP_LOCK_received == 1){MC_Tasks.Decrypt = 1;}else{Errors_USART_COMP.LOCKisLost = 1;}
				}
			}
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
