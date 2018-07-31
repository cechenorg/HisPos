﻿using His_Pos.Class;
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    ///     PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        private Prescription _currentPrescription = new Prescription();
        private bool _isChanged;
        private int selfCost;
        private SystemType cusHhistoryFilterCondition = SystemType.ALL;

        public int SelfCost
        {
            get { return selfCost; }
            set
            {
                selfCost = value;
                CountCharge();
                NotifyPropertyChanged("SelfCost");
            }
        }

        private double medProfit;

        public double MedProfit
        {
            get { return medProfit; }
            set
            {
                medProfit = value;
                NotifyPropertyChanged("MedProfit");
            }
        }

        private int copayment;

        public int Copayment
        {
            get { return copayment; }
            set
            {
                copayment = value;
                CurrentPrescription.Treatment.Copayment.Point = copayment;
                CountCharge();
                NotifyPropertyChanged("Copayment");
            }
        }

        private int charge;

        public int Charge
        {
            get { return charge; }
            set
            {
                charge = value;
                NotifyPropertyChanged("Charge");
            }
        }

        private int deposit;

        public int Deposit
        {
            get { return deposit; }
            set
            {
                deposit = value;
                CountCharge();
                NotifyPropertyChanged("Deposit");
            }
        }

        private int pay;

        public int Pay
        {
            get { return pay; }
            set
            {
                pay = value;
                Change = pay - Charge;
                NotifyPropertyChanged("Pay");
            }
        }

        private int change;

        public int Change
        {
            get { return change; }
            set
            {
                change = value;
                NotifyPropertyChanged("Change");
            }
        }

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
        public CustomerHistoryMaster CurrentCustomerHistoryMaster { get; set; }

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            SelfCost = 0;
            Copayment = 0;
            MedProfit = 0;
            Deposit = 0;
            Pay = 0;
            Change = 0;
            GetPrescriptionData();
        }

        public Prescription CurrentPrescription
        {
            get => _currentPrescription;
            set
            {
                _currentPrescription = value;
                NotifyPropertyChanged("CurrentPrescription");
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

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            TreatmentCases = new ObservableCollection<TreatmentCase>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
        }

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageWindow m;
            ConfirmWindow c;
            if (CurrentPrescription.CheckPrescriptionData().Equals(""))
            {
                var declareData = new DeclareData(CurrentPrescription);
                var declareDb = new DeclareDb();
                DeclareTrade declareTrade = new DeclareTrade(CurrentPrescription.Customer.Id, MainWindow.CurrentUser.Id, SelfCost.ToString(), Deposit.ToString(), Charge.ToString(), Copayment.ToString(), Pay.ToString(), Change.ToString(), "現金");
                declareDb.InsertDb(declareData, declareTrade);
                m = new MessageWindow("處方登錄成功", MessageType.SUCCESS);
                m.Show();
            }
            else
            {
                c = new ConfirmWindow("處方資料有誤:" + CurrentPrescription.ErrorMessage + "是否修改或忽略?", MessageType.WARNING);
                //m = new MessageWindow("處方資料有誤:" + Prescription.ErrorMessage + "是否修改或忽略?", MessageType.ERROR);
                //var declareData = new DeclareData(Prescription);
                //var declareDb = new DeclareDb();
                //declareDb.InsertDb(declareData);
                c.ShowDialog();
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (CurrentPrescription.Medicines.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionMedicines.SelectedItem = selectedItem;
                return;
            }
            PrescriptionMedicines.SelectedIndex = CurrentPrescription.Medicines.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearMedicine(CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
            SetChanged();
            CurrentPrescription.Medicines.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }

        private void SetChanged()
        {
            if (IsFirst) return;
            _isChanged = true;
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

            if (CurrentPrescription.Medicines.Count > 0)
            {
                if (CurrentPrescription.Medicines.Count == currentRow)
                {
                    CurrentPrescription.Medicines.Add(declareMedicine);
                    medicineCodeAuto.Text = "";
                }
                else
                {
                    CurrentPrescription.Medicines[currentRow] = declareMedicine;
                }
            }
            else
            {
                CurrentPrescription.Medicines.Add(declareMedicine);
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
                        FindUsagesQuickName(sender);
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!CurrentPrescription.Medicines[currentRowIndex].PaySelf)
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in CurrentPrescription.Medicines)
            {
                if (!medicine.PaySelf)
                    medicinesHcCost += medicine.TotalPrice;
                else
                {
                    medicinesSelfCost += medicine.TotalPrice;
                }
                purchaseCosts += medicine.Cost * medicine.Amount;
            }
            SelfCost = Convert.ToInt16(Math.Ceiling(medicinesSelfCost));//自費金額
            Copayment = CountCopaymentCost(medicinesHcCost);//部分負擔
            MedProfit = (medicinesHcCost + medicinesSelfCost - purchaseCosts);//藥品毛利
        }

        private void CountCharge()
        {
            Charge = Deposit + SelfCost + Copayment;
            Change = pay - Charge;
        }

        /*
        * 藥費部分負擔
        */

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

        private void MedTotalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CountMedicinesCost();
        }

        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }

        private void FindUsagesQuickName(object sender)
        {
            int currentRow = GetCurrentRowIndex(sender);
            TextBox t = sender as TextBox;
            if (!string.IsNullOrEmpty(t.Text))
            {
                CurrentPrescription.Medicines[currentRow].Usage = Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                if (CurrentPrescription.Medicines[currentRow].Usage != null)
                    t.Text = CurrentPrescription.Medicines[currentRow].Usage.Name;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPatentDataFromIcCard();
        }

        private void LoadPatentDataFromIcCard()
        {
            //_res = HisApiBase.csOpenCom(0);
            //if (_res != 0) return;
            //_res = HisApiBase.csVerifySAMDC();
            //if (_res != 0) return;
            //GetBasicData();
            //if (!_currentCustomer.CheckCustomerExist(_currentCustomer.IdNumber))
            //{
            //    _currentCustomer.InsertCustomerData(_currentCustomer);
            //}
            //HisApiBase.csCloseCom();
            CurrentPrescription.Customer.Name = "林連義進";
            CurrentPrescription.Customer.Birthday = "037/10/01";
            CurrentPrescription.Customer.IcNumber = "S88824769A";
            CurrentPrescription.Customer.Gender = true;
            CurrentPrescription.Customer.IcCard = new IcCard("900000000720", new IcMarks("1", "3", new NewbornsData()), "91/07/25", "108/01/01", 5, new IcCardPay(), new IcCardPrediction(), new Pregnant(), new Vaccination(), "");
            CurrentPrescription.Customer.Id = "1";
            PatientName.Text = CurrentPrescription.Customer.Name;
            PatientId.Text = CurrentPrescription.Customer.IcNumber;
            PatientBirthday.Text = CurrentPrescription.Customer.Birthday;
            CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(MainWindow.CurrentUser.Id);
            CusHistoryMaster.ItemsSource = CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            CusHistoryMaster.SelectedIndex = 0;
        }

        private bool CusHistoryFilter(object item)
        {
            if (cusHhistoryFilterCondition == SystemType.ALL) return true;

            if (((CustomerHistoryMaster)item).Type == cusHhistoryFilterCondition)
                return true;
            return false;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            cusHhistoryFilterCondition = (SystemType)Int16.Parse(radioButton.Tag.ToString());

            if (CusHistoryMaster is null) return;
            CusHistoryMaster.Items.Filter = CusHistoryFilter;
        }

        private void Prescription_MouseEnter(object sender, MouseEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            Debug.Assert(dataGrid != null, nameof(dataGrid) + " != null");
            dataGrid.Focus();
        }

        private void Prescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CusHistoryMaster.SelectedItem is null)
            {
                CusHistoryMaster.SelectedIndex = 0;
                return;
            }
            CustomerHistoryMaster selectedMaster = CusHistoryMaster.SelectedItem as CustomerHistoryMaster;
            selectedMaster.HistoryCollection = CurrentCustomerHistoryMaster.getCustomerHistoryDetails(selectedMaster.Type, selectedMaster.CustomerHistoryDetailId);
            SetCusHistoryDetail(CusHistoryMaster.SelectedItem as CustomerHistoryMaster);
        }

        private void SetCusHistoryDetail(CustomerHistoryMaster selectedItem)
        {
            switch (selectedItem.Type)
            {
                case SystemType.HIS:
                    CusHistoryDetailPos.Visibility = Visibility.Collapsed;
                    CusHistoryDetailHis.Visibility = Visibility.Visible;
                    CusHistoryDetailHis.ItemsSource = selectedItem.HistoryCollection;
                    break;
                case SystemType.POS:
                    CusHistoryDetailHis.Visibility = Visibility.Collapsed;
                    CusHistoryDetailPos.Visibility = Visibility.Visible;
                    CusHistoryDetailPos.ItemsSource = selectedItem.HistoryCollection;
                    break;
            }
        }
    }
}