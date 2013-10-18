//===================================================================================================
//=======================================���������� ������===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//----------------------------------������ ������� SPI ���������-------------------------------------
#define SPI_DEVICE_Number_DAC_IonSource				1
#define SPI_DEVICE_Number_DAC_Detector				2
#define SPI_DEVICE_Number_DAC_Inlet					3
#define SPI_DEVICE_Number_DAC_Scaner				4
#define SPI_DEVICE_Number_DAC_Condensator			5
#define SPI_DEVICE_Number_ADC_IonSource				6
#define SPI_DEVICE_Number_ADC_Detector				7
#define SPI_DEVICE_Number_ADC_Inlet					8
#define SPI_DEVICE_Number_ADC_MSV					9
//-----------------------------------------������ ������---------------------------------------------
#define COMMAND_MC_get_Version						1	//�������: ��������� ������ ��������
#define COMMAND_MC_get_Birthday						2	//�������: ��������� ���� �������� ��������
#define COMMAND_MC_get_CPUfreq						3	//�������: ��������� ������� ��
#define COMMAND_MC_reset							4	//�������: ������������ ��
#define COMMAND_MC_wait								5	//�������: ��������� �� � ��������
#define COMMAND_checkCommandStack					8	//�������: ������� ���� ������� (�� ���� ���� ������ � ������ �������)
#define COMMAND_retransmitToTIC						11	//�������: ��������������� ������ ������
		
#define COMMAND_LOCK								13	//�������: ����� ������� ������������� ���� (�����)

#define COMMAND_MC_get_Status						20  //�������: ��������� ��������� ��

#define COMMAND_RTC_set_Period						30	//�������: ������ �������� ����� �������
#define COMMAND_COUNTERS_start						31	//�������: ������ ���� ���������
#define COMMAND_COUNTERS_get_Count					32	//�������: ��������� � �������� ���������
#define COMMAND_COUNTERS_stop						33	//�������: ���������� �������
#define COMMAND_RTC_set_Prescaler					34	//�������: ������ �������� RTC
#define COMMAND_RTC_get_Status						35	//�������: ��������� ��������� ��������

#define COMMAND_DAC_set_Voltage						40	//�������: ������ DAC'� ����������
#define COMMAND_ADC_get_Voltage						41	//�������: ��������� � ADC ����������
//������� DAC'��
#define COMMAND_IonSource_EC_set_Voltage			42	//�������: ������ ���������� �������
#define COMMAND_IonSource_Ion_set_Voltage			43	//�������: ������ ���������� ���������
#define COMMAND_IonSource_F1_set_Voltage			44	//�������: ������ ���������� �������� 1
#define COMMAND_IonSource_F2_set_Voltage			45	//�������: ������ ���������� �������� 2
#define COMMAND_Detector_DV1_set_Voltage			46	//�������: ������ ���������� DV1
#define COMMAND_Detector_DV2_set_Voltage			47	//�������: ������ ���������� DV2
#define COMMAND_Detector_DV3_set_Voltage			48	//�������: ������ ���������� DV3
#define COMMAND_Inlet_set_Voltage					49	//�������: ������ ���������� ����������
#define COMMAND_Heater_set_Voltage					50	//�������: ������ ���������� �����������
#define COMMAND_Scaner_Parent_set_Voltage			51	//�������: ������ ������������ ����������
#define COMMAND_Scaner_Scan_set_Voltage				52	//�������: ������ ����������� ����������
#define COMMAND_Condensator_set_Voltage				53	//�������: ������ ���������� ������������

#define COMMAND_KEY									58	//�������: ����� ������� ���������� � ���� (����)

//������� ADC'��
#define COMMAND_IonSource_EC_get_Voltage			60	//�������: ��������� ���������� �������
#define COMMAND_IonSource_Ion_get_Voltage			61	//�������: ��������� ���������� ���������
#define COMMAND_IonSource_F1_get_Voltage			62	//�������: ��������� ���������� �������� 1
#define COMMAND_IonSource_F2_get_Voltage			63	//�������: ��������� ���������� �������� 2
#define COMMAND_Detector_DV1_get_Voltage			64	//�������: ��������� ���������� DV1
#define COMMAND_Detector_DV2_get_Voltage			65	//�������: ��������� ���������� DV2
#define COMMAND_Detector_DV3_get_Voltage			66	//�������: ��������� ���������� DV3
#define COMMAND_Inlet_get_Voltage					67	//�������: ��������� ���������� ����������
#define COMMAND_Heater_get_Voltage					68	//�������: ��������� ���������� �����������
#define COMMAND_MSV_get_Voltage						70	//�������: ��������� ���������� ������������ ("+" ��� "-") ��� ������� (������������ ��� �����������)

