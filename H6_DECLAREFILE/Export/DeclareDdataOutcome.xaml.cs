using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using JetBrains.Annotations;
using Prescription = His_Pos.Class.Prescription;

namespace His_Pos.H6_DECLAREFILE.Export
{
    /// <summary>
    /// DeclareDdataOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class DeclareDdataOutcome : Window, INotifyPropertyChanged
    {
        private bool isFirst = true;
        public ObservableCollection<Hospital> Hospitals { get; set; }
        private ObservableCollection<TreatmentCase> _treatmentCaseCollection;

        public ObservableCollection<TreatmentCase> TreatmentCaseCollection
        {
            get => _treatmentCaseCollection;
            set
            {
                _treatmentCaseCollection = value;
                OnPropertyChanged(nameof(TreatmentCaseCollection));
            }
        }

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

        private ObservableCollection<PaymentCategory> _paymentCategoryCollection;

        public ObservableCollection<PaymentCategory> PaymentCategoryCollection
        {
            get => _paymentCategoryCollection;
            set
            {
                _paymentCategoryCollection = value;
                OnPropertyChanged(nameof(PaymentCategoryCollection));
            }
        }

        private ObservableCollection<Copayment> _copaymentCollection;

        public ObservableCollection<Copayment> CopaymentCollection
        {
            get => _copaymentCollection;
            set
            {
                _copaymentCollection = value;
                OnPropertyChanged(nameof(CopaymentCollection));
            }
        }

        private ObservableCollection<Division> _divisionCollection;

        public ObservableCollection<Division> DivisionCollection
        {
            get => _divisionCollection;
            set
            {
                _divisionCollection = value;
                OnPropertyChanged(nameof(DivisionCollection));
            }
        }

        private ObservableCollection<Hospital> _hospitalCollection;

        public ObservableCollection<Hospital> HospitalCollection
        {
            get => _hospitalCollection;
            set
            {
                _hospitalCollection = value;
                OnPropertyChanged(nameof(HospitalCollection));
            }
        }

        public ObservableCollection<DeclareMedicine> DeclareMedicinesData { get; set; }
        public ObservableCollection<DeclareMedicine> PrescriptionMedicines { get; set; }
        public ObservableCollection<Usage> Usages { get; set; }
        private string _gender;

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private Prescription _currentDeclareFileDdata;

        public Prescription CurrentPrescription
        {
            get => _currentDeclareFileDdata;
            set
            {
                _currentDeclareFileDdata = value;
                OnPropertyChanged(nameof(CurrentPrescription));
            }
        }

        public DeclareDdataOutcome()
        {
            InitializeComponent();
        }

        private ObservableCollection<object> Medicines;

        public DeclareDdataOutcome(DeclareFileDdata d, ObservableCollection<Hospital> hospitals)
        {
            InitializeComponent();
            Hospitals = hospitals;
            CurrentPrescription = new Prescription(d);
            InitData(d);
            InitDataChanged();
        }

        private void InitData(DeclareFileDdata declareFileDdata)
        {
            if (ExportView.Instance is null) return;
            var loadingWindow = new LoadingWindow();
            loadingWindow.ChangeLoadingMessage("處方申報資料載入中...");
            loadingWindow.Show();
            DivisionCollection = ExportView.Instance.DivisionCollection;
            CopaymentCollection = ExportView.Instance.CopaymentCollection;
            PaymentCategoryCollection = ExportView.Instance.PaymentCategoryCollection;
            AdjustCaseCollection = ExportView.Instance.AdjustCaseCollection;
            TreatmentCaseCollection = ExportView.Instance.TreatmentCaseCollection;
            HospitalCollection = ExportView.Instance.HospitalCollection;
            DeclareMedicinesData = ExportView.Instance.DeclareMedicinesData;
            ReleasePalace.Text = HospitalCollection.SingleOrDefault(h => h.Id.Equals(CurrentPrescription.Treatment.MedicalInfo.Hospital.Id))?.FullName;
            Division.ItemsSource = DivisionCollection;
            Division.Text = DivisionCollection.SingleOrDefault(d => d.Id.Equals(CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Id))?.FullName;
            CopaymentCode.ItemsSource = CopaymentCollection;
            CopaymentCode.Text =
                CopaymentCollection.SingleOrDefault(c => c.Id.Equals(CurrentPrescription.Treatment.Copayment.Id))?.FullName;
            PaymentCategory.ItemsSource = PaymentCategoryCollection;
            PaymentCategory.Text = PaymentCategoryCollection.SingleOrDefault(p => p.Id.Equals(CurrentPrescription.Treatment.PaymentCategory.Id))
                ?.FullName;
            AdjustCase.ItemsSource = AdjustCaseCollection;
            AdjustCase.Text = AdjustCaseCollection.SingleOrDefault(a => a.Id.Equals(CurrentPrescription.Treatment.AdjustCase.Id))?.FullName;
            TreatmentCase.ItemsSource = TreatmentCaseCollection;
            TreatmentCase.Text = TreatmentCaseCollection.SingleOrDefault(t => t.Id.Equals(CurrentPrescription.Treatment.MedicalInfo.TreatmentCase.Id))
                ?.FullName;
            //foreach (var p in declareFileDdata.Dbody.Pdata)
            //{
            //    if (p.P1.Equals("1") && !p.P2.Contains("MA"))
            //        PrescriptionMedicines.Add(DeclareMedicinesData.SingleOrDefault(m => m.Id.Equals(p.P2)));
            //}

            PrescriptionSet.ItemsSource = PrescriptionMedicines;
            loadingWindow.Close();
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void DataChanged()
        {
            if (isFirst) return;

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

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection =
                new ObservableCollection<Hospital>(Hospitals.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50)
                    .ToList());
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MedicineTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.Text.Length;
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;

            if (selectedItem is IDeletable)
            {
                if (PrescriptionMedicines.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionSet.SelectedItem = selectedItem;
                return;
            }

            PrescriptionSet.SelectedIndex = PrescriptionMedicines.Count;

        }


        private void DataGridRow_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMedicine(PrescriptionMedicines[PrescriptionSet.SelectedIndex]);
            DataChanged();
            PrescriptionMedicines.RemoveAt(PrescriptionSet.SelectedIndex);
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
            med.Dosage = string.Empty;
        }

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;
            DataChanged();
            if (medicineCodeAuto is null) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty &&
                    ((ObservableCollection<object>) medicineCodeAuto.ItemsSource).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                {
                    medicineCodeAuto.SelectedItem = (medicineCodeAuto.ItemsSource as ObservableCollection<object>)?[0];
                }
                else
                    return;
            }

