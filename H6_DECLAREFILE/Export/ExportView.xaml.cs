using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.PrescriptionInquire;
using JetBrains.Annotations;

namespace His_Pos.H6_DECLAREFILE.Export
{
    /// <inheritdoc />
    /// <summary>
    /// ExportView.xaml 的互動邏輯
    /// </summary>
    public partial class ExportView : UserControl,INotifyPropertyChanged
    {
        public static ExportView Instance;
        private ObservableCollection<DeclareFile> _declareFiles;

        public ObservableCollection<DeclareFile> DeclareFiles
        {
            get => _declareFiles;
            set
            {
                _declareFiles = value;
                OnPropertyChanged(nameof(DeclareFiles));
            }
        }

        public ObservableCollection<Hospital> HospitalCollection { get; set; }
        public ObservableCollection<Division> DivisionCollection { get; set; }
        public ObservableCollection<AdjustCase> AdjustCaseCollection { get; set; }
        public ObservableCollection<string> CustomerName = new ObservableCollection<string>();
        private ObservableCollection<DeclareFileDdata> _prescriptionCollection;

        public ObservableCollection<DeclareFileDdata> PrescriptionCollection
        {
            get => _prescriptionCollection;
            set
            {
                _prescriptionCollection = value;
                OnPropertyChanged(nameof(PrescriptionCollection));
            }
        }

        private DeclareFile _selectedFile;
        public DeclareFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private Ddata _selectedPrescription;
        public Ddata SelectedPrescription
        {
            get => _selectedPrescription;
            set
            {
                _selectedPrescription = value;
                OnPropertyChanged(nameof(SelectedPrescription));
            }
        }

        private string _startDateStr;

        public string StartDateStr
        {
            get => _startDateStr;
            set
            {
                _startDateStr = value;
                OnPropertyChanged(nameof(StartDateStr));
            }
        }

        private string _endDateStr;

        public string EndDateStr
        {
            get => _endDateStr;
            set
            {
                _endDateStr = value;
                OnPropertyChanged(nameof(EndDateStr));
            }
        }

        public ObservableCollection<Copayment> CopaymentCollection { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategoryCollection { get; set; }
        public ObservableCollection<TreatmentCase> TreatmentCaseCollection { get; set; }
        public ObservableCollection<DeclareMedicine> DeclareMedicinesData { get; set; }

        public ExportView()
        {
            InitializeComponent();
            InitializeDeclareFiles();
            Instance = this;
            DataContext = this;
        }

        private void InitializeDeclareFiles()
        {
            SelectedFile = new DeclareFile();
            var load = new LoadingWindow();
            load.GetDeclareFileData(this);
            load.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DeclareFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFile = (DeclareFile)(sender as DataGrid)?.SelectedItem;

            foreach (var ddata in SelectedFile.FileContent.Ddata)
            {
                SelectedFile.PrescriptionDdatas.Add(new DeclareFileDdata(ddata));
            }

            foreach (var d in SelectedFile.PrescriptionDdatas)
            {
                foreach (var errorList in SelectedFile.ErrorPrescriptionList.ErrorList)
                {
                    if (!errorList.PrescriptionId.Equals(d.DecId) || errorList.Error.Count <= 0) continue;
                    d.HasError = true;
                    d.CanDeclare = false;
                }
            }
            PrescriptionCollection = new ObservableCollection<DeclareFileDdata>(SelectedFile.PrescriptionDdatas);
            if (ErrorDec.IsChecked == true)
            {
                foreach (var d in SelectedFile.PrescriptionDdatas)
                {
                    if (d.HasError)
                    {
                        d.CanDeclare = false;
                    }
                }
            }
            else
            {
                foreach (var d in SelectedFile.PrescriptionDdatas)
                {
                    d.CanDeclare = true;
                }
            }
        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(HospitalCollection.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            var sDate = StartDateStr.Equals(string.Empty) ? string.Empty : "0" + SelectedFile.DeclareYear + "/" + StartDateStr;

            var eDate = EndDateStr.Equals(string.Empty) ? string.Empty : "0" + SelectedFile.DeclareYear + "/" + EndDateStr;

            var adjustId = string.Empty;
            if (AdjustCaseCombo.Text != string.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            var insName = string.Empty;
            if (ReleasePalace.Text != string.Empty)
                insName = ReleasePalace.Text.Split(' ')[1];
            
        }

        private void CreateDeclareFileClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveDeclareFileButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void PatientNamePopulating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<string> tempCollection = new ObservableCollection<string>();
            foreach (var cusName in PrescriptionCollection)
            {
                tempCollection.Add(cusName.Dbody.D20);
            }
            CustomerName.Clear();
            CustomerName = new ObservableCollection<string>(tempCollection.Where(x => x.Contains(PatientName.Text)).Take(50).ToList());
            PatientName.ItemsSource = CustomerName;
            PatientName.PopulateComplete();
        }

        private void DatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ////2018/9/9
            ////2018/19/9
            ////2018/19/19
            var d = sender as DatePicker;
            DateTime date = Convert.ToDateTime(d.SelectedDate);
            string result = date.Month.ToString().PadLeft(2,'0') + "/" + date.Day.ToString().PadLeft(2,'0');
            if (d.Name.Equals("Start"))
                StartDateStr = result;
            else
                EndDateStr = result;
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            PrescriptionList.SelectedItem = (sender as DataGridRow)?.Item;
        }

        private void ShowInquireOutcome(object sender, MouseButtonEventArgs e)
        {
            DeclareDdataOutcome ddataOutcome = new DeclareDdataOutcome((DeclareFileDdata)PrescriptionList.SelectedItem, HospitalCollection);
            ddataOutcome.Show();
        }
    }
}
