//======================================РАДИСТ========================================
//Байты <250>, <251> и <252> запрещены.
//      <key>  - Ключ   - открывает передачу - <250>
//      <lock> - Затвор - завершает передачу - <251>
//      <door> - Дверь  - командный байт     - <252>
//          Модифицирует следующий за дверью байт
//          Дешифровка модификаций:
//              <door><0> = <250>
//              <door><1> = <251>
//              <door><2> = <252>
//----------------------------------СЕРВИСНЫЕ БАЙТЫ------------------------------------
#define key					250
#define lock				251
#define door				252
//-------------------------------------ШИФРОВАНИЕ--------------------------------------
#define door_key			0	//Хотим передать байт равный key
#define door_lock			1	//Хотим передать байт равный lock
#define door_door			2	//Хотим передать байт равный door
//-------------------------------------ПЕРЕМЕННЫЕ--------------------------------------
//PC
byte PC_buffer = 0;				//Буфер приёма
byte PC_MEM[100];				//Память принятого пакета
byte PC_MEM_length = 0;			//Длина принятого пакета
byte PC_receiving = 0;			//Булка: был получен ключ, идёт приём пакета
byte PC_door = 0;				//Булка: был получен командный символ
byte transmiting = 0;			//Флаг 1 = "Идёт передача"
byte Queue[256];				//Массив байтов (очередь) на отправку сообщений
byte Queue_pointer = 0;			//Указатель на следующий чистый байт в Queue
byte Queue_lengthes[256];		//Массив длин сообщений в очереди
byte Queue_lengthes_pointer = 0;//Указатель на следующий чистый байт Queue_lengthes
//--------------------------------ОПРЕДЕЛЕНИЯ ФУНКЦИЙ----------------------------------
void receiving(void);
void usart_putryte(byte BYTE);
void transmit_next(void);
void transmit(byte DATA[], byte DATA_length);
void transmit_2rytes(byte BYTE_1, byte BYTE_2);
void transmit_3rytes(byte BYTE_1, byte BYTE_2, byte BYTE_3);
byte calcCheckSum(byte DATA[], byte DATA_length);
//-------------------------------------ФУНКЦИИ-----------------------------------------
void receiving(void)
{
	//ПРЕРЫВАНИЕ: 
	//СОБЫТИЕ: Принимаем байт по протоколу "Радист-2"
	PC_buffer = receive();				//Принимаем байт
	if(PC_receiving)
	{ //Мы в режиме приёма байтов
		PC_timer.CTRLA = TC_Off;					//Отключаем таймер
		switch(PC_buffer)
		{
			case door: PC_door = 1; //Принятый байт - дверь - следующий байт нужно модифицировать
				break;
			case lock: //Принятый байт - затвор - проверь контрольную сумму и декодировать
				if (PC_door)
				{
					//LOG_write_WrongByteAfterDoor;							//Записываем в журнал
					PC_receiving = 0;										//Завершаем приём
					break;
				}
				PC_receiving = 0;											//Завершаем приём
				byte received_CheckSum = PC_MEM[--PC_MEM_length];			//Вынимаем контрольную сумму
				byte calced_CheckSum = calcCheckSum(PC_MEM, PC_MEM_length); //Высчитываем контрольную сумму
				if(received_CheckSum == calced_CheckSum) { decode(); }
				else { transmit_2rytes(TOKEN_ASYNCHRO,ERROR_DLP_wrongCheckSum); }
				break;
			case key: PC_receiving = 0; //Ошибка приёма
				break;
			default:
				if (PC_door)
				{
					switch(PC_buffer)
					{
						case door_key: PC_buffer = key;
							break;
						case door_lock: PC_buffer = lock;
							break;
						case door_door: PC_buffer = door;
							break;
						default:
							//LOG_write_WrongByteAfterDoor;
							PC_receiving = 0;
							break;
					}
					PC_door = 0;
				}
				PC_MEM[PC_MEM_length++] = PC_buffer;
				break;
		}
	}
	else
	{ //Режим тишины
		if(PC_buffer == key)
		{
			//Переходим в режим приёма, очищаем буфер, сбрасываем и запускаем таймер
			PC_receiving = 1;
			PC_MEM_length = 0;
			PC_timer.CNT = 0;
			PC_timer.CTRLA = TC_125kHz;
		}
	}
}
void usart_putryte(byte BYTE)
{
	switch(BYTE)
	{
		case key: usart_putchar(USART_PC, door);
		usart_putchar(USART_PC, door_key);
		break;
		case lock:usart_putchar(USART_PC, door);
		usart_putchar(USART_PC, door_lock);
		break;
		case door:usart_putchar(USART_PC, door);
		usart_putchar(USART_PC, door_door);
		break;
		default: usart_putchar(USART_PC, BYTE);
		break;
	}
}
void transmit_next(void)
{
	transmiting = 1;
	byte DATA_length = Queue_lengthes[0];
	byte CS = calcCheckSum(Queue, DATA_length);
	//2245:70,2 мкс - 22 байта
	usart_putchar(USART_PC, key);
	for(int i = 0; i < DATA_length; i++)
	{
		usart_putryte(Queue[i]);
	}
	usart_putryte(CS);
	usart_putchar(USART_PC, lock);
	//72675:2,27 мс - 22 байта
	//Сдвигаем очередь
	cli();
	Queue_pointer -= DATA_length;
	for (int i = 0; i < Queue_pointer; i++)
	{
		Queue[i] = Queue[i + DATA_length];
	}
	//Сдвигаем массив длин сообщений
	Queue_lengthes_pointer--;
	for (int i = 0; i < Queue_lengthes_pointer; i++)
	{
		Queue_lengthes[i] = Queue_lengthes[i + 1];
	}
	sei();
	transmiting = 0;
	//72734:2,27 мс - 0 байт
}
void transmit(byte DATA[], byte DATA_length)
{
	//ФУНКЦИЯ: Формирует пакет на передачу и отправляет его
	//Если уже происходит передача (transmiting == 1), то добавляем пакет в очередь и выходим
	//0:0
	if(DATA_length == 0) { return; }
	cli();
	Queue_lengthes[Queue_lengthes_pointer++] = DATA_length;
	for (int i = 0; i < DATA_length; i++)
	{
		Queue[Queue_pointer++] = DATA[i];
	}
	sei();
	//1298:40,6 мкс - 22 байта
	if(transmiting) { return; }
	while(Queue_lengthes_pointer > 0) { transmit_next(); }
}
void transmit_2rytes(byte BYTE_1, byte BYTE_2)
{
	//ФУНКЦИЯ: Отсылает пакет из двух байт
	byte DATA[2] = { BYTE_1, BYTE_2 };
	transmit(DATA, 2);
}
void transmit_3rytes(byte BYTE_1, byte BYTE_2, byte BYTE_3)
{
	//ФУНКЦИЯ: Отсылает пакет из двух байт
	byte DATA[3] = { BYTE_1, BYTE_2, BYTE_3 };
	transmit(DATA, 3);
}
byte calcCheckSum(byte DATA[], byte DATA_length)
{
	//ФУНКЦИЯ: Вычисляет контрольную сумму пакета
	byte checkSum = 0;
	for(byte i = 0; i < DATA_length; i++)
	{
		checkSum -= DATA[i];
	}
	return checkSum;
}
