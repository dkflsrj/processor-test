//===================================================================================================
//=======================================ДЕШИФРАТОР КОМАНД===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//----------------------------------СПИСОК НОМЕРОВ SPI УСТРОЙСТВ-------------------------------------
#define SPI_DEVICE_Number_DAC_PSIS					1
#define SPI_DEVICE_Number_DAC_DPS					2
#define SPI_DEVICE_Number_DAC_PSInl					3
#define SPI_DEVICE_Number_DAC_Scaner				4
#define SPI_DEVICE_Number_DAC_Condensator			5
#define SPI_DEVICE_Number_ADC_PSIS					6
#define SPI_DEVICE_Number_ADC_DPS					7
#define SPI_DEVICE_Number_ADC_PSInl					8
#define SPI_DEVICE_Number_ADC_MSV					9
//-----------------------------------------СПИСОК КОМАНД---------------------------------------------
#define COMMAND_MC_get_Version						1	//Команда: Запросить версию прошивки
#define COMMAND_MC_get_Birthday						2	//Команда: Запросить дату создания прошивки
#define COMMAND_MC_get_CPUfreq						3	//Команда: Запросить частоту МК
#define COMMAND_MC_reset							4	//Команда: Перезагрузка МК
#define COMMAND_MC_wait								5	//Команда: Перевести МК в ожидание
#define COMMAND_checkCommandStack					8	//Команда: Вернуть стэк команда (МК ведёт счёт команд с самого запуска)

#define COMMAND_LOCK								13	//Команда: Любая команда заканчивается этой (затвор)

#define COMMAND_MC_get_Status						20  //Команда: Запросить состояние МК
//COUNTERS
#define COMMAND_COUNTERS_start						30	//Команда: Начать счёт импульсов
#define COMMAND_COUNTERS_stop						31	//Команда: Остановить счётчик
#define COMMAND_COUNTERS_sendResults				32	//Команда: Послать результаты счёта
//Команды DAC'ам
#define COMMAND_PSIS_set_Voltage					40	//Команда: Задать напряжение DAC'у Ионного Источника
#define COMMAND_DPS_set_Voltage						41	//Команда: Задать напряжение DAC'у Детектора
#define COMMAND_Scaner_set_Voltage					42	//Команда: Задать напряжение DAC'у Сканера
#define COMMAND_Condensator_set_Voltage				43	//Команда: Задать напряжение конденсатора
#define COMMAND_PSInl_set_Voltage					44	//Команда: Задать напряжение натекателя
//TIC
#define COMMAND_TIC_retransmit						50	//Команда: Ретранслировать данные TIC'у
#define COMMAND_TIC_set_Gauges						51	//Команда: Задать датчики и пороги
#define COMMAND_TIC_restartMonitoring						52	//Команда: Вкл\Выкл мониторинг TIC'a
#define COMMAND_TIC_send_TIC_MEM					53	//Команда: Вернуть память TIC_MEM

#define COMMAND_KEY									58	//Команда: Любая команда начинается с этой (ключ)

//Команды ADC'ам
#define COMMAND_PSIS_get_Voltage					60	//Команда: Запросить напряжение у ADC Ионного Источника
#define COMMAND_DPS_get_Voltage						61	//Команда: Запросить напряжение DV1
#define COMMAND_MSV_get_Voltage						62	//Команда: Запросить напряжение конденсатора ("+" или "-") или сканера (родительское или сканирующее)
#define COMMAND_PSInl_get_Voltage					63	//Команда: Запросить напряжение натекателя

#define COMMAND_Flags_set							70	//Команда: Установить флаги (SEMV1,SEMV2,SEMV3,SPUMP,iEDCD,iHVE)
//-----------------------------------------------LAM'ы-----------------------------------------------
//ПОЯСНЕНИЯ: Асинхронные сообщения информирующие ПК о чём либо.
//Метка
#define TOCKEN_LookAtMe					254
//Номера асинхронных сообщений (обрати внимание)
#define LAM_RTC_end						1	//Счётчики закончили счёт
//-----------------------------------------------ОШИБКИ----------------------------------------------
//ПОЯСНЕНИЯ: Обычные ошибки, которые не ставят под угрозу работу системы.
//Метка
#define TOCKEN_ERROR					255	//Метка ошибки
//ErrorNums
#define ERROR_Decoder					1	//Ошибка декодера (нет такой команды).
#define ERROR_CheckSum					2	//Ошибка контрольной суммы. Несовпадает!
//-----------------------------------------КРИТИЧЕСКИЕ ОШИБКИ----------------------------------------
//ПОЯСНЕНИЯ: Критические ошибки, которые ставят под угрозу работу системы.
//Метка
#define TOCKEN_CRITICAL_ERROR					252	//Метка критической ошибки
//ErrorNums
#define CRITICAL_ERROR_TIC_HVE_error_decode		1	//Ошибка декодировки данных от TIC'а при запросе HVE
#define CRITICAL_ERROR_TIC_HVE_error_noResponse	2	//Ошибка при запросе HVE, TIC не ответил.
//------------------------------------------ВНУТРЕННИЕ ОШИБКИ----------------------------------------
//ПОЯСНЕНИЯ: Внутренние ошибки МК. Нежелательные и запрещённые состояния.
//Метка
#define TOCKEN_INTERNAL_ERROR			253	//Метка внутренней ошибки
//Номера внутренних ошибок
#define INTERNAL_ERROR_USART_PC			1	//Внутренняя ошибка приёма данных от ПК
#define INTERNAL_ERROR_SPI				2	//SPI-устройства с таким номером нет!
#define INTERNAL_ERROR_TIC_State		3	//Неверное состояние TIC таймера!
//---------------------------------------------ДЕШИФРАТОР--------------------------------------------

//----------------------------------------ПРОСТЫЕ КОМАНДЫ-----------------------------------------
#define MC_transmit_Status			transmit_3bytes(COMMAND_MC_get_Status, 0, *pointer_Errors_USART_PC)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
