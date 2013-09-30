/**
 * \file
 *
 * \brief User board configuration template
 *
 */

#ifndef CONF_BOARD_H
#define CONF_BOARD_H


// �������� �����
// 	#define LED_VD1 IOPORT_CREATE_PIN(PORTB,0)
// 	#define LED_VD2 IOPORT_CREATE_PIN(PORTB,2)
// 	#define LED_VD3 IOPORT_CREATE_PIN(PORTB,3)
// 	#define LED_VD4 IOPORT_CREATE_PIN(PORTD,0)
// 	#define LED_VD5 IOPORT_CREATE_PIN(PORTD,1)
// 	#define LED_VD6 IOPORT_CREATE_PIN(PORTD,2)
// 	#define LED_VD7 IOPORT_CREATE_PIN(PORTD,3)
// 	#define LED_VD8 IOPORT_CREATE_PIN(PORTD,4)
// 	
// 	#define A0		IOPORT_CREATE_PIN(PORTA,0)
// 	#define A1		IOPORT_CREATE_PIN(PORTA,1)
// 	#define A2		IOPORT_CREATE_PIN(PORTA,2)
// 	#define A3		IOPORT_CREATE_PIN(PORTA,3)
// 	#define A4		IOPORT_CREATE_PIN(PORTA,4)
// 	#define A5		IOPORT_CREATE_PIN(PORTA,5)
// 	#define A6		IOPORT_CREATE_PIN(PORTA,6)
// 	#define A7		IOPORT_CREATE_PIN(PORTA,7)
// 	
// 	#define RXD		IOPORT_CREATE_PIN(PORTE,2)
// 	#define TXD		IOPORT_CREATE_PIN(PORTE,3)
// 	
// 	#define LDAC	IOPORT_CREATE_PIN(PORTC,0)
// 	#define SDAC	IOPORT_CREATE_PIN(PORTC,1)
// 	#define SADC	IOPORT_CREATE_PIN(PORTC,2)
// 	#define SS		IOPORT_CREATE_PIN(PORTC,4)
// 	#define MOSI	IOPORT_CREATE_PIN(PORTC,5)
// 	#define MISO	IOPORT_CREATE_PIN(PORTC,6)
// 	#define SCLK	IOPORT_CREATE_PIN(PORTC,7)

//��������� �����
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
#define pin_COA		IOPORT_CREATE_PIN(PORTC, 0)		//������� � [�������?]
#define pin_COB		IOPORT_CREATE_PIN(PORTC, 1)		//������� B [�������?]
#define pin_COC		IOPORT_CREATE_PIN(PORTC, 2)		//������� C [�������?]
#define pin_iHVE	IOPORT_CREATE_PIN(PORTC, 3)		//����, ���\���� ������� ���������� (������ �� �� ������)
#define pin_iRDUN	IOPORT_CREATE_PIN(PORTC, 4)		//SPI, ���������� �� ������ ������
#define pin_SDIN	IOPORT_CREATE_PIN(PORTC, 5)		//SPI, �������� ������
#define pin_MISO	IOPORT_CREATE_PIN(PORTC, 6)		//SPI, ���� ������
#define pin_SCLK	IOPORT_CREATE_PIN(PORTC, 7)		//SPI, ����� �������
//PORT D
#define pin_SPUMP	IOPORT_CREATE_PIN(PORTD, 0)		//ON\OFF ��������� ������
#define pin_SEMV1	IOPORT_CREATE_PIN(PORTD, 1)			//ON\OFF �������� �������
#define pin_RXD0	IOPORT_CREATE_PIN(PORTD, 2)		//USART COMP, ����
#define pin_TXD0	IOPORT_CREATE_PIN(PORTD, 3)		//USART COMP, ��������
#define pin_SEMV2	IOPORT_CREATE_PIN(PORTD, 4)			//ON\OFF �������� �������
#define pin_SEMV3	IOPORT_CREATE_PIN(PORTD, 5)			//ON\OFF �������� �������
#define pin_DDminus	IOPORT_CREATE_PIN(PORTD, 6)		//USB DD- (�� ������������)
#define pin_DDplus	IOPORT_CREATE_PIN(PORTD, 7)		//USB DD+ (�� ������������)
//PORT E
#define pin_iECINL	IOPORT_CREATE_PIN(PORTE, 0)		//SPI SS, ���������� ������, ����������
#define pin_iWINL	IOPORT_CREATE_PIN(PORTE, 1)		//SPI SS, ������, ����������
#define pin_RXE0	IOPORT_CREATE_PIN(PORTE, 2)		//USART TIC, ����
#define pin_TXE0	IOPORT_CREATE_PIN(PORTE, 3)		//USART TIC, ��������




#endif



