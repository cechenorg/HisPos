using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;
using ComboBox = System.Windows.Controls.ComboBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.PrescriptionInquire
{
    public partial class PrescriptionInquireView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Hospital> Hospitals { get; set; }
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
        public ObservableCollection<DeclareMedicine> DeclareMedicinesData { get; set; }
        public static PrescriptionInquireView Instance;
        private Function f = new Function();

        public PrescriptionInquireView()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
            start.SelectedDate = DateTime.Now.AddMonths(-3);
            end.SelectedDate = DateTime.Now;
            LoadAdjustCases();
            Hospitals = HospitalCollection;
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
            
            var sDate = start.Text == "" ?"" : "0" + taiwanCalendar.GetYear(Convert.ToDateTime(start.Text)) + "/" + Convert.ToDateTime(start.Text).ToString("MM/dd");
            
            var eDate = end.Text == "" ? "" : "0" + taiwanCalendar.GetYear(Convert.ToDateTime(end.Text)) + "/" + Convert.ToDateTime(end.Text).ToString("MM/dd");
            string adjustId = "";
            if (AdjustCaseCombo.Text != String.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            string insName = "";
            if (ReleasePalace.Text != String.Empty)
                insName = ReleasePalace.Text.Split(' ')[1];
            PrescriptionOverview = PrescriptionDB.GetPrescriptionOverviewBySearchCondition(sDate, eDate, PatientName.Text, adjustId, HisPerson.Text, insName);

        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(Hospitals.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
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
            fdlg.Title = "C# Corner Open File Dialog";
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
    }
}