            var declareMedicine = (DeclareMedicine) (medicineCodeAuto.SelectedItem as DeclareMedicine)?.Clone();
            var currentRow = GetCurrentRowIndex(sender);

            if (PrescriptionMedicines.Count > 0)
            {
                if (PrescriptionMedicines.Count == currentRow)
                {
                    PrescriptionMedicines.Add(declareMedicine);
                    medicineCodeAuto.Text = string.Empty;
                }
                else
                {
                    PrescriptionMedicines[currentRow] = declareMedicine;
                }
            }
            else
            {
                PrescriptionMedicines.Add(declareMedicine);
                medicineCodeAuto.Text = string.Empty;
            }
        }

        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                {
                    var temp = new List<TextBox>();

                    NewFunction.FindChildGroup<TextBox>(PrescriptionSet, textBox.Name, ref temp);

                    for (int x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(textBox))
                        {
                            return x;
                        }
                    }

                    break;
                }
                case CheckBox checkBox:
                {
                    var temp = new List<CheckBox>();

                    NewFunction.FindChildGroup<CheckBox>(PrescriptionSet, checkBox.Name, ref temp);

                    for (int x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(checkBox))
                        {
                            return x;
                        }
                    }

                    break;
                }
                case AutoCompleteBox _:
                {
                    var temp = new List<AutoCompleteBox>();
                    var autoCompleteBox = sender as AutoCompleteBox;
                    NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionSet, autoCompleteBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(sender))
                        {
                            return x;
                        }
                    }

                    break;
                }
            }

            return -1;
        }

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;

            if (medicineCodeAuto is null) return;

            var result = DeclareMedicinesData.Where(x =>
                x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
            Medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = Medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
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

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var objectName = (sender as Control)?.Name;
            //按 Enter 下一欄
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
                        NewFunction.FindChildGroup(PrescriptionSet, "Usage", ref nextTextBox);
                        break;
                    case "Usage":
                        FindUsagesQuickName(sender);
                        NewFunction.FindChildGroup(PrescriptionSet, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup(PrescriptionSet, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup(PrescriptionSet, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!PrescriptionMedicines[currentRowIndex].PaySelf)
                        {
                            NewFunction.FindChildGroup(PrescriptionSet, "MedicineCodeAuto",
                                ref nextAutoCompleteBox);
                            NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text",
                                ref nextTextBox);
                            nextTextBox[0].Focus();
                            return;
                        }
                        else
                        {
                            NewFunction.FindChildGroup(PrescriptionSet, "Price", ref nextTextBox);
                            nextTextBox[currentRowIndex].Focus();
                        }

                        break;

                    case "Price":
                        NewFunction.FindChildGroup(PrescriptionSet, "MedicineCodeAuto", ref nextAutoCompleteBox);
                        NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                        nextTextBox[0].Focus();
                        return;
                }

                nextTextBox[currentRowIndex].Focus();
                nextTextBox[currentRowIndex].CaretIndex = 0;
            }
        }

        private void FindUsagesQuickName(object sender)
        {
            var currentRow = GetCurrentRowIndex(sender);
            if (sender is TextBox t && !string.IsNullOrEmpty(t.Text))
            {
                if (Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text)) != null)
                {
                    PrescriptionMedicines[currentRow].Usage = Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                    if (PrescriptionMedicines[currentRow].Usage != null)
                        t.Text = PrescriptionMedicines[currentRow].Usage.Name;
                }
            }
        }

        private void Dosage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = textBox.Text.Length;
            }
        }

        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var t = sender as TextBox;
            if (string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }

        private void MedTotalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isFirst) return;
            CountMedicinesCost();
        }

        private void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in PrescriptionMedicines)
            {
                if (!medicine.PaySelf)
                    medicinesHcCost += medicine.TotalPrice;
                else
                {
                    medicinesSelfCost += medicine.TotalPrice;
                }
                purchaseCosts += medicine.Cost * medicine.Amount;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
