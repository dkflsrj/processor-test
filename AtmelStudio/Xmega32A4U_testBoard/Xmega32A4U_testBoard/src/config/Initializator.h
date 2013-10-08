//===================================================================================================
//=======================================»Õ»÷»¿À»«¿“Œ–===========================================
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
//SPI
#define SPI_init						spi_master_init(&SPIC);									 \
										spi_enable(&SPIC)			//¬ÍÎ˛˜‡ÂÏ ÿœ» ‡
//RTC
#define RTC_init						rtc_init();												 \
										CLK.RTCCTRL = 13 // RTC 1.024Í√ˆ
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






#endif /* _TC_H_ */