#define COMMAND_Flags_set							80	//�������: ���������� ����� (SEMV1,SEMV2,SEMV3,SPUMP,iEDCD,iHVE) 
//-----------------------------------------------������----------------------------------------------
//���������: ������ �������� � ������� <key><ERROR_token><ErrorNum><data[]><CS><lock>
//�����
#define ERROR_Token						0	//����� ������
//ErrorNums
#define ERROR_Decoder					1	//������ �������� (�������). ��� �����.
#define ERROR_WhereIsKEY				2	//������ �������� (����). ��� ��?
#define ERROR_WhereIsLOCK				3	//������ �������� (������). ��� ��?
#define ERROR_CheckSum					4	//������ �������� (�����.�����). �����������!	
#define ERROR_wrong_SPI_DEVICE_Number   5	//���������� ������! SPI-���������� � ����� ������� ���!
//---------------------------------------------����������--------------------------------------------
#define Decode(BYTES)																							\
	switch(BYTES[0])																							\
	{																											\
		case COMMAND_MC_get_Status:					MC_transmit_Status;											\
			break;																								\
		case COMMAND_MC_get_CPUfreq:				MC_transmit_CPUfreq();										\
			break;																								\
		case COMMAND_MC_get_Version:				MC_transmit_Version;										\
			break;																								\
		case COMMAND_MC_get_Birthday:				MC_transmit_Birthday();										\
			break;																								\
		case COMMAND_DAC_set_Voltage:				SPI_DAC_send(BYTES);										\
			break;																								\
		case COMMAND_ADC_get_Voltage:				SPI_ADC_send(BYTES);										\
			break;																								\
		case COMMAND_COUNTERS_start:				COUNTERS_start();											\
			break;																								\
		case COMMAND_RTC_set_Period:				RTC_set_Period(BYTES);										\
			break;																								\
		case COMMAND_RTC_set_Prescaler:				RTC_setPrescaler(BYTES);									\
			break;																								\
		case COMMAND_COUNTERS_get_Count:			COUNTERS_transmit_Result();									\
			break;																								\
		case COMMAND_COUNTERS_stop:					COUNTERS_stop();											\
			break;																								\
		case COMMAND_MC_reset:						MC_reset();													\
			break;																								\
		case COMMAND_RTC_get_Status:				RTC_transmit_Status;										\
			break;																								\
		case COMMAND_retransmitToTIC:				TIC_transmit(BYTES);										\
			break;																								\
		case COMMAND_checkCommandStack:				transmit_2bytes(COMMAND_checkCommandStack,CommandStack);	\
			break;																								\
		case COMMAND_IonSource_EC_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_Ion_set_Voltage:		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F1_set_Voltage:		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F2_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_Detector_DV1_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV2_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV3_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Inlet_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Heater_set_Voltage: 			SPI_send(SPI_DEVICE_Number_DAC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Scaner_Parent_set_Voltage: 	SPI_send(SPI_DEVICE_Number_DAC_Scaner,BYTES);				\
			break;																								\
		case COMMAND_Scaner_Scan_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Scaner,BYTES);				\
			break;																								\
		case COMMAND_Condensator_set_Voltage: 		SPI_send(SPI_DEVICE_Number_DAC_Condensator,BYTES);			\
			break;																								\
		case COMMAND_IonSource_EC_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_Ion_get_Voltage: 	SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F1_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_IonSource_F2_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_IonSource,BYTES);			\
			break;																								\
		case COMMAND_Detector_DV1_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV2_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Detector_DV3_get_Voltage: 		SPI_send(SPI_DEVICE_Number_ADC_Detector,BYTES);				\
			break;																								\
		case COMMAND_Inlet_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_Heater_get_Voltage: 			SPI_send(SPI_DEVICE_Number_ADC_Inlet,BYTES);				\
			break;																								\
		case COMMAND_MSV_get_Voltage: 				SPI_send(SPI_DEVICE_Number_ADC_MSV,BYTES);					\
			break;																								\
		case COMMAND_Flags_set: 					checkFlags(BYTES[1]);										\
			break;																								\
		default: transmit_3bytes(ERROR_Token, ERROR_Decoder, BYTES[0]);											\
	}
//----------------------------------------������� �������-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)
#define RTC_transmit_Status			transmit_2bytes(COMMAND_RTC_get_Status, RTC_Status)


//============================================THE END=============================================
#endif /* _TC_H_ */
