/**
 * \file
 *
 * \brief User board configuration template
 *
 */

#ifndef CONF_BOARD_H
#define CONF_BOARD_H
//------------------------------------������������ ��� �� ���---------------------------------------
//PORT A
#define pin_iWRIS	IOPORT_CREATE_PIN(PORTA, 0)		//SPI SS, ������ ������� ���������
#define pin_iECIS	IOPORT_CREATE_PIN(PORTA, 1)		//SPI SS, ���������� ������ ������� ���������
#define pin_iECSV	IOPORT_CREATE_PIN(PORTA, 2)		//SPI SS, ���������� ������ �������
#define pin_iWRSV	IOPORT_CREATE_PIN(PORTA, 3)		//SPI SS, ������ �������
#define pin_iWRCV	IOPORT_CREATE_PIN(PORTA, 4)		//SPI SS, ������ ������������
#define pin_iECVD	IOPORT_CREATE_PIN(PORTA, 5)		//SPI SS, ���������� ������ ���������
#define pin_iWRVD	IOPORT_CREATE_PIN(PORTA, 6)		//SPI SS, ������	���������
#define pin_iEDCD	IOPORT_CREATE_PIN(PORTA, 7)		//����, ���\���� Distance Control Detector
//PORT B
#define pin_ADC8	IOPORT_CREATE_PIN(PORTB, 0)		//������ ��� �� (�� ������������)
#define pin_ADC9	IOPORT_CREATE_PIN(PORTB, 1)		//������ ��� �� (�� ������������)
#define pin_DAC0	IOPORT_CREATE_PIN(PORTB, 2)		//������ ��� �� (�� ������������)
#define pin_DAC1	IOPORT_CREATE_PIN(PORTB, 3)		//������ ��� �� (�� ������������)
//PORT C
#define pin_COA		IOPORT_CREATE_PIN(PORTC, 0)		//������� �
#define pin_COB		IOPORT_CREATE_PIN(PORTC, 1)		//������� B
#define pin_COC		IOPORT_CREATE_PIN(PORTC, 2)		//������� C
#define pin_iHVE	IOPORT_CREATE_PIN(PORTC, 3)		//����, ���\���� ������� ���������� (������ �� �� ������)
#define pin_iRDUN	IOPORT_CREATE_PIN(PORTC, 4)		//SPI, ���������� �� ������ ������
#define pin_SDIN	IOPORT_CREATE_PIN(PORTC, 5)		//SPI, �������� ������
#define pin_MISO	IOPORT_CREATE_PIN(PORTC, 6)		//SPI, ���� ������
#define pin_SCLK	IOPORT_CREATE_PIN(PORTC, 7)		//SPI, ����� �������
//PORT D
#define pin_SPUMP	IOPORT_CREATE_PIN(PORTD, 0)		//ON\OFF ��������� ������
#define pin_SEMV1	IOPORT_CREATE_PIN(PORTD, 1)			//ON\OFF �������� �������
#define pin_RXD0	IOPORT_CREATE_PIN(PORTD, 2)		//USART PC, ����
#define pin_TXD0	IOPORT_CREATE_PIN(PORTD, 3)		//USART PC, ��������
#define pin_SEMV2	IOPORT_CREATE_PIN(PORTD, 4)			//ON\OFF �������� �������
#define pin_SEMV3	IOPORT_CREATE_PIN(PORTD, 5)			//ON\OFF �������� �������
#define pin_DDminus	IOPORT_CREATE_PIN(PORTD, 6)		//USB DD- (�� ������������)
#define pin_DDplus	IOPORT_CREATE_PIN(PORTD, 7)		//USB DD+ (�� ������������)
//PORT E
#define pin_iECINL	IOPORT_CREATE_PIN(PORTE, 0)		//SPI SS, ���������� ������, ����������
#define pin_iWINL	IOPORT_CREATE_PIN(PORTE, 1)		//SPI SS, ������, ����������
#define pin_RXE0	IOPORT_CREATE_PIN(PORTE, 2)		//USART TIC, ����
#define pin_TXE0	IOPORT_CREATE_PIN(PORTE, 3)		//USART TIC, ��������
//----------------------------------------------------------------------------------------
#define pin_iHVE_high		PORTC.OUTSET = 8
#define pin_iHVE_low		PORTC.OUTCLR = 8
#define pin_iEDCD_high		PORTA.OUTSET = 128
#define pin_iEDCD_low		PORTA.OUTCLR = 128
#define pin_SEMV1_high		PORTD.OUTSET = 2
#define pin_SEMV1_low		PORTD.OUTCLR = 2
#define pin_SEMV2_high		PORTD.OUTSET = 16
#define pin_SEMV2_low		PORTD.OUTCLR = 16
#define pin_SEMV3_high		PORTD.OUTSET = 32
#define pin_SEMV3_low		PORTD.OUTCLR = 32
#define pin_SPUMP_high		PORTD.OUTSET = 1
#define pin_SPUMP_low		PORTD.OUTCLR = 1

#define pin_iECIS_high		PORTA.OUTSET = 2		//SPI SS, ���������� ������ ������� �����
#define pin_iECSV_high		PORTA.OUTSET = 4		//SPI SS, ���������� ������ �������
#define pin_iECVD_high		PORTA.OUTSET = 32		//SPI SS, ���������� ������ ���������
#define pin_iECINL_high		PORTE.OUTSET = 1		//SPI SS, ���������� ������, ����������
#define pin_iRDUN_high		PORTC.OUTSET = 16		//SPI, ���������� �� ������ ������
#define pin_iECIS_low		PORTA.OUTCLR = 2		//SPI SS, ���������� ������ ������� �����
#define pin_iECSV_low		PORTA.OUTCLR = 4		//SPI SS, ���������� ������ �������
#define pin_iECVD_low		PORTA.OUTCLR = 32		//SPI SS, ���������� ������ ���������
#define pin_iECINL_low		PORTE.OUTCLR = 1		//SPI SS, ���������� ������, ����������
#define pin_iRDUN_low		PORTC.OUTCLR = 16		//SPI, ���������� �� ������ ������

#define DWR(ADDRESS,BYTE)	*(byte*)(ADDRESS) = BYTE	//Direct WRite. ������ ������ ����� � ������� �� ������
//#define pin(PORT,PIN)		
#endif



