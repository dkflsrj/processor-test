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
//ТЕСТОВАЯ ПЛАТА
// 		//LEDs
// 		gpio_configure_pin(LED_VD1,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD2,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD3,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD4,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD5,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD6,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD7,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		gpio_configure_pin(LED_VD8,IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 	
// 		//A0-A7
// 		ioport_configure_port_pin(&PORTA, PIN0_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTA, PIN1_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTA, PIN2_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		ioport_configure_port_pin(&PORTA, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		ioport_configure_port_pin(&PORTA, PIN4_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTA, PIN5_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTA, PIN6_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTA, PIN7_bm, IOPORT_DIR_INPUT);
// 		//USART
// 		ioport_configure_port_pin(&PORTE, PIN2_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTE, PIN3_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
// 		//SPI
// 		ioport_configure_port_pin(&PORTC, PIN0_bm, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);	//Всегда низким, так как выдаём напряг сразу по закрытию канала
// 		ioport_configure_port_pin(&PORTC, PIN1_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT); 
// 		ioport_configure_port_pin(&PORTC, PIN2_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
// 	
// 		ioport_configure_port_pin(&PORTC, PIN4_bm, IOPORT_PULL_UP | IOPORT_DIR_OUTPUT);//IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTC, PIN5_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
// 		ioport_configure_port_pin(&PORTC, PIN6_bm, IOPORT_DIR_INPUT);
// 		ioport_configure_port_pin(&PORTC, PIN7_bm, IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
// 		
// 		ioport_configure_port_pin(&PORTD, PIN5_bm, IOPORT_DIR_INPUT);

//ПРОЕКТНАЯ ПЛАТА
//------------------------------------КОНФИГУРАЦИИ ДЛЯ МСП---------------------------------------
	//USART COMP
	gpio_configure_pin(pin_RXD0,	IOPORT_DIR_INPUT);
	gpio_configure_pin(pin_TXD0,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//USART TIC
	gpio_configure_pin(pin_RXE0,	IOPORT_DIR_INPUT);
	gpio_configure_pin(pin_TXE0,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//SPI
	gpio_configure_pin(pin_SCLK,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_MISO,	IOPORT_DIR_INPUT);
	gpio_configure_pin(pin_SDIN,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iRDUN,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECIS,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRIS,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECSV,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRSV,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRCV,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECVD,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRVD,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECINL,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWINL,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	//HVE
	gpio_configure_pin(pin_iHVE,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	//ADC
		//не используется
	//DAC
		//не используется
	//Ключ дистанционного управления
	gpio_configure_pin(pin_iEDCD,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	//Вентиля
	gpio_configure_pin(pin_SEMV1, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_SEMV2, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_SEMV3, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//PUMP
	gpio_configure_pin(pin_SPUMP, IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	//СЧЁТЧИКИ
	gpio_configure_pin(pin_COA, IOPORT_DIR_INPUT);
	gpio_configure_pin(pin_COB, IOPORT_DIR_INPUT);
	gpio_configure_pin(pin_COC, IOPORT_DIR_INPUT);


}
