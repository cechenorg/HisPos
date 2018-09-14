using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
using His_Pos.PrescriptionInquire;
using His_Pos.Service;
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
        private ObservableCollection<AdjustCase> _adjustCaseCollection;

        public ObservableCollection<AdjustCase> AdjustCaseCollection
        {
            get => _adjustCaseCollection;
            set
            {
                _adjustCaseCollection = value;
                OnPropertyChanged(nameof(AdjustCaseCollection));
            }
        }

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

            if (SelectedFile == null) return;
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
            if (PrescriptionList is null) return;
            PrescriptionList.Items.Filter = CollectionViewSource_Filter;
        }

        private bool CollectionViewSource_Filter(object item)
        {
            if (string.IsNullOrEmpty(StartDateStr) && string.IsNullOrEmpty(EndDateStr) && AdjustCaseCombo.SelectedItem == null && ReleasePalace.SelectedItem == null && 
                HisPerson.SelectedItem == null)
                return true;
            bool accepted = true;
            var sDate = string.Empty;
            var eDate = string.Empty;
            if (!string.IsNullOrEmpty(StartDateStr))
                sDate = StartDateStr.Equals(string.Empty) ? string.Empty : "0" + SelectedFile.DeclareYear + "/" + StartDateStr;
            if (!string.IsNullOrEmpty(EndDateStr))
                eDate = EndDateStr.Equals(string.Empty) ? string.Empty : "0" + SelectedFile.DeclareYear + "/" + EndDateStr;

            var adjustId = string.Empty;
            if (AdjustCaseCombo.Text != string.Empty)
                adjustId = AdjustCaseCombo.Text.Substring(0, 1);
            var hospitalId = string.Empty;
            if (ReleasePalace.Text != string.Empty)
                hospitalId = ReleasePalace.Text.Split(' ')[1];
            DeclareFileDdata d = (DeclareFileDdata) item;
            if (d != null)
                // If filter is turned on, filter completed items.
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    if (!StartDateStr.Split('/')[0].Equals(d.Dbody.D23.Split('/')[1]))
                        accepted = false;
                    else if (int.Parse(StartDateStr.Split('/')[1]) > int.Parse(d.Dbody.D23.Split('/')[2]))
                        accepted = false;
                    else
                        accepted = true;
                }
                if (!string.IsNullOrEmpty(eDate))
                {
                    if (!EndDateStr.Split('/')[0].Equals(d.Dbody.D23.Split('/')[1]))
                        accepted = false;
                    else if (int.Parse(EndDateStr.Split('/')[1]) < int.Parse(d.Dbody.D23.Split('/')[2]))
                        accepted = false;
                    else
                        accepted = true;
                }

                if (!string.IsNullOrEmpty(adjustId))
                    accepted = d.Dhead.D1.Equals(adjustId);

                if (!string.IsNullOrEmpty(hospitalId))
                    accepted = d.Dbody.D21.Equals(hospitalId);

                if (HisPerson.SelectedItem != null)
                    accepted = d.Dbody.D25.Equals(((MedicalPersonnel)HisPerson.SelectedItem).IcNumber);
            }
            return accepted;
        }

        private void CreateDeclareFileClick(object sender, RoutedEventArgs e)
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
            var d = sender as DatePicker;
            var date = Convert.ToDateTime(d.SelectedDate);
            var result = date.Month.ToString().PadLeft(2,'0') + "/" + date.Day.ToString().PadLeft(2,'0');
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
            var ddataOutcome = new DeclareDdataOutcome((DeclareFileDdata)PrescriptionList.SelectedItem);
            ddataOutcome.Show();
        }

        public void UpdateDataFromOutcome(DeclareFileDdata declareFileDdata)
        {
            for (var i = 0; i < PrescriptionCollection.Count; i++)
            {
                if (PrescriptionCollection[i].DecId != declareFileDdata.DecId) continue;
                PrescriptionCollection[i] = declareFileDdata;
                break;
            }
            OnPropertyChanged(nameof(PrescriptionCollection));
            PrescriptionList.Items.Filter = p => true;
        }
    }
}
