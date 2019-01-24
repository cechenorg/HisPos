using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.Declare;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;

namespace His_Pos.NewClass.Prescription.IcData.Upload
{
    [XmlRoot(ElementName = "RECS")]
    public class Recs
    {
        [XmlElement(ElementName = "REC")]
        public List<Rec> Rec { get; set; }
    }


    [XmlRoot(ElementName = "REC")]
    public class Rec
    {
        public Rec(Header header, MainMessage main)
        {
            HeaderMessage = header;
            MainMessage = main;
        }
        public Rec(Prescription p,IcErrorCode e = null)
        {
            HeaderMessage = e is null ? 
                new Header {DataFormat = "1", DataType = "1", UploadVersion = "1.0"} 
                : new Header { DataFormat = "1", DataType = "2", UploadVersion = "1.0" };
            MainMessage = new MainMessage(p,e,false);
        }

        public Rec(DataRow row)
        {

        }

        public Rec()
        {

        }

        [XmlElement(ElementName = "MSH")]
        public Header HeaderMessage { get; set; }
        [XmlElement(ElementName = "MB")]
        public MainMessage MainMessage { get; set; }
    }

    [XmlRoot(ElementName = "MSH")]
    public class Header
    {
        public Header()
        {
        }

        /*
         * V : 必填欄位 ~ : 不填欄位 * : 選填欄位
         * 資料格式 :
         * 1:正常上傳
         * 2:異常上傳
         * 3:補正上傳 (正常資料)
         * 4:補正上傳 (異常資料))
         */
        //V
        [XmlElement(ElementName = "A00")]
        public string DataType { get; set; } = "1";//資料型態

        //V
        [XmlElement(ElementName = "A01")]
        public string DataFormat { get; set; }//資料格式

        //V
        [XmlElement(ElementName = "A02")]
        public string UploadVersion { get; set; } = "1.0";//上傳版本 (就醫上傳版本現階段均為1.0)
    }

