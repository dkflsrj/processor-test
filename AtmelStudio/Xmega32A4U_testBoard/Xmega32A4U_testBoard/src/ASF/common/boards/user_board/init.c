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
	gpio_configure_pin(pin_iECIS,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRIS,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECSV,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRSV,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRCV,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECVD,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWRVD,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iECINL,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
	gpio_configure_pin(pin_iWINL,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	//HVE (Высокий уровень - 12В отключено, низкий - включено)
	gpio_configure_pin(pin_iHVE,	IOPORT_INIT_HIGH | IOPORT_DIR_OUTPUT);
	//ADC
		//не используется
	//DAC
		//не используется
	//Ключ дистанционного управления (Высокий уровень - ручное, низкий - дистанционное)
	gpio_configure_pin(pin_iEDCD,	IOPORT_INIT_LOW | IOPORT_DIR_OUTPUT);
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
