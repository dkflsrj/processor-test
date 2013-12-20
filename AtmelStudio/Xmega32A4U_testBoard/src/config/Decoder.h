//===================================================================================================
//=======================================���������� ������===========================================
//===================================================================================================

#ifndef Decoder
#define Decoder
//----------------------------------������ ������� SPI ���������-------------------------------------
#define SPI_DEVICE_Number_DAC_PSIS					1
#define SPI_DEVICE_Number_DAC_DPS					2
#define SPI_DEVICE_Number_DAC_PSInl					3
#define SPI_DEVICE_Number_DAC_Scaner				4
#define SPI_DEVICE_Number_DAC_Condensator			5
#define SPI_DEVICE_Number_ADC_PSIS					6
#define SPI_DEVICE_Number_ADC_DPS					7
#define SPI_DEVICE_Number_ADC_PSInl					8
#define SPI_DEVICE_Number_ADC_MSV					9
//-----------------------------------------������ ������---------------------------------------------
#define COMMAND_MC_get_Version						1	//�������: ��������� ������ ��������
#define COMMAND_MC_get_Birthday						2	//�������: ��������� ���� �������� ��������
#define COMMAND_MC_get_CPUfreq						3	//�������: ��������� ������� ��
#define COMMAND_MC_reset							4	//�������: ������������ ��
#define COMMAND_MC_wait								5	//�������: ��������� �� � ��������
#define COMMAND_checkCommandStack					8	//�������: ������� ���� ������� (�� ���� ���� ������ � ������ �������)

#define COMMAND_LOCK								13	//�������: ����� ������� ������������� ���� (������)

#define COMMAND_MC_get_Status						20  //�������: ��������� ��������� ��
//COUNTERS
#define COMMAND_COUNTERS_start						30	//�������: ������ ���� ���������
#define COMMAND_COUNTERS_stop						31	//�������: ���������� �������
#define COMMAND_COUNTERS_sendResults				32	//�������: ������� ���������� �����
//������� DAC'��
#define COMMAND_PSIS_set_Voltage					40	//�������: ������ ���������� DAC'� ������� ���������
#define COMMAND_DPS_set_Voltage						41	//�������: ������ ���������� DAC'� ���������
#define COMMAND_Scaner_set_Voltage					42	//�������: ������ ���������� DAC'� �������
#define COMMAND_Condensator_set_Voltage				43	//�������: ������ ���������� ������������
#define COMMAND_PSInl_set_Voltage					44	//�������: ������ ���������� ����������
//TIC
#define COMMAND_TIC_retransmit						50	//�������: ��������������� ������ TIC'�
#define COMMAND_TIC_set_Gauges						51	//�������: ������ ������� � ������
#define COMMAND_TIC_restartMonitoring						52	//�������: ���\���� ���������� TIC'a
#define COMMAND_TIC_send_TIC_MEM					53	//�������: ������� ������ TIC_MEM

#define COMMAND_KEY									58	//�������: ����� ������� ���������� � ���� (����)

//������� ADC'��
#define COMMAND_PSIS_get_Voltage					60	//�������: ��������� ���������� � ADC ������� ���������
#define COMMAND_DPS_get_Voltage						61	//�������: ��������� ���������� DV1
#define COMMAND_MSV_get_Voltage						62	//�������: ��������� ���������� ������������ ("+" ��� "-") ��� ������� (������������ ��� �����������)
#define COMMAND_PSInl_get_Voltage					63	//�������: ��������� ���������� ����������

#define COMMAND_Flags_set							70	//�������: ���������� ����� (SEMV1,SEMV2,SEMV3,SPUMP,iEDCD,iHVE)
//-----------------------------------------------LAM'�-----------------------------------------------
//���������: ����������� ��������� ������������� �� � ��� ����.
//�����
#define TOCKEN_LookAtMe					254
//������ ����������� ��������� (������ ��������)
#define LAM_RTC_end						1	//�������� ��������� ����
//-----------------------------------------------������----------------------------------------------
//���������: ������� ������, ������� �� ������ ��� ������ ������ �������.
//�����
#define TOCKEN_ERROR					255	//����� ������
//ErrorNums
#define ERROR_Decoder					1	//������ �������� (��� ����� �������).
#define ERROR_CheckSum					2	//������ ����������� �����. �����������!
//-----------------------------------------����������� ������----------------------------------------
//���������: ����������� ������, ������� ������ ��� ������ ������ �������.
//�����
#define TOCKEN_CRITICAL_ERROR					252	//����� ����������� ������
//ErrorNums
#define CRITICAL_ERROR_TIC_HVE_error_decode		1	//������ ����������� ������ �� TIC'� ��� ������� HVE
#define CRITICAL_ERROR_TIC_HVE_error_noResponse	2	//������ ��� ������� HVE, TIC �� �������.
//------------------------------------------���������� ������----------------------------------------
//���������: ���������� ������ ��. ������������� � ����������� ���������.
//�����
#define TOCKEN_INTERNAL_ERROR			253	//����� ���������� ������
//������ ���������� ������
#define INTERNAL_ERROR_USART_PC			1	//���������� ������ ����� ������ �� ��
#define INTERNAL_ERROR_SPI				2	//SPI-���������� � ����� ������� ���!
#define INTERNAL_ERROR_TIC_State		3	//�������� ��������� TIC �������!
//---------------------------------------------����������--------------------------------------------

//----------------------------------------������� �������-----------------------------------------
#define MC_transmit_Status			transmit_3bytes(COMMAND_MC_get_Status, 0, *pointer_Errors_USART_PC)
#define MC_transmit_Version			transmit_2bytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
