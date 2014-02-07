//===================================================================================================
//=======================================ИНИЦИАЛИЗАТОР===========================================
//===================================================================================================

#ifndef Initializator
#define Initializator
//Service
#define sei_COX							PMIC.CTRL |= 4
#define sei_PC							PMIC.CTRL |= 2
#define sei_TIC							PMIC.CTRL |= 1
#define cli_COX							PMIC.CTRL &= 251
#define cli_PC							PMIC.CTRL &= 253
#define cli_TIC							PMIC.CTRL &= 254
//USART PC (USARTD0)
#define USART_PC						&USARTD0
#define USART_PC_BAUDRATE				120811//128000
#define USART_PC_CHAR_LENGTH			USART_CHSIZE_8BIT_gc
#define USART_PC_PARITY					USART_PMODE_DISABLED_gc
#define USART_PC_STOP_BIT				true
#define USART_PC_init					usart_init_rs232(USART_PC, &USART_PC_OPTIONS);					\
										usart_set_rx_interrupt_level(USART_PC,USART_INT_LVL_MED)
//USART	TIC (USARTE0)
#define USART_TIC						&USARTE0
#define USART_TIC_BAUDRATE				9024//9600
#define USART_TIC_CHAR_LENGTH			USART_CHSIZE_8BIT_gc
#define USART_TIC_PARITY				USART_PMODE_DISABLED_gc
#define USART_TIC_STOP_BIT				true
#define USART_TIC_init					usart_init_rs232(USART_TIC, &USART_TIC_OPTIONS);				\
										usart_set_rx_interrupt_level(USART_TIC,USART_INT_LVL_LO)
//RTC
#define RTC_init						rtc_init();														\
										CLK.RTCCTRL = 13 // RTC 1.024кГц
//SYSCLK
#define SYSCLK_init						osc_enable(OSC_ID_RC32MHZ);										\
										osc_wait_ready(OSC_ID_RC32MHZ);									\
										ccp_write_io((uint8_t *)&CLK.CTRL, CONFIG_SYSCLK_SOURCE);		\
										Assert(CLK.CTRL == CONFIG_SYSCLK_SOURCE);						\
										osc_disable(OSC_ID_RC2MHZ)
//COUNTERS
#define COA								TCC0
#define COB								TCD0
#define COC								TCE0
#define TIC_timer						TCC1
#define PC_timer						TCD1
#define Counters_init					tc_enable(&COA);												\
										tc_enable(&COB);												\
										tc_enable(&COC);												\
										tc_enable(&TIC_timer);											\
										tc_enable(&PC_timer);											\
										tc_set_overflow_interrupt_callback(&COA, ISR_COA);				\
										tc_set_overflow_interrupt_callback(&COB, ISR_COB);				\
										tc_set_overflow_interrupt_callback(&COC, ISR_COC);				\
										tc_set_overflow_interrupt_callback(&TIC_timer, ISR_TIC_timer);	\
										tc_set_overflow_interrupt_callback(&PC_timer, ISR_PC_timer);	\
										tc_set_wgm(&COA, TC_WG_NORMAL);									\
										tc_set_wgm(&COB, TC_WG_NORMAL);									\
										tc_set_wgm(&COC, TC_WG_NORMAL);									\
										tc_set_wgm(&TIC_timer, TC_WG_NORMAL);							\
										tc_set_wgm(&PC_timer, TC_WG_NORMAL);							\
										tc_set_overflow_interrupt_level(&COA,TC_INT_LVL_HI);			\
										tc_set_overflow_interrupt_level(&COB,TC_INT_LVL_HI);			\
										tc_set_overflow_interrupt_level(&COC,TC_INT_LVL_HI);			\
										tc_set_overflow_interrupt_level(&TIC_timer,TC_INT_LVL_LO);		\
										tc_set_overflow_interrupt_level(&PC_timer,TC_INT_LVL_MED);		\
										PORTC.PIN0CTRL = PORT_ISC_RISING_gc;							\
										PORTC.PIN1CTRL = PORT_ISC_RISING_gc;							\
										PORTC.PIN2CTRL = PORT_ISC_RISING_gc;							\
										EVSYS_SetEventSource( 0, EVSYS_CHMUX_PORTC_PIN0_gc );			\
										EVSYS_SetEventChannelFilter( 0, EVSYS_DIGFILT_3SAMPLES_gc );	\
										EVSYS_SetEventSource( 2, EVSYS_CHMUX_PORTC_PIN1_gc );			\
										EVSYS_SetEventChannelFilter( 2, EVSYS_DIGFILT_3SAMPLES_gc );	\
										EVSYS_SetEventSource( 4, EVSYS_CHMUX_PORTC_PIN2_gc );			\
										EVSYS_SetEventChannelFilter( 4, EVSYS_DIGFILT_3SAMPLES_gc )	
