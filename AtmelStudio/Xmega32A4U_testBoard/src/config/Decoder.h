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

#define COMMAND_COUNTERS_start						30	//Команда: Начать счёт импульсов
#define COMMAND_COUNTERS_stop						31	//Команда: Остановить счётчик
#define COMMAND_COUNTERS_set_All					32	//Команда: Задать всё!
#define COMMAND_COUNTERS_LookAtMe					33  //Команда: LAM сигнал по окончании измерения (МК ожидает указаний)
//Команды DAC'ам
#define COMMAND_IonSource_set_Voltage				40	//Команда: Задать напряжение DAC'у Ионного Источника
#define COMMAND_Detector_set_Voltage				41	//Команда: Задать напряжение DAC'у Детектора
#define COMMAND_Scaner_set_Voltage					42	//Команда: Задать напряжение DAC'у Сканера
#define COMMAND_Condensator_set_Voltage				43	//Команда: Задать напряжение конденсатора

#define COMMAND_Inlet_set_Voltage					49	//Команда: Задать напряжение натекателя
#define COMMAND_Heater_set_Voltage					50	//Команда: Задать напряжение нагревателя


#define COMMAND_KEY									58	//Команда: Любая команда начинается с этой (ключ)

//Команды ADC'ам
#define COMMAND_IonSource_get_Voltage				60	//Команда: Запросить напряжение у ADC Ионного Источника
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

//----------------------------------------ПРОСТЫЕ КОМАНДЫ-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_Status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
