﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Service;
using MahApps.Metro.Controls;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    ///     PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        private Prescription _prescription = new Prescription();
        private bool IsChanged;

        private readonly bool IsFirst = true;
        public ObservableCollection<object> Medicines;
        private ObservableCollection<Usage> usages = new ObservableCollection<Usage>();

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            GetPrescriptionData();
            LoadPrescriptionData();
        }

        public ObservableCollection<DeclareMedicine> DeclareMedicines { get; set; }

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
                    (obj as DeclareMedicine).Id is null
                        ? false
                        : (obj as DeclareMedicine).Id.ToLower().Contains(searchText.ToLower())
                          || (obj as DeclareMedicine).ChiName.ToLower().Contains(searchText.ToLower()) ||
                          (obj as DeclareMedicine).EngName.ToLower().Contains(searchText.ToLower());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadPrescriptionData()
        {
            LoadHospitalData();
            LoadTreatmentCases();
            LoadPaymentCategories();
            LoadCopayments();
            LoadAdjustCases();
            usages = UsageDb.GetUsages();
        }

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;
            PrescriptionMedicines.ItemsSource = Prescription.Medicines;
        }

        /*
         *載入醫療院所資料
         */

        private void LoadHospitalData()
        {
            var hospitalDb = new HospitalDb();
            hospitalDb.GetData();
            ReleaseHospital.ItemsSource = hospitalDb.HospitalsCollection;
            LoadDivisionsData();
        }

        /*
         * 載入科別資料
         */

        private void LoadDivisionsData()
        {
            var divisionDb = new DivisionDb();
            divisionDb.GetData();
            DivisionCombo.ItemsSource = divisionDb.Divisions;
        }

        /*
         *載入給付類別
         */

        private void LoadPaymentCategories()
        {
            var paymentCategoryDb = new PaymentCategroyDb();
            paymentCategoryDb.GetData();
            PaymentCategoryCombo.ItemsSource = paymentCategoryDb.PaymentCategories;
        }

        /*
         *載入部分負擔
         */

        private void LoadCopayments()
        {
            var copaymentDb = new CopaymentDb();
            copaymentDb.GetData();
            CopaymentCombo.ItemsSource = copaymentDb.Copayments;
        }

        /*
         *載入原處方案件類別
         */

        private void LoadTreatmentCases()
        {
            var treatmentCases = new TreatmentCaseDb();
            treatmentCases.GetData();
            TreatmentCaseCombo.ItemsSource = treatmentCases.TreatmentCases;
        }

        /*
         *載入原處方案件類別
         */

        private void LoadAdjustCases()
        {
            var adjustCaseDb = new AdjustCaseDb();
            adjustCaseDb.GetData();
            AdjustCaseCombo.ItemsSource = adjustCaseDb.AdjustCases;
        }

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            IsChanged = true;
        }

        private void SetIsChanged(object sender, EventArgs e)
        {
            SetChanged();
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

            Prescription.Medicines.Add(medicineCodeAuto.SelectedItem as DeclareMedicine);

            medicineCodeAuto.Text = "";
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
                        if (currentRowIndex == Prescription.Medicines.Count - 1)
                        {
                            var autoList = new List<AutoCompleteBox>();
                            NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, "MedicineCodeAuto", ref autoList);
                            NewFunction.FindChildGroup<TextBox>(autoList[currentRowIndex + 1], "Text", ref nextTextBox);
                            nextTextBox[0].Focus();
                        }
                        else
                        {
                            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Dosage", ref nextTextBox);
                            nextTextBox[currentRowIndex + 1].Focus();
                        }
                        return;
                }
                nextTextBox[currentRowIndex].Focus();
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
            return -1;
        }

        private void PrescriptionMedicines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            return;
        }

        private void PrescriptionMedicines_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if(e.NewItem == null)
                return;
        }
    }
}