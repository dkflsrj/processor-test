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

//#define COMMAND_LOCK								13	//Команда: Любая команда заканчивается этой (затвор)

#define COMMAND_MC_get_Status						20  //Команда: Запросить состояние МК
//COUNTERS
#define COMMAND_COUNTERS_start						30	//Команда: Начать счёт импульсов
#define COMMAND_COUNTERS_stop						31	//Команда: Остановить счётчик
#define COMMAND_COUNTERS_sendResults				32	//Команда: Послать результаты счёта
#define COMMAND_COUNTERS_delayedStart				33  //Команда: Начать счёт с выставлением напряжений и задержкой
//Команды DAC'ам
#define COMMAND_PSIS_set_Voltage					40	//Команда: Задать напряжение DAC'у Ионного Источника
#define COMMAND_DPS_set_Voltage						41	//Команда: Задать напряжение DAC'у Детектора
#define COMMAND_Scaner_set_Voltage					42	//Команда: Задать напряжение DAC'у Сканера
#define COMMAND_Condensator_set_Voltage				43	//Команда: Задать напряжение конденсатора
#define COMMAND_PSInl_set_Voltage					44	//Команда: Задать напряжение натекателя
//TIC
#define COMMAND_TIC_retransmit						50	//Команда: Ретранслировать данные TIC'у
#define COMMAND_TIC_getStatus						51	//Команда: Запросить последний полученный МК статус TIC'a
#define COMMAND_TIC_setJitter						52	//Команда: Устанавливает допустимое количество "дрожания" статуса датчиков
#define COMMAND_TIC_send_TIC_MEM					53	//Команда: Вернуть память TIC_MEM
#define COMMAND_TIC_default_reset					54  //Команда: Сбросить флаг TIC_default

//#define COMMAND_KEY								58	//Команда: Любая команда начинается с этой (ключ)

//Команды ADC'ам
#define COMMAND_PSIS_get_Voltage					60	//Команда: Запросить напряжение у ADC Ионного Источника
#define COMMAND_DPS_get_Voltage						61	//Команда: Запросить напряжение DV1
#define COMMAND_MSV_get_Voltage						62	//Команда: Запросить напряжение конденсатора ("+" или "-") или сканера (родительское или сканирующее)
#define COMMAND_PSInl_get_Voltage					63	//Команда: Запросить напряжение натекателя
#define COMMAND_SPI_get_AllVoltages					64	//Команда: Запросить значение всех напряжений всех ADC

#define COMMAND_Flags_HVE							71
#define COMMAND_Flags_PRGE							72
#define COMMAND_Flags_EDCD							73
#define COMMAND_Flags_SEMV1							74
#define COMMAND_Flags_SEMV2							75
#define COMMAND_Flags_SEMV3							76
#define COMMAND_Flags_SPUMP							77
//------------------АСИНХРОННЫЕ СООБЩЕНИЯ---------------------
//Асинхронные сообщения присылаются в следующем виде: <TOKEN_ASYNCHRO><ID>
#define TOKEN_ASYNCHRO								0
//IDs:
#define ERROR_DLP_wrongCheckSum						1	//Ошибка контрольной суммы. Несовпадает!

#define ERROR_DECODER_wrongCommand					10  //Ошибка декодера (нет такой команды).

#define LAM_RTC_end									20	//Счётчики закончили счёт
#define LAM_SPI_conf_done							21	//После включения HVE все SPI устройства были настроены!
#define LAM_HVE_TIC_approve							22	//После одобрения TIC'ом включения высокого напряжения
#define LAM_HVE_TIC_disapprove						23	//После запрета TIC'а на включение высого напряжения 
#define LAM_HVE_TIC_R1_off							24	//МК выключил вентиль EMV1 т.к. TIC прислал статус R1 не равный 4

#define CRITICAL_ERROR_TIC_HVE_error_decode			30	//Ошибка декодировки данных от TIC'а при запросе HVE
#define CRITICAL_ERROR_TIC_HVE_error_noResponse		31	//Ошибка при запросе HVE, TIC не ответил.

#define INTERNAL_ERROR_USART_PC						40	//Внутренняя ошибка приёма данных от ПК
#define INTERNAL_ERROR_SPI							41	//SPI-устройства с таким номером нет!
#define INTERNAL_ERROR_TIC_State					42	//Неверное состояние TIC таймера!
//----------------------------------------ПРОСТЫЕ КОМАНДЫ-----------------------------------------
#define MC_transmit_Status			transmit_3rytes(COMMAND_MC_get_Status, *pointer_Errors_USART_TIC, *pointer_Errors_USART_PC)
#define MC_transmit_Version			transmit_2rytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
