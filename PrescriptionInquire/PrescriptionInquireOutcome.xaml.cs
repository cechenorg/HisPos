using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media.Imaging;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.TreatmentCase;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionInquireOutcome : Window
    {
        private static DeclareData _inquiredPrescription;
        private static ObservableCollection<DeclareDetail> medicineList = new ObservableCollection<DeclareDetail>();
        public PrescriptionInquireOutcome(DeclareData inquired)
        {
            InitializeComponent();
            _inquiredPrescription = inquired;
            DataContext = _inquiredPrescription;
            InitialItemSource();
        }
        private void InitialItemSource()
        {
            SetPatientData();
            SetTreatmentData();
            GetMedicinesNameFromDb();
        }
        /*
         * 設定處方Treatment
         */
        private void SetTreatmentData()
        {
            AdjustCase.Content = AdjustCaseDb.GetAdjustCase(_inquiredPrescription.Prescription.Treatment.AdjustCase.Id);//調劑案件
            PaymentCategory.Content = PaymentCategroyDb.GetPaymentCategory(_inquiredPrescription.Prescription.Treatment.PaymentCategory.Id);//給付類別
            CopaymentCode.Content = CopaymentDb.GetCopayment(_inquiredPrescription.Prescription.Treatment.Copayment.Id);
            SetTreatmentCaseContent();//原處方案件
            SetMedicalInfoData();//MedicalInfo資料
        }
        /*
         * 設定處方MedicalInfo
         */
        private void SetMedicalInfoData()
        {
            ReleasePalace.Content = _inquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.GetFullHospitalData();//釋出院所
            Division.Content = _inquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.GetFullDivisonData();//就醫科別
            SetDiseaseCode();//診斷代碼
        }

        /*
         * 取得藥品名稱
         */ 
        private void GetMedicinesNameFromDb()
        {
            var connection = new DbConnection(Settings.Default.SQL_global);
            var sqlParameters = new List<SqlParameter>();
            for (var i = 0; i < _inquiredPrescription.DeclareDetails.Count - 1; i++)
            {
                sqlParameters.Clear();
                if (_inquiredPrescription.DeclareDetails[i].MedicalId == string.Empty)
                    break;
                sqlParameters.Add(new SqlParameter("ID", _inquiredPrescription.DeclareDetails[i].MedicalId));
                var name = connection.ExecuteProc("[HIS_POS_DB].[GET].[MEDICINEBYID]", sqlParameters);
                _inquiredPrescription.DeclareDetails[i].Name = name.Rows[0]["PRO_NAME"].ToString();
                medicineList.Add(_inquiredPrescription.DeclareDetails[i]);
            }
            PrescriptionSet.ItemsSource = medicineList;
        }
        /*
         * 設定原處方案件類別
         */
        private void SetTreatmentCaseContent()
        {
            var t = new TreatmentCaseDb();
            TreatmentCase.Content = t.GetTreatmentCase(_inquiredPrescription.Prescription.Treatment.MedicalInfo
                .TreatmentCase.Id);
        }
        /*
         * 設定診斷代碼
         */
        private void SetDiseaseCode()
        {
            var diseaseCodeCount = _inquiredPrescription.Prescription.Treatment.MedicalInfo.DiseaseCodes.Count;
            switch (diseaseCodeCount)
            {
                case 1:
                    MainDiagnosis.Content = _inquiredPrescription.Prescription.Treatment.MedicalInfo.DiseaseCodes[0].Id;
                    break;
                case 2:
                    MainDiagnosis.Content = _inquiredPrescription.Prescription.Treatment.MedicalInfo.DiseaseCodes[0].Id;
                    SeconDiagnosis.Content = _inquiredPrescription.Prescription.Treatment.MedicalInfo.DiseaseCodes[1].Id;
                    break;
            }
        }
        /*
         * 設定病患基本資料
         */
        private void SetPatientData()
        {
            var patient = _inquiredPrescription.Prescription.Treatment.Customer;
            var patientGenderIcon = new BitmapImage(new Uri(@"..\Images\Male.png", UriKind.Relative));
            var patientIdIcon = new BitmapImage(new Uri(@"..\Images\ID_Card.png", UriKind.Relative));
            var patientBirthIcon = new BitmapImage(new Uri(@"..\Images\birthday.png", UriKind.Relative));
            var patientEmergentPhoneIcon = new BitmapImage(new Uri(@"..\Images\Phone.png", UriKind.Relative));
            PatientName.SetIconSource(patientGenderIcon);
            PatientId.SetIconSource(patientIdIcon);
            PatientBirthday.SetIconSource(patientBirthIcon);
            PatientTel.SetIconSource(patientEmergentPhoneIcon);
            PatientTel.SetIconLabel(200, 50, patient.ContactInfo.Tel);
            PatientBirthday.SetIconLabel(100, 50, patient.Birthday);
            PatientId.SetIconLabel(200, 50, patient.IcNumber);
            PatientName.SetIconLabel(200, 50, patient.Name);
        }
    }
}
