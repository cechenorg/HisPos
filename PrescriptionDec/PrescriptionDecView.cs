using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;

namespace His_Pos.PrescriptionDec
{
    /*
     *初始化元件
     */
    public partial class PrescriptionDecView
    {
        
        private ProductDb _productDb = new ProductDb();
        /*
         *初始化UI元件資料
         */
        private void InitializeUiElementData()
        {
            LoadCopayments();
            LoadAdjustCases();
            LoadHospitalData();
            LoadTreatmentCases();
            LoadPaymentCategories();
            InitializeUiElementResource();
        }
        private void InitializeUiElementResource()
        {
            PatientName.SetIconSource(new BitmapImage(new Uri(@"..\Images\Male.png", UriKind.Relative)));
            PatientId.SetIconSource(new BitmapImage(new Uri(@"..\Images\ID_Card.png", UriKind.Relative)));
            PatientBirthday.SetIconSource(new BitmapImage(new Uri(@"..\Images\birthday.png", UriKind.Relative)));
            Deposit.Text = "0";
            SelfCost.Text = "0";
            Copayment.Text = "0";
            TotalPrice.Text = "0";
            CustomPay.Text = "0";
            Change.Content = "0";
        }
        /*
         *載入醫療院所資料
         */
        private void LoadHospitalData()
        {
            var institutions = new Institutions();
            institutions.GetData();
            ReleasePalace.ItemsSource = institutions.InstitutionsCollection;
            LoadDivisionsData();
        }
        /*
         * 載入科別資料
         */
        private void LoadDivisionsData()
        {
            DivisionDb.GetData();
            foreach (var division in DivisionDb.DivisionsList)
            {
                DivisionCombo.Items.Add(division.Id + ". " + division.Name);
            }
        }
        /*
         *載入給付類別
         */
        private void LoadPaymentCategories()
        {
            PaymentCategroyDb.GetData();
            foreach (var paymentCategory in PaymentCategroyDb.PaymentCategoryList)
            {
                PaymentCategoryCombo.Items.Add(paymentCategory.Id + ". " + paymentCategory.Name);
            }
        }
        /*
         *載入部分負擔
         */
        private void LoadCopayments()
        {
            CopaymentDb.GetData();
            foreach (var copayment in CopaymentDb.CopaymentList)
            {
                CopaymentCombo.Items.Add(copayment.Id + ". " + copayment.Name);
            }
        }
        /*
         *載入原處方案件類別
         */
        private void LoadTreatmentCases()
        {
            var treatmentCases = new TreatmentCaseDb();
            treatmentCases.GetData();
            foreach (var treatmentCase in treatmentCases.TreatmentCaseLsit)
            {
                TreatmentCaseCombo.Items.Add(treatmentCase.Id + ". " + treatmentCase.Name);
            }
        }
        /*
         *載入原處方案件類別
         */
        private void LoadAdjustCases()
        {
            AdjustCaseDb.GetData();
            foreach (var adjustCase in AdjustCaseDb.AdjustCaseList)
            {
                AdjustCaseCombo.Items.Add(adjustCase.Id + ". " + adjustCase.Name);
            }
        }
        /*
         * 藥品Autocomplete like by Id
         */
        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineAuto = sender as AutoCompleteBox;
            Debug.Assert(medicineAuto != null, nameof(medicineAuto) + " != null");
            var tmp = MainWindow.MedicineDataTable.Select("HISMED_ID Like '%" + medicineAuto.Text + "%' OR PRO_NAME Like '%" + medicineAuto.Text + "%'");
            MedicineList.Clear();
            foreach (var d in tmp.Take(50))
            {
                var medicine = ProductDb.GetMedicineData(d);
                MedicineList.Add(medicine);
            }
            medicineAuto.ItemsSource = MedicineList;
            medicineAuto.PopulateComplete();
        }
        /*
         * 價格無條件進位
         */
        private int PriceConvert(double price)
        {
            return Convert.ToInt16(Math.Ceiling(price));
        }
        /*
         * 計算單一藥品總價
         */
        private void CountMedicineTotalPrice(Medicine medicine)
        {
            if (medicine.PaySelf)
                medicine.TotalPrice = medicine.Price * medicine.Total;
            medicine.TotalPrice = medicine.HcPrice * medicine.Total;
        }
        /*
         * 計算處方總藥價
         */
        private void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in PrescriptionList)
            {
                if (!medicine.PaySelf)
                    medicinesHcCost += medicine.TotalPrice;
                else
                {
                    medicinesSelfCost += medicine.TotalPrice;
                }
                purchaseCosts += medicine.Cost*medicine.Total;
            }
            SelfCost.Text = PriceConvert(medicinesSelfCost).ToString();//自費金額
            Copayment.Text = CountCopaymentCost(medicinesHcCost);//部分負擔
            PrescriptionProfit.Content = (medicinesHcCost + medicinesSelfCost - purchaseCosts).ToString();//藥品毛利
        }
        /*
         * 藥費部分負擔
         */
        private string CountCopaymentCost(double medicinesHcCost)
        {
            const string free = "0";
            const string max = "200";
            if (medicinesHcCost >= 1001)
                return max;
            var times = medicinesHcCost / 100;
            if (times<=1)
                return free;
            const int grades = 20;
            return (Convert.ToInt16(Math.Floor(times) * grades)).ToString();
        }
        private void CostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox costTextBox && costTextBox.Text.Length > 0 && TotalPrice != null)
            {
                if(Copayment.Text.Equals(string.Empty) || SelfCost.Text.Equals(string.Empty) || Deposit.Text.Equals(string.Empty)) return;
                TotalPrice.Text = (PriceConvert(double.Parse(Copayment.Text) + double.Parse(SelfCost.Text) + double.Parse(Deposit.Text))).ToString();
            }
        }

        private void TraverseVisualTree(Visual myMainWindow)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(myMainWindow);
            for (int i = 0; i < childrenCount; i++)
            {
                var visualChild = (Visual)VisualTreeHelper.GetChild(myMainWindow, i);
                if (visualChild is TextBox)
                {
                    TextBox tb = (TextBox)visualChild;
                    tb.Clear();
                }
                TraverseVisualTree(visualChild);
            }
            PrescriptionList.Clear();
            InitializeUiElementResource();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private bool HistoryFilter(object item)
        {
            
            return false;
        }
        
    }
}
