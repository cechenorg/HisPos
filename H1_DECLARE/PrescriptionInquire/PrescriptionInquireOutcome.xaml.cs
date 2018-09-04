using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionInquireOutcome : Window, INotifyPropertyChanged
    {
        private bool isFirst = true;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private ObservableCollection<TreatmentCase> treatmentCaseCollection;
        public ObservableCollection<TreatmentCase> TreatmentCaseCollection
        {
            get
            {
                return treatmentCaseCollection;
            }
            set
            {
                treatmentCaseCollection = value;
                NotifyPropertyChanged("TreatmentCaseCollection");
            }
        }
        private ObservableCollection<AdjustCase> adjustCaseCollection;
        public ObservableCollection<AdjustCase> AdjustCaseCollection
        {
            get
            {
                return adjustCaseCollection;
            }
            set
            {
                adjustCaseCollection = value;
                NotifyPropertyChanged("AdjustCaseCollection");
            }
        }
        private ObservableCollection<PaymentCategory> paymentCategoryCollection;
        public ObservableCollection<PaymentCategory> PaymentCategoryCollection
        {
            get
            {
                return paymentCategoryCollection;
            }
            set
            {
                paymentCategoryCollection = value;
                NotifyPropertyChanged("PaymentCategoryCollection");
            }
        }
        private ObservableCollection<Copayment> copaymentCollection;
        public ObservableCollection<Copayment> CopaymentCollection
        {
            get
            {
                return copaymentCollection;
            }
            set
            {
                copaymentCollection = value;
                NotifyPropertyChanged("CopaymentCollection");
            }
        }
        private ObservableCollection<Division> divisionCollection;
        public ObservableCollection<Division> DivisionCollection
        {
            get
            {
                return divisionCollection;
            }
            set
            {
                divisionCollection = value;
                NotifyPropertyChanged("DivisionCollection");
            }
        }
        private ObservableCollection<Hospital> hospitalCollection;
        public ObservableCollection<Hospital> HospitalCollection
        {
            get
            {
                return hospitalCollection;
            }
            set
            {
                hospitalCollection = value;
                NotifyPropertyChanged("HospitalCollection");
            }
        }
        private DeclareTrade declareTrade;
        
        public DeclareTrade DeclareTrade
        {
            get
            {
                return declareTrade;
            }
            set
            {
                declareTrade = value;
                NotifyPropertyChanged("DeclareTrade");
            }
        }
        public ObservableCollection<DeclareMedicine> DeclareMedicinesData { get; set; }
        private static DeclareData inquiredPrescription;
        public DeclareData InquiredPrescription
        {
            get
            {
                return inquiredPrescription;
            }
            set
            {
                inquiredPrescription = value;
                NotifyPropertyChanged("InquiredPrescription");
            }
        }
        private ObservableCollection<DeclareMedicine> declareDetails = new ObservableCollection<DeclareMedicine>();
        public ObservableCollection<DeclareMedicine> DeclareDetails
        {
            get
            {
                return declareDetails;
            }
            set
            {
                declareDetails = value;
                NotifyPropertyChanged("DeclareDetails");
            }
        }
        private ObservableCollection<object> Medicines;
        private string DecMasId;
        public PrescriptionInquireOutcome(DeclareData inquired)
        {
            InitializeComponent();
            isFirst = true;
            DataContext = this;
            DeclareTrade = DeclareTradeDb.GetDeclarTradeByMasId(inquired.DecMasId);
            DecMasId = inquired.DecMasId;
            InquiredPrescription = inquired;
            foreach (DeclareMedicine newDeclareDetail in InquiredPrescription.Prescription.Medicines) {
                DeclareMedicine declareDetailClone = (DeclareMedicine)newDeclareDetail.Clone();
                DeclareDetails.Add(declareDetailClone);
            }
            SetPatientData();
            InitData();
            InitDataChanged();
        }

        private void SetPatientData()
        {
            var patient = InquiredPrescription.Prescription.Customer;
            var patientGenderIcon = new BitmapImage(new Uri(@"..\..\Images\Male.png", UriKind.Relative));
            var patientIdIcon = new BitmapImage(new Uri(@"..\..\Images\ID_Card.png", UriKind.Relative));
            var patientBirthIcon = new BitmapImage(new Uri(@"..\..\Images\birthday.png", UriKind.Relative));
            //var patientEmergentPhoneIcon = new BitmapImage(new Uri(@"..\..\Images\Phone.png", UriKind.Relative));
            //PatientName.SetIconSource(patientGenderIcon);
            //PatientId.SetIconSource(patientIdIcon);
            //PatientBirthday.SetIconSource(patientBirthIcon);
            //PatientTel.SetIconSource(patientEmergentPhoneIcon);
            //PatientTel.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.ContactInfo.Tel);
            //PatientBirthday.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.Birthday);
            //PatientId.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.IcNumber);
            //PatientName.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.Name);
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

            ReleasePalace.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.FullName;
            Division.ItemsSource = DivisionCollection;
            Division.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
            CopaymentCode.ItemsSource = CopaymentCollection;
            CopaymentCode.Text = InquiredPrescription.Prescription.Treatment.Copayment.FullName;
            PaymentCategory.ItemsSource = PaymentCategoryCollection;

            PaymentCategory.Text = InquiredPrescription.Prescription.Treatment.PaymentCategory.FullName;
            AdjustCase.ItemsSource = AdjustCaseCollection;
            AdjustCase.Text = InquiredPrescription.Prescription.Treatment.AdjustCase.FullName;
            TreatmentCase.ItemsSource = TreatmentCaseCollection;
            TreatmentCase.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
            loadingWindow.Close();
        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(HospitalCollection.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
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
                    (medicineCodeAuto.ItemsSource as ObservableCollection<object>).Count != 0 && medicineCodeAuto.Text.Length >= 4){
                    medicineCodeAuto.SelectedItem = (medicineCodeAuto.ItemsSource as ObservableCollection<object>)[0];
                }
                else
                    return;
            }
            DeclareMedicine declareMedicine = (DeclareMedicine)(medicineCodeAuto.SelectedItem as DeclareMedicine).Clone();
            int currentRow = GetCurrentRowIndex(sender);

            if (DeclareDetails.Count > 0)
            {
                if (DeclareDetails.Count == currentRow)
                {
                    DeclareDetails.Add(declareMedicine);
                    medicineCodeAuto.Text = "";
                }
                else
                {
                    DeclareDetails[currentRow] = declareMedicine;
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
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (DeclareDetails.Contains(selectedItem))
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";

                PrescriptionSet.SelectedItem = selectedItem;
                return;
            }
            PrescriptionSet.SelectedIndex = DeclareDetails.Count;

        }


        private void DataGridRow_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable) (leaveItem as IDeletable).Source = string.Empty;
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
        private void DeleteDot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClearMedicine(DeclareDetails[PrescriptionSet.SelectedIndex]);
            DataChanged();
            DeclareDetails.RemoveAt(PrescriptionSet.SelectedIndex);
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
        private void Dosage_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionLength = textBox.Text.Length;
            }
        }
        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
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
            var objectName = (sender as Control).Name;
            //按 Enter 下一欄
            //if (e.Key == Key.Enter)
            //{
            //    e.Handled = true;
            //    var nextTextBox = new List<TextBox>();
            //    var nextAutoCompleteBox = new List<AutoCompleteBox>();
            //    var currentRowIndex = GetCurrentRowIndex(sender);

            //    if (currentRowIndex == -1) return;

            //    switch (objectName)
            //    {
            //        case "Dosage":
            //            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Usage", ref nextTextBox);
            //            break;

            //        case "Usage":
            //            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
            //            break;

            //        case "MedicineDays":
            //            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
            //            break;

            //        case "MedicineTotal":
            //            NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Position", ref nextTextBox);
            //            break;

            //        case "Position":
            //            if (!Prescription.Medicines[currentRowIndex].PaySelf)
            //            {
            //                NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
            //                NewFunction.FindChildGroup<TextBox>(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
            //                nextTextBox[0].Focus();
            //                return;
            //            }
            //            else
            //            {
            //                NewFunction.FindChildGroup<TextBox>(PrescriptionMedicines, "Price", ref nextTextBox);
            //                nextTextBox[currentRowIndex].Focus();
            //            }
            //            break;

            //        case "Price":
            //            NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
            //            NewFunction.FindChildGroup<TextBox>(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
            //            nextTextBox[0].Focus();
            //            return;
            //    }
            //    nextTextBox[currentRowIndex].Focus();
            //    nextTextBox[currentRowIndex].CaretIndex = 0;
            //}
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

                NewFunction.FindChildGroup<TextBox>(PrescriptionSet, textBox.Name, ref temp);

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

                NewFunction.FindChildGroup<CheckBox>(PrescriptionSet, checkBox.Name, ref temp);

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
                NewFunction.FindChildGroup<AutoCompleteBox>(PrescriptionSet, autoCompleteBox.Name, ref temp);
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
            ConfirmWindow c;
            InquiredPrescription.Prescription.Medicines = DeclareDetails;
            if (InquiredPrescription.Prescription.CheckPrescriptionData().Count == 0)
            {
                InquiredPrescription.Prescription.Customer.Birthday = DateTimeExtensions.YearFormatTransfer(InquiredPrescription.Prescription.Customer.Birthday);
                var declareData = new DeclareData(InquiredPrescription.Prescription);
                declareData.DecMasId = InquiredPrescription.DecMasId;
                var declareDb = new DeclareDb();
                DeclareTrade declareTrade = new DeclareTrade(InquiredPrescription.Prescription.Customer.Id, MainWindow.CurrentUser.Id, SelfCost.Content.ToString(), Deposit.Content.ToString(), Charge.Content.ToString(), Copayment.Content.ToString(), Pay.Content.ToString(), Change.Content.ToString(), "現金");
                declareDb.UpdateDeclareData(declareData, declareTrade);
                //declareDb.InsertInventoryDb(declareData, "處方登陸");
                m = new MessageWindow("處方修改成功", MessageType.SUCCESS);
                m.Show();
                InitDataChanged();
                PrescriptionOverview prescriptionOverview = new PrescriptionOverview(InquiredPrescription);
                PrescriptionInquireView.Instance.UpdateDataFromOutcome(prescriptionOverview);
            }
            else
            {
                c = new ConfirmWindow("處方資料有誤:" + InquiredPrescription.Prescription.EList + "是否修改或忽略?", MessageType.WARNING);
                //m = new MessageWindow("處方資料有誤:" + Prescription.ErrorMessage + "是否修改或忽略?", MessageType.ERROR);
                //var declareData = new DeclareData(Prescription);
                //var declareDb = new DeclareDb();
                //declareDb.InsertDb(declareData);
                c.ShowDialog();
            }
          
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            InquiredPrescription = PrescriptionDB.GetDeclareDataById(DecMasId);

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
            isFirst = false;
        }
    }
}