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
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.PrescriptionInquire
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
        public ObservableCollection<TreatmentCase> TreatmentCaseCollection { get; set; }
        public ObservableCollection<AdjustCase> AdjustCaseCollection { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategoryCollection { get; set; }
        public ObservableCollection<Copayment> CopaymentCollection { get; set; }
        public ObservableCollection<Division> DivisionCollection { get; set; }
        public ObservableCollection<Hospital> HospitalCollection { get; set; }
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
            PrescriptionInquireOutcome prescriptionInquireOutcome = new PrescriptionInquireOutcome(PrescriptionDB.GetDeclareDataById(selectedItem.Decmas_Id));
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
            AdjustCaseCombo.ItemsSource = AdjustCaseDb.GetData();
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
            PrescriptionOverview = PrescriptionDB.GetPrescriptionOverviewBySearchCondition(StartDate, EndDate, PatientName.Text, adjustId, HisPerson.Text, insName);
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
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(HospitalCollection.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
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
        private void start_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (sender is System.Windows.Controls.TextBox t) {
                t.SelectionStart = 0;
                t.SelectionLength = t.Text.Length;
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
            PrescriptionOverview = PrescriptionDB.GetChronicOverviewBySearchCondition(StartDate, EndDate, PatientName.Text, adjustId, HisPerson.Text, insName);
            if (DataPrescription.Items.Count > 0)
                DataPrescription.ScrollIntoView(DataPrescription.Items[DataPrescription.Items.Count - 1]);

            SumPrescriptValue();
        }
    }
}