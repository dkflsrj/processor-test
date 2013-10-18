//===================================================================================================
//=======================================ДЕШИФРАТОР КОМАНД===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//----------------------------------СПИСОК НОМЕРОВ SPI УСТРОЙСТВ-------------------------------------
#define SPI_DEVICE_Number_DAC_IonSource				1
#define SPI_DEVICE_Number_DAC_Detector				2
#define SPI_DEVICE_Number_DAC_Inlet					3
#define SPI_DEVICE_Number_DAC_Scaner				4
#define SPI_DEVICE_Number_DAC_Condensator			5
#define SPI_DEVICE_Number_ADC_IonSource				6
#define SPI_DEVICE_Number_ADC_Detector				7
#define SPI_DEVICE_Number_ADC_Inlet					8
#define SPI_DEVICE_Number_ADC_MSV					9
//-----------------------------------------СПИСОК КОМАНД---------------------------------------------
#define COMMAND_MC_get_Version						1	//Команда: Запросить версию прошивки
#define COMMAND_MC_get_Birthday						2	//Команда: Запросить дату создания прошивки
#define COMMAND_MC_get_CPUfreq						3	//Команда: Запросить частоту МК
#define COMMAND_MC_reset							4	//Команда: Перезагрузка МК
#define COMMAND_MC_wait								5	//Команда: Перевести МК в ожидание
#define COMMAND_checkCommandStack					8	//Команда: Вернуть стэк команда (МК ведёт счёт команд с самого запуска)
#define COMMAND_retransmitToTIC						11	//Команда: Ретранслировать данные насосу
		
#define COMMAND_LOCK								13	//Команда: Любая команда заканчивается этой (замок)

#define COMMAND_MC_get_Status						20  //Команда: Запросить состояние МК

#define COMMAND_RTC_set_Period						30	//Команда: Задать интервал счёта времени
#define COMMAND_COUNTERS_start						31	//Команда: Начать счёт импульсов
#define COMMAND_COUNTERS_get_Count					32	//Команда: Запросить у счётчика результат
#define COMMAND_COUNTERS_stop						33	//Команда: Остановить счётчик
#define COMMAND_RTC_set_Prescaler					34	//Команда: Задать делитель RTC
#define COMMAND_RTC_get_Status						35	//Команда: Запросить состояние счётчика

#define COMMAND_DAC_set_Voltage						40	//Команда: Задать DAC'у напряжение
#define COMMAND_ADC_get_Voltage						41	//Команда: Запросить у ADC напряжение
//Команды DAC'ам
#define COMMAND_IonSource_EC_set_Voltage			42	//Команда: Задать напряжение эмиссии
#define COMMAND_IonSource_Ion_set_Voltage			43	//Команда: Задать напряжение ионизации
#define COMMAND_IonSource_F1_set_Voltage			44	//Команда: Задать напряжение фокусное 1
#define COMMAND_IonSource_F2_set_Voltage			45	//Команда: Задать напряжение фокусное 2
#define COMMAND_Detector_DV1_set_Voltage			46	//Команда: Задать напряжение DV1
#define COMMAND_Detector_DV2_set_Voltage			47	//Команда: Задать напряжение DV2
#define COMMAND_Detector_DV3_set_Voltage			48	//Команда: Задать напряжение DV3
#define COMMAND_Inlet_set_Voltage					49	//Команда: Задать напряжение натекателя
#define COMMAND_Heater_set_Voltage					50	//Команда: Задать напряжение нагревателя
#define COMMAND_Scaner_Parent_set_Voltage			51	//Команда: Задать родительское напряжение
#define COMMAND_Scaner_Scan_set_Voltage				52	//Команда: Задать сканирующее напряжение
#define COMMAND_Condensator_set_Voltage				53	//Команда: Задать напряжение конденсатора

#define COMMAND_KEY									58	//Команда: Любая команда начинается с этой (ключ)

//Команды ADC'ам
#define COMMAND_IonSource_EC_get_Voltage			60	//Команда: Запросить напряжение эмиссии
#define COMMAND_IonSource_Ion_get_Voltage			61	//Команда: Запросить напряжение ионизации
#define COMMAND_IonSource_F1_get_Voltage			62	//Команда: Запросить напряжение фокусное 1
#define COMMAND_IonSource_F2_get_Voltage			63	//Команда: Запросить напряжение фокусное 2
#define COMMAND_Detector_DV1_get_Voltage			64	//Команда: Запросить напряжение DV1
#define COMMAND_Detector_DV2_get_Voltage			65	//Команда: Запросить напряжение DV2
#define COMMAND_Detector_DV3_get_Voltage			66	//Команда: Запросить напряжение DV3
#define COMMAND_Inlet_get_Voltage					67	//Команда: Запросить напряжение натекателя
#define COMMAND_Heater_get_Voltage					68	//Команда: Запросить напряжение нагревателя
#define COMMAND_MSV_get_Voltage						70	//Команда: Запросить напряжение конденсатора ("+" или "-") или сканера (родительское или сканирующее)

