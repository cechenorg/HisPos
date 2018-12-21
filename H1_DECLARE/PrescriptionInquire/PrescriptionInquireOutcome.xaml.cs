using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.Class.DiseaseCode;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
using His_Pos.Class.Position;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.H1_DECLARE.PrescriptionDec2;
using His_Pos.H1_DECLARE.PrescriptionInquire;
using His_Pos.HisApi;
using His_Pos.Interface;
using His_Pos.Service;
using His_Pos.Struct.IcData;
using His_Pos.ViewModel;
using MaterialDesignThemes.Wpf;
using MoreLinq;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionInquireOutcome : Window, INotifyPropertyChanged
    {
        private IcErrorCodeWindow icErrorWindow;
        public IcErrorCodeWindow.IcErrorCode SelectedErrorCode { get; set; }
        public static bool IsAdjust = false;
        public bool IsGetMedicalNumber { get; set; }
        public readonly byte[] BasicDataArr = new byte[72];
        public BasicData CusBasicData;
        public SeqNumber Seq; //取得之就醫序號資料
        private readonly List<string> _prescriptionSignatureList = new List<string>(); //處方簽章

        public ObservableCollection<TreatmentDataNoNeedHpc> TreatRecCollection { get; set; } =
            new ObservableCollection<TreatmentDataNoNeedHpc>(); //就醫紀錄
        private bool _isFirst = true;
        private bool _isPredictChronic;
        private DeclareData _currentDeclareData;
        public event PropertyChangedEventHandler PropertyChanged;
        public PrescriptionInquireOutcome Instance;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        private string _tempMedicalNumber;

        public string TempMedicalNumber
        {
            get => _tempMedicalNumber;
            set
            {
                _tempMedicalNumber = value;
                NotifyPropertyChanged(nameof(TempMedicalNumber));
            }
        }
        private ObservableCollection<TreatmentCase> _treatmentCaseCollection;
        public ObservableCollection<TreatmentCase> TreatmentCaseCollection
        {
            get => _treatmentCaseCollection;
            set
            {
                _treatmentCaseCollection = value;
                NotifyPropertyChanged(nameof(TreatmentCaseCollection));
            }
        }
        private ObservableCollection<AdjustCase> _adjustCaseCollection;
        public ObservableCollection<AdjustCase> AdjustCaseCollection
        {
            get => _adjustCaseCollection;
            set
            {
                _adjustCaseCollection = value;
                NotifyPropertyChanged(nameof(AdjustCaseCollection));
            }
        }
        private ObservableCollection<PaymentCategory> _paymentCategoryCollection;
        public ObservableCollection<PaymentCategory> PaymentCategoryCollection
        {
            get => _paymentCategoryCollection;
            set
            {
                _paymentCategoryCollection = value;
                NotifyPropertyChanged(nameof(PaymentCategoryCollection));
            }
        }
        private ObservableCollection<Copayment> _copaymentCollection;
        public ObservableCollection<Copayment> CopaymentCollection
        {
            get => _copaymentCollection;
            set
            {
                _copaymentCollection = value;
                NotifyPropertyChanged(nameof(CopaymentCollection));
            }
        }
        private ObservableCollection<Division> _divisionCollection;
        public ObservableCollection<Division> DivisionCollection
        {
            get => _divisionCollection;
            set
            {
                _divisionCollection = value;
                NotifyPropertyChanged(nameof(DivisionCollection));
            }
        }
        private ObservableCollection<Hospital> _hospitalCollection;
        public ObservableCollection<Hospital> HospitalCollection
        {
            get => _hospitalCollection;
            set
            {
                _hospitalCollection = value;
                NotifyPropertyChanged(nameof(HospitalCollection));
            }
        }
        private DeclareTrade _declareTrade;
        
        public DeclareTrade DeclareTrade
        {
            get => _declareTrade;
            set
            {
                _declareTrade = value;
                NotifyPropertyChanged(nameof(DeclareTrade));
            }
        }
        public ObservableCollection<Product> DeclareMedicinesData { get; set; }
        private static DeclareData _inquiredPrescription;
        public DeclareData InquiredPrescription
        {
            get => _inquiredPrescription;
            set
            {
                _inquiredPrescription = value;
                PatientGender.Content = _inquiredPrescription.Prescription.Customer.IcNumber.Substring(1, 1).Equals("2") ? "女" : "男";
                NotifyPropertyChanged("InquiredPrescription");
            }
        }
        private ObservableCollection<Product> _declareDetails = new ObservableCollection<Product>();
        public ObservableCollection<Product> DeclareDetails
        {
            get => _declareDetails;
            set
            {
                _declareDetails = value;
                NotifyPropertyChanged(nameof(DeclareDetails));
            }
        }
        private ObservableCollection<Product> originDeclareDetails = new ObservableCollection<Product>();
        public ObservableCollection<Product> OriginDeclareDetails
        {
            get => originDeclareDetails;
            set
            {
                originDeclareDetails = value;
                NotifyPropertyChanged(nameof(OriginDeclareDetails));
            }
        }
        private ObservableCollection<object> _medicines;
        private readonly string _decMasId;
        public PrescriptionInquireOutcome(DeclareData inquired,bool isPredictChronic)
        {
            InitializeComponent();
            Instance = this;
            IsAdjust = false;
            _isFirst = true;
            DataContext = this;
            _isPredictChronic = isPredictChronic;
            DeclareTrade = DeclareTradeDb.GetDeclarTradeByMasId(inquired.DecMasId);
            _decMasId = inquired.DecMasId;
            InquiredPrescription = inquired;
            
                if (!string.IsNullOrEmpty(InquiredPrescription.Prescription.ChronicSequence) && int.Parse(InquiredPrescription.Prescription.ChronicSequence) > 1)
                {
                    InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber = "IC0" + InquiredPrescription.Prescription.ChronicSequence;
                    TempMedicalNumber = InquiredPrescription.Prescription.OriginalMedicalNumber;
                }
                else
                {
                    TempMedicalNumber = InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber;
                }
            
            foreach (var newDeclareDetail in InquiredPrescription.Prescription.Medicines) {
                if (newDeclareDetail is DeclareMedicine)
                {
                    var declareDetailClone = (DeclareMedicine)((DeclareMedicine)newDeclareDetail).Clone();
                    declareDetailClone.Amount = ((DeclareMedicine)newDeclareDetail).Amount;
                    DeclareDetails.Add(declareDetailClone);
                    var tempdeclareDetailClone = (DeclareMedicine)((DeclareMedicine)newDeclareDetail).Clone();
                    tempdeclareDetailClone.Amount = ((DeclareMedicine)newDeclareDetail).Amount;
                    OriginDeclareDetails.Add(tempdeclareDetailClone);
                }
                else if (newDeclareDetail is PrescriptionOTC)
                {
                    var otc = (PrescriptionOTC)((PrescriptionOTC)newDeclareDetail).Clone();
                    DeclareDetails.Add(otc);
                    var tempotc = (PrescriptionOTC)((PrescriptionOTC)newDeclareDetail).Clone();
                    OriginDeclareDetails.Add(tempotc);
                }
            }
            InitData();
            InitDataChanged();
            MakeUp.Visibility = inquired.Prescription.IsGetIcCard ? Visibility.Collapsed : Visibility.Visible;
        }

      
        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }
        private void DataChanged()
        {
            if (_isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonImportXml.IsEnabled = true;
        }
        
        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonImportXml.IsEnabled = false;
        }
        private void InitData() {
            if (PrescriptionInquireView.Instance is null) return;
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.ChangeLoadingMessage("處方詳細資料載入中...");
            loadingWindow.Show();
            DivisionCollection = PrescriptionInquireView.Instance.DivisionCollection;
            CopaymentCollection = PrescriptionInquireView.Instance.CopaymentCollection;
            PaymentCategoryCollection = PrescriptionInquireView.Instance.PaymentCategoryCollection;
            AdjustCaseCollection = PrescriptionInquireView.Instance.AdjustCaseCollection;
            TreatmentCaseCollection = PrescriptionInquireView.Instance.TreatmentCaseCollection;
            HospitalCollection = PrescriptionInquireView.Instance.HospitalCollection;
            DeclareMedicinesData =  PrescriptionInquireView.Instance.DeclareMedicinesData;
            SetTreatmentData();
            InquiredPrescription.Prescription.Treatment.Copayment = CopaymentCollection.SingleOrDefault(c =>
                c.Id.Equals(InquiredPrescription.Prescription.Treatment.Copayment.Id));
            loadingWindow.Close();
        }

        private void SetTreatmentData()
        {
            var doctor = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Doctor;

            var tmpHospital = HospitalCollection.SingleOrDefault(h =>
                h.Id.Equals(InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Id));

            var tmpDivision = DivisionCollection.SingleOrDefault(d => d.Id.Equals(InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Division.Id));
            if (tmpHospital != null)
            {
                tmpHospital.Division = tmpDivision;
                tmpHospital.Doctor = doctor;
                InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital = tmpHospital;
            }
            Division.SelectedItem = tmpDivision;
            var tmpCopayment = CopaymentCollection.SingleOrDefault(c =>
                c.Id.Equals(InquiredPrescription.Prescription.Treatment.Copayment.Id));

            if (tmpCopayment != null)
            {
                tmpCopayment.Point = InquiredPrescription.Prescription.Treatment.Copayment.Point;
                InquiredPrescription.Prescription.Treatment.Copayment = tmpCopayment;
            }

            if (!string.IsNullOrEmpty(InquiredPrescription.Prescription.Treatment.MedicalInfo.MainDiseaseCode.Id))
            {
                InquiredPrescription.Prescription.Treatment.MedicalInfo.MainDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(InquiredPrescription.Prescription.Treatment.MedicalInfo.MainDiseaseCode.Id)[0].ICD10;
                if(!string.IsNullOrEmpty(InquiredPrescription.Prescription.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                    InquiredPrescription.Prescription.Treatment.MedicalInfo.SecondDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(InquiredPrescription.Prescription.Treatment.MedicalInfo.SecondDiseaseCode.Id)[0].ICD10;
            }
            InquiredPrescription.Prescription.Treatment.AdjustCase = AdjustCaseCollection.SingleOrDefault(a =>
                a.Id.Equals(InquiredPrescription.Prescription.Treatment.AdjustCase.Id));

            InquiredPrescription.Prescription.Treatment.PaymentCategory = PaymentCategoryCollection.SingleOrDefault(p =>
                p.Id.Equals(InquiredPrescription.Prescription.Treatment.PaymentCategory.Id));

            InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase =
                TreatmentCaseCollection.SingleOrDefault(t =>
                    t.Id.Equals(InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase.Id));
        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            var tempCollection = new ObservableCollection<Hospital>(HospitalCollection.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;
            DataChanged();
            if (medicineCodeAuto is null) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty &&
                    ((ObservableCollection<object>) medicineCodeAuto.ItemsSource).Count != 0 && medicineCodeAuto.Text.Length >= 4){
                    medicineCodeAuto.SelectedItem = ((ObservableCollection<object>) medicineCodeAuto.ItemsSource)?[0];
                }
                else
                    return;
            } 
            var declareMedicine = ((DeclareMedicine)medicineCodeAuto.SelectedItem).DeepCloneViaJson();
            if (declareMedicine != null && (declareMedicine.Id.EndsWith("00") || declareMedicine.Id.EndsWith("G0")))
                declareMedicine.Position = MainWindow.Positions.SingleOrDefault(p => p.Id.Contains("PO"))?.Id;
            var currentRow = GetCurrentRowIndex(sender);

            if (DeclareDetails.Count > 0)
            {
                if (DeclareDetails.Count == currentRow)
                {
                    DeclareDetails.Add(declareMedicine);
                    medicineCodeAuto.Text = "";
                }
                else
                {
                    DeclareDetails[currentRow].Id = declareMedicine.Id;
                    DeclareDetails[currentRow].Name = declareMedicine.Name;
                }
            }
            else
            {
                DeclareDetails.Add(declareMedicine);
                medicineCodeAuto.Text = "";
            } 
            MedicineCodeMoveFocus(GetCurrentRowIndex(sender));
        }
        private void MedicineCodeMoveFocus(int currentRow)
        {
            if (currentRow == -1) return;
            PrescriptionMedicines.CurrentCell = new DataGridCellInfo(
                PrescriptionMedicines.Items[currentRow], DosageText);
            if (PrescriptionMedicines.CurrentCell.Item is null) return;
            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column?.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild =
                (UIElement)VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            firstChild.Focus();
        }
        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;

            if (selectedItem is IDeletable)
            {
                if (DeclareDetails.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionMedicines.SelectedItem = selectedItem;
                return;
            }
            PrescriptionMedicines.SelectedIndex = DeclareDetails.Count;

        }


        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
        }
        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            if (!(sender is AutoCompleteBox medicineCodeAuto)) return;
            if (DeclareMedicinesData == null) return;
            
            var result = DeclareMedicinesData.Where(x =>
                (x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())) &&
                DeclareDetails.Count(med => med.Id == x.Id) == 0
                ).Take(50).Select(x => x); 
            _medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = _medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DeclareDetails[PrescriptionMedicines.SelectedIndex] is DeclareMedicine)
            {
                ClearMedicine((DeclareMedicine)DeclareDetails[PrescriptionMedicines.SelectedIndex]);
            }
            else
            {
                ClearPrescriptionOtc((PrescriptionOTC)DeclareDetails[PrescriptionMedicines.SelectedIndex]);
            }
            DataChanged();
            DeclareDetails.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }
        private void ClearMedicine(DeclareMedicine med)
        {
            med.PaySelf = false;
            med.Cost = 0;
            med.TotalPrice = 0;
            med.Amount = 0;
            med.CountStatus = string.Empty;
            med.FocusColumn = string.Empty;
            med.Usage = new Usage();
            med.Days = string.Empty;
            med.Position = string.Empty;
            med.Source = string.Empty;
            med.Dosage = 0;
        }

        private void ClearPrescriptionOtc(PrescriptionOTC med)
        {
            med.PaySelf = false;
            med.Cost = 0;
            med.TotalPrice = 0;
            med.Amount = 0;
            med.CountStatus = string.Empty;
            med.FocusColumn = string.Empty;
            med.Usage = new Usage();
            med.Days = string.Empty;
            med.Position = string.Empty;
            med.Source = string.Empty;
            med.Dosage = 0;
        }
        private void Dosage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
        }
        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox t && string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }
        private void MedTotalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFirst) return;
            CountMedicinesCost();
        }
        private void CountMedicinesCost()
        {
            if (DeclareTrade == null) return;
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in DeclareDetails)
            {
                if (medicine is DeclareMedicine declareMedicine)
                {
                    if (!declareMedicine.PaySelf)
                        medicinesHcCost += declareMedicine.TotalPrice;
                    else
                        medicinesSelfCost += declareMedicine.TotalPrice;
                    purchaseCosts += declareMedicine.Cost * declareMedicine.Amount;
                }
                else if (medicine is PrescriptionOTC otc)
                {
                    medicinesSelfCost += otc.TotalPrice;
                    purchaseCosts += otc.Cost * otc.Amount;
                }

            }
            DeclareTrade.PaySelf = Math.Ceiling(medicinesSelfCost).ToString();//自費金額
            DeclareTrade.CopayMent = CountCopaymentCost(medicinesHcCost).ToString();//部分負擔
            //MedProfit = (medicinesHcCost + medicinesSelfCost - purchaseCosts);//藥品毛利
        }
        private int CountCopaymentCost(double medicinesHcCost)
        {
            const int free = 0;
            const int max = 200;
            if (medicinesHcCost >= 1001)
                return max;
            var times = medicinesHcCost / 100;
            if (times <= 1)
                return free;
            const int grades = 20;
            return Convert.ToInt16(Math.Floor(times) * grades);
        }
        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            TextBox t = sender as TextBox;
            DependencyObject dpobj = sender as DependencyObject;
            string name = dpobj.GetValue(FrameworkElement.NameProperty) as string;
            if (name.Equals("Usage"))
            {
                foreach (var u in MainWindow.Usages)
                {
                    if (t.Text.Equals(u.QuickName))
                        t.Text = u.Name;
                }
            }
            e.Handled = true;
            MoveFocusNext(sender);
        }
        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionMedicines.CurrentCell.Column is null) return;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }
                    if ((child is TextBox || child is AutoCompleteBox || child is CheckBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is AutoCompleteBox) && firstChild.Focusable)
                firstChild.Focus();
        }
        private int GetCurrentRowIndex(object sender)
        {
            if (sender is TextBox)
            {
                List<TextBox> temp = new List<TextBox>();
                TextBox textBox = sender as TextBox;

                NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, textBox.Name, ref temp);

                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        return x;
                    }
                }
            }
            else if (sender is CheckBox)
            {
                List<CheckBox> temp = new List<CheckBox>();
                CheckBox checkBox = sender as CheckBox;

                NewFunction.FindChildGroup<CheckBox>(PrescriptionMedicines, checkBox.Name, ref temp);

                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        return x;
                    }
                }
            }
            else if (sender is AutoCompleteBox)
            {
                List<AutoCompleteBox> temp = new List<AutoCompleteBox>();
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, autoCompleteBox.Name, ref temp);
                for (int x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(sender))
                    {
                        return x;
                    }
                }
            }

            return -1;
        }
        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as DeclareMedicine)?.Id is null
                        ? false
                        : (obj as DeclareMedicine).Id.ToLower().Contains(searchText.ToLower())
                          || (obj as DeclareMedicine).ChiName.ToLower().Contains(searchText.ToLower()) ||
                          (obj as DeclareMedicine).EngName.ToLower().Contains(searchText.ToLower());
            }
        }

        private void ButtonImportXml_Click(object sender, RoutedEventArgs e)
        {
            MessageWindow m;
            CheckMedicalNumber();
            InquiredPrescription.Prescription.Medicines = DeclareDetails;
            InquiredPrescription.Prescription.EList.Error = new List<Error>();
            InquiredPrescription.Prescription.EList.Error.Clear();
            InquiredPrescription.Prescription.EList.Error = InquiredPrescription.Prescription.CheckPrescriptionData();

            if (InquiredPrescription.Prescription.CheckPrescriptionData().Count == 0)
            {
                _currentDeclareData = new DeclareData(InquiredPrescription.Prescription);
                _currentDeclareData.DecMasId = InquiredPrescription.DecMasId;


                if (!_isPredictChronic)
                {
                    StockAdjustmentWindow stockAdjustmentWindow = new StockAdjustmentWindow(InquiredPrescription.DecMasId, OriginDeclareDetails, DeclareDetails);
                    stockAdjustmentWindow.ShowDialog();
                }
                if (IsAdjust || _isPredictChronic) { 
                    var declareDb = new DeclareDb();
                      declareDb.UpdateDeclareData(_currentDeclareData);
                    m = new MessageWindow("處方修改成功", MessageType.SUCCESS, true);
                    m.ShowDialog();
                    InitDataChanged();
                    PrescriptionOverview prescriptionOverview = new PrescriptionOverview(InquiredPrescription);
                    PrescriptionInquireView.Instance.UpdateDataFromOutcome(prescriptionOverview);
                    Close();
                } 
            }
            else
            {
                var errorMessage = "處方資料錯誤 : \n";
                foreach (var error in InquiredPrescription.Prescription.EList.Error)
                {
                    errorMessage += error.Content + "\n";
                }
                m = new MessageWindow(errorMessage, MessageType.ERROR, true);
                m.ShowDialog();
            }
            IsAdjust = false;
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectAll();
            DataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
          //  InquiredPrescription = PrescriptionDB.GetDeclareDataById(_decMasId);

          //ReleasePalace.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.FullName;
          //Division.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
          //CopaymentCode.Text = InquiredPrescription.Prescription.Treatment.Copayment.FullName;
          //PaymentCategory.Text = InquiredPrescription.Prescription.Treatment.PaymentCategory.FullName;
          //AdjustCase.Text = InquiredPrescription.Prescription.Treatment.AdjustCase.FullName;
          //TreatmentCase.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
             
          //  DeclareDetails.Clear();
          //  foreach (DeclareMedicine newDeclareDetail in InquiredPrescription.Prescription.Medicines)
          //  {
          //      DeclareMedicine declareDetailClone = (DeclareMedicine)newDeclareDetail.Clone();
          //      DeclareDetails.Add(declareDetailClone);
          //  }
          //  InitDataChanged();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isFirst = false;
        }

        private void MedicineTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
        }

        private void MakeUp_ButtonClick(object sender, RoutedEventArgs e)
        {
            if(!InquiredPrescription.Prescription.IsGetIcCard)
            {
                var loading = new LoadingWindow();
                loading.LoginIcData(Instance);
                loading.ShowDialog();
                if (IsGetMedicalNumber)
                    CreatIcUploadData();
                else
                    CreatIcErrorUploadData(SelectedErrorCode);
                ProductDb.InsertCashFow("補卡退還押金", (int.Parse(DeclareTrade.Deposit)*-1).ToString(), "DecMasId", InquiredPrescription.DecMasId);
                DeclareDb.UpdateDeclareRegisterMakeUp(InquiredPrescription.DecMasId);
                var m = new MessageWindow("補卡作業已完成", MessageType.SUCCESS, true);
                m.ShowDialog();
            }
        }

        #region 每日上傳.讀寫卡相關函數
        public void LogInIcData()
        {
            try
            {
                ReadCustomerTreatRecord();
                var cs = new ConvertData();
                var icPrescripList = new List<IcPrescriptData>();
                foreach (var med in InquiredPrescription.Prescription.Medicines)
                {
                    if (!(med is DeclareMedicine)) continue;
                    if (!((DeclareMedicine)med).PaySelf)
                    {
                        icPrescripList.Add(new IcPrescriptData((DeclareMedicine)med));
                    }
                }
                if (!InquiredPrescription.Prescription.IsGetIcCard) return;
                var pPatientId = new byte[10];
                var pPatientBitrhDay = new byte[10];
                Array.Copy(BasicDataArr, 32, pPatientId, 0, 10);
                Array.Copy(BasicDataArr, 42, pPatientBitrhDay, 0, 7);
                var pDateTime = cs.StringToBytes(Seq.TreatDateTime + " ", 14);
                foreach (var icPrescript in icPrescripList)
                {
                    var pData = cs.StringToBytes(icPrescript.DataStr, 61);
                    var pDataInput = pDateTime.Concat(pData).ToArray();
                    var strLength = 40;
                    var icData = new byte[40];
                    var res = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
                    if (res == 0)
                    {
                        res = HisApiBase.hisWritePrescriptionSign(pDateTime, pPatientId, pPatientBitrhDay, pDataInput,
                            icData, ref strLength);
                        if (res == 0)
                            _prescriptionSignatureList.Add(cs.ByToString(icData, 0, 40));
                        HisApiBase.csCloseCom();
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                var m = new MessageWindow("LogInIcData()", MessageType.ERROR, true);
                m.ShowDialog();
            }
        }

        public void CreatIcUploadData()
        {
            try
            {
                var medicalDatas = new List<MedicalData>();
                var icData = new IcData(Seq, InquiredPrescription.Prescription, CusBasicData, InquiredPrescription);
                var mainMessage = new MainMessage(icData);
                mainMessage.IcMessage.ActualTreatDate =
                    DateTimeExtensions.ConvertToTaiwanCalender(InquiredPrescription.Prescription.Treatment.AdjustDate,
                        false);
                var headerMessage = new Header { DataFormat = "1" };
                var icRecord = new REC(headerMessage, mainMessage);
                int sigCount = 0;
                for (var i = 0; i < InquiredPrescription.Prescription.Medicines.Count(m => (m is DeclareMedicine med) && !med.PaySelf); i++)
                {
                    if (InquiredPrescription.DeclareDetails[i].P1MedicalOrder.Equals("9"))
                        continue;
                    bool isMedicine = false;
                    foreach (var m in InquiredPrescription.Prescription.Medicines)
                    {
                        if (InquiredPrescription.DeclareDetails[i].P2MedicalId.Equals(m.Id))
                        {
                            isMedicine = true;
                            break;
                        }
                    }
                    if (!isMedicine)
                        continue;
                    var medicalData = new MedicalData
                    {
                        MedicalOrderTreatDateTime = Seq.TreatDateTime,
                        MedicalOrderCategory = InquiredPrescription.DeclareDetails[i].P1MedicalOrder,
                        TreatmentProjectCode = InquiredPrescription.DeclareDetails[i].P2MedicalId,
                        Usage = InquiredPrescription.DeclareDetails[i].P4Usage,
                        Days = InquiredPrescription.DeclareDetails[i].P11Days.ToString(),
                        TotalAmount = InquiredPrescription.DeclareDetails[i].P7Total.ToString(),
                    };
                    if (_prescriptionSignatureList.Count > 0)
                    {
                        medicalData.PrescriptionSignature = _prescriptionSignatureList[sigCount];
                    }
                    if (!string.IsNullOrEmpty(InquiredPrescription.DeclareDetails[i].P5Position))
                        medicalData.TreatmentPosition = InquiredPrescription.DeclareDetails[i].P5Position;
                    switch (medicalData.MedicalOrderCategory)
                    {
                        case "1":
                        case "A":
                            medicalData.PrescriptionDeliveryMark = "02";
                            break;
                        case "2":
                        case "B":
                            medicalData.PrescriptionDeliveryMark = "06";
                            break;
                        case "3":
                        case "C":
                        case "4":
                        case "D":
                        case "5":
                        case "E":
                            medicalData.PrescriptionDeliveryMark = "04";
                            break;
                    }
                    sigCount++;
                    medicalDatas.Add(medicalData);
                }
                icRecord.MainMessage.MedicalMessageList = medicalDatas;
                icRecord.SerializeDailyUploadObject();
                var d = new DeclareDb();
                d.InsertDailyUpload(icRecord.SerializeDailyUploadObject());
            }
            catch (Exception ex)
            {
                Action creatIcUploadDataDelegate = delegate ()
                {
                    var m = new MessageWindow(ex.Message, MessageType.ERROR, true);
                    m.ShowDialog();
                };
                Instance.Dispatcher.BeginInvoke(creatIcUploadDataDelegate);
            }
        }

        //異常上傳
        public void CreatIcErrorUploadData(IcErrorCodeWindow.IcErrorCode errorCode)
        {
            Action creatIcUploadDataDelegate = delegate ()
            {
                try
                {
                    var medicalDatas = new List<MedicalData>();
                    if (icErrorWindow.SelectedItem == null) return;
                    var icData = new IcData(InquiredPrescription.Prescription, errorCode, InquiredPrescription);
                    var mainMessage = new MainMessage(icData);
                    var headerMessage = new Header { DataFormat = "2" };
                    var icRecord = new REC(headerMessage, mainMessage);

                    for (var i = 0; i < InquiredPrescription.Prescription.Medicines.Count(m => (m is DeclareMedicine med) && !med.PaySelf); i++)
                    {
                        if (InquiredPrescription.DeclareDetails[i].P1MedicalOrder.Equals("9"))
                            continue;
                        var medicalData = new MedicalData
                        {
                            MedicalOrderTreatDateTime = icData.TreatmentDateTime,
                            MedicalOrderCategory = InquiredPrescription.DeclareDetails[i].P1MedicalOrder,
                            TreatmentProjectCode = InquiredPrescription.DeclareDetails[i].P2MedicalId,
                            Usage = InquiredPrescription.DeclareDetails[i].P4Usage,
                            Days = InquiredPrescription.DeclareDetails[i].P11Days.ToString(),
                            TotalAmount = InquiredPrescription.DeclareDetails[i].P7Total.ToString(),
                        };
                        if (!string.IsNullOrEmpty(InquiredPrescription.DeclareDetails[i].P5Position))
                            medicalData.TreatmentPosition = InquiredPrescription.DeclareDetails[i].P5Position;
                        switch (medicalData.MedicalOrderCategory)
                        {
                            case "1":
                            case "A":
                                medicalData.PrescriptionDeliveryMark = "01";
                                break;
                            case "2":
                            case "B":
                                medicalData.PrescriptionDeliveryMark = "06";
                                break;
                            case "3":
                            case "C":
                            case "4":
                            case "D":
                            case "5":
                            case "E":
                                medicalData.PrescriptionDeliveryMark = "04";
                                break;
                        }
                        medicalDatas.Add(medicalData);
                    }

                    icRecord.MainMessage.MedicalMessageList = medicalDatas;
                    icRecord.SerializeObject();
                    var d = new DeclareDb();
                    d.InsertDailyUpload(icRecord.SerializeObject());
                }
                catch (Exception ex)
                {
                    var m = new MessageWindow(ex.Message, MessageType.ERROR, true);
                    m.ShowDialog();
                }
            };
            Instance.Dispatcher.BeginInvoke(creatIcUploadDataDelegate);
        }
        #endregion
        private void start_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox t)
            {
                t.SelectionStart = 0;
                t.SelectionLength = t.Text.Length;
            }
        }

        //自費 實收新增資料 SelfCost:自費 Pay:實收
        private void ButtonPrintMedBag_Click(object sender, RoutedEventArgs e) {
            var declareData = new DeclareData(InquiredPrescription.Prescription);
            int.TryParse(DeclareTrade.ReceiveMoney, out var receive);
            NewFunction.PrintMedBag(InquiredPrescription.Prescription, declareData, declareData.D33DrugsPoint, 0, 0, "查詢", receive, null, Instance);
        }

        private void ReadCustomerTreatRecord()
        {
            if (HisApiBase.GetStatus(2))
            {
                var strLength = 72;
                var icData = new byte[72];
                HisApiBase.OpenCom();
                if (MainWindow.Instance.HisApiErrorCode == 0)
                {
                    MainWindow.Instance.SetCardReaderStatus("健保卡資料讀取中...");
                    MainWindow.Instance.HisApiErrorCode = HisApiBase.hisGetBasicData(icData, ref strLength);
                    if (MainWindow.Instance.HisApiErrorCode == 0)
                    {
                        MainWindow.Instance.SetCardReaderStatus("健保卡讀取成功");
                        InquiredPrescription.Prescription.IsGetIcCard = true;
                        icData.CopyTo(BasicDataArr, 0);
                        CusBasicData = new BasicData(icData);
                        HisApiBase.CloseCom();
                        var tmpMedicalNumber = InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber.DeepCloneViaJson();
                        InquiredPrescription.Prescription.Customer = new Customer(CusBasicData);
                        InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber = tmpMedicalNumber;
                        CustomerDb.LoadCustomerData(InquiredPrescription.Prescription.Customer);
                    }
                    HisApiBase.CloseCom();
                }
                ReadTreatRecord();
            }
            else
            {
                MainWindow.Instance.SetCardReaderStatus("安全模組未認證");
            }
        }
        public void ReadTreatRecord()
        {
            var strLength = 296;
            var icData = new byte[296];
            var cs = new ConvertData();
            var cTreatItem = cs.StringToBytes("AF\0", 3);
            //新生兒就醫註記,長度兩個char
            var cBabyTreat = TreatRecCollection.Count > 0
                ? cs.StringToBytes(TreatRecCollection[0].NewbornTreatmentMark + "\0", 3)
                : cs.StringToBytes(" ", 2);
            //補卡註記,長度一個char
            var cTreatAfterCheck = new byte[] { 1 };
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
            var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, icData, ref strLength);
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csCloseCom();
            //取得就醫序號
            if (res == 0)
            {
                IsGetMedicalNumber = true;
                Seq = new SeqNumber(icData);
            }

            //取得就醫紀錄
            strLength = 498;
            icData = new byte[498];
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
            res = HisApiBase.hisGetTreatmentNoNeedHPC(icData, ref strLength);
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csCloseCom();
            if (res == 0)
            {
                var startIndex = 84;
                for (var i = 0; i < 6; i++)
                {
                    if (icData[startIndex + 3] == 32) break;
                    TreatRecCollection.Add(new TreatmentDataNoNeedHpc(icData, startIndex, false));
                    startIndex += 69;
                }
            }
        }
        private void CheckMedicalNumber()
        {
            if (!string.IsNullOrEmpty(InquiredPrescription.Prescription.ChronicSequence) && int.Parse(InquiredPrescription.Prescription.ChronicSequence) > 1)
            {
                InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber = "IC0" + InquiredPrescription.Prescription.ChronicSequence;
                InquiredPrescription.Prescription.OriginalMedicalNumber = TempMedicalNumber;
            }
            else
            {
                InquiredPrescription.Prescription.Customer.IcCard.MedicalNumber = TempMedicalNumber;
            }
        }

        private void DiseaseCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter)
            {
                if (textBox.Text.Contains(" "))
                    textBox.Text = string.Empty;
                return;
            }

            DependencyObject textBoxDependency = textBox;
            var name = textBoxDependency.GetValue(NameProperty);
            if (name.Equals("MainDiagnosis"))
            {
                if ((!string.IsNullOrEmpty(InquiredPrescription.Prescription.Treatment.MedicalInfo.MainDiseaseCode.Id) &&
                     textBox.Text.Contains(" ")) || string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    SecondDiagnosis.Focus();
                    return;
                }
            }
            else if (name.Equals("SecondDiagnosis"))
            {
                if (string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    ChronicTotal.Focus();
                    return;
                }
            }

            if (textBox.Text.Length < 3)
            {
                var m = new MessageWindow("請輸入完整疾病代碼", MessageType.WARNING, true);
                m.ShowDialog();
                return;
            }

            if (DiseaseCodeDb.GetDiseaseCodeById(textBox.Text).DistinctBy(d => d.ICD10.Id).DistinctBy(d => d.ICD9.Id)
                    .ToList().Count == 1)
            {
                var selectedDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(textBox.Text)[0].ICD10;
                if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                {
                    var m = new MessageWindow("查無疾病代碼", MessageType.WARNING, true)
                    {
                        Owner = Application.Current.MainWindow
                    };
                    m.ShowDialog();
                    return;
                }

                if (textBoxDependency.GetValue(NameProperty) is string &&
                    textBoxDependency.GetValue(NameProperty).Equals("MainDiagnosis"))
                    InquiredPrescription.Prescription.Treatment.MedicalInfo.MainDiseaseCode = selectedDiseaseCode;
                else
                {
                    if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                    {
                        var m = new MessageWindow("查無疾病代碼", MessageType.WARNING, true)
                        {
                            Owner = Application.Current.MainWindow
                        };
                        m.ShowDialog();
                        return;
                    }

                    if ((selectedDiseaseCode.Id.Equals("查無疾病代碼") && textBox.Text.Contains(" ")) ||
                        string.IsNullOrEmpty(textBox.Text.Trim()))
                    {
                        ChronicTotal.Focus();
                    }
                    else
                    {
                        InquiredPrescription.Prescription.Treatment.MedicalInfo.SecondDiseaseCode = selectedDiseaseCode;
                    }
                }
            }
            else
            {
                var disease = new DiseaseCodeSelectDialog(textBox.Text, (string)name);
                if (disease.DiseaseCollection.Count > 1)
                    disease.Show();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PrescriptionMedicines.SelectedIndex == -1 || InquiredPrescription.Prescription.Medicines.Count == 0) return;
            if (!(InquiredPrescription.Prescription.Medicines[PrescriptionMedicines.SelectedIndex] is DeclareMedicine med)) return;
            var m = new MedicineInfoWindow(MedicineDb.GetMedicalInfoById(med.Id));
            m.Show();
        }
    }
}