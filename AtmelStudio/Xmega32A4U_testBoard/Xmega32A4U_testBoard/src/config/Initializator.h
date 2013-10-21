//===================================================================================================
//=======================================ИНИЦИАЛИЗАТОР===========================================
//===================================================================================================

#ifndef Initializator
#define Initializator
//USART COMP (USARTE0)
#define USART_COMP						&USARTD0
#define USART_COMP_BAUDRATE				128000
#define USART_COMP_CHAR_LENGTH			USART_CHSIZE_8BIT_gc
#define USART_COMP_PARITY				USART_PMODE_DISABLED_gc
#define USART_COMP_STOP_BIT				true
#define USART_COMP_init					usart_init_rs232(USART_COMP, &USART_COMP_OPTIONS);		 \
										usart_set_rx_interrupt_level(USART_COMP,USART_INT_LVL_MED)
//USART	TIC (USARTE0)
#define USART_TIC						&USARTD0
#define USART_TIC_BAUDRATE				128000
#define USART_TIC_CHAR_LENGTH			USART_CHSIZE_8BIT_gc
#define USART_TIC_PARITY				USART_PMODE_DISABLED_gc
#define USART_TIC_STOP_BIT				true
#define USART_TIC_init					usart_init_rs232(USART_COMP, &USART_TIC_OPTIONS);		 \
										usart_set_rx_interrupt_level(USART_TIC,USART_INT_LVL_MED)
//RTC
#define RTC_init						rtc_init();												 \
										CLK.RTCCTRL = 13 // RTC 1.024кГц
//SYSCLK
#define SYSCLK_init						osc_enable(OSC_ID_RC32MHZ);								 \
										osc_wait_ready(OSC_ID_RC32MHZ);							 \
										ccp_write_io((uint8_t *)&CLK.CTRL, CONFIG_SYSCLK_SOURCE);\
										Assert(CLK.CTRL == CONFIG_SYSCLK_SOURCE);				 \
										osc_disable(OSC_ID_RC2MHZ)
//COUNTERS
#define Counters_init					tc_enable(&TCC0);										 \
										tc_enable(&TCC1);										 \
										tc_enable(&TCD0);										 \
										tc_enable(&TCD1);										 \
										tc_enable(&TCE0);										 \
										tc_set_overflow_interrupt_callback(&TCC1, ISR_TCC1);	 \
										tc_set_overflow_interrupt_callback(&TCD1, ISR_TCD1);	 \
										tc_set_overflow_interrupt_callback(&TCE0, ISR_TCE0);	 \
										tc_set_wgm(&TCC0, TC_WG_NORMAL);						 \
										tc_set_wgm(&TCC1, TC_WG_NORMAL);						 \
										tc_set_wgm(&TCD0, TC_WG_NORMAL);						 \
										tc_set_wgm(&TCD1, TC_WG_NORMAL);						 \
										tc_set_wgm(&TCE0, TC_WG_NORMAL);						 \
										tc_set_overflow_interrupt_level(&TCC0,TC_INT_LVL_OFF);	 \
										tc_set_overflow_interrupt_level(&TCC1,TC_INT_LVL_LO);	 \
										tc_set_overflow_interrupt_level(&TCD0,TC_INT_LVL_OFF);	 \
										tc_set_overflow_interrupt_level(&TCD1,TC_INT_LVL_LO);	 \
										tc_set_overflow_interrupt_level(&TCE0,TC_INT_LVL_LO);	 \
//----------------------------------------КОНФИГУРАЦИИ ПОРТОВ--------------------------------------
//ПОРТ С (0x0640 DIR = 0xB8 | 0x0644 OUT = 0xB8)	-!Оптимизация: совпадение значенией DIR и OUT!-
//	  № пина	Имя		DIR		OUT		Описание
//		 0	    COA		 0		 0		Счётчик А [Двойной?]
//		 1	    COB		 0		 0		Счётчик B [Двойной?]
//		 2	    COC		 0		 0		Счётчик C [Двойной?]
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
//		 2	    RXD0     0		 0		USART COMP, приём
//		 3	    TXD0     1		 0		USART COMP, передача
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