    [XmlRoot(ElementName = "MB")]
    public class MainMessage
    {
        public MainMessage() { }
        public MainMessage(IcData icData)
        {
            IcMessage = icData;
        }
        public MainMessage(Prescription p, IcErrorCode e,bool makeUp)
        {
            IcMessage = new IcData(p,e,makeUp);
            MedicalMessageList = new List<MedicalData>();
            var treatDateTime = DateTimeEx.ToStringWithSecond(p.Card.MedicalNumberData.TreatDateTime);
            var medList = p.Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial) && !m.PaySelf).ToList();
            for (var i = 0; i < medList.Count; i++)
            {
                MedicalMessageList.Add(e is null
                    ? new MedicalData(medList[i], treatDateTime, p.PrescriptionSign[i])
                    : new MedicalData(medList[i], treatDateTime));
            }
        }
        [XmlElement(ElementName = "MB1")]
        public IcData IcMessage { get; set; }
        [XmlElement(ElementName = "MB2")]
        public List<MedicalData> MedicalMessageList { get; set; } = new List<MedicalData>();
    }

    [XmlRoot(ElementName = "MB1")]
    public class IcData
    {
        public IcData() { }
        public IcData(Prescription p, IcErrorCode e, bool makeUp)
        {
            var seq = p.Card.MedicalNumberData;
            if (e is null)
            {
                CardNo = p.Card.CardNumber;
                SamCode = seq.SamId;
                SecuritySignature = seq.SecuritySignature;
                IDNumber = p.Card.IDNumber;
                BirthDay = DateTimeEx.ConvertToTaiwanCalender(p.Card.PatientBasicData.Birthday, false);
                MedicalNumber = string.Empty;
                PharmacyId = seq.InstitutionId;
            }
            else
            {
                IDNumber = p.Patient.IDNumber;
                BirthDay = DateTimeEx.ConvertToTaiwanCalender((DateTime)p.Patient.Birthday, false);
                TreatmentDateTime = DateTimeEx.ToStringWithSecond(DateTime.Now);
                MedicalNumber = e.ID;
                PharmacyId = ViewModelMainWindow.CurrentPharmacy.Id;
            }
            MedicalPersonIcNumber = p.Treatment.Pharmacist.IdNumber;
            MainDiagnosisCode = p.Treatment.MainDisease.ID;
            if (!string.IsNullOrEmpty(p.Treatment.SubDisease.ID))
                SecondDiagnosisCode = p.Treatment.SubDisease.ID;
            MedicalFee = (p.PrescriptionPoint.MedicinePoint + p.PrescriptionPoint.SpecialMaterialPoint +
                             p.PrescriptionPoint.CopaymentPoint + p.PrescriptionPoint.MedicalServicePoint).ToString();
            CopaymentFee = p.PrescriptionPoint.CopaymentPoint.ToString();
        }
        public IcData(SeqNumber seq,Class.Prescription currentPrescription,BasicData customerData,DeclareData currentDeclareData)
        {
            SamCode = seq.SamId;
            CardNo = currentPrescription.Customer.IcCard.CardNo;
            IDNumber = currentPrescription.Customer.IcCard.IcNumber;
            BirthDay = DateTimeEx.ConvertToTaiwanCalender(customerData.Birthday,false);
            TreatmentDateTime = DateTimeEx.ConvertToTaiwanCalenderWithTime(seq.TreatDateTime);
            MedicalNumber = string.Empty;
            PharmacyId = seq.InstitutionId;
            MedicalPersonIcNumber = currentPrescription.Pharmacy.MedicalPersonnel.IcNumber;
            SecuritySignature = seq.SecuritySignature;
            MainDiagnosisCode = currentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(currentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                SecondDiagnosisCode = currentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            MedicalFee = (currentDeclareData.D33DrugsPoint + currentDeclareData.D31SpecailMaterialPoint +
                             currentDeclareData.D17CopaymentPoint + currentDeclareData.D38MedicalServicePoint).ToString();
            CopaymentFee = currentDeclareData.D17CopaymentPoint.ToString();
        }

        public IcData(Class.Prescription current,IcErrorCodeWindow.IcErrorCode errorCode,DeclareData currentDeclareData)
        {
            IDNumber = current.Customer.IcCard.IcNumber;
            BirthDay = DateTimeEx.ConvertToTaiwanCalender(current.Customer.Birthday, false);
            var pBuffer = new byte[13];
            var iBufferlength = 13;
            var now = DateTime.Now;
            if (HisApiBase.GetStatus(1) && HisApiBase.GetStatus(2))
            {
                if (HisApiBase.csGetDateTime(pBuffer, ref iBufferlength) == 0)
                {
                    TreatmentDateTime = ConvertData.ByToString(pBuffer, 0, iBufferlength);
                    HisApiBase.CloseCom();
                }
                else
                {
                    TreatmentDateTime = (now.Year - 1911) + now.Month.ToString().PadLeft(2, '0') +
                                        now.Day.ToString().PadLeft(2, '0') + now.Hour.ToString().PadLeft(2, '0') +
                                        now.Minute.ToString().PadLeft(2, '0') + now.Second.ToString().PadLeft(2, '0');
                }
            }
            else
            {
                TreatmentDateTime = (now.Year - 1911) + now.Month.ToString().PadLeft(2, '0') +
                                    now.Day.ToString().PadLeft(2, '0') + now.Hour.ToString().PadLeft(2, '0') +
                                    now.Minute.ToString().PadLeft(2, '0') + now.Second.ToString().PadLeft(2, '0');
            }
            PharmacyId = ViewModelMainWindow.CurrentPharmacy.Id;
            
            MedicalNumber = errorCode.Id;
            MedicalPersonIcNumber = current.Pharmacy.MedicalPersonnel.IcNumber;
            MainDiagnosisCode = current.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(current.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                SecondDiagnosisCode = current.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            MedicalFee = (currentDeclareData.D33DrugsPoint + currentDeclareData.D31SpecailMaterialPoint +
                             currentDeclareData.D17CopaymentPoint + currentDeclareData.D38MedicalServicePoint).ToString();
            CopaymentFee = currentDeclareData.D17CopaymentPoint.ToString();
        }
        //1,3 V  2,4 ~
        [XmlElement(ElementName = "A11")]
        public string CardNo { get; set; }//卡片號碼 (get by HISAPI : csGetCardNo)

        //V
        [XmlElement(ElementName = "A12")]
        public string IDNumber { get; set; }//身分證號或 身分證明文件號碼

        //V
        [XmlElement(ElementName = "A13")]
        public string BirthDay { get; set; }//出生日期

        //V
        [XmlElement(ElementName = "A14")]
        public string PharmacyId { get; set; }//健保資料段 8-6.醫療院所代碼

        //V
        [XmlElement(ElementName = "A15")]
        public string MedicalPersonIcNumber { get; set; }//健保資料段 8-7-1.醫事人員身分證號

        //1,3 V  2,4 ~ 
        [XmlElement(ElementName = "A16")]
        public string SamCode { get; set; }//安全模組代碼

        //V
        [XmlElement(ElementName = "A17")]
        public string TreatmentDateTime { get; set; } //健保資料段 8-3.就診日期時間 (get by HISAPI : hisGetSeqNumber256)

        //*
        [XmlElement(ElementName = "A18")]
        public string MedicalNumber { get; set; }//健保資料段 8-5.就醫序號(get by HISAPI : hisGetSeqNumber256)

        //V
        [XmlElement(ElementName = "A19")]
        public string MakeUpMark { get; set; } = "1";//健保資料段 8-4.補卡註記(get by HISAPI : hisGetTreatmentNoNeedHPC)
        
        //*
        [XmlElement(ElementName = "A20")]
        public string NewbornBirthDay { get; set; }//健保資料段 7-1.新生兒出生日期

        //*
        [XmlElement(ElementName = "A21")]
        public string NewbornBabyMark { get; set; }//健保資料段 7-2.新生兒胞胎註記

        //V
        [XmlElement(ElementName = "A23")]
        public string TreatmentCategory { get; set; } = "AF";//健保資料段 8-1.就醫類別

        //*
        [XmlElement(ElementName = "A24")]
        public string NewbornTreatmentMark { get; set; }//健保資料段 8-2.新生兒就醫註記

        //1,3 V  2,4 ~
        [XmlElement(ElementName = "A22")]
        public string SecuritySignature { get; set; }//健保資料段 8-7-2安全簽章

        //V
        [XmlElement(ElementName = "A25")]
        public string MainDiagnosisCode { get; set; }//健保資料段 8-8.主要診斷碼

        //*
        [XmlElement(ElementName = "A26")]
        public string SecondDiagnosisCode { get; set; }//健保資料段 8-8.主要診斷碼

        //V
        [XmlElement(ElementName = "A31")]
        public string MedicalFee { get; set; }//健保資料段 8-10-1.門診醫療費用 （當次） (get by HISAPI : hisGetTreatmentNoNeedHPC)

        //*
        [XmlElement(ElementName = "A32")]
        public string CopaymentFee { get; set; }//健保資料段 8-10-2.門診部分負擔費用（當次）(get by HISAPI : hisGetTreatmentNoNeedHPC)

        //*
        [XmlElement(ElementName = "A33")]
        public string HospitalizationFee { get; set; }//健保資料段 8-10-3.住院醫療費用(當次)(get by HISAPI : hisGetTreatmentNoNeedHPC)

        //*
        [XmlElement(ElementName = "A34")]
        public string HospitalizationCopaymentFeeLess { get; set; }// 健保資料段 8-10-4.住院部分負擔費用（當次急性30天、慢性180天以下）(get by HISAPI : hisGetTreatmentNoNeedHPC)

        //*
        [XmlElement(ElementName = "A35")]
        public string HospitalizationCopaymentFeeMore { get; set; }//健保資料段8-10-5.住院部分負擔費用（當次急性31天、慢性181天以上）(get by HISAPI : hisGetTreatmentNoNeedHPC)

        [XmlElement(ElementName = "A54")]
        public string ActualTreatDate { get; set; }//健保資料段8-10-5.住院部分負擔費用（當次急性31天、慢性181天以上）(get by HISAPI : hisGetTreatmentNoNeedHPC)

    }

    [XmlRoot(ElementName = "MB2")]
    public class MedicalData
    {
        public MedicalData()
        {

        }
        public MedicalData(Medicine med,string treatDateTime, string preSig = null)
        {
            MedicalOrderTreatDateTime = treatDateTime;
            MedicalOrderCategory = med is MedicineSpecialMaterial ? "4" : "1";
            TreatmentProjectCode = med.ID;
            if (!string.IsNullOrEmpty(med.PositionName))
                TreatmentPosition = med.PositionName;
            Usage = med.UsageName;
            Days = med.Days.ToString();
            TotalAmount = $"{med.Amount:00000.0}";
            switch (MedicalOrderCategory)
            {
                case "1":
                    PrescriptionDeliveryMark = "01";
                    break;
                case "A":
                    PrescriptionDeliveryMark = "02";
                    break;
                case "2":
                    PrescriptionDeliveryMark = "05";
                    break;
                case "B":
                    PrescriptionDeliveryMark = "06";
                    break;
                case "3":
                case "4":
                case "5":
                    PrescriptionDeliveryMark = "03";
                    break;
                case "C":
                case "D":
                case "E":
                    PrescriptionDeliveryMark = "04";
                    break;
            }
            if(!string.IsNullOrEmpty(preSig))
                PrescriptionSignature = preSig;
        }
        //V 
        [XmlElement(ElementName = "A71")]
        public string MedicalOrderTreatDateTime { get; set; }//醫療專區 1-1.醫令就診日期時間

        //V 
        [XmlElement(ElementName = "A72")]
        public string MedicalOrderCategory { get; set; }//醫療專區 1-2-1醫令類別

        //V 
        [XmlElement(ElementName = "A73")]
        public string TreatmentProjectCode { get; set; }//醫療專區 1-2-2.診療項目代號

        //*
        [XmlElement(ElementName = "A74")]
        public string TreatmentPosition { get; set; }//醫療專區 1-2-3診療部位

        //V 
        [XmlElement(ElementName = "A75")]
        public string Usage { get; set; }//醫療專區 1-2-4.用法

        //V 
        [XmlElement(ElementName = "A76")]
        public string Days { get; set; }// 醫療專區 1-2-5天數

        //V 
        [XmlElement(ElementName = "A77")]
        public string TotalAmount { get; set; }//醫療專區 1-2-6.總量

        //V
        [XmlElement(ElementName = "A78")]
        public string PrescriptionDeliveryMark { get; set; }//醫療專區 1-2-7交付處方註記

        //1,3 V 2,4 ~
        [XmlElement(ElementName = "A79")]
        public string PrescriptionSignature { get; set; }//醫療專區 1-2-8處方簽章

        //*
        [XmlElement(ElementName = "A80")]
        public string AllergyMedicineUploadMark { get; set; }//醫療專區 過敏藥物上傳註記

        //*
        [XmlElement(ElementName = "A81")]
        public string AllergyMedicine { get; set; }//過敏藥物
    }
}
