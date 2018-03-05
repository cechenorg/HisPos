using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionRevise
{
    /// <summary>
    /// PrescriptionRevise.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionReviseView : UserControl
    {
        struct DeclareHistory
        {
            private string _id;
            private string _sentDate;

            public DeclareHistory(string id, string sentDate)
            {
                _id = id;
                _sentDate = sentDate;
            }

            public string GetId()
            {
                return _id;
            }
            public string GetSentDate()
            {
                return _sentDate;
            }
        }

        private struct DeclaredPrescription
        {
            public string DecMasId { get; set; }
            public string CasCatId { get; set; }
            public string DivName { get; set; }
            public string OrderId { get; set; }
            public string CusName { get; set; }
            public string EmpName { get; set; }
            public string DecMasTreatDate { get; set; }
            public string InsName { get; set; }
            public DeclaredPrescription(string decMasId, string casCatId, string divName, string orderId, string cusName, string empName, string decMasTreatDate, string insName)
            {
                DecMasId = decMasId;
                CasCatId = casCatId;
                DivName = divName;
                OrderId = orderId;
                CusName = cusName;
                EmpName = empName;
                DecMasTreatDate = decMasTreatDate;
                InsName = insName;
            }
        }
        private string _id = string.Empty;
        private DeclareData _declare;
        private Prescription _prescription;
        private ObservableCollection<DeclareHistory> DeclareHistories { get; } = new ObservableCollection<DeclareHistory>();
        private ObservableCollection<DeclaredPrescription> DeclaredPrescriptions { get; set; } = new ObservableCollection<DeclaredPrescription>();
        public PrescriptionReviseView()
        {
            InitializeComponent();
            InitialReviseUiElement();
            DataContext = this;
        }

        private void InitialReviseUiElement()
        {
            string[] caseCategory = { "1: 一般處方調劑", "2: 慢性病連續處方調劑", "3: 日劑藥費", "4: 肺結核個案DOTS執行服務費", "5: 協助辦理門診戒菸計畫", "D: 藥事居家照護" };

            for (int i = 0; i < 6; i++)
            {
                CaseCateCombo.Items.Add(caseCategory[i]);
            }

            var cui = new CultureInfo("zh-TW", true)
            {
                DateTimeFormat =
                {
                    Calendar = new TaiwanCalendar(),
                    ShortDatePattern = "yy/MM/dd",
                    DateSeparator = "/"
                }
            };
            Thread.CurrentThread.CurrentCulture = cui;
            sdatePicker1.SelectedDateFormat = DatePickerFormat.Short;
            edatePicker1.SelectedDateFormat = DatePickerFormat.Short;
        }

        private void LoadDeclareMonth()
        {
            if (sdatePicker1.SelectedDate == null)
            {
                MessageBox.Show("請選擇起始日期");
                return;
            }
            if (edatePicker1.SelectedDate == null)
            {
                MessageBox.Show("請選擇結束日期");
                return;
            }
            var dd = new DbConnection(Settings.Default.SQL_global);
            var list = new List<SqlParameter>();
            var function = new Function();
            string sDate = function.dateFormatConvert(Convert.ToDateTime(sdatePicker1.SelectedDate));
            string eDate = function.dateFormatConvert(Convert.ToDateTime(edatePicker1.SelectedDate));
            list.Add(new SqlParameter("SDATE", sDate));
            list.Add(new SqlParameter("EDATE", eDate));
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREHISTORY]", list);

            foreach (DataRow d in table.Rows)
            {
                DeclareHistory declareHistory = new DeclareHistory(d["HISDEC_ID"].ToString(), d["HISDEC_SENTDATE"].ToString().Substring(0, 9));
                DeclareHistories.Add(declareHistory);
                DeclareFile.Items.Add(d["HISDEC_SENTDATE"].ToString().Substring(0, 9));
            }
        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            Debug.Assert(row != null, nameof(row) + " != null");
            row.InputBindings.Add(new MouseBinding(ShowCustomDialogCommand,
                new MouseGesture() { MouseAction = MouseAction.LeftDoubleClick }));
        }

        private ICommand _showCustomDialogCommand;

        private ICommand ShowCustomDialogCommand
        {
            get
            {
                return _showCustomDialogCommand ?? (_showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private void RunCustomFromVm()
        {
            DeclaredPrescription selected = PrescriptionSet.SelectedItem is DeclaredPrescription ? (DeclaredPrescription)PrescriptionSet.SelectedItem : new DeclaredPrescription();
            var masId = selected.DecMasId;
            ParseDeclareDataXml(masId);
        }

        private void LoadDeclaredPrescriptions(object sender, RoutedEventArgs e)
        {
            DeclaredPrescriptions.Clear();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var list = new List<SqlParameter>();
            list.Add(new SqlParameter("DECID", _id));
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREDATABYDECID]", list);
            foreach (DataRow d in table.Rows)
            {
                DeclaredPrescription dec = new DeclaredPrescription(d["HISDECMAS_ID"].ToString(), d["HISCASCAT_ID"].ToString(), d["HISDIV_NAME"].ToString(), d["ORDER_ID"].ToString(), d["CUS_NAME"].ToString(), d["EMP_NAME"].ToString(), d["HISDECMAS_TREATDATE"].ToString(), d["INS_NAME"].ToString());
                DeclaredPrescriptions.Add(dec);
            }
            PrescriptionSet.ItemsSource = DeclaredPrescriptions;
        }

        private void DeclareFile_DropDownOpened(object sender, EventArgs e)
        {
            DeclareFile.Items.Clear();
            LoadDeclareMonth();
        }

        private void DeclareFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            string select;
            if (comboBox != null && comboBox.SelectedItem == null)
                select = comboBox.Text;
            else
            {
                select = comboBox.SelectedItem.ToString();
            }
            foreach (var history in DeclareHistories)
            {
                if (history.GetSentDate().Contains(select))
                    _id = history.GetId();
            }
        }

        private void ParseDeclareDataXml(string masId)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var list = new List<SqlParameter>();
            list.Add(new SqlParameter("MASID", masId));
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[DECLAREDATABYMASID]", list);
            var xml = new XmlDocument();
            xml.LoadXml(table.Rows[0]["HISDECMAS_DETXML"].ToString());
            var cus = new Customer();
            var medicalCase = xml.SelectSingleNode("ddata/dhead/d22")?.InnerText;
            var copayment = xml.SelectSingleNode("ddata/dhead/d15")?.InnerText;
            var medicalPersonnel = xml.SelectSingleNode("ddata/dhead/d25")?.InnerText;
            var division = xml.SelectSingleNode("ddata/dhead/d13")?.InnerText;
            var treatDate = xml.SelectSingleNode("ddata/dhead/d23")?.InnerText;
            var medicineDays = short.Parse(xml.SelectSingleNode("ddata/dhead/d30")?.InnerText);
            var treatmentCode1 = xml.SelectSingleNode("ddata/dhead/d8")?.InnerText;
            var treatmentCode2 = xml.SelectSingleNode("ddata/dhead/d9")?.InnerText;
            var doctorName = xml.SelectSingleNode("ddata/dhead/d24")?.InnerText;
            int chronicSequence = 0;
            if (xml.SelectSingleNode("ddata/dhead/d35") != null)
                chronicSequence = int.Parse(xml.SelectSingleNode("ddata/dhead/d35").InnerText);
            int chronicDeliveryTimes = 0;
            if (xml.SelectSingleNode("ddata/dhead/d36") != null)
                chronicDeliveryTimes = int.Parse(xml.SelectSingleNode("ddata/dhead/d36")?.InnerText);
            cus.Birthday = xml.SelectSingleNode("ddata/dhead/d6")?.InnerText;
            cus.IcNumber = xml.SelectSingleNode("ddata/dhead/d3")?.InnerText;
            cus.Name = xml.SelectSingleNode("ddata/dhead/d20")?.InnerText;
            _declare = new DeclareData();
            //_declare.Details.Clear();
            //var pelemList = xml.GetElementsByTagName("pdata");
            //for (var i = 0; i < pelemList.Count; i++)
            //{
            //    var p12 = pelemList[i].InnerXml.Contains("p12") ? "" : xml.SelectSingleNode("ddata/pdata/p12")?.InnerText;
            //    var p13 = pelemList[i].InnerXml.Contains("p13") ? "" : xml.SelectSingleNode("ddata/pdata/p13")?.InnerText;
            //    _declare.AddDetail(new DeclareDetail(
            //        xml.SelectSingleNode("ddata/pdata/p1")?.InnerText,
            //        xml.SelectSingleNode("ddata/pdata/p2")?.InnerText,
            //        xml.SelectSingleNode("ddata/pdata/p3")?.InnerText,
            //        xml.SelectSingleNode("ddata/pdata/p4")?.InnerText,
            //        xml.SelectSingleNode("ddata/pdata/p5")?.InnerText,
            //        xml.SelectSingleNode("ddata/pdata/p6")?.InnerText,
            //        double.Parse(xml.SelectSingleNode("ddata/pdata/p7")?.InnerText ?? throw new InvalidOperationException()),
            //        double.Parse(xml.SelectSingleNode("ddata/pdata/p8")?.InnerText ?? throw new InvalidOperationException()),
            //        int.Parse(xml.SelectSingleNode("ddata/pdata/p9")?.InnerText ?? throw new InvalidOperationException()),
            //        int.Parse(xml.SelectSingleNode("ddata/pdata/p10")?.InnerText ?? throw new InvalidOperationException()),
            //        p12,
            //        p13));
            //}
            //_declare.ID = masId;
            PrescriptionReviseOutcome prescriptionReviseOutcome /*= new PrescriptionReviseOutcome(_declare)*/;
            prescriptionReviseOutcome.Show();
        }
        private Prescription GetPrescriptionData(XmlDocument xml)
        {
            return new Prescription(GetIcCard(xml), GetPharmacy(xml), GetTreatment(xml), GetMedicines());
        }

        private List<Medicine> GetMedicines()
        {
            throw new NotImplementedException();
        }

        private Treatment GetTreatment(XmlDocument xml)
        {
            return new Treatment(GetMedicalInfo(xml),GetPaymentCategory(xml), GetCopayment(xml), GetAdjustCase(xml), GetTreatmentDate(xml), GetAdjustDate(xml),GetMedicineDays(xml),GetMedicalPersonId(xml),GetCustomer(xml));
        }

        private Customer GetCustomer(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private string GetMedicalPersonId(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private string GetMedicineDays(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private DateTime GetAdjustDate(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private DateTime GetTreatmentDate(XmlDocument xml)
        {
            DateTime treatDate = new DateTime();
            if (xml.SelectSingleNode("ddata/dhead/d14") != null)
                return treatDate;
            return Convert.ToDateTime(xml.SelectSingleNode("ddata/dhead/d14")?.InnerText);
        }

        private AdjustCase GetAdjustCase(XmlDocument xml)
        {
            return new AdjustCase(xml.SelectSingleNode("ddata/dhead/d1")?.InnerText, AdjustCaseDb.GetAdjustCase(xml.SelectSingleNode("ddata/dhead/d1")?.InnerText));
        }

        private Copayment GetCopayment(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private PaymentCategory GetPaymentCategory(XmlDocument xml)
        {
            var payment = new PaymentCategory();
            if (xml.SelectSingleNode("ddata/dhead/d5") == null)
                return payment;
            payment.Id = xml.SelectSingleNode("ddata/dhead/d5")?.InnerText;
            payment.Name = PaymentCategroyDb.GetPaymentCategory(payment.Id);
            return payment;
        }

        private MedicalInfo GetMedicalInfo(XmlDocument xml)
        {
            return new MedicalInfo(GetHospital(xml),GetSpecialCode(xml),GetDiseaseCode(xml),GetTreatmentCase(xml));  
        }

        private TreatmentCase GetTreatmentCase(XmlDocument xml)
        {
            var t = new TreatmentCaseDb();
            var id = xml.SelectSingleNode("ddata/dhead/d22")?.InnerText;
            var name = t.GetTreatmentCase(xml.SelectSingleNode("ddata/dhead/d22")?.InnerText);
            return new TreatmentCase(id,name);
        }

        private List<DiseaseCode> GetDiseaseCode(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private SpecialCode GetSpecialCode(XmlDocument xml)
        {
            var special = new SpecialCode();
            if (xml.SelectSingleNode("ddata/dhead/d26") == null)
                return special;
            special.Id = xml.SelectSingleNode("ddata/dhead/d26")?.InnerText;
            return special;
        }

        private Hospital GetHospital(XmlDocument xml)
        {
            var hospital = new Hospital {Id = xml.SelectSingleNode("ddata/dhead/d21")?.InnerText};
            return hospital;
        }

        private Pharmacy GetPharmacy(XmlDocument xml)
        {
            throw new NotImplementedException();
        }

        private IcCard GetIcCard(XmlDocument xml)
        {
            return new IcCard
            {
                ICNumber = xml.SelectSingleNode("ddata/dhead/d3")?.InnerText,
                MedicalNumber = xml.SelectSingleNode("ddata/dhead/d7")?.InnerText
            };
        }
    }
}
