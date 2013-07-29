/**
 * \file
 *
 * \brief User board initialization template
 *
 */

#include <asf.h>
#include <board.h>
#include <conf_board.h>

void board_init(void)
{
	/* This function is meant to contain board-specific initialization code
	 * for, e.g., the I/O pins. The initialization can rely on application-
	 * specific board configuration, found in conf_board.h.
	 */
	//LEDs
	ioport_configure_port_pin(&PORTB, PIN0_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTB, PIN2_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTB, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTD, PIN0_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTD, PIN1_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTD, PIN2_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTD, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTD, PIN4_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//A0-A7
	ioport_configure_port_pin(&PORTA, PIN0_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTA, PIN1_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTA, PIN2_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTA, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTA, PIN4_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTA, PIN5_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTA, PIN6_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTA, PIN7_bm, IOPORT_DIR_INPUT);
	//USART
	ioport_configure_port_pin(&PORTE, PIN2_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTE, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//SPI
	ioport_configure_port_pin(&PORTC, PIN0_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);	//Всегда низким, так как выдаём напряг сразу по закрытию канала
	ioport_configure_port_pin(&PORTC, PIN1_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT); 
	ioport_configure_port_pin(&PORTC, PIN2_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);

	ioport_configure_port_pin(&PORTC, PIN4_bm, IOPORT_PULL_UP | IOPORT_DIR_OUTPUT);//IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTC, PIN5_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	ioport_configure_port_pin(&PORTC, PIN6_bm, IOPORT_DIR_INPUT);
	ioport_configure_port_pin(&PORTC, PIN7_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
}
