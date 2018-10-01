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
using His_Pos.Class.Declare;

namespace His_Pos.Class
{
    public class Treatment : INotifyPropertyChanged
    {
        public Treatment(DataRow row)
        {
            MedicalInfo = new MedicalInfo(row);
            Copayment = new Copayment.Copayment(row);
            AdjustCase = new AdjustCase.AdjustCase(row);
            MedicineDays = "0";
            TreatmentDate = DateTimeExtensions.ToUsDate(row["HISDECMAS_PRESCRIPTIONDATE"].ToString().Substring(1, 9));
            TreatmentDate.ToShortDateString();
            AdjustDate = DateTimeExtensions.ToUsDate(row["HISDECMAS_TREATDATE"].ToString().Substring(1, 9));
            AdjustDate.ToShortDateString();
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
        }
        public Treatment(XmlNode xml) {
            MedicalInfo = new MedicalInfo(xml);
            PaymentCategory = new PaymentCategory.PaymentCategory(xml);
            Copayment = new Copayment.Copayment(xml);
            AdjustCase = new AdjustCase.AdjustCase(xml);
            AdjustDate = xml.SelectSingleNode("d23") == null ? new DateTime() : DateTime.ParseExact(xml.SelectSingleNode("d23").InnerText, "yyyMMdd", CultureInfo.InvariantCulture).AddYears(1911);
            TreatmentDate = xml.SelectSingleNode("d14") == null ? new DateTime() : DateTime.ParseExact(xml.SelectSingleNode("d14").InnerText, "yyyMMdd", CultureInfo.InvariantCulture).AddYears(1911);
            MedicineDays = xml.SelectSingleNode("d30") == null ? null : xml.SelectSingleNode("d30").InnerText;
        }

        public Treatment(DeclareFileDdata d)
        {
            MedicalInfo = new MedicalInfo(d);
            PaymentCategory = new PaymentCategory.PaymentCategory(d);
            Copayment = new Copayment.Copayment(d);
            AdjustCase = new AdjustCase.AdjustCase(d);
            MedicineDays = !string.IsNullOrEmpty(d.Dbody.D30) ? d.Dbody.D30 : string.Empty;
        }

        private MedicalInfo _medicalInfo;
        //d8 d9 國際疾病分類碼 d13就醫科別  d21原處方服務機構代號 d22原處方服務機構之案件分類 d24診治醫師代號 d26原處方服務機構之特定治療項目代號
        public MedicalInfo MedicalInfo
        {
            get => _medicalInfo;
            set
            {
                _medicalInfo = value;
                NotifyPropertyChanged(nameof(MedicalInfo));
            }
        }

        private PaymentCategory.PaymentCategory _paymentCategory;

        public PaymentCategory.PaymentCategory PaymentCategory
        {
            get => _paymentCategory;
            set
            {
                _paymentCategory = value;
                NotifyPropertyChanged(nameof(PaymentCategory));
            }
        } //d5 給付類別

        private Copayment.Copayment _copayment;

        public Copayment.Copayment Copayment
        {
            get => _copayment;
            set
            {
                _copayment = value;
                NotifyPropertyChanged(nameof(Copayment));
            }
        } //d15 部分負擔代碼

        private AdjustCase.AdjustCase _adjustCase;

        public AdjustCase.AdjustCase AdjustCase
        {
            get => _adjustCase;
            set
            {
                _adjustCase = value;
                NotifyPropertyChanged(nameof(AdjustCase));
            }
        } //d1 案件分類

        private string _adjustDateStr;

        public string AdjustDateStr
        {
            get { return _adjustDateStr; }
            set
            {
                _adjustDateStr = value;
                NotifyPropertyChanged("AdjustDateStr");
            }
        }

        private string _treatDateStr;

        public string TreatDateStr
        {
            get { return _treatDateStr; }
            set
            {
                _treatDateStr = value;
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
                if(value != DateTime.MinValue)
                    TreatDateStr = DateTimeExtensions.ToSimpleTaiwanDate(value);
                NotifyPropertyChanged("TreatmentDate");
            }
        }

        private DateTime _adjustDate;

        public DateTime AdjustDate
        {
            get { return _adjustDate; }
            set
            {
                _adjustDate = value;
                if (value != DateTime.MinValue)
                    AdjustDateStr = DateTimeExtensions.ToSimpleTaiwanDate(value);
                NotifyPropertyChanged("AdjustDate");
            }
        }//d23 調劑日期

        public string MedicineDays { get; set; }//d30  給藥日份

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