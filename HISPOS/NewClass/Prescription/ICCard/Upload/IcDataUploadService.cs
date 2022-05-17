using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;

namespace His_Pos.NewClass.Prescription.ICCard.Upload
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

        public Rec(Prescription p, bool isMakeUp, ErrorUploadWindowViewModel.IcErrorCode e = null)
        {
            HeaderMessage = new Header();
            HeaderMessage.DataFormat = "1";
            HeaderMessage.DataFormat = e is null ? "1" : "2";

            HeaderMessage.UploadVersion = "1.0";
            MainMessage = new MainMessage(p, e, isMakeUp);
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

        //V
        [XmlElement(ElementName = "A00")]
        public string DataType { get; set; } = "1";//資料型態

        /*
         * V : 必填欄位 ~ : 不填欄位 * : 選填欄位
         * 資料格式 :
         * 1:正常上傳
         * 2:異常上傳
         * 3:補正上傳 (正常資料)
         * 4:補正上傳 (異常資料))
         */

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
        public MainMessage()
        {
        }

        public MainMessage(IcData icData)
        {
            IcMessage = icData;
        }

        public MainMessage(Prescription p, ErrorUploadWindowViewModel.IcErrorCode e, bool makeUp)
        {
            IcMessage = new IcData(p, e, makeUp);
            MedicalMessageList = new List<MedicalData>();
            var treatDateTime = IcMessage.TreatmentDateTime;
            var medList = p.Medicines.Where(m => (m is MedicineNHI || m is MedicineSpecialMaterial || m is MedicineVirtual) && !m.PaySelf).ToList();
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
        public IcData()
        {
        }

        public IcData(Prescription p, ErrorUploadWindowViewModel.IcErrorCode e, bool makeUp)
        {
            var seq = p.Card.MedicalNumberData;
            if (!string.IsNullOrEmpty(p.Card.TreatDateTime))
            {
                TreatmentDateTime = p.Card.TreatDateTime;
            }
            else
            {
                TreatmentDateTime = DateTimeEx.ToStringWithSecond(DateTime.Now);
                try
                {
                    if (HisApiFunction.OpenCom() && HisApiBase.hisGetCardStatus(1) == 2)
                    {
                        var iBufferLength = 13;
                        var pBuffer = new byte[iBufferLength];
                        var res = HisApiBase.csGetDateTime(pBuffer, ref iBufferLength);
                        TreatmentDateTime = res == 0 ?
                            ConvertData.ByToString(pBuffer, 0, 13) :
                            DateTimeEx.ToStringWithSecond(DateTime.Now);
                        HisApiFunction.CloseCom();
                    }
                    else
                    {
                        TreatmentDateTime = DateTimeEx.ToStringWithSecond(DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    //(20220513)處方調劑讀取健保卡閃退
                    MessageWindow.ShowMessage(Resources.控制軟體異常, MessageType.ERROR);
                }
            }
            if (e is null)
            {
                CardNo = p.Card.CardNumber;
                SamCode = seq.SamId;
                SecuritySignature = seq.SecuritySignature;
                IDNumber = p.Card.IDNumber;
                BirthDay = DateTimeEx.ConvertToTaiwanCalender(p.Card.PatientBasicData.Birthday);
                MedicalNumber = string.Empty;
                PharmacyId = seq.InstitutionId;
            }
            else
            {
                IDNumber = p.Patient.IDNumber;
                BirthDay = DateTimeEx.ConvertToTaiwanCalender((DateTime)p.Patient.Birthday);
                MedicalNumber = e.ID;
                PharmacyId = ViewModelMainWindow.CurrentPharmacy.ID;
            }
            if (makeUp || DateTime.Compare(((DateTime)p.AdjustDate).Date, DateTime.Now.Date) < 0)
                MakeUpMark = "2";
            MedicalPersonIcNumber = p.Pharmacist.IDNumber;
            MainDiagnosisCode = p.MainDisease.ID;
            if (!string.IsNullOrEmpty(p.SubDisease?.ID))
                SecondDiagnosisCode = p.SubDisease.ID;
            MedicalFee = (p.PrescriptionPoint.MedicinePoint + p.PrescriptionPoint.SpecialMaterialPoint +
                             p.PrescriptionPoint.CopaymentPoint + p.PrescriptionPoint.MedicalServicePoint).ToString();
            CopaymentFee = p.PrescriptionPoint.CopaymentPoint.ToString();
            if (makeUp || DateTime.Compare(((DateTime)p.AdjustDate).Date, DateTime.Now.Date) < 0)
                ActualTreatDate = DateTimeEx.ConvertToTaiwanCalender((DateTime)p.AdjustDate);
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

        public MedicalData(Medicine.Base.Medicine med, string treatDateTime, string preSig = null)
        {
            MedicalOrderTreatDateTime = treatDateTime;
            MedicalOrderCategory = med is MedicineSpecialMaterial ? "4" : "1";
            TreatmentProjectCode = med is MedicineSpecialMaterial ? med.ID.Substring(0, 12) : med.ID;
            if (!string.IsNullOrEmpty(med.PositionID))
                TreatmentPosition = med.PositionID;
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
            if (!string.IsNullOrEmpty(preSig))
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