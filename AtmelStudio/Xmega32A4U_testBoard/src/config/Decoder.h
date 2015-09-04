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

//#define COMMAND_LOCK								13	//�������: ����� ������� ������������� ���� (������)

#define COMMAND_MC_get_Status						20  //�������: ��������� ��������� ��
//COUNTERS
#define COMMAND_COUNTERS_start						30	//�������: ������ ���� ���������
#define COMMAND_COUNTERS_stop						31	//�������: ���������� �������
#define COMMAND_COUNTERS_sendResults				32	//�������: ������� ���������� �����
#define COMMAND_COUNTERS_delayedStart				33  //�������: ������ ���� � ������������ ���������� � ���������
//������� DAC'��
#define COMMAND_PSIS_set_Voltage					40	//�������: ������ ���������� DAC'� ������� ���������
#define COMMAND_DPS_set_Voltage						41	//�������: ������ ���������� DAC'� ���������
#define COMMAND_Scaner_set_Voltage					42	//�������: ������ ���������� DAC'� �������
#define COMMAND_Condensator_set_Voltage				43	//�������: ������ ���������� ������������
#define COMMAND_PSInl_set_Voltage					44	//�������: ������ ���������� ����������
//TIC
#define COMMAND_TIC_retransmit						50	//�������: ��������������� ������ TIC'�
#define COMMAND_TIC_getStatus						51	//�������: ��������� ��������� ���������� �� ������ TIC'a
#define COMMAND_TIC_setJitter						52	//�������: ������������� ���������� ���������� "��������" ������� ��������
#define COMMAND_TIC_send_TIC_MEM					53	//�������: ������� ������ TIC_MEM
#define COMMAND_TIC_default_reset					54  //�������: �������� ���� TIC_default

//#define COMMAND_KEY								58	//�������: ����� ������� ���������� � ���� (����)

//������� ADC'��
#define COMMAND_PSIS_get_Voltage					60	//�������: ��������� ���������� � ADC ������� ���������
#define COMMAND_DPS_get_Voltage						61	//�������: ��������� ���������� DV1
#define COMMAND_MSV_get_Voltage						62	//�������: ��������� ���������� ������������ ("+" ��� "-") ��� ������� (������������ ��� �����������)
#define COMMAND_PSInl_get_Voltage					63	//�������: ��������� ���������� ����������
#define COMMAND_SPI_get_AllVoltages					64	//�������: ��������� �������� ���� ���������� ���� ADC

#define COMMAND_Flags_HVE							71
#define COMMAND_Flags_PRGE							72
#define COMMAND_Flags_EDCD							73
#define COMMAND_Flags_SEMV1							74
#define COMMAND_Flags_SEMV2							75
#define COMMAND_Flags_SEMV3							76
#define COMMAND_Flags_SPUMP							77
//------------------����������� ���������---------------------
//����������� ��������� ����������� � ��������� ����: <TOKEN_ASYNCHRO><ID>
#define TOKEN_ASYNCHRO								0
//IDs:
#define ERROR_DLP_wrongCheckSum						1	//������ ����������� �����. �����������!

#define ERROR_DECODER_wrongCommand					10  //������ �������� (��� ����� �������).

#define LAM_RTC_end									20	//�������� ��������� ����
#define LAM_SPI_conf_done							21	//����� ��������� HVE ��� SPI ���������� ���� ���������!
#define LAM_HVE_TIC_approve							22	//����� ��������� TIC'�� ��������� �������� ����������
#define LAM_HVE_TIC_disapprove						23	//����� ������� TIC'� �� ��������� ������ ���������� 
#define LAM_HVE_TIC_R1_off							24	//�� �������� ������� EMV1 �.�. TIC ������� ������ R1 �� ������ 4

#define CRITICAL_ERROR_TIC_HVE_error_decode			30	//������ ����������� ������ �� TIC'� ��� ������� HVE
#define CRITICAL_ERROR_TIC_HVE_error_noResponse		31	//������ ��� ������� HVE, TIC �� �������.

#define INTERNAL_ERROR_USART_PC						40	//���������� ������ ����� ������ �� ��
#define INTERNAL_ERROR_SPI							41	//SPI-���������� � ����� ������� ���!
#define INTERNAL_ERROR_TIC_State					42	//�������� ��������� TIC �������!
//----------------------------------------������� �������-----------------------------------------
#define MC_transmit_Status			transmit_3rytes(COMMAND_MC_get_Status, *pointer_Errors_USART_TIC, *pointer_Errors_USART_PC)
#define MC_transmit_Version			transmit_2rytes(COMMAND_MC_get_Version, MC_version)


//============================================THE END=============================================
#endif /* _TC_H_ */