//Таймера
#define TC_31kHz						7
#define TC_125kHz						0//6			
#define TC_500kHz						5
#define TC_4MHz							4
#define TC_8MHz							3
#define TC_16MHz						2
#define TC_32MHz						1
#define TC_Off							0
//#define TC_4MHz_
//#define TC_125kHz_200ms					25000
//----------------------------------------КОНФИГУРАЦИИ ПОРТОВ--------------------------------------
//ПОРТ С (0x0640 DIR = 0xB8 | 0x0644 OUT = 0xB8)	-!Оптимизация: совпадение значенией DIR и OUT!-
//	  № пина	Имя		DIR		OUT		Описание
//		 0	    COA		 0		 0		Счётчик А
//		 1	    COB		 0		 0		Счётчик B
//		 2	    COC		 0		 0		Счётчик C
//		 3	    iHVE     1		 1		CPU Ключ, вкл\выкл высокое напряжение (Решает МК по насосу)
//		 4	    iRDUN    1		 1		SPI ADC, разрешение на чтение вообще
//		 5	    SDIN     1		 1		SPI, передача данных
//		 6	    MISO     0		 0		SPI, приём данных
//		 7      SCLK	 1		 1		SPI, вывод частоты
//ПОРТ А (0x0600 DIR = 0xFF | 0x0604 OUT = 0x59)
//	  № пина	Имя		DIR		OUT		Описание
//		 0	   iWRIS     1		 1		SPI SS DAC PSIS, запись Ионного Источника
//		 1	   iECIS     1		 0		SPI SS ADC PSIS, разрешение чтения Ионного Источника
//		 2	   iECSV     1		 0		SPI SS ADC MSV, разрешение чтения Сканера
//		 3	   iWRSV     1		 1		SPI SS DAC MSV, запись Сканера
//		 4	   iWRCV     1		 1		SPI SS DAC MSV, запись Конденсатора
//		 5	   iECVD     1		 0		SPI SS ADC DPS, разрешение чтения Детектора
//		 6	   iWRVD     1		 1		SPI SS DAC DPS, запись	Детектора
//		 7	   iEDCD     1		 0		Ключ DPS, вкл\выкл Distance Control Detector
//ПОРТ В (0x0620 DIR = 0x00 | 0x0624 OUT = 0x00)	-!Оптимизация: совпадение значенией DIR и OUT!-
//	  № пина	Имя		DIR		OUT		Описание
//	-!НЕИСПОЛЬЗУЕТСЯ!-
//ПОРТ D (0x0660 DIR = 0x3B | 0x0664 OUT = 0x00)
//	  № пина	Имя		DIR		OUT		Описание
//		 0	    SPUMP    1		 0		Ключ, ON\OFF Включение насоса
//		 1	    SEMV1    1		 0		Ключ, ON\OFF Открытие клапана
//		 2	    RXD0     0		 0		USART PC, приём
//		 3	    TXD0     1		 0		USART PC, передача
//		 4	    SEMV2    1		 0		Ключ, ON\OFF Открытие клапана
//		 5	    SEMV3    1		 0		Ключ, ON\OFF Открытие клапана
//		 6	    DD-      1		 0		USB DD- (НЕ ИСПОЛЬЗУЕТСЯ)
//		 7	    DD+      1		 0		USB DD+ (НЕ ИСПОЛЬЗУЕТСЯ)
//ПОРТ E (0x0680 DIR = 0x0B | 0x0684 OUT = 0x02)
//	  № пина	Имя		DIR		OUT		Описание
//		 0	    iECINL   1		 0		SPI SS PSInl, разрешение чтения, НАТЕКАТЕЛЬ
//		 1	    iWINL    1		 1		SPI SS PSInl, запись, НАТЕКАТЕЛЬ
//		 2	    RXE0     0		 0		USART TIC, приём
//		 3	    TXE0     1		 0		USART TIC, передача
//ПОРТ R (0x07E0 DIR = 0x00 | 0x07E4 OUT = 0x00)	-!Оптимизация: совпадение значенией DIR и OUT!-
//	  № пина	Имя		DIR		OUT		Описание
//	-!НЕИСПОЛЬЗУЕТСЯ!-
//КОНФИГУРАЦИЯ: R16 = DIR, R17 = OUT. Конфигурируем по портам согласно списку выше)
//ПРИМЕЧАНИЕ: В первую очередь конфигурируем пин HVE так как от него зависит работа DC-DC 12V (надо успеть его выключить)
#define confPORTs					asm("LDI R16,0xB8								\n\t" \
										"STS 0x0640,R16								\n\t" \
										"STS 0x0644,R16								\n\t" \
										"LDI R16,0xFF								\n\t" \
										"LDI R17,0x59								\n\t" \
										"STS 0x0600,R16								\n\t" \
										"STS 0x0604,R17								\n\t" \
										"LDI R16,0x00								\n\t" \
										"STS 0x0620,R16								\n\t" \
										"STS 0x0624,R16								\n\t" \
										"LDI R16,0x3B								\n\t" \
										"LDI R17,0x00								\n\t" \
										"STS 0x0660,R16								\n\t" \
										"STS 0x0664,R17								\n\t" \
										"LDI R16,0x0B								\n\t" \
										"LDI R17,0x02								\n\t" \
										"STS 0x0680,R16								\n\t" \
										"STS 0x0684,R17								\n\t" \
										"LDI R16,0x00								\n\t" \
										"STS 0x07E0,R16								\n\t" \
										"STS 0x07E4,R16								\n\t")




#endif /* _TC_H_ */