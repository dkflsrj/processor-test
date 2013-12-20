/**
 * \file
 *
 * \brief User board configuration template
 *
 */

#ifndef CONF_BOARD_H
#define CONF_BOARD_H
//------------------------------------КОНФИГУРАЦИИ ДЛЯ МК МСП---------------------------------------
//PORT A
#define pin_iWRIS	IOPORT_CREATE_PIN(PORTA, 0)		//SPI SS, запись Ионного Источника
#define pin_iECIS	IOPORT_CREATE_PIN(PORTA, 1)		//SPI SS, разрешение чтения Ионного Источника
#define pin_iECSV	IOPORT_CREATE_PIN(PORTA, 2)		//SPI SS, разрешение чтения Сканера
#define pin_iWRSV	IOPORT_CREATE_PIN(PORTA, 3)		//SPI SS, запись Сканера
#define pin_iWRCV	IOPORT_CREATE_PIN(PORTA, 4)		//SPI SS, запись Конденсатора
#define pin_iECVD	IOPORT_CREATE_PIN(PORTA, 5)		//SPI SS, разрешение чтения Детектора
#define pin_iWRVD	IOPORT_CREATE_PIN(PORTA, 6)		//SPI SS, запись	Детектора
#define pin_iEDCD	IOPORT_CREATE_PIN(PORTA, 7)		//Ключ, вкл\выкл Distance Control Detector
//PORT B
#define pin_ADC8	IOPORT_CREATE_PIN(PORTB, 0)		//Первый АЦП МК (НЕ ИСПОЛЬЗУЕТСЯ)
#define pin_ADC9	IOPORT_CREATE_PIN(PORTB, 1)		//Второй АЦП МК (НЕ ИСПОЛЬЗУЕТСЯ)
#define pin_DAC0	IOPORT_CREATE_PIN(PORTB, 2)		//Первый ЦАП МК (НЕ ИСПОЛЬЗУЕТСЯ)
#define pin_DAC1	IOPORT_CREATE_PIN(PORTB, 3)		//Второй ЦАП МК (НЕ ИСПОЛЬЗУЕТСЯ)
//PORT C
#define pin_COA		IOPORT_CREATE_PIN(PORTC, 0)		//Счётчик А
#define pin_COB		IOPORT_CREATE_PIN(PORTC, 1)		//Счётчик B
#define pin_COC		IOPORT_CREATE_PIN(PORTC, 2)		//Счётчик C
#define pin_iHVE	IOPORT_CREATE_PIN(PORTC, 3)		//Ключ, вкл\выкл высокое напряжение (Решает МК по насосу)
#define pin_iRDUN	IOPORT_CREATE_PIN(PORTC, 4)		//SPI, разрешение на чтение вообще
#define pin_SDIN	IOPORT_CREATE_PIN(PORTC, 5)		//SPI, передача данных
#define pin_MISO	IOPORT_CREATE_PIN(PORTC, 6)		//SPI, приём данных
#define pin_SCLK	IOPORT_CREATE_PIN(PORTC, 7)		//SPI, вывод частоты
//PORT D
#define pin_SPUMP	IOPORT_CREATE_PIN(PORTD, 0)		//ON\OFF Включение насоса
#define pin_SEMV1	IOPORT_CREATE_PIN(PORTD, 1)			//ON\OFF Открытие клапана
#define pin_RXD0	IOPORT_CREATE_PIN(PORTD, 2)		//USART PC, приём
#define pin_TXD0	IOPORT_CREATE_PIN(PORTD, 3)		//USART PC, передача
#define pin_SEMV2	IOPORT_CREATE_PIN(PORTD, 4)			//ON\OFF Открытие клапана
#define pin_SEMV3	IOPORT_CREATE_PIN(PORTD, 5)			//ON\OFF Открытие клапана
#define pin_DDminus	IOPORT_CREATE_PIN(PORTD, 6)		//USB DD- (НЕ ИСПОЛЬЗУЕТСЯ)
#define pin_DDplus	IOPORT_CREATE_PIN(PORTD, 7)		//USB DD+ (НЕ ИСПОЛЬЗУЕТСЯ)
//PORT E
#define pin_iECINL	IOPORT_CREATE_PIN(PORTE, 0)		//SPI SS, разрешение чтения, НАТЕКАТЕЛЬ
#define pin_iWINL	IOPORT_CREATE_PIN(PORTE, 1)		//SPI SS, запись, НАТЕКАТЕЛЬ
#define pin_RXE0	IOPORT_CREATE_PIN(PORTE, 2)		//USART TIC, приём
#define pin_TXE0	IOPORT_CREATE_PIN(PORTE, 3)		//USART TIC, передача
//----------------------------------------------------------------------------------------
#define pin_iHVE_high		PORTC.OUTSET = 8
#define pin_iHVE_low		PORTC.OUTCLR = 8

#endif



