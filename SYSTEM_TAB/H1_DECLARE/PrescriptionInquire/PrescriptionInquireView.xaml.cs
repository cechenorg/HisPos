using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.TreatmentCase;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.Service;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionInquire
{
    public partial class PrescriptionInquireView : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<PrescriptionOverview> prescriptionOverview = new ObservableCollection<PrescriptionOverview>();
        public ObservableCollection<PrescriptionOverview> PrescriptionOverview {
            get
            {
                return prescriptionOverview;
            }
            set
            {
                prescriptionOverview = value;
                NotifyPropertyChanged("PrescriptionOverview");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public PrescriptionCases TreatmentCaseCollection { get; set; }
        public AdjustCases AdjustCaseCollection { get; set; }
        public PaymentCategories PaymentCategoryCollection { get; set; }
        public Copayments CopaymentCollection { get; set; }
        public Divisions DivisionCollection { get; set; }
        public Institutions HospitalCollection { get; set; }
        public ObservableCollection<Product> DeclareMedicinesData { get; set; }
        public static PrescriptionInquireView Instance;
        private Function f = new Function();

        private DateTime startDate = DateTime.Now.AddMonths(-3);
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }
        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }
        private string totalAmount = "0";
        public string TotalAmount
        {
            get => totalAmount;
            set
            {
                totalAmount = value;
                NotifyPropertyChanged("TotalAmount");
            }
        }

        private string chronicAmount = "0";
        public string ChronicAmount
        {
            get => chronicAmount;
            set
            {
                chronicAmount = value;
                NotifyPropertyChanged("ChronicAmount");
            }
        }
        private double totalPoint = 0;
        public double TotalPoint
        {
            get => totalPoint;
            set
            {
                totalPoint = value;
                NotifyPropertyChanged("TotalPoint");
            }
        }
        private double medDeclarePrice = 0;
        public double MedDeclarePrice
        {
            get => medDeclarePrice;
            set
            {
                medDeclarePrice = value;
                NotifyPropertyChanged("MedDeclarePrice");
            }
        }
        private double copaymenTPrice = 0;
        public double CopaymenTPrice
        {
            get => copaymenTPrice;
            set
            {
                copaymenTPrice = value;
                NotifyPropertyChanged("CopaymenTPrice");
            }
        }
        private double medServicePrice = 0;
        public double MedServicePrice
        {
            get => medServicePrice;
            set
            {
                medServicePrice = value;
                NotifyPropertyChanged("MedServicePrice");
            }
        }
        private double medUseePrice = 0;
        public double MedUseePrice
        {
            get => medUseePrice;
            set
            {
                medUseePrice = value;
                NotifyPropertyChanged("MedUseePrice");
            }
        }
        private double profit = 0;
        public double Profit
        {
            get => profit;
            set
            {
                profit = value;
                NotifyPropertyChanged("Profit");
            }
        }
        public PrescriptionInquireView()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show(); 
            LoadAdjustCases(); 
        }

        private void ShowInquireOutcome(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (PrescriptionOverview)(sender as DataGridRow).Item;
            DeclareData declareData = new DeclareData();/// PrescriptionDB.GetDeclareDataById(selectedItem.Decmas_Id);
            if (declareData is null)
            {
                MessageWindow.ShowMessage("查無處方 請聯絡資訊人員", MessageType.ERROR);
                 
                return;
            }
            PrescriptionInquireOutcome prescriptionInquireOutcome = new PrescriptionInquireOutcome(declareData, selectedItem.IsPredictChronic);
            prescriptionInquireOutcome.Show();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            DataPrescription.SelectedItem = (sender as DataGridRow).Item;
        }

        /*
        *載入原處方案件類別
        */

        private void LoadAdjustCases()
        {
           /// AdjustCaseCombo.ItemsSource = AdjustCaseDb.GetData();
        }
        
        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            prescriptionOverview.Clear();
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
             
            string adjustId = "";
            if (AdjustCaseCombo.Text != String.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            string insName = "";
            if (ReleasePalace.Text != String.Empty)
                insName = ReleasePalace.Text.Split(' ')[1];
           /// PrescriptionOverview = PrescriptionDB.GetPrescriptionOverviewBySearchCondition(StartDate, EndDate, PatientName.Text, adjustId, HisPerson.Text, insName);
            if(DataPrescription.Items.Count > 0)
                 DataPrescription.ScrollIntoView(DataPrescription.Items[DataPrescription.Items.Count - 1]);

            SumPrescriptValue();
        }
        private void SumPrescriptValue() {
         
            TotalAmount = PrescriptionOverview.Count.ToString();
            ChronicAmount = PrescriptionOverview.Count(pre => !string.IsNullOrEmpty(pre.ChronicStatus)).ToString();
            MedDeclarePrice = 0;
            MedServicePrice = 0;
            MedUseePrice = 0;
            Profit = 0;
            TotalPoint = 0;
            CopaymenTPrice = 0;
            foreach (PrescriptionOverview row in PrescriptionOverview) {
                MedDeclarePrice += row.MedDeclarePoint;
                MedServicePrice += row.MedServicePrice;
                MedUseePrice += row.MedUseePrice;
                CopaymenTPrice += row.CopaymentPrice;
                Profit += row.Profit;
                TotalPoint += Convert.ToInt32(row.Point); 
            }
            MedServicePrice *= 0.9;
        }
        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            Institutions tempCollection = new Institutions(true);
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }
        public void UpdateDataFromOutcome(PrescriptionOverview prescriptionOverview) {
            for (int i =0;i< PrescriptionOverview.Count;i++) {
                if (PrescriptionOverview[i].Decmas_Id == prescriptionOverview.Decmas_Id) {
                    PrescriptionOverview[i] = prescriptionOverview;
                    break;
                }
            }
            NotifyPropertyChanged("PrescriptionOverview");
            DataPrescription.Items.Filter = p => true;
        }
        private void ButtonImportXml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "選擇申報檔";
            fdlg.InitialDirectory = @"c:\";   //@是取消转义字符的意思
            fdlg.Filter = "Xml健保申報檔案|*.xml";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                DeclareDb declareDb = new DeclareDb();
                    var loadingWindow = new LoadingWindow();
                    loadingWindow.ImportXmlFile(this, fdlg.FileName);
                    loadingWindow.Show();
            }

        }

        private void ButtonChronicSearch_Click(object sender, RoutedEventArgs e)
        {
            prescriptionOverview.Clear();
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            string adjustId = "";
            if (AdjustCaseCombo.Text != String.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            string insName = "";
            if (ReleasePalace.Text != String.Empty)
                insName = ReleasePalace.Text.Split(' ')[1];
            ///PrescriptionOverview = PrescriptionDB.GetChronicOverviewBySearchCondition(StartDate, EndDate, PatientName.Text, adjustId, HisPerson.Text, insName);
            if (DataPrescription.Items.Count > 0)
                DataPrescription.ScrollIntoView(DataPrescription.Items[DataPrescription.Items.Count - 1]);

            SumPrescriptValue();
        }
    }
}