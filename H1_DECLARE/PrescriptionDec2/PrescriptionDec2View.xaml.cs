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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private int _selfCost;
        private SystemType _cusHhistoryFilterCondition = SystemType.ALL;

        public int SelfCost
        {
            get => _selfCost;
            set
            {
                _selfCost = value;
                CountCharge();
                NotifyPropertyChanged(nameof(SelfCost));
            }
        }

        private double _medProfit;

        public double MedProfit
        {
            get => _medProfit;
            set
            {
                _medProfit = value;
                NotifyPropertyChanged(nameof(MedProfit));
            }
        }

        private int _copayment;

        public int Copayment
        {
            get => _copayment;
            set
            {
                _copayment = value;
                CurrentPrescription.Treatment.Copayment.Point = _copayment;
                CountCharge();
                NotifyPropertyChanged(nameof(Copayment));
            }
        }

        private int _charge;

        public int Charge
        {
            get => _charge;
            set
            {
                _charge = value;
                NotifyPropertyChanged(nameof(Charge));
            }
        }

        private int _deposit;

        public int Deposit
        {
            get => _deposit;
            set
            {
                _deposit = value;
                CountCharge();
                NotifyPropertyChanged(nameof(Deposit));
            }
        }

        private int _pay;

        public int Pay
        {
            get => _pay;
            set
            {
                _pay = value;
                Change = _pay - Charge;
                NotifyPropertyChanged(nameof(Pay));
            }
        }

        private int _change;

        public int Change
        {
            get => _change;
            set
            {
                _change = value;
                NotifyPropertyChanged(nameof(Change));
            }
        }

        private readonly bool _isFirst = true;
        private ObservableCollection<object> _medicines;
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
            SetDefaultFieldsValue();
            GetPrescriptionData();
        }

        private void SetDefaultFieldsValue()
        {
            DataContext = this;
            SelfCost = 0;
            Copayment = 0;
            MedProfit = 0;
            Deposit = 0;
            Pay = 0;
            Change = 0;
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
                    !((obj as DeclareMedicine)?.Id is null) && (((DeclareMedicine) obj).Id.ToLower().Contains(searchText.ToLower())
                                                                || ((DeclareMedicine) obj).ChiName.ToLower().Contains(searchText.ToLower()) ||
                                                                ((DeclareMedicine) obj).EngName.ToLower().Contains(searchText.ToLower()));
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
                declareDb.InsertInventoryDb(declareData,"處方登陸");
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
            if (_isFirst) return;
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
            _medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = _medicines;
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
                    ((ObservableCollection<object>) medicineCodeAuto.ItemsSource).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                    medicineCodeAuto.SelectedItem = ((ObservableCollection<object>) medicineCodeAuto.ItemsSource)?[0];
                else
                    return;
            }

            var declareMedicine = (DeclareMedicine)((DeclareMedicine)medicineCodeAuto.SelectedItem)?.Clone();
            var currentRow = GetCurrentRowIndex(sender);

            if (CurrentPrescription.Medicines.Count > 0)
            {
                if (CurrentPrescription.Medicines.Count == currentRow)
                {
                    CurrentPrescription.Medicines.Add(declareMedicine);
                    medicineCodeAuto.Text = string.Empty;
                }
                else
                {
                    CurrentPrescription.Medicines[currentRow] = declareMedicine;
                }
            }
            else
            {
                CurrentPrescription.Medicines.Add(declareMedicine);
                medicineCodeAuto.Text = string.Empty;
            }
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
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Usage", ref nextTextBox);
                        break;

                    case "Usage":
                        FindUsagesQuickName(sender);
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!CurrentPrescription.Medicines[currentRowIndex].PaySelf)
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
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                        NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
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

                NewFunction.FindChildGroup(PrescriptionMedicines, objectName, ref thisTextBox);

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
            if (sender is TextBox textBox)
            {
                var temp = new List<TextBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref temp);
                for (var x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(textBox))
                        return x;
                }
            }
            else if (sender is CheckBox checkBox)
            {
                var temp = new List<CheckBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, checkBox.Name, ref temp);
                for (var x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(checkBox))
                        return x;
                }
            }
            else if (sender is AutoCompleteBox autoCompleteBox)
            {
                var temp = new List<AutoCompleteBox>();
                NewFunction.FindChildGroup(PrescriptionMedicines, autoCompleteBox.Name, ref temp);
                for (var x = 0; x < temp.Count; x++)
                {
                    if (temp[x].Equals(autoCompleteBox))
                        return x;
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
            Change = _pay - Charge;
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
            if (sender is TextBox t && string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }

        private void FindUsagesQuickName(object sender)
        {
            var currentRow = GetCurrentRowIndex(sender);
            if (sender is TextBox t && !string.IsNullOrEmpty(t.Text))
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
            CurrentPrescription.Customer.IcNumber = "S18824769A";
            CheckPatientGender();
            CurrentPrescription.Customer.IcCard = new IcCard("900000000720", new IcMarks("1", "3", new NewbornsData()), "91/07/25", "108/01/01", 5, new IcCardPay(), new IcCardPrediction(), new Pregnant(), new Vaccination(), "");
            CurrentPrescription.Customer.Id = "1";
            PatientName.Text = CurrentPrescription.Customer.Name;
            PatientId.Text = CurrentPrescription.Customer.IcNumber;
            PatientBirthday.Text = CurrentPrescription.Customer.Birthday;
            CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(MainWindow.CurrentUser.Id);
            CusHistoryMaster.ItemsSource = CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            CusHistoryMaster.SelectedIndex = 0;
        }

        private void CheckPatientGender()
        {
            if (CurrentPrescription.Customer.IcNumber.IndexOf("1", StringComparison.Ordinal) == 1)
                CurrentPrescription.Customer.Gender = true;
            else if(CurrentPrescription.Customer.IcNumber.IndexOf("2", StringComparison.Ordinal) == 1)
            {
                CurrentPrescription.Customer.Gender = false;
            }
            else
            {
                CurrentPrescription.Customer.Gender = true;
            }
            PatientGender.Content = CurrentPrescription.Customer.Gender ? "男" : "女";
        }

        private bool CusHistoryFilter(object item)
        {
            if (_cusHhistoryFilterCondition == SystemType.ALL) return true;

            if (((CustomerHistoryMaster)item).Type == _cusHhistoryFilterCondition)
                return true;
            return false;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
                _cusHhistoryFilterCondition = (SystemType) short.Parse(radioButton.Tag.ToString());

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

            if (CusHistoryMaster.SelectedItem is CustomerHistoryMaster selectedMaster)
                selectedMaster.HistoryCollection =
                    CurrentCustomerHistoryMaster.getCustomerHistoryDetails(selectedMaster.Type,
                        selectedMaster.CustomerHistoryDetailId);
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