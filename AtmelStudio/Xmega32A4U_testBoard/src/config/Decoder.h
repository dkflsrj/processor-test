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

#define COMMAND_COUNTERS_start						30	//�������: ������ ���� ���������
#define COMMAND_COUNTERS_stop						31	//�������: ���������� �������
#define COMMAND_COUNTERS_set_All					32	//�������: ������ ��!
#define COMMAND_COUNTERS_LookAtMe					33  //�������: LAM ������ �� ��������� ��������� (�� ������� ��������)
//������� DAC'��
#define COMMAND_IonSource_set_Voltage				40	//�������: ������ ���������� DAC'� ������� ���������
#define COMMAND_Detector_set_Voltage				41	//�������: ������ ���������� DAC'� ���������
#define COMMAND_Scaner_set_Voltage					42	//�������: ������ ���������� DAC'� �������
#define COMMAND_Condensator_set_Voltage				43	//�������: ������ ���������� ������������

#define COMMAND_Inlet_set_Voltage					49	//�������: ������ ���������� ����������
#define COMMAND_Heater_set_Voltage					50	//�������: ������ ���������� �����������


#define COMMAND_KEY									58	//�������: ����� ������� ���������� � ���� (����)

//������� ADC'��
#define COMMAND_IonSource_get_Voltage				60	//�������: ��������� ���������� � ADC ������� ���������
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

//----------------------------------------������� �������-----------------------------------------
#define MC_transmit_Status			transmit_2bytes(COMMAND_MC_get_Status, MC_Status)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
