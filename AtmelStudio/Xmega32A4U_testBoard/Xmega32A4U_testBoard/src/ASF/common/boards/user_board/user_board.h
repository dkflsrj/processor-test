/**
 * \file
 *
 * \brief User board definition template
 *
 */

 /* This file is intended to contain definitions and configuration details for
 * features and devices that are available on the board, e.g., frequency and
 * startup time for an external crystal, external memory devices, LED and USART
 * pins.
 */

#ifndef USER_BOARD_H
#define USER_BOARD_H

#include <conf_board.h>

	#define LED_VD1 IOPORT_CREATE_PIN(PORTB,0)
	#define LED_VD2 IOPORT_CREATE_PIN(PORTB,2)
	#define LED_VD3 IOPORT_CREATE_PIN(PORTB,3)
	#define LED_VD4 IOPORT_CREATE_PIN(PORTD,0)
	#define LED_VD5 IOPORT_CREATE_PIN(PORTD,1)
	#define LED_VD6 IOPORT_CREATE_PIN(PORTD,2)
	#define LED_VD7 IOPORT_CREATE_PIN(PORTD,3)
	#define LED_VD8 IOPORT_CREATE_PIN(PORTD,4)
	
	#define A0		IOPORT_CREATE_PIN(PORTA,0)
	#define A1		IOPORT_CREATE_PIN(PORTA,1)
	#define A2		IOPORT_CREATE_PIN(PORTA,2)
	#define A3		IOPORT_CREATE_PIN(PORTA,3)
	#define A4		IOPORT_CREATE_PIN(PORTA,4)
	#define A5		IOPORT_CREATE_PIN(PORTA,5)
	#define A6		IOPORT_CREATE_PIN(PORTA,6)
	#define A7		IOPORT_CREATE_PIN(PORTA,7)
	
	#define RXD		IOPORT_CREATE_PIN(PORTE,2)
	#define TXD		IOPORT_CREATE_PIN(PORTE,3)
	
	#define LDAC	IOPORT_CREATE_PIN(PORTC,0)
	#define SDAC	IOPORT_CREATE_PIN(PORTC,1)
	#define SADC	IOPORT_CREATE_PIN(PORTC,2)
	#define SS		IOPORT_CREATE_PIN(PORTC,4)
	#define MOSI	IOPORT_CREATE_PIN(PORTC,5)
	#define MISO	IOPORT_CREATE_PIN(PORTC,6)
	#define SCLK	IOPORT_CREATE_PIN(PORTC,7)
	
// External oscillator settings.
// Uncomment and set correct values if external oscillator is used.

// External oscillator frequency
//#define BOARD_XOSC_HZ          8000000

// External oscillator type.
//!< External clock signal
//#define BOARD_XOSC_TYPE        XOSC_TYPE_EXTERNAL
//!< 32.768 kHz resonator on TOSC
//#define BOARD_XOSC_TYPE        XOSC_TYPE_32KHZ
//!< 0.4 to 16 MHz resonator on XTALS
//#define BOARD_XOSC_TYPE        XOSC_TYPE_XTAL

// External oscillator startup time
//#define BOARD_XOSC_STARTUP_US  500000


#endif // USER_BOARD_H
