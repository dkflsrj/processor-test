//===================================================================================================
//=======================================ДЕШИФРАТОР КОМАНД===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//-----------------------------------------СПИСОК КОМАНД---------------------------------------------
#define COMMAND_MC_get_Version			1	//Команда: Запросить версию прошивки
#define COMMAND_MC_get_Birthday			2	//Команда: Запросить дату создания прошивки
#define COMMAND_MC_get_CPUfreq			3	//Команда: Запросить частоту МК
#define COMMAND_MC_reset				4	//Команда: Перезагрузка МК
#define COMMAND_MC_wait					5	//Команда: Перевести МК в ожидание
#define COMMAND_showTCD2_CNTh			6	//Команда: Показать младший байт TCD2
#define COMMAND_showTCD2_CNTl			7	//Команда: Показать старший байт TCD2

#define COMMAND_showByte				10	//Команда: Показать байт на светодиодах
#define COMMAND_retransmitToTIC			11	//Команда: Ретранслировать данные насосу

#define COMMAND_LOCK					13	//Команда: Любая команда заканчивается этой (замок)

#define COMMAND_MC_get_Status			20  //Команда: Запросить состояние МК

#define COMMAND_COA_set_MeasureTime	30	//Команда: Задать интервал счёта времени
#define COMMAND_COA_start				31	//Команда: Начать счёт импульсов
#define COMMAND_COA_get_Count			32	//Команда: Запросить у счётчика результат
#define COMMAND_COA_stop				33	//Команда: Остановить счётчик
#define COMMAND_RTC_set_Prescaler		34	//Команда: Задать делитель RTC
#define COMMAND_COA_get_Status			35	//Команда: Запросить состояние счётчика
#define COMMAND_COA_set_Delay			36	//Команда: Задать задержку между измерениями
#define COMMAND_COA_set_Quantity		37	//Команда: Задать количество измерений

#define COMMAND_DAC_set_voltage			40	//Команда: Задать DAC'у напряжение
#define COMMAND_ADC_get_voltage			41	//Команда: Запросить у ADC напряжение

#define COMMAND_KEY						58	//Команда: Любая команда начинается с этой (ключ)
//-----------------------------------------------ОШИБКИ----------------------------------------------
//ПОЯСНЕНИЯ: Ошибка приходит в формате <key><ERROR_token><ErrorNum><data[]><CS><lock>
//Метка
#define ERROR_Token						0
//ErrorNums
#define ERROR_Decoder					1

//---------------------------------------------ДЕШИФРАТОР--------------------------------------------
#define Decode(BYTES)																				\
	switch(BYTES[0])																				\
	{																								\
		case COMMAND_MC_get_Status: MC_transmit_Status;												\
			break;																					\
		case COMMAND_showByte:	showMeByte(BYTES[1]);												\
			break;																					\
		case COMMAND_MC_get_CPUfreq: MC_transmit_CPUfreq();											\
			break;																					\
		case COMMAND_MC_get_Version: MC_transmit_Version;												\
			break;																					\
		case COMMAND_MC_get_Birthday: MC_transmit_Birthday();										\
			break;																					\
		case COMMAND_DAC_set_voltage: SPI_DAC_send(BYTES);											\
			break;																					\
		case COMMAND_ADC_get_voltage: SPI_ADC_send(BYTES);											\
			break;																					\
		case COMMAND_COA_start: COA_start();														\
			break;																					\
		case COMMAND_COA_set_MeasureTime: COA_set_MeasureTime(BYTES);								\
			break;																					\
		case COMMAND_RTC_set_Prescaler: RTC_setPrescaler(BYTES);									\
			break;																					\
		case COMMAND_COA_get_Count: COA_transmit_Result();											\
			break;																					\
		case COMMAND_COA_stop: COA_stop();															\
			break;																					\
		case COMMAND_MC_reset: MC_reset();															\
			break;																					\
		case COMMAND_COA_set_Delay: COA_set_MeasureDelay(BYTES[1]);									\
			break;																					\
		case COMMAND_COA_set_Quantity: COA_set_MeasureQuontity(BYTES);								\
			break;																					\
		case COMMAND_COA_get_Status: COA_transmit_Status;											\
			break;																					\
		case COMMAND_retransmitToTIC: TIC_transmit(BYTES);											\
			break;																					\
		default: FATAL_ERROR;																		\
	}
//----------------------------------------ПРОСТЫЕ КОМАНДЫ-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)
#define COA_transmit_Status			transmit_2bytes(COMMAND_COA_get_Status, COA_Status)


//============================================THE END=============================================
#endif /* _TC_H_ */
