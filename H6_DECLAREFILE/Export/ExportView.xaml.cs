using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.RDLC;
using JetBrains.Annotations;

namespace His_Pos.H6_DECLAREFILE.Export
{
    /// <summary>
    /// ExportView.xaml 的互動邏輯
    /// </summary>
    public partial class ExportView : INotifyPropertyChanged
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
            {
                if (!string.IsNullOrEmpty(sDate))
                {
                    if (!StartDateStr.Split('/')[0].Equals(d.Dbody.D23.Split('/')[1]))
                        accepted = false;
                    else if (int.Parse(StartDateStr.Split('/')[1]) > int.Parse(d.Dbody.D23.Split('/')[2]))
                        accepted = false;
                }
                if (!string.IsNullOrEmpty(eDate))
                {
                    if (!EndDateStr.Split('/')[0].Equals(d.Dbody.D23.Split('/')[1]))
                        accepted = false;
                    else if (int.Parse(EndDateStr.Split('/')[1]) < int.Parse(d.Dbody.D23.Split('/')[2]))
                        accepted = false;
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
            var declareDb = new DeclareDb();
            var declaredPharmacy = new Pharmacy();
            var tmpTdata = SelectedFile.FileContent.Tdata;
            declaredPharmacy.Ddata = SortDdata();
            declaredPharmacy.Ddata = declareDb.SortDdataByCaseId(declaredPharmacy);
            var tdata = new Tdata
            {
                T1 = "30",
                T2 = MainWindow.CurrentPharmacy.Id,
                T3 = tmpTdata.T3,
                T4 = "2",
                T5 = "1",
                T6 = (DateTime.Now.Year - 1911) + DateTime.Now.Month.ToString().PadLeft(2, '0'),
                T7 = declareDb.CountPrescriptionByCase(declaredPharmacy.Ddata, 1).ToString(),
                T8 = declaredPharmacy.Ddata.Where(d => !d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dbody.D16)).ToString(),
                T9 = declareDb.CountPrescriptionByCase(declaredPharmacy.Ddata, 2).ToString(),
                T10 = declaredPharmacy.Ddata.Where(d => d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dbody.D16)).ToString(),
                T11 = declaredPharmacy.Ddata.Count.ToString()
            };
            var firstAdjustDate = "31";
            foreach (var d in declaredPharmacy.Ddata)
            {
                if (int.Parse(d.Dbody.D23.Split('/')[2]) < int.Parse(firstAdjustDate))
                    firstAdjustDate = d.Dbody.D23.Split('/')[2];
            }
            tdata.T12 = (int.Parse(tdata.T8) + int.Parse(tdata.T10)).ToString();
            tdata.T13 = (int.Parse(SelectedFile.DeclareYear)-1911) + "/" + SelectedFile.DeclareMonth + "/" + firstAdjustDate;
            var lastAdjustDate = "01";
            foreach (var d in declaredPharmacy.Ddata)
            {
                if (int.Parse(d.Dbody.D23.Split('/')[2]) > int.Parse(lastAdjustDate))
                    lastAdjustDate = d.Dbody.D23.Split('/')[2];
            }
            tdata.T14 = (int.Parse(SelectedFile.DeclareYear)-1911) + "/" + SelectedFile.DeclareMonth + "/" + lastAdjustDate;
            declaredPharmacy.Tdata = tdata;

            var f = new Function();
            XDocument result;
            var xmlSerializer = new XmlSerializer(declaredPharmacy.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, declaredPharmacy);
                var document = XDocument.Parse(ReportService.PrettyXml(textWriter));
                var root = XElement.Parse(document.ToString());
                root.Element("ddata")?.Element("decId")?.Remove();
                document = XDocument.Load(root.CreateReader());
                document.Root?.RemoveAttributes();
                result = document;
            }
            //匯出xml檔案
            f.ExportXml(result, "匯出申報XML檔案");
        }

        private List<Ddata> SortDdata()
        {
            var canDeclared = new List<Ddata>();
            if (ErrorDec.IsChecked == true)
            {
                var i = 0;
                foreach (var ddata in PrescriptionCollection)
                {
                    if(ddata.CanDeclare)
                        canDeclared.Add(SelectedFile.FileContent.Ddata[i]);
                    i++;
                }

            }
            else
            {
                foreach (var ddata in SelectedFile.FileContent.Ddata)
                {
                    canDeclared.Add(ddata);
                }
            }
            
            var ddatas = canDeclared.GroupBy(d => d.Dbody.D25)
                .ToList();
            var sorteDdatas = new List<Ddata>();
            //每位調劑藥師處方排序
            foreach (var group in ddatas)
            {
                var tmpGroup = group.OrderBy(d => d.Dbody.D38);//依藥事服務費點數排序
                var count = 1;
                foreach (var tmpG in tmpGroup)
                {
                    //每人每日1 - 80件內 => 診療項目代碼: 05202B . 支付點數 : 48
                    if (count < 80)
                    {
                        tmpG.Dbody.D37 = "0502B";
                        tmpG.Dbody.D38 = "48";
                    }
                    //每人每日81 - 100件內 => 診療項目代碼: 05234D . 支付點數 : 15
                    else if (count < 100 && count >= 80)
                    {
                        tmpG.Dbody.D37 = "05234D";
                        tmpG.Dbody.D38 = "15";
                    }
                    //每人每日100件以上 => 診療項目代碼: 0502B . 支付點數 : 0
                    else
                    {
                        tmpG.Dbody.D37 = "0502B";
                        tmpG.Dbody.D38 = "0";
                    }
                    count++;
                }
                sorteDdatas.AddRange(tmpGroup);
            }
            int[] sequence = { 0, 0, 0, 0 };
            foreach (var t in sorteDdatas)
            {
                switch (t.Dhead.D1)
                {
                    case "1":
                        sequence[0]++;
                        t.Dhead.D2 = sequence[0].ToString();
                        break;
                    case "2":
                        sequence[1]++;
                        t.Dhead.D2 = sequence[1].ToString();
                        break;
                    case "3":
                        sequence[2]++;
                        t.Dhead.D2 = sequence[2].ToString();
                        break;
                    case "5":
                        sequence[3]++;
                        t.Dhead.D2 = sequence[3].ToString();
                        break;
                    case "D":
                        sequence[4]++;
                        t.Dhead.D2 = sequence[4].ToString();
                        break;
                }
            }
            return sorteDdatas;
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

        private void ErrorDec_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox c)
            {
                if (c.IsChecked == true)
                {
                    foreach (var p in PrescriptionCollection)
                    {
                        if (p.CanDeclare && p.HasError)
                            p.CanDeclare = false;
                    }
                }
                else
                {
                    foreach (var p in PrescriptionCollection)
                    {
                        p.CanDeclare = true;
                    }
                }
            }

        }
    }
}
