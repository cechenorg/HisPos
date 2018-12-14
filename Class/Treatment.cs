using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using His_Pos.Service;
using System.Data;
using System.Linq;
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
            TreatmentDate =  Convert.ToDateTime(Convert.ToInt32(row["HISDECMAS_PRESCRIPTIONDATE"].ToString().Substring(0, 4)) + row["HISDECMAS_PRESCRIPTIONDATE"].ToString().Substring(4, 6));
            TreatmentDate.ToShortDateString();
            AdjustDate = Convert.ToDateTime(Convert.ToInt32(row["HISDECMAS_TREATDATE"].ToString().Substring(0, 4)) + row["HISDECMAS_TREATDATE"].ToString().Substring(4, 6));
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
        public Treatment(XmlDocument xml)
        {
            MedicalInfo = new MedicalInfo(xml);
            PaymentCategory = new PaymentCategory.PaymentCategory(xml);
            AdjustCase = new AdjustCase.AdjustCase();
            AdjustCase.Id = "1";
            string treatDate = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/continous_prescription").Attributes["start_at"].Value == "" ? xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case").Attributes["date"].Value : xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/continous_prescription").Attributes["start_at"].Value;
            TreatmentDate = Convert.ToDateTime(treatDate.Substring(0, 3) + "-" + treatDate.Substring(3, 2) + "-" + treatDate.Substring(5, 2)).AddYears(1911);
            TreatChineseDate = treatDate.Substring(0, 3) + "/" + treatDate.Substring(3, 2) + "/" + treatDate.Substring(5, 2);
            AdjustDate = DateTime.Now;
            MedicineDays = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/orders").Attributes["days"].Value;
            Copayment = new Copayment.Copayment(xml);
        }
        public Treatment(DeclareFileDdata d)
        {
            MedicalInfo = new MedicalInfo(d);
            PaymentCategory = MainWindow.PaymentCategory.SingleOrDefault(p=>p.Id.Equals(d.Dhead.D5))?.DeepCloneViaJson();
            Copayment = MainWindow.Copayments.SingleOrDefault(c=>c.Id.Equals(d.Dhead.D15))?.DeepCloneViaJson();
            AdjustCase = MainWindow.AdjustCases.SingleOrDefault(a=>a.Id.Equals(d.Dhead.D1))?.DeepCloneViaJson();
            MedicineDays = !string.IsNullOrEmpty(d.Dbody.D30) ? d.Dbody.D30 : string.Empty;
            var year = int.Parse(d.Dhead.D14.Substring(0, 3)) + 1911;
            var month = int.Parse(d.Dhead.D14.Substring(3, 2));
            var date = int.Parse(d.Dhead.D14.Substring(5, 2));
            TreatmentDate = new DateTime(year,month,date);
            year = int.Parse(d.Dhead.D23.Substring(0, 3)) + 1911;
            month = int.Parse(d.Dhead.D23.Substring(3, 2));
            date = int.Parse(d.Dhead.D23.Substring(5, 2));
            AdjustDate = new DateTime(year, month, date);
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

        private DateTime treatDate;

        public DateTime TreatmentDate//d14 就醫(處方)日期
        {
            get => treatDate;
            set
            {
                treatDate = value;
                NotifyPropertyChanged(nameof(TreatmentDate));
            }
        }
        private string treatChineseDate;

        public string TreatChineseDate//d14 就醫(處方)日期 民國
        {
            get => treatChineseDate;
            set
            {
                treatChineseDate = value;
                NotifyPropertyChanged(nameof(TreatChineseDate));
            }
        }
        private DateTime _adjustDate;

        public DateTime AdjustDate
        {
            get => _adjustDate;
            set
            {
                _adjustDate = value;
                NotifyPropertyChanged(nameof(AdjustDate));
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