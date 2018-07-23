using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.Service;
using System.Data;

namespace His_Pos.Class
{
    public class Treatment : INotifyPropertyChanged
    {
        public Treatment(DataRow row)
        {
            MedicalInfo = new MedicalInfo(row);
            Copayment = new Copayment.Copayment();
            AdjustCase = new AdjustCase.AdjustCase(row);
            MedicineDays = "0";
            TreatmentDate = DateTimeExtensions.ToUsDate(row["HISDECMAS_TREATDATE"].ToString().Substring(1, 9));
            AdjustDate = DateTime.Today;
            PaymentCategory = new PaymentCategory.PaymentCategory(row);
        }

        public Treatment()
        {
            MedicalInfo = new MedicalInfo();
            Copayment = new Copayment.Copayment();
            AdjustCase = new AdjustCase.AdjustCase();
            MedicineDays = "0";
            TreatmentDate = DateTime.Today;
            AdjustDate = DateTime.Today;
        }

        public Treatment(MedicalInfo medicalInfo, PaymentCategory.PaymentCategory paymentCategory, Copayment.Copayment copayment, AdjustCase.AdjustCase adjustCase, DateTime treatmentDate, DateTime adjustDate, string medicineDays, string medicalPersonId)
        {
            MedicalInfo = medicalInfo;
            PaymentCategory = paymentCategory;
            Copayment = copayment;
            AdjustCase = adjustCase;
            TreatmentDate = treatmentDate;
            AdjustDate = adjustDate;
            MedicineDays = medicineDays;
            MedicalPersonId = medicalPersonId;
        }
        public Treatment(XmlNode xml) {
            MedicalInfo = new MedicalInfo(xml);
            PaymentCategory = new PaymentCategory.PaymentCategory(xml);
            Copayment = new Copayment.Copayment(xml);
            AdjustCase = new AdjustCase.AdjustCase(xml);
            TreatmentDate = DateTimeExtensions.ToUsDate(xml.SelectSingleNode("d30") == null ? null : xml.SelectSingleNode("d30").InnerText);
            MedicineDays = xml.SelectSingleNode("d30") == null ? null : xml.SelectSingleNode("d30").InnerText;
            MedicalPersonId = xml.SelectSingleNode("d25") == null ? null : xml.SelectSingleNode("d25").InnerText;
        }
        //d8 d9 國際疾病分類碼 d13就醫科別  d21原處方服務機構代號 d22原處方服務機構之案件分類 d24診治醫師代號 d26原處方服務機構之特定治療項目代號
        public MedicalInfo MedicalInfo { get; set; }

        public PaymentCategory.PaymentCategory PaymentCategory { get; set; } = new PaymentCategory.PaymentCategory();//d5 給付類別
        public Copayment.Copayment Copayment { get; set; } //d15 部分負擔代碼
        public AdjustCase.AdjustCase AdjustCase { get; set; } //d1 案件分類
        private string adjustDateStr;

        public string AdjustDateStr
        {
            get { return DateTimeExtensions.ToSimpleTaiwanDate(AdjustDate); }
            set
            {
                adjustDateStr = value;
                NotifyPropertyChanged("AdjustDateStr");
            }
        }

        private string treatDateStr;

        public string TreatDateStr
        {
            get { return DateTimeExtensions.ToSimpleTaiwanDate(TreatmentDate); }
            set
            {
                treatDateStr = value;
                NotifyPropertyChanged("TreatDateStr");
            }
        }

        private DateTime treatDate;

        public DateTime TreatmentDate//d14 就醫(處方)日期
        {
            get { return treatDate; }
            set
            {
                treatDate = value;
                TreatDateStr = DateTimeExtensions.ToSimpleTaiwanDate(value);
                NotifyPropertyChanged("TreatmentDate");
            }
        }

        private DateTime adjustDate;

        public DateTime AdjustDate
        {
            get { return adjustDate; }
            set
            {
                adjustDate = value;
                AdjustDateStr = DateTimeExtensions.ToSimpleTaiwanDate(value);
                NotifyPropertyChanged("AdjustDate");
            }
        }//d23 調劑日期

        public string MedicineDays { get; set; }//d30  給藥日份
        public string MedicalPersonId { get; set; }//d25 醫事人員代號

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}