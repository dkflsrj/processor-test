//===================================================================================================
//=======================================�������������===========================================
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
										CLK.RTCCTRL = 13 // RTC 1.024���
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
//----------------------------------------������������ ������--------------------------------------
//���� � (0x0640 DIR = 0xB8 | 0x0644 OUT = 0xB8)	-!�����������: ���������� ��������� DIR � OUT!-
//	  � ����	���		DIR		OUT		��������
//		 0	    COA		 0		 0		������� � [�������?]
//		 1	    COB		 0		 0		������� B [�������?]
//		 2	    COC		 0		 0		������� C [�������?]
//		 3	    iHVE     1		 1		CPU ����, ���\���� ������� ���������� (������ �� �� ������)
//		 4	    iRDUN    1		 1		SPI ADC, ���������� �� ������ ������
//		 5	    SDIN     1		 1		SPI, �������� ������
//		 6	    MISO     0		 0		SPI, ���� ������
//		 7      SCLK	 1		 1		SPI, ����� �������
//���� � (0x0600 DIR = 0xFF | 0x0604 OUT = 0x59)
//	  � ����	���		DIR		OUT		��������
//		 0	   iWRIS     1		 1		SPI SS DAC PSIS, ������ ������� ���������
//		 1	   iECIS     1		 0		SPI SS ADC PSIS, ���������� ������ ������� ���������
//		 2	   iECSV     1		 0		SPI SS ADC MSV, ���������� ������ �������
//		 3	   iWRSV     1		 1		SPI SS DAC MSV, ������ �������
//		 4	   iWRCV     1		 1		SPI SS DAC MSV, ������ ������������
//		 5	   iECVD     1		 0		SPI SS ADC DPS, ���������� ������ ���������
//		 6	   iWRVD     1		 1		SPI SS DAC DPS, ������	���������
//		 7	   iEDCD     1		 0		���� DPS, ���\���� Distance Control Detector
//���� � (0x0620 DIR = 0x00 | 0x0624 OUT = 0x00)	-!�����������: ���������� ��������� DIR � OUT!-
//	  � ����	���		DIR		OUT		��������
//	-!��������������!-
//���� D (0x0660 DIR = 0x3B | 0x0664 OUT = 0x00)
//	  � ����	���		DIR		OUT		��������
//		 0	    SPUMP    1		 0		����, ON\OFF ��������� ������
//		 1	    SEMV1    1		 0		����, ON\OFF �������� �������
//		 2	    RXD0     0		 0		USART COMP, ����
//		 3	    TXD0     1		 0		USART COMP, ��������
//		 4	    SEMV2    1		 0		����, ON\OFF �������� �������
//		 5	    SEMV3    1		 0		����, ON\OFF �������� �������
//		 6	    DD-      1		 0		USB DD- (�� ������������)
//		 7	    DD+      1		 0		USB DD+ (�� ������������)
//���� E (0x0680 DIR = 0x0B | 0x0684 OUT = 0x02)
//	  � ����	���		DIR		OUT		��������
//		 0	    iECINL   1		 0		SPI SS PSInl, ���������� ������, ����������
//		 1	    iWINL    1		 1		SPI SS PSInl, ������, ����������
//		 2	    RXE0     0		 0		USART TIC, ����
//		 3	    TXE0     1		 0		USART TIC, ��������
//���� R (0x07E0 DIR = 0x00 | 0x07E4 OUT = 0x00)	-!�����������: ���������� ��������� DIR � OUT!-
//	  � ����	���		DIR		OUT		��������
//	-!��������������!-
//������������: R16 = DIR, R17 = OUT. ������������� �� ������ �������� ������ ����)
//����������: � ������ ������� ������������� ��� HVE ��� ��� �� ���� ������� ������ DC-DC 12V (���� ������ ��� ���������)
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