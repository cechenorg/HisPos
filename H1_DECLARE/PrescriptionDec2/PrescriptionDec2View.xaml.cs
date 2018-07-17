using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    ///     PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        private Prescription _prescription = new Prescription();
        private bool isChanged;

        private readonly bool IsFirst = true;
        private ObservableCollection<object> Medicines;
        public ObservableCollection<Hospital> HosiHospitals { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }
        public ObservableCollection<TreatmentCase> TreatmentCases { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategories { get; set; }
        public ObservableCollection<Copayment> Copayments { get; set; }
        public ObservableCollection<AdjustCase> AdjustCases { get; set; }
        public ObservableCollection<Usage> Usages { get; set; }
        public ObservableCollection<DeclareMedicine> DeclareMedicines { get; set; }
        

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            GetPrescriptionData();
        }

        public Prescription Prescription
        {
            get => _prescription;
            set
            {
                _prescription = value;
                NotifyPropertyChanged("Prescription");
            }
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

        public void LoadPrescriptionData()
        {
            UsageDb.GetUsages();
        }

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            TreatmentCases = new ObservableCollection<TreatmentCase>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
        }

        /*
         *載入原處方案件類別
         */

        private void LoadTreatmentCases()
        {
            TreatmentCaseDb.GetData();
            TreatmentCaseCombo.ItemsSource = TreatmentCaseDb.GetData();
        }

        /*
         *載入原處方案件類別
         */

        private void LoadAdjustCases()
        {
            AdjustCaseCombo.ItemsSource = AdjustCaseDb.GetData();
        }

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            CheckIcNumber(Prescription.Customer.IcCard.IcNumber);
            CheckBirthDay(Prescription.Customer.Birthday);
        }

        private void CheckBirthDay(string customerBirthday)
        {
            Regex birth = new Regex(@"[0-9]{7}");
            if (birth.IsMatch(customerBirthday))
            {
                string year = customerBirthday.Substring(0, 3);
                string month = customerBirthday.Substring(3, 2);
                string date = customerBirthday.Substring(5, 2);
                Prescription.Customer.Birthday = year + "/" + month + "/" + date;
            }
            //error
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (Prescription.Medicines.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionMedicines.SelectedItem = selectedItem;
                return;
            }
            PrescriptionMedicines.SelectedIndex = Prescription.Medicines.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMedicine(Prescription.Medicines[PrescriptionMedicines.SelectedIndex]);
            SetChanged();
            Prescription.Medicines.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }

        private void SetChanged()
        {
            if (IsFirst) return;
            isChanged = true;
        }

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;

            if (medicineCodeAuto is null) return;

            var result = DeclareMedicines.Where(x =>
                x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
            Medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = Medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;
            SetChanged();
            if (medicineCodeAuto is null) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty &&
                    (medicineCodeAuto.ItemsSource as ObservableCollection<object>).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                    medicineCodeAuto.SelectedItem = (medicineCodeAuto.ItemsSource as ObservableCollection<object>)[0];
                else
                    return;
            }

            DeclareMedicine declareMedicine = (DeclareMedicine)(medicineCodeAuto.SelectedItem as DeclareMedicine).Clone();
            int currentRow = GetCurrentRowIndex(sender);

            if (Prescription.Medicines.Count > 0)
            {
                if (Prescription.Medicines.Count == currentRow)
                {
                    Prescription.Medicines.Add(declareMedicine);
                    medicineCodeAuto.Text = "";
                }
                else
                {
                    Prescription.Medicines[currentRow] = declareMedicine;
                }
            }
            else
            {
                Prescription.Medicines.Add(declareMedicine);
                medicineCodeAuto.Text = "";
            }
            
        }

        public void ClearMedicine(DeclareMedicine med)
        {
            med.PaySelf = false;
            med.Cost = 0;
            med.TotalPrice = 0;
            med.Amount = 0;
            med.CountStatus = "";
            med.FocusColumn = "";
            med.Usage = new Usage();
            med.Days = "";
            med.Position = "";
            med.Source = "";
            med.Dosage = "";
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var objectName = (sender as Control).Name;

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
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Usage", ref nextTextBox);
                        break;

                    case "Usage":
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!Prescription.Medicines[currentRowIndex].PaySelf)
                        {
                            NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                            NewFunction.FindChildGroup<TextBox>(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                            nextTextBox[0].Focus();
                            return;
                        }
                        else
                        {
                            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Price", ref nextTextBox);
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

            //按 Up Down
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
                var thisTextBox = new List<TextBox>();
                var currentRowIndex = GetCurrentRowIndex(sender);

                if (currentRowIndex == -1) return;

                NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, objectName, ref thisTextBox);

                int newIndex = (e.Key == Key.Up) ? currentRowIndex - 1 : currentRowIndex + 1;

                if (newIndex < 0)
                    newIndex = 0;
                else if (newIndex >= thisTextBox.Count)
                    newIndex = thisTextBox.Count - 1;

                thisTextBox[newIndex].Focus();
            }
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

        //return 1 : 長度不足 2 : 性別碼錯誤 3 : 首碼錯誤 4 : 檢查碼錯誤
        private string CheckIcNumber(string vid)
        {
            var firstEng = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            var aa = vid.ToUpper();
            var chackFirstEnd = false;
            if (aa.Trim().Length != 10) return "1";
            var firstNo = Convert.ToByte(aa.Trim().Substring(1, 1));
            if (firstNo > 2 || firstNo < 1)
            {
                return "性別碼(第一位數字)錯誤，男性為 1 女性為 2";
            }
            int x;
            for (x = 0; x < firstEng.Count; x++)
            {
                if (aa.Substring(0, 1) == firstEng[x])
                {
                    aa = string.Format("{0}{1}", x + 10, aa.Substring(1, 9));
                    chackFirstEnd = true;
                    break;
                }
            }
            if (!chackFirstEnd)
                return "3";
            int i = 1;
            int ss = int.Parse(aa.Substring(0, 1));
            while (aa.Length > i)
            {
                ss = ss + (int.Parse(aa.Substring(i, 1)) * (10 - i));
                i++;
            }
            aa = ss.ToString();
            if (vid.Substring(9, 1) == "0")
            {
                if (aa.Substring(aa.Length - 1, 1) == "0")
                {
                    return "0";
                }
                return "4";
            }
            if (vid.Substring(9, 1) == (10 - int.Parse(aa.Substring(aa.Length - 1, 1))).ToString())
            {
                return "0";
            }
            return "4";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MedicineTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = textBox.Text.Length;
            }
        }
        
    }
}