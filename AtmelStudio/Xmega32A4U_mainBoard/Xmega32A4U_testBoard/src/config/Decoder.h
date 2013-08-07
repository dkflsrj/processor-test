//===================================================================================================
//=======================================���������� ������===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//-----------------------------------------������ ������---------------------------------------------
#define COMMAND_MC_get_Version			1	//�������: ��������� ������ ��������
#define COMMAND_MC_get_Birthday			2	//�������: ��������� ���� �������� ��������
#define COMMAND_MC_get_CPUfreq			3	//�������: ��������� ������� ��
#define COMMAND_MC_reset				4	//�������: ������������ ��
#define COMMAND_MC_wait					5	//�������: ��������� �� � ��������
#define COMMAND_showTCD2_CNTh			6	//�������: �������� ������� ���� TCD2
#define COMMAND_showTCD2_CNTl			7	//�������: �������� ������� ���� TCD2

#define COMMAND_showByte				10	//�������: �������� ���� �� �����������
#define COMMAND_retransmitToTIC			11	//�������: ��������������� ������ ������

#define COMMAND_LOCK					13	//�������: ����� ������� ������������� ���� (�����)

#define COMMAND_MC_get_Status			20  //�������: ��������� ��������� ��

#define COMMAND_COA_set_MeasureTime	30	//�������: ������ �������� ����� �������
#define COMMAND_COA_start				31	//�������: ������ ���� ���������
#define COMMAND_COA_get_Count			32	//�������: ��������� � �������� ���������
#define COMMAND_COA_stop				33	//�������: ���������� �������
#define COMMAND_RTC_set_Prescaler		34	//�������: ������ �������� RTC
#define COMMAND_COA_get_Status			35	//�������: ��������� ��������� ��������
#define COMMAND_COA_set_Delay			36	//�������: ������ �������� ����� �����������
#define COMMAND_COA_set_Quantity		37	//�������: ������ ���������� ���������

#define COMMAND_DAC_set_voltage			40	//�������: ������ DAC'� ����������
#define COMMAND_ADC_get_voltage			41	//�������: ��������� � ADC ����������

#define COMMAND_KEY						58	//�������: ����� ������� ���������� � ���� (����)
//-----------------------------------------------������----------------------------------------------
//���������: ������ �������� � ������� <key><ERROR_token><ErrorNum><data[]><CS><lock>
//�����
#define ERROR_Token						0
//ErrorNums
#define ERROR_Decoder					1

//---------------------------------------------����������--------------------------------------------
#define Decode(BYTES)																				\
	switch(BYTES[0])																				\
	{																								\
		case COMMAND_MC_get_Status: MC_transmit_Status;												\
			break;																					\
		case COMMAND_showByte:	showMeByte(BYTES[1]);												\
			break;																					\
		case COMMAND_MC_get_CPUfreq: MC_transmit_CPUfreq();											\
			break;																					\
		case COMMAND_MC_get_Version: MC_transmit_Version;												\
			break;																					\
		case COMMAND_MC_get_Birthday: MC_transmit_Birthday();										\
			break;																					\
		case COMMAND_DAC_set_voltage: SPI_DAC_send(BYTES);											\
			break;																					\
		case COMMAND_ADC_get_voltage: SPI_ADC_send(BYTES);											\
			break;																					\
		case COMMAND_COA_start: COA_start();														\
			break;																					\
		case COMMAND_COA_set_MeasureTime: COA_set_MeasureTime(BYTES);								\
			break;																					\
		case COMMAND_RTC_set_Prescaler: RTC_setPrescaler(BYTES);									\
			break;																					\
		case COMMAND_COA_get_Count: COA_transmit_Result();											\
			break;																					\
		case COMMAND_COA_stop: COA_stop();															\
			break;																					\
		case COMMAND_MC_reset: MC_reset();															\
			break;																					\
		case COMMAND_COA_set_Delay: COA_set_MeasureDelay(BYTES[1]);									\
			break;																					\
		case COMMAND_COA_set_Quantity: COA_set_MeasureQuontity(BYTES);								\
			break;																					\
		case COMMAND_COA_get_Status: COA_transmit_Status;											\
			break;																					\
		case COMMAND_retransmitToTIC: TIC_transmit(BYTES);											\
			break;																					\
		default: FATAL_ERROR;																		\
	}
//----------------------------------------������� �������-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)
#define COA_transmit_Status			transmit_2bytes(COMMAND_COA_get_Status, COA_Status)


//============================================THE END=============================================
#endif /* _TC_H_ */
