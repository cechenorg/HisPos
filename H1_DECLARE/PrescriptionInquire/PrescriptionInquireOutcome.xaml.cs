using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Service;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionInquireOutcome : Window, INotifyPropertyChanged
    {
        private bool _isFirst = true;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
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
        public ObservableCollection<DeclareMedicine> DeclareMedicinesData { get; set; }
        private static DeclareData _inquiredPrescription;
        public DeclareData InquiredPrescription
        {
            get => _inquiredPrescription;
            set
            {
                _inquiredPrescription = value;
                PatientGender.Content = _inquiredPrescription.Prescription.Customer.IcNumber.Substring(1, 1).Equals("2") ? "女" : "男";
                NotifyPropertyChanged(nameof(InquiredPrescription));
            }
        }
        private ObservableCollection<DeclareMedicine> _declareDetails = new ObservableCollection<DeclareMedicine>();
        public ObservableCollection<DeclareMedicine> DeclareDetails
        {
            get => _declareDetails;
            set
            {
                _declareDetails = value;
                NotifyPropertyChanged(nameof(DeclareDetails));
            }
        }
        private ObservableCollection<object> _medicines;
        private readonly string _decMasId;
        public PrescriptionInquireOutcome(DeclareData inquired)
        {
            InitializeComponent();
            _isFirst = true;
            DataContext = this;
            DeclareTrade = DeclareTradeDb.GetDeclarTradeByMasId(inquired.DecMasId);
            _decMasId = inquired.DecMasId;
            InquiredPrescription = inquired;
            foreach (var newDeclareDetail in InquiredPrescription.Prescription.Medicines) {
                var declareDetailClone = (DeclareMedicine)newDeclareDetail.Clone();
                DeclareDetails.Add(declareDetailClone);
            }
            InitData();
            InitDataChanged();
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
            DeclareMedicinesData = PrescriptionInquireView.Instance.DeclareMedicinesData;
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

            var tmpCopayment = CopaymentCollection.SingleOrDefault(c =>
                c.Id.Equals(InquiredPrescription.Prescription.Treatment.Copayment.Id));

            if (tmpCopayment != null)
            {
                tmpCopayment.Point = InquiredPrescription.Prescription.Treatment.Copayment.Point;
                InquiredPrescription.Prescription.Treatment.Copayment = tmpCopayment;
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
            var declareMedicine = (DeclareMedicine)(medicineCodeAuto.SelectedItem as DeclareMedicine)?.Clone();
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
                x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
            _medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = _medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMedicine(DeclareDetails[PrescriptionMedicines.SelectedIndex]);
            DataChanged();
            DeclareDetails.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }
        public void ClearMedicine(DeclareMedicine med)
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
            med.Dosage = string.Empty;
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
                if (!medicine.PaySelf)
                    medicinesHcCost += medicine.TotalPrice;
                else
                {
                    medicinesSelfCost += medicine.TotalPrice;
                }
                purchaseCosts += medicine.Cost * medicine.Amount;
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
            var objectName = (sender as Control)?.Name;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var nextTextBox = new List<TextBox>();
                var nextAutoCompleteBox = new List<AutoCompleteBox>();
                var currentRowIndex = GetCurrentRowIndex(sender);

                if (currentRowIndex == -1) return;

                switch (objectName)
                {
                    case "Dosage":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Usage", ref nextTextBox);
                        break;

                    case "Usage":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!InquiredPrescription.Prescription.Medicines[currentRowIndex].PaySelf)
                        {
                            NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                            NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                            nextTextBox[0].Focus();
                            return;
                        }
                        else
                        {
                            NewFunction.FindChildGroup(PrescriptionMedicines, "Price", ref nextTextBox);
                            nextTextBox[currentRowIndex].Focus();
                        }
                        break;

                    case "Price":
                        NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                        NewFunction.FindChildGroup<TextBox>(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                        nextTextBox[0].Focus();
                        return;
                }
                nextTextBox[currentRowIndex].Focus();
                nextTextBox[currentRowIndex].CaretIndex = 0;
            }
            ////按 Up Down
            //if (e.Key == Key.Up || e.Key == Key.Down)
            //{
            //    e.Handled = true;
            //    var thisTextBox = new List<TextBox>();
            //    var currentRowIndex = GetCurrentRowIndex(sender);

            //    if (currentRowIndex == -1) return;

            //    NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, objectName, ref thisTextBox);

            //    int newIndex = (e.Key == Key.Up) ? currentRowIndex - 1 : currentRowIndex + 1;

            //    if (newIndex < 0)
            //        newIndex = 0;
            //    else if (newIndex >= thisTextBox.Count)
            //        newIndex = thisTextBox.Count - 1;

            //    thisTextBox[newIndex].Focus();
            //}
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
            InquiredPrescription.Prescription.Medicines = DeclareDetails;
            InquiredPrescription.Prescription.EList.Error = new List<Error>();
            InquiredPrescription.Prescription.EList.Error.Clear();
            InquiredPrescription.Prescription.EList.Error = InquiredPrescription.Prescription.CheckPrescriptionData();

            if (InquiredPrescription.Prescription.CheckPrescriptionData().Count == 0)
            {
                InquiredPrescription.Prescription.Customer.Birthday = DateTimeExtensions.YearFormatTransfer(InquiredPrescription.Prescription.Customer.Birthday);
                var declareData = new DeclareData(InquiredPrescription.Prescription);
                declareData.DecMasId = InquiredPrescription.DecMasId;
                
                var declareDb = new DeclareDb();
                //SelfCost.Content = SelfCost.Content.ToString() == null ? "0" : SelfCost.Content.ToString();
                //Deposit.Content = Deposit.Content.ToString() == null ? "0" : Deposit.Content.ToString();
                //Charge.Content = Charge.Content.ToString() == null ? "0" : Charge.Content.ToString();
                //Copayment.Content = Copayment.Content.ToString() == null ? "0" : Copayment.Content.ToString();
                //Pay.Content = Pay.Content.ToString() == null ? "0" : Pay.Content.ToString();
                //Change.Content = Change.Content.ToString() == null ? "0" : Change.Content.ToString();
               
                //DeclareTrade declareTrade = new DeclareTrade(InquiredPrescription.Prescription.Customer.Id, MainWindow.CurrentUser.Id, SelfCost.Content.ToString(), Deposit.Content.ToString(), Charge.Content.ToString(), Copayment.Content.ToString(), Pay.Content.ToString(), Change.Content.ToString(), "現金");
                declareDb.UpdateDeclareData(declareData);
                m = new MessageWindow("處方修改成功", MessageType.SUCCESS);
                m.Show();
                InitDataChanged();
                PrescriptionOverview prescriptionOverview = new PrescriptionOverview(InquiredPrescription);
                PrescriptionInquireView.Instance.UpdateDataFromOutcome(prescriptionOverview);
            }
            else
            {
                var errorMessage = "處方資料錯誤 : \n";
                foreach (var error in InquiredPrescription.Prescription.EList.Error)
                {
                    errorMessage += error.Content + "\n";
                }
                m = new MessageWindow(errorMessage, MessageType.ERROR);
                m.Show();
            }
          
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            InquiredPrescription = PrescriptionDB.GetDeclareDataById(_decMasId);

          ReleasePalace.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.FullName;
          Division.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
          CopaymentCode.Text = InquiredPrescription.Prescription.Treatment.Copayment.FullName;
          PaymentCategory.Text = InquiredPrescription.Prescription.Treatment.PaymentCategory.FullName;
          AdjustCase.Text = InquiredPrescription.Prescription.Treatment.AdjustCase.FullName;
          TreatmentCase.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase.FullName;


            DeclareDetails.Clear();
            foreach (DeclareMedicine newDeclareDetail in InquiredPrescription.Prescription.Medicines)
            {
                DeclareMedicine declareDetailClone = (DeclareMedicine)newDeclareDetail.Clone();
                DeclareDetails.Add(declareDetailClone);
            }
            InitDataChanged();
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

      
    }
}