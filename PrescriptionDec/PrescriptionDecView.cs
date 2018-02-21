using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Service;

namespace His_Pos.PrescriptionDec
{
    /*
     *初始化元件
     */
    public partial class PrescriptionDecView
    {
        private int _historyFilterCondition = -1;
        private readonly Function _function = new Function();
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
            ReleaseInstitution.ItemsSource = institutions.InstitutionsCollection;
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
            var tmp = MainWindow.MedicineDataTable.Select("HISMED_ID Like '" + medicineAuto.Text + "%' OR PRO_NAME Like '" + medicineAuto.Text + "%'");
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
            PrescriptionProfit.Content = (medicinesHcCost + medicinesSelfCost - purchaseCosts).ToString(CultureInfo.InvariantCulture);//藥品毛利
        }
        /*
         * 藥費部分負擔
         */
        private static string CountCopaymentCost(double medicinesHcCost)
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
            if (!(sender is TextBox costTextBox) || costTextBox.Text.Length <= 0 || TotalPrice == null) return;
            if(Copayment.Text.Equals(string.Empty) || SelfCost.Text.Equals(string.Empty) || Deposit.Text.Equals(string.Empty)) return;
            TotalPrice.Text = (PriceConvert(double.Parse(Copayment.Text) + double.Parse(SelfCost.Text) + double.Parse(Deposit.Text))).ToString();
        }

        private void TraverseVisualTree(Visual myMainWindow)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(myMainWindow);
            for (var i = 0; i < childrenCount; i++)
            {
                var visualChild = (Visual)VisualTreeHelper.GetChild(myMainWindow, i);
                if (visualChild is TextBox tb)
                {
                    tb.Clear();
                }
                TraverseVisualTree(visualChild);
            }
            PrescriptionList.Clear();
            InitializeUiElementResource();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Prescription == null) return;
            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, nameof(checkBox) + " != null");
            if (checkBox.Tag.ToString().Equals("0") && _historyFilterCondition == 2)
                _historyFilterCondition = 0;
            else if (checkBox.Tag.ToString().Equals("1") && _historyFilterCondition == 2)
                _historyFilterCondition = 1;
            else
            {
                _historyFilterCondition = -1;
                Prescription.Items.Filter = null;
                return;
            }
            Prescription.Items.Filter = HistoryFilter;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Prescription == null) return;
            var checkBox = sender as CheckBox;
            Debug.Assert(checkBox != null, nameof(checkBox) + " != null");
            if (checkBox.Tag.ToString().Equals("0") && _historyFilterCondition == -1)
                _historyFilterCondition = 1;
            else if (checkBox.Tag.ToString().Equals("1") && _historyFilterCondition == -1)
                _historyFilterCondition = 0;
            else
                _historyFilterCondition = 2;
            Prescription.Items.Filter = HistoryFilter;
        }

        private bool HistoryFilter(object item)
        {
            return ((CustomerHistory)item).Type == _historyFilterCondition;
        }

        private void Prescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)e.Source).SelectedIndex == -1)
            {
                TransactionDetail.Visibility = Visibility.Collapsed;
                PrescriptionDetail.Visibility = Visibility.Collapsed;

                GrayArea.Visibility = Visibility.Visible;
                return;
            }
            if (((DataGrid)e.Source).CurrentItem == null) return;
            Console.WriteLine(CustomerHistoryList.IndexOf((CustomerHistory)((DataGrid)e.Source).CurrentItem).ToString());

            var current = CustomerHistoryList.IndexOf((CustomerHistory)((DataGrid)e.Source).CurrentItem);

            if (CustomerHistoryList[current].Type == 0)
            {
                PrescriptionDetail.Visibility = Visibility.Collapsed;
                TransactionDetail.Visibility = Visibility.Visible;

                GrayArea.Visibility = Visibility.Collapsed;
                TransactionDetail.ItemsSource = CustomerHistoryList[current].CustomHistories;
            }
            else
            {
                TransactionDetail.Visibility = Visibility.Collapsed;
                PrescriptionDetail.Visibility = Visibility.Visible;

                GrayArea.Visibility = Visibility.Collapsed;
                PrescriptionDetail.ItemsSource = CustomerHistoryList[current].CustomHistories;
            }
        }
        private void Prescription_MouseEnter(object sender, MouseEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            Debug.Assert(dataGrid != null, nameof(dataGrid) + " != null");
            dataGrid.Focus();
        }
        /*
         *確認輸入
         */
        private void CheckPrescriptionInfo()
        {
            CheckAdjustCase();
            CheckReleaseInstitution();
            CheckDivision();
            CheckDoctor();
            CheckMedicalNumber();
            CheckDatePicker();
            CheckDiseaseCodes();
            CheckTreatmentCase();
            CheckPaymentCategory();
            CheckCopayment();
            CheckChronicTimes();
            CheckSpecialCode();
        }

        /*
         *確認調劑案件D1
         */
        private void CheckAdjustCase()
        {
            var adjustCase = AdjustCaseCombo.Text;
            if (Function.CheckEmptyInput(adjustCase, "請選擇調劑案件"))
            {
                return;
            }
            prescription.Treatment.AdjustCase.Id = adjustCase.Substring(0, 1);
            prescription.Treatment.AdjustCase.Name = adjustCase.Substring(2);
        }
        /*
         * 確認釋出院所D21
         */
        private void CheckReleaseInstitution()
        {
            var releaseInstitution = ReleaseInstitution.Text;
            if(Function.CheckEmptyInput(releaseInstitution, "請選擇釋出院所"))
            {
                return;
            }
            if (ReleaseInstitution.Text.StartsWith("N"))
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    MessageBox.Show("非藥事居家照護(調劑案件:D).協助辦理門診戒菸計畫(調劑案件:5)者，釋出院所不可為\"N\"");
                prescription.Treatment.MedicalInfo.Hospital.Id = ReleaseInstitution.Text.Substring(0, 1);
            }
            else
            {
                prescription.Treatment.MedicalInfo.Hospital.Id = ReleaseInstitution.Text.Substring(0, 10);
                prescription.Treatment.MedicalInfo.Hospital.Name = ReleaseInstitution.Text.Substring(ReleaseInstitution.Text.IndexOf(" ") + 1);
            }
        }
        /*
         * 判斷調劑案件為藥是居家照護及協助戒菸計畫
         */
        private bool CheckHomeCareAndSmokingCessation()
        {
            if (AdjustCaseCombo.Text == string.Empty)
                MessageBox.Show("請選擇調劑案件");
            return AdjustCaseCombo.Text.StartsWith("D") || AdjustCaseCombo.Text.StartsWith("5");
        }
        /*
         * 確認就醫科別D13
         */
        private void CheckDivision()
        {
            var division = DivisionCombo.Text;
            if (DivisionCombo.Text != string.Empty)
            {
                prescription.Treatment.MedicalInfo.Hospital.Division.Id = division.Substring(0, 1);
                prescription.Treatment.MedicalInfo.Hospital.Division.Name =
                    DivisionCombo.Text.Substring(division.IndexOf(" ") + 1);
            }
            else
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    MessageBox.Show("請選擇就醫科別");
            }
        }
        /*
         * 確認診治醫生D24
         */
        private void CheckDoctor()
        {
            if (DoctorId.Text != string.Empty) return;
            if(CheckHomeCareAndSmokingCessation()) return;
            prescription.Treatment.MedicalInfo.Hospital.Doctor.Id = prescription.Treatment.MedicalInfo.Hospital.Id;
        }
        /*
         * 確認就醫序號D7
         */
        private void CheckMedicalNumber()
        {
            var medicalNumber = MedicalNumber.Text;
            if (Function.CheckEmptyInput(medicalNumber, "請填寫就醫序號"))
            {
                return;
            }
            if (int.Parse(ChronicSequence.Text) > 1)
                prescription.IcCard.MedicalNumber = "IC0" + ChronicSequence.Text;
            if (CheckHomeCareAndSmokingCessation())
                prescription.IcCard.MedicalNumber = "N";
            if (!medicalNumber.Contains("IC") && medicalNumber != "N")
            {
                if (Function.IsNumeric(medicalNumber) == false)
                    MessageBox.Show("就醫序號輸入格式錯誤");
            }
            prescription.IcCard.MedicalNumber = medicalNumber;
        }
        /*
         * 確認就醫.調劑日期D14.D23
         */
        private void CheckDatePicker()
        {
            if (TreatmentDate.Text == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    MessageBox.Show("請選擇就醫日期");
                    return;
                }
            }
            prescription.Treatment.TreatmentDate = TreatmentDate.Text;
            if (Function.CheckEmptyInput(AdjustDate.Text, "請填寫調劑日期,如為藥是居家照護請填寫訪視日期"))
            {
                return;
            }
            prescription.Treatment.AdjustDate = AdjustDate.Text;
        }
        /*
         * 確認國際疾病代碼D8.D9
         */
        private void CheckDiseaseCodes()
        {
            var mainDiagnosis = new DiseaseCode();
            if (MainDiagnosis.Text == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    MessageBox.Show("請填寫主要診斷代碼");
                    return;
                }
            }
            mainDiagnosis.Id = MainDiagnosis.Text;
            prescription.Treatment.MedicalInfo.DiseaseCodes.Add(mainDiagnosis);
            if (SeconDiagnosis.Text == string.Empty) return;
            var secondDiagnosis = new DiseaseCode();
            prescription.Treatment.MedicalInfo.DiseaseCodes.Add(secondDiagnosis);
        }
        /*
         *確認處方案件D22
         */
        private void CheckTreatmentCase()
        {
            var treatmentCaseStr = TreatmentCaseCombo.Text;
            if (TreatmentCaseCombo.Text == string.Empty)
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    MessageBox.Show("請選擇處方案件");
                else
                {
                    prescription.Treatment.MedicalInfo.TreatmentCase.Id = string.Empty;
                    prescription.Treatment.MedicalInfo.TreatmentCase.Name = string.Empty;
                }
            }
            prescription.Treatment.MedicalInfo.TreatmentCase.Id = treatmentCaseStr.Substring(0, 2);
            prescription.Treatment.MedicalInfo.TreatmentCase.Name = treatmentCaseStr.Substring(treatmentCaseStr.IndexOf(" ") + 1);
        }
        /*
         * 確認給付類別D5
         */
        private void CheckPaymentCategory()
        {
            var paymentCategory = PaymentCategoryCombo.Text;
            if (paymentCategory == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    MessageBox.Show("請選擇給付類別");
                    return;
                }
                prescription.Treatment.PaymentCategory.Id = string.Empty;
                prescription.Treatment.PaymentCategory.Name = string.Empty;
            }
            prescription.Treatment.PaymentCategory.Id = paymentCategory.Substring(0, 1);
            prescription.Treatment.PaymentCategory.Name = paymentCategory.Substring(2);
        }
        /*
         * 確認部分負擔代碼D15
         */
        private void CheckCopayment()
        {
            var copayment = CopaymentCombo.Text;
            if (Function.CheckEmptyInput(copayment, "請選擇部分負擔"))
                return;
            prescription.Treatment.Copayment.Id = copayment.Substring(0, 3);
            prescription.Treatment.Copayment.Name = copayment.Substring(4);
            prescription.Treatment.Copayment.Point = Convert.ToInt32(Copayment.Text);
            if (prescription.Treatment.Copayment.Id == "903")
            {
                var dateTimeExtensions = new DateTimeExtensions();
                var newBornBirth = dateTimeExtensions.ToUsDate(prescription.IcCard.IcMarks.NewbornsData.Birthday);
                var newBornAge = DateTime.Now - newBornBirth;
                CheckNewBornAge(newBornAge);
            }
        }
        /*
         * 確認新生兒就醫
         */
        private void CheckNewBornAge(TimeSpan newBornAge)
        {
            if (newBornAge.Days <= 60) return;
            MessageBox.Show("新生兒依附註記方式就醫者新生兒年齡應小於60日");
        }

        /*
         * 確認慢箋領藥次數D35.36
         */
        private void CheckChronicTimes()
        {
            if (!AdjustCaseCombo.Text.StartsWith("2")) return;
            if (ChronicSequence.Text == string.Empty || ChronicTotal.Text == string.Empty)
                MessageBox.Show("請填寫領藥次數(調劑序號/可調劑次數)");
        }
        /*
         * 確認原處方服務機構之特定治療項目代號D26
         */
        private void CheckSpecialCode()
        {
            var specialCode = SpecialCode.Text;
            if (specialCode == string.Empty)
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                {
                    MessageBox.Show("請填寫特定治療項目代號");
                    return;
                }
                prescription.Treatment.MedicalInfo.SpecialCode.Id = string.Empty;
                prescription.Treatment.MedicalInfo.SpecialCode.Name = string.Empty;
            }
            else
            {
                prescription.Treatment.MedicalInfo.SpecialCode.Id = specialCode.Substring(0, 2);
                prescription.Treatment.MedicalInfo.SpecialCode.Name = specialCode.Substring(3);
            }
        }
    }
}
