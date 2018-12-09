using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.HisApi;
using His_Pos.Service;
using His_Pos.Struct.IcData;
using His_Pos.ViewModel;

namespace His_Pos.Class.Declare.IcDataUpload
{
    public class IcDataUploadService
    {
        public IcDataUpload IcDataUploadTable { get; set; }
    }
    public class IcDataUpload
    {
        public RECS IcDataList { get; set; }
    }

    [XmlRoot(ElementName = "RECS")]
    public class RECS
    {
        [XmlElement(ElementName = "REC")]
        public List<REC> REC { get; set; }
    }


    [XmlRoot(ElementName = "REC")]
    public class REC
    {
        public REC(Header header, MainMessage main)
        {
            HeaderMessage = header;
            MainMessage = main;
        }

        public REC(DataRow row)
        {
        }

        public REC()
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
        [XmlElement(ElementName = "MB1")]
        public IcData IcMessage { get; set; }
        [XmlElement(ElementName = "MB2")]
        public List<MedicalData> MedicalMessageList { get; set; } = new List<MedicalData>();
    }

    [XmlRoot(ElementName = "MB1")]
    public class IcData
    {
        public IcData() { }
        public IcData(SeqNumber seq,Prescription currentPrescription,BasicData customerData,DeclareData currentDeclareData)
        {
            SamCode = seq.SamId;
            CardNo = currentPrescription.Customer.IcCard.CardNo;
            IcNumber = currentPrescription.Customer.IcCard.IcNumber;
            BirthDay = customerData.Birthday;
            TreatmentDateTime = seq.TreatDateTime;
            MedicalNumber = seq.MedicalNumber;
            PharmacyId = seq.InstitutionId;
            MedicalPersonIcNumber = currentPrescription.Pharmacy.MedicalPersonnel.IcNumber;
            SecuritySignature = seq.SecuritySignature;
            MainDiagnosisCode = currentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(currentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                SecondDiagnosisCode = currentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            OutpatientFee = (currentDeclareData.DrugsPoint + currentDeclareData.SpecailMaterialPoint +
                             currentDeclareData.CopaymentPoint + currentDeclareData.MedicalServicePoint).ToString();
            OutpatientCopaymentFee = currentDeclareData.CopaymentPoint.ToString();
        }

        public IcData(Prescription current,IcErrorCodeWindow.IcErrorCode errorCode,DeclareData currentDeclareData)
        {
            IcNumber = current.Customer.IcCard.IcNumber;
            BirthDay = DateTimeExtensions.ConvertToTaiwanCalender(current.Customer.Birthday, false);
            var cs = new ConvertData();
            var pBuffer = new byte[13];
            var iBufferlength = 13;
            var now = DateTime.Now;
            if (HisApiBase.GetStatus(1) && HisApiBase.GetStatus(2))
            {
                if (HisApiBase.csGetDateTime(pBuffer, ref iBufferlength) == 0)
                {
                    TreatmentDateTime = cs.ByToString(pBuffer, 0, iBufferlength);
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
            PharmacyId = MainWindow.CurrentPharmacy.Id;
            MedicalNumber = errorCode.Id;
            MedicalPersonIcNumber = current.Pharmacy.MedicalPersonnel.IcNumber;
            MainDiagnosisCode = current.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(current.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                SecondDiagnosisCode = current.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            OutpatientFee = (currentDeclareData.DrugsPoint + currentDeclareData.SpecailMaterialPoint +
                             currentDeclareData.CopaymentPoint + currentDeclareData.MedicalServicePoint).ToString();
            OutpatientCopaymentFee = currentDeclareData.CopaymentPoint.ToString();
        }
        //1,3 V  2,4 ~ 
        [XmlElement(ElementName = "A16")]
        public string SamCode { get; set; }//安全模組代碼

        //1,3 V  2,4 ~
        [XmlElement(ElementName = "A11")]
        public string CardNo { get; set; }//卡片號碼 (get by HISAPI : csGetCardNo)

        //V
        [XmlElement(ElementName = "A12")]
        public string IcNumber { get; set; }//身分證號或 身分證明文件號碼

        //V
        [XmlElement(ElementName = "A13")]
        public string BirthDay { get; set; }//出生日期

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

        //V
        [XmlElement(ElementName = "A17")]
        public string TreatmentDateTime { get; set; } //健保資料段 8-3.就診日期時間 (get by HISAPI : hisGetSeqNumber256)

        //*
        [XmlElement(ElementName = "A18")]
        public string MedicalNumber { get; set; }//健保資料段 8-5.就醫序號(get by HISAPI : hisGetSeqNumber256)

        //V
        [XmlElement(ElementName = "A19")]
        public string MakeUpMark { get; set; } = "1";//健保資料段 8-4.補卡註記(get by HISAPI : hisGetTreatmentNoNeedHPC)

        //V
        [XmlElement(ElementName = "A14")]
        public string PharmacyId { get; set; }//健保資料段 8-6.醫療院所代碼

        //V
        [XmlElement(ElementName = "A15")]
        public string MedicalPersonIcNumber { get; set; }//健保資料段 8-7-1.醫事人員身分證號

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
        public string OutpatientFee { get; set; }//健保資料段 8-10-1.門診醫療費用 （當次） (get by HISAPI : hisGetTreatmentNoNeedHPC)

        //*
        [XmlElement(ElementName = "A32")]
        public string OutpatientCopaymentFee { get; set; }//健保資料段 8-10-2.門診部分負擔費用（當次）(get by HISAPI : hisGetTreatmentNoNeedHPC)

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
