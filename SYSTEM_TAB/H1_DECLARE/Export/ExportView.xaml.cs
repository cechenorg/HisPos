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
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.RDLC;
using His_Pos.Service;
using JetBrains.Annotations;
using AdjustCase = His_Pos.Class.AdjustCase.AdjustCase;
using PaymentCategory = His_Pos.Class.PaymentCategory.PaymentCategory;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.Export
{
    /// <summary>
    /// ExportView.xaml 的互動邏輯
    /// </summary>
    public partial class ExportView :UserControl, INotifyPropertyChanged
    {
        public static ExportView Instance;
        private ObservableCollection<oDeclareFile> _declareFiles;

        public ObservableCollection<oDeclareFile> DeclareFiles
        {
            get => _declareFiles;
            set
            {
                _declareFiles = value;
                OnPropertyChanged(nameof(DeclareFiles));
            }
        }

        private Institutions _hospitalCollection;

        public Institutions HospitalCollection
        {
            get => _hospitalCollection;
            set
            {
                _hospitalCollection = value;
                OnPropertyChanged(nameof(HospitalCollection));
            }
        }

        private Divisions _divisionCollection;

        public Divisions DivisionCollection
        {
            get => _divisionCollection;
            set
            {
                _divisionCollection = value;
                OnPropertyChanged(nameof(DivisionCollection));
            }
        }

        private AdjustCases _adjustCaseCollection;

        public AdjustCases AdjustCaseCollection
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

        private oDeclareFile _selectedFile;
        public oDeclareFile SelectedFile
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

        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private Copayments copaymentCollection;

        public Copayments CopaymentCollection
        {
            get => copaymentCollection;
            set
            {
                copaymentCollection = value;
                OnPropertyChanged(nameof(CopaymentCollection));
            }
        }
        private PaymentCategories paymentCategoryCollection;
        public PaymentCategories PaymentCategoryCollection
        {
            get => paymentCategoryCollection;
            set
            {
                paymentCategoryCollection = value;
                OnPropertyChanged(nameof(PaymentCategoryCollection));
            }
        }
        private PrescriptionCases treatmentCaseCollection;

        public PrescriptionCases TreatmentCaseCollection
        {
            get => treatmentCaseCollection;
            set
            {
                treatmentCaseCollection = value;
                OnPropertyChanged(nameof(TreatmentCaseCollection));
            }
        }
        private ObservableCollection<DeclareMedicine> declareMedicinesData;

        public ObservableCollection<DeclareMedicine> DeclareMedicinesData
        {
            get => declareMedicinesData;
            set
            {
                declareMedicinesData = value;
                OnPropertyChanged(nameof(DeclareMedicinesData));
            }
        }

        public ExportView()
        {
            InitializeComponent();
            SelectedFile = new oDeclareFile();
            DataContext = this;
            Instance = this;
            InitializeDeclareFiles();
        }

        private void InitializeDeclareFiles()
        {
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
            SelectedFile = (oDeclareFile)(sender as DataGrid)?.SelectedItem;

            if (SelectedFile?.FileContent == null) return;
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
            Institutions tempCollection = new Institutions(true);
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
            var d = (DeclareFileDdata)item;
            var startDateAccept = true;
            var date = new DateTime(int.Parse(d.Dhead.D23.Substring(0,3)) +1911, int.Parse(d.Dhead.D23.Substring(3, 2)), int.Parse(d.Dhead.D23.Substring(5, 2)));
            var startDateValid = DateTime.TryParse($"{Start.Text.Split('/')[0]}/{Start.Text.Split('/')[1]}/{Start.Text.Split('/')[2]}", out _);
            if (!Start.Text.Contains(" ") && startDateValid)
                startDateAccept = date.Date >= StartDate.Date;
            var endDateAccept = true;
            var endDateValid = DateTime.TryParse($"{End.Text.Split('/')[0]}/{End.Text.Split('/')[1]}/{End.Text.Split('/')[2]}", out _);
            if (!End.Text.Contains(" ") && endDateValid)
                endDateAccept = date.Date <= EndDate.Date;
            var adjustCaseAccept = true;
            if (AdjustCaseCombo.SelectedIndex != -1)
                adjustCaseAccept = d.Dhead.D1.Equals(((AdjustCase) AdjustCaseCombo.SelectedItem).Id.Trim());
            var hospitalAccept = true;
            if (!string.IsNullOrEmpty(ReleasePalace.Text) && ReleasePalace.SelectedItem != null)
                hospitalAccept = d.Dhead.D21.Equals(((Hospital) ReleasePalace.SelectedItem).Id.Trim());
            return startDateAccept && endDateAccept && adjustCaseAccept && hospitalAccept;
        }

        private void CreateDeclareFileClick(object sender, RoutedEventArgs e)
        {
            var declareDb = new DeclareDb();
            var declaredPharmacy = new Class.Declare.Pharmacy();
            var tmpTdata = SelectedFile.FileContent.Tdata;
            declaredPharmacy.Ddata = SortDdata();
            ///declaredPharmacy.Ddata = declareDb.SortDdataByCaseId(declaredPharmacy);
            var tdata = new Tdata
            {
                T1 = "30",
                T2 = ViewModelMainWindow.CurrentPharmacy.Id,
                T3 = tmpTdata.T3,
                T4 = "2",
                T5 = "1",
                T6 = (DateTime.Now.Year - 1911) + DateTime.Now.Month.ToString().PadLeft(2, '0'),
                ///T7 = declareDb.CountPrescriptionByCase(declaredPharmacy.Ddata, 1).ToString(),
                T8 = declaredPharmacy.Ddata.Where(d => !d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dhead.D16)).ToString(),
                ///T9 = declareDb.CountPrescriptionByCase(declaredPharmacy.Ddata, 2).ToString(),
                T10 = declaredPharmacy.Ddata.Where(d => d.Dhead.D1.Equals("2")).Sum(d => int.Parse(d.Dhead.D16)).ToString(),
                T11 = declaredPharmacy.Ddata.Count.ToString()
            };
            var firstAdjustDate = "31";
            foreach (var d in declaredPharmacy.Ddata)
            {
                if (int.Parse(d.Dhead.D23.Substring(5,2)) < int.Parse(firstAdjustDate))
                    firstAdjustDate = d.Dhead.D23.Substring(5, 2);
            }
            tdata.T12 = (int.Parse(tdata.T8) + int.Parse(tdata.T10)).ToString();
            tdata.T13 = (int.Parse(SelectedFile.DeclareYear)-1911) + SelectedFile.DeclareMonth  + firstAdjustDate;
            var lastAdjustDate = "01";
            foreach (var d in declaredPharmacy.Ddata)
            {
                if (int.Parse(d.Dhead.D23.Substring(5, 2)) > int.Parse(lastAdjustDate))
                    lastAdjustDate = d.Dhead.D23.Substring(5, 2);
            }
            tdata.T14 = (int.Parse(SelectedFile.DeclareYear)-1911) + SelectedFile.DeclareMonth  + lastAdjustDate;
            declaredPharmacy.Tdata = tdata;
            
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
            Function.ExportXml(result, "匯出申報XML檔案");
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
            
            var ddatas = canDeclared.GroupBy(d => d.Dhead.D25)
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
        private void SelectionStart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (sender)
            {
                case AutoCompleteBox a:
                    if (a.Template.FindName("Text", a) is TextBox txt)
                    {
                        txt.SelectionStart = 0;
                        txt.SelectionLength = txt.Text.Length;
                    }
                    break;
                case TextBox t:
                    t.SelectionStart = 0;
                    t.SelectionLength = t.Text.Length;
                    break;
            }
        }
    }
}
