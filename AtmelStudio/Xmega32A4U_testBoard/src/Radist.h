//======================================РАДИСТ========================================
//ПРОТОКОЛ "РАДИСТ-1":
//		<key><rytes><door><compliments><checkSum><lock> - полный пакет
//		<key><rytes><checkSum><lock> - краткий пакет
//		Полный пакет содержит дверь отделяющую райты от комплиментов.
//		Краткий пакет содержит райты гарантированно преобразующиеся в байты(0...249) без комплиментов
//ПОРЯДОК:
//		При приёме пакета отмечаем был ли байт-дверь.
//		Отбрасываем ключ, затвор, контрольную сумму
//		Если двери не было, то ничего не делаем. Всё уже преобразовано.
//		Если дверь всё таки была (одна!!!), то пакет полный.
//		Считаем и сохраняем в буфер райты
//		Считаем и сохраняем в буфер комплименты
//		Количество райтов и комплиментов должно быть соответствующим!
//		Если бит-комплимент соответствующий байту = 1, то райт должен быть меньше 6 (иначе будет 256 и более)
//		и прибавляем 250.
//		Если бит-комплимент = 0, то райт равен байту.
//		Сохраняем получившиеся байты в память PC_MEM и декодируем
//----------------------------------СЕРВИСНЫЕ БАЙТЫ-----------------------------------
#define key					250
#define lock				254
#define door				251
//-------------------------------------ПЕРЕМЕННЫЕ--------------------------------------
//PC
byte PC_buffer = 0;				//Буфер приёма
byte PC_MEM[100];				//Память принятого пакета
byte PC_MEM_length = 0;			//Длина принятого пакета
byte PC_receiving = 0;			//Булка: был получен ключ, идёт приём пакета
byte PC_door = 0;				//Булка: была ли обнаружена дверь в пакете
byte PC_Compliments[7];			//Память комплиментов
byte PC_Compliments_length = 0;	//Длина памяти комплиментов
//--------------------------------ОПРЕДЕЛЕНИЯ ФУНКЦИЙ----------------------------------
void receiving(void);
void transmit(byte DATA[], byte DATA_length);
void transmit_2rytes(byte BYTE_1, byte BYTE_2);
void transmit_3rytes(byte BYTE_1, byte BYTE_2, byte BYTE_3);
byte calcCheckSum(byte DATA[], byte DATA_length);
//-------------------------------------ФУНКЦИИ-----------------------------------------
void receiving(void)
{
	//ПРЕРЫВАНИЕ: На выводе iRXF (порт B, вывод 1) зафиксирован спад
	//СОБЫТИЕ: Принимаем байт по протоколу "Радист-1"
	PC_buffer = *USART_PC.DATA;//receive();				//Принимаем байт
	PC_timer.CNT = 0;
	if(PC_buffer == key)
	{
		//Принятый байт - ключ!
		if (PC_receiving)
		{
			//LOG_write_DoubleKey;
		}
		PC_receiving = 1;
		PC_MEM_length = 0;
		PC_door = 0;
		PC_timer.CTRLA = TC_125kHz;
		return;
	}
	if(PC_receiving)
	{
		if(PC_buffer == lock)
		{
			PC_timer.CTRLA = TC_Off;
			if(PC_MEM_length < 4)
			{
				transmit_2rytes(TOKEN_ASYNCHRO,ERROR_DLP_tooShortPacket);
				return;
			}
			PC_receiving = 0;	//Завершаем приём
			PC_MEM_length--;	//Отсекаем контрольную сумму
			byte received_CheckSum = PC_MEM[PC_MEM_length];	//Вынимаем контрольную сумму
			byte calced_CheckSum = calcCheckSum(PC_MEM, PC_MEM_length); //Высчитываем контрольную сумму
			//Сверяем контрольные суммы
			if(received_CheckSum == calced_CheckSum)
			{
				//Полный пакет или краткий?
				if (PC_door > 0)
				{
					//Полный пакет! Преобразуем райты и комплименты в байты
					byte Rytes_length = 0;			//Длина байтов данных
					byte outdoor = 1;				//Булка: двери ещё не было
					PC_Compliments_length = 0;
					//Подсчитаем количество райтов данных, отсечём дверь и соберём комплименты
					for(byte i = 0; i < PC_MEM_length; i++)
					{
						if(outdoor)
						{
							Rytes_length++;	//Двери ещё не было, этой райт данных
						}
						else
						{
							PC_Compliments[PC_Compliments_length] = PC_MEM[i];	//дверь была, это комплимент
							PC_Compliments_length++;
						}
						if (PC_MEM[i] == door)
						{
							outdoor = 0;		//Дверь детектед!
							Rytes_length--;		//Исправляем длину райтов данных, чтобы дверь не попала в их число
						}
					}
					if(Rytes_length > (PC_Compliments_length * 7))
					{
						transmit_2rytes(TOKEN_ASYNCHRO,ERROR_DLP_wrongCompliments);
						return;
					}
					byte bit = 0;					//бит в комплименте 0...6
					byte i_compliment = 0;			//индекс комплимента
					PC_MEM_length = Rytes_length;		//Приравниваем память принятых данных к реальной длине данных
					//Теперь переберём райты данных, прибавляя 250 к райту, если соответствующий ему бит-комплимент равен 1
					for (byte i = 0; i < PC_MEM_length; i++)
					{
						if (bit == 7)
						{
							bit = 0;
							i_compliment++;
						}
						if ((PC_Compliments[i_compliment] >> bit) & 1)
						{
							PC_MEM[i] += 250;
						}
						bit++;
					}
				}
				//Ура райты переведены в байты!
				decode(); //Декодируем пакет
				return;
			}
			transmit_2rytes(TOKEN_ASYNCHRO,ERROR_DLP_wrongCheckSum);
			return;
		}
		if (PC_buffer == door)
		{
			PC_door++;
			if (PC_door > 1)
			{
				//LOG_write_DoubleDoor;
				PC_receiving = 0;
				return;
			}
		}
		else if(PC_buffer > 250)
		{
			//LOG_write_Reserved_byte_received;
			PC_timer.CTRLA = TC_Off;
			PC_receiving = 0;
			return;
		}
		PC_MEM[PC_MEM_length] = PC_buffer;
		PC_MEM_length++;
	}
	else
	{
		//LOG_write_Noise_detected;	//Шум на линии
	}
}
void transmit(byte DATA[], byte DATA_length)
{
	//ФУНКЦИЯ: Формирует пакет на передачу и отправляет его
	//ПОЯСНЕНИЯ:
	//    1.  Имеется массив байтов DATA на передачу.
	//    2.  DATA проверяется на наличие байтов значением больше 249, если такие есть переход на пункт 6
	//    3.  Короткий пакет: Райты на передачу равны байтам.
	//    4.  Составляется короткий пакет, подсчитывается контрольная сумма и передаётся. Выход.
	//    5.  Полный пакет: байты преобразуются в райты и комплименты.
	//    6.  Составляется полный пакет, подсчитывается контрольная сумма и передаётся.
	//ВРЕМЯ: 11 байт - ~2 мс при 2МГц
	byte wrongBytes = 0;
	byte k = 0;
	byte Compliments[7];
	byte Compliments_length = 1;
	byte Rytes[50];
	byte Rytes_length = 0;
	Compliments[0] = 0;				//Обнуляем комплимент, иначе останется мусор
	Rytes[Rytes_length] = 0;		//Будущий ключ
	Rytes_length++;
	for (byte i = 0; i < DATA_length; i++)
	{
		if (k == 7)
		{
			Compliments[Compliments_length] = 0;
			Compliments_length++;
			k = 0;
		}
		Rytes[Rytes_length] = DATA[i];
		Rytes_length++;
		if (DATA[i] > 249)
		{
			wrongBytes++;
			Compliments[Compliments_length - 1] += (1 << k);
			Rytes[Rytes_length - 1] -= 250;
		}
		k++;
	}
	//Нужна ли дверь?
	if (wrongBytes > 0)
	{
		//Нужна! Полный пакет!
		Rytes[Rytes_length] = door;
		Rytes_length++;
		for (byte i = 0; i < Compliments_length; i++)
		{
			Rytes[Rytes_length] = Compliments[i];
			Rytes_length++;
		}
	}
	Rytes[Rytes_length] = calcCheckSum(Rytes,Rytes_length);
	Rytes_length++;
	Rytes[0] = key;
	Rytes[Rytes_length] = lock;
	Rytes_length++;
	//Передача
	for(int i = 0; i < Rytes_length; i++) { usart_putchar(USART_PC, Rytes[i]); }
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
	int16_t checkSum = 250;
	for(byte i = 0; i < DATA_length; i++)
	{
		checkSum -= DATA[i];
	}
	while(checkSum < 0)
	{
		checkSum += 250;
	}
	return checkSum;
}