#define COMMAND_Flags_set							80	//Команда: Установить флаги (SEMV1,SEMV2,SEMV3,SPUMP,iEDCD,iHVE) 
//-----------------------------------------------ОШИБКИ----------------------------------------------
//ПОЯСНЕНИЯ: Ошибка приходит в формате <key><ERROR_token><ErrorNum><data[]><CS><lock>
//Метка
#define ERROR_Token						0	//Метка ошибки
//ErrorNums
#define ERROR_Decoder					1	//Ошибка декодера (команда). Нет такой.
#define ERROR_WhereIsKEY				2	//Ошибка декодера (Ключ). Где он?
#define ERROR_WhereIsLOCK				3	//Ошибка декодера (Затвор). Где он?
#define ERROR_CheckSum					4	//Ошибка декодера (Контр.сумма). Несовпадает!	
#define ERROR_wrong_SPI_DEVICE_Number   5	//Внутренняя ошибка! SPI-устройства с таким номером нет!
//---------------------------------------------ДЕШИФРАТОР--------------------------------------------
#define Decode(BYTES)																							\
	switch(BYTES[0])																							\
	{																											\
		case COMMAND_MC_get_Status:					MC_transmit_Status;											\
			break;																								\
		case COMMAND_MC_get_CPUfreq:				MC_transmit_CPUfreq();										\
			break;																								\
		case COMMAND_MC_get_Version:				MC_transmit_Version;										\
			break;																								\
		case COMMAND_MC_get_Birthday:				MC_transmit_Birthday();										\
			break;																								\
		case COMMAND_DAC_set_Voltage:				SPI_DAC_send(BYTES);										\
			break;																								\
		case COMMAND_ADC_get_Voltage:				SPI_ADC_send(BYTES);										\
			break;																								\
		case COMMAND_COUNTERS_start:				COUNTERS_start();											\
			break;																								\
		case COMMAND_RTC_set_Period:				RTC_set_Period(BYTES);										\
			break;																								\
		case COMMAND_RTC_set_Prescaler:				RTC_setPrescaler(BYTES);									\
			break;																								\
		case COMMAND_COUNTERS_get_Count:			COUNTERS_transmit_Result();									\
			break;																								\
		case COMMAND_COUNTERS_stop:					COUNTERS_stop();											\
			break;																								\
		case COMMAND_MC_reset:						MC_reset();													\
			break;																								\
		case COMMAND_RTC_get_Status:				RTC_transmit_Status;										\
			break;																								\
		case COMMAND_retransmitToTIC:				TIC_transmit(BYTES);										\
			break;																								\
		case COMMAND_checkCommandStack:				transmit_2bytes(COMMAND_checkCommandStack,CommandStack);	\
			break;																								\
		case COMMAND_IonSource_EC_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_Ion_set_Voltage:		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F1_set_Voltage:		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F2_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_Detector_DV1_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV2_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV3_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Inlet_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Heater_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Scaner_Parent_set_Voltage: 	SPI_send(SPI_DEVICE_Number_DAC_Scaner,BYTES);				\
			break;																								\
		case COMMAND_Scaner_Scan_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Scaner,BYTES);				\
			break;																								\
		case COMMAND_Condensator_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Condensator,BYTES);			\
			break;																								\
		case COMMAND_IonSource_EC_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_Ion_get_Voltage: 	SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F1_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F2_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_Detector_DV1_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV2_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV3_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Inlet_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Heater_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_MSV_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_MSV,BYTES);					\
			break;																								\
		case COMMAND_Flags_set: 					checkFlags(BYTES[1]);										\
			break;																								\
		default: transmit_3bytes(ERROR_Token, ERROR_Decoder, BYTES[0]);											\
	}
//----------------------------------------ПРОСТЫЕ КОМАНДЫ-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)
#define RTC_transmit_Status			transmit_2bytes(COMMAND_RTC_get_Status, RTC_Status)


//============================================THE END=============================================
#endif /* _TC_H_ */
