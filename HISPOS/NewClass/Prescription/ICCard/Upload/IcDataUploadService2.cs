using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.ErrorUploadWindow;
using His_Pos.HisApi;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using DateTimeEx = His_Pos.Service.DateTimeExtensions;

namespace His_Pos.NewClass.Prescription.ICCard.Upload
{
    public class IcDataUploadService2
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
                HeaderMessage.DataFormat = "A";
                HeaderMessage.DataFormat = e is null ? "A" : e.Content;

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
            [XmlElement(ElementName = "H00")]
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
            [XmlElement(ElementName = "H01")]
            public string DataFormat { get; set; }//資料格式

            //V
            //[XmlElement(ElementName = "A02")]
            //public string UploadVersion { get; set; } = "2.0";//上傳版本 (就醫上傳版本現階段均為1.0)
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
                TreatmentDateTime = p.Card.TreatDateTime.PadRight(13, '0');
                CardNo = p.Card.CardNumber;
                SamCode = seq.SamId;
                SecuritySignature = seq.SecuritySignature;
                IDNumber = p.Card.IDNumber;
                BirthDay = p.Card.PatientBasicData.Birthday != null && p.Card.PatientBasicData.Birthday != new DateTime() ? p.Card.PatientBasicData.Birthday.ToString("yyyMMdd") : DateTimeEx.ConvertToTaiwanCalender(Convert.ToDateTime(p.Patient.Birthday));
                PharmacyId = ViewModelMainWindow.CurrentPharmacy.ID;

                AdjustDay = p.AdjustDay;
                OrTreatmentDateTime = DateTimeEx.ConvertToTaiwanCalender(Convert.ToDateTime(p.TreatDate)).PadRight(13, '0');
                if(p.PaymentCategory != null)
                {
                    PaymentCategory = p.PaymentCategory.ID;
                }

                TreatmentCode = p.TreatmentCode;
                OrTreatmentCode = string.IsNullOrEmpty(p.OrigTreatmentCode) ? "99999999999999999999" : p.OrigTreatmentCode;
                CuOrgCode = p.Institution.ID;

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
                    ActualTreatDate = DateTimeEx.ConvertToTaiwanCalender((DateTime)p.AdjustDate).PadRight(13, '0');
            }

            //1,3 V  2,4 ~
            [XmlElement(ElementName = "M01")]
            public string SamCode { get; set; }//安全模組代碼

            //1,3 V  2,4 ~
            [XmlElement(ElementName = "M02")]
            public string CardNo { get; set; }//卡片號碼 (get by HISAPI : csGetCardNo)

            //V
            [XmlElement(ElementName = "M03")]
            public string IDNumber { get; set; }//身分證號或 身分證明文件號碼

            //V
            [XmlElement(ElementName = "M04")]
            public string BirthDay { get; set; }//出生日期

            //V
            [XmlElement(ElementName = "M05")]
            public string PharmacyId { get; set; }//健保資料段 8-6.醫療院所代碼

            //V
            [XmlElement(ElementName = "M06")]
            public string MedicalPersonIcNumber { get; set; }//健保資料段 8-7-1.醫事人員身分證號

            //V
            [XmlElement(ElementName = "M07")]
            public string TreatmentCategory { get; set; } = "AF";//健保資料段 8-1.就醫類別

            //*
            [XmlElement(ElementName = "M08")]
            public string NewbornBirthDay { get; set; }//健保資料段 7-1.新生兒出生日期

            //*
            [XmlElement(ElementName = "M09")]
            public string NewbornBabyMark { get; set; }//健保資料段 7-2.新生兒胞胎註記

            //*
            [XmlElement(ElementName = "M10")]
            public string NewbornTreatmentMark { get; set; }//健保資料段 8-2.新生兒就醫註記

            //V
            [XmlElement(ElementName = "M11")]
            public string TreatmentDateTime { get; set; } //健保資料段 8-3.就診日期時間 (get by HISAPI : hisGetSeqNumber256)

            //V
            [XmlElement(ElementName = "M12")]
            public string MakeUpMark { get; set; } = "1";//健保資料段 8-4.補卡註記(get by HISAPI : hisGetTreatmentNoNeedHPC)

            //*
            [XmlElement(ElementName = "M13")]
            public string MedicalNumber { get; set; }//健保資料段 8-5.就醫序號(get by HISAPI : hisGetSeqNumber256)

            //1,3 V  2,4 ~
            [XmlElement(ElementName = "M14")]
            public string SecuritySignature { get; set; }//健保資料段 8-7-2安全簽章

            
            [XmlElement(ElementName = "M15")]
            public string TreatmentCode { get; set; }//健保資料段 8-7-2就醫識別碼

            [XmlElement(ElementName = "M16")]
            public string OrTreatmentCode { get; set; }//健保資料段 8-7-2原就醫識別碼

            [XmlElement(ElementName = "M17")]
            public string CuOrgCode { get; set; }//健保資料段 8-7-2原處方服務機構代號 

            [XmlElement(ElementName = "M18")]
            public string OrMedicalNumber { get; set; }//健保資料段 8-7-2原處方就醫序號

            [XmlElement(ElementName = "M19")]
            public string OrTreatmentDateTime { get; set; }//健保資料段 8-7-2原就診日期時間

            [XmlElement(ElementName = "M20")]
            public string AdjustDay { get; set; }//健保資料段 8-7-2給藥天數 

            [XmlElement(ElementName = "M21")]
            public string ChronicGiveDay { get; set; }//健保資料段 8-7-2慢性病連續處方箋總給藥天數
                                                      //
            [XmlElement(ElementName = "M22")]
            public string ControlChronicGiveDay { get; set; }//健保資料段 8-7-2管制藥品專用處方箋(慢連箋)總給藥天數

            [XmlElement(ElementName = "M23")]
            public string AdjustmentMethod { get; set; }//健保資料段 8-8.處方調劑方式

            [XmlElement(ElementName = "M24")]
            public string AdjustCountA { get; set; }//健保資料段 8-8.可調劑次數_A-⼀般處方箋

            [XmlElement(ElementName = "M25")]
            public string AdjustCountB { get; set; }//健保資料段 8-8.可調劑次數_B-慢性病處方箋

            [XmlElement(ElementName = "M26")]
            public string AdjustCountC { get; set; }//健保資料段 8-8.連續處方箋可調劑次數_C-慢性病連續處方箋

            [XmlElement(ElementName = "M27")]
            public string AdjustCountD { get; set; }//健保資料段 8-8.可調劑次數_D-管制藥品專用處方箋(⼀般)

            [XmlElement(ElementName = "M28")]
            public string AdjustCountE { get; set; }//可調劑次數_E-管制藥品專用處方箋(慢箋)

            [XmlElement(ElementName = "M29")]
            public string AdjustCountF { get; set; }//連續處方箋可調劑次數_F-管制藥品專用處方箋(慢連箋) 

            [XmlElement(ElementName = "M30")]
            public string TreatmentCountA { get; set; }//物理治療數量/已執⾏數量 

            [XmlElement(ElementName = "M31")]
            public string TreatmentCountB { get; set; }//職能治療數量/已執⾏數量

            [XmlElement(ElementName = "M32")]
            public string TreatmentCountC { get; set; }//語言治療數量/已執⾏數量

            [XmlElement(ElementName = "M33")]
            public string AdjuctRowC { get; set; }//當次調劑連續處方箋次數/序號_C-慢性病 連續處方箋 

            [XmlElement(ElementName = "M34")]
            public string AdjuctRowD { get; set; }//當次調劑連續處方箋次數/序號_F-管制藥 品專用處方箋(慢連箋) 

            //V
            [XmlElement(ElementName = "M35")]
            public string MainDiagnosisCode { get; set; }//健保資料段 8-8.主要診斷碼

            //*
            [XmlElement(ElementName = "M36")]
            public string SecondDiagnosisCode { get; set; }//健保資料段 8-8.主要診斷碼

            //V
            [XmlElement(ElementName = "M44")]
            public string MedicalFee { get; set; }//健保資料段 8-10-1.門診醫療費用 （當次） (get by HISAPI : hisGetTreatmentNoNeedHPC)

            //*
            [XmlElement(ElementName = "M45")]
            public string CopaymentFee { get; set; }//健保資料段 8-10-2.門診部分負擔費用（當次）(get by HISAPI : hisGetTreatmentNoNeedHPC)

            //*
            [XmlElement(ElementName = "M46")]
            public string HospitalizationFee { get; set; }//健保資料段 8-10-3.住院醫療費用(當次)(get by HISAPI : hisGetTreatmentNoNeedHPC)

            //*
            [XmlElement(ElementName = "M47")]
            public string HospitalizationCopaymentFeeLess { get; set; }// 健保資料段 8-10-4.住院部分負擔費用（當次急性30天、慢性180天以下）(get by HISAPI : hisGetTreatmentNoNeedHPC)

            //*
            [XmlElement(ElementName = "M48")]
            public string HospitalizationCopaymentFeeMore { get; set; }//健保資料段8-10-5.住院部分負擔費用（當次急性31天、慢性181天以上）(get by HISAPI : hisGetTreatmentNoNeedHPC)

            [XmlElement(ElementName = "M49")]
            public string ActualTreatDate { get; set; }//健保資料段8-10-5.住院部分負擔費用（當次急性31天、慢性181天以上）(get by HISAPI : hisGetTreatmentNoNeedHPC)

            [XmlElement(ElementName = "M51")]
            public string PaymentCategory { get; set; }//健保資料段8-10-5 給付類別
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
                PositionID = med.PositionID;

                MedicalCategory = "A";
                switch (MedicalOrderCategory)
                {
                    case "1":
                        PrescriptionDeliveryMark = "1";
                        break;

                    case "A":
                        PrescriptionDeliveryMark = "2";
                        break;

                    case "2":
                        PrescriptionDeliveryMark = "5";
                        break;

                    case "B":
                        PrescriptionDeliveryMark = "6";
                        break;

                    case "3":
                    case "4":
                    case "5":
                        PrescriptionDeliveryMark = "3";
                        break;

                    case "C":
                    case "D":
                    case "E":
                        PrescriptionDeliveryMark = "4";
                        break;
                }
                if (!string.IsNullOrEmpty(preSig))
                    PrescriptionSignature = preSig;
            }

            //V
            [XmlElement(ElementName = "D01")]
            public string MedicalOrderTreatDateTime { get; set; }//醫療專區 1-1.醫令就診日期時間

            //V
            [XmlElement(ElementName = "D02")]
            public string MedicalOrderCategory { get; set; }//醫療專區 1-2-1醫令類別

            //V
            [XmlElement(ElementName = "D03")]
            public int MedicalRow { get; set; }//醫療專區 1-2-1醫令序號

            [XmlElement(ElementName = "D04")]
            public string MedicalCategory { get; set; }//醫療專區 1-2-1醫令序號

            //V
            [XmlElement(ElementName = "D05")]
            public string PrescriptionDeliveryMark { get; set; }//醫療專區 1-2-7交付處方註記

            //V
            [XmlElement(ElementName = "D06")]
            public string TreatmentProjectCode { get; set; }//醫療專區 1-2-2.診療項目代號

            //*
            [XmlElement(ElementName = "D07")]
            public string TreatmentPosition { get; set; }//醫療專區 1-2-3診療部位

            //V
            [XmlElement(ElementName = "D08")]
            public string Usage { get; set; }//醫療專區 1-2-4.用法

            //V
            [XmlElement(ElementName = "D09")]
            public string Days { get; set; }// 醫療專區 1-2-5天數

            //V
            [XmlElement(ElementName = "D10")]
            public string TotalAmount { get; set; }//醫療專區 1-2-6.總量

            //1,3 V 2,4 ~
            [XmlElement(ElementName = "D11")]
            public string PrescriptionSignature { get; set; }//醫療專區 1-2-8處方簽章

            [XmlElement(ElementName = "D14")]
            public string PositionID { get; set; }//醫療專區 1-2-6.用途

            //*
            [XmlElement(ElementName = "E01")]
            public string AllergyMedicineUploadMark { get; set; }//醫療專區 過敏藥物上傳註記

            //*
            [XmlElement(ElementName = "E04")]
            public string AllergyMedicine { get; set; }//過敏藥物
        }
    }
}
