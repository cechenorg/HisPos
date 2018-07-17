using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Person;
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
        private ObservableCollection<BitmapImage> _genderIcons = new ObservableCollection<BitmapImage>();
        /*
         *初始化UI元件資料
         */

        private void InitializeUiElementData()
        {
            DataContext = this;
            LoadCopayments();
            LoadAdjustCases();
            LoadHospitalData();
            LoadTreatmentCases();
            LoadPaymentCategories();
            InitializeUiElementResource();
        }

        private void InitializeUiElementResource()
        {
            _genderIcons.Add(new BitmapImage(new Uri(@"..\..\Images\Male.png", UriKind.Relative)));
            _genderIcons.Add(new BitmapImage(new Uri(@"..\..\Images\Female.png", UriKind.Relative)));
            IcPatientId.Source = new BitmapImage(new Uri(@"..\..\Images\ID_Card.png", UriKind.Relative));
            IcPatientBirthday.Source = new BitmapImage(new Uri(@"..\..\Images\birthday.png", UriKind.Relative));
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
            ReleaseInstitution.ItemsSource = HospitalDb.GetData();
            LoadDivisionsData();
        }

        /*
         * 載入科別資料
         */

        private void LoadDivisionsData()
        {
            //foreach (var division in DivisionDb.Divisions)
            //{
            //    DivisionCombo.Items.Add(division.Id + ". " + division.Name);
            //}
        }

        /*
         *載入給付類別
         */

        private void LoadPaymentCategories()
        {
            foreach (var paymentCategory in PaymentCategroyDb.GetData())
            {
                PaymentCategoryCombo.Items.Add(paymentCategory.Id + ". " + paymentCategory.Name);
            }
        }

        /*
         *載入部分負擔
         */

        private void LoadCopayments()
        {
            CopaymentCombo.ItemsSource = CopaymentDb.GetData();
        }

        /*
         *載入原處方案件類別
         */

        private void LoadTreatmentCases()
        {
            foreach (var treatmentCase in TreatmentCaseDb.GetData())
            {
                TreatmentCaseCombo.Items.Add(treatmentCase.Id + ". " + treatmentCase.Name);
            }
        }

        /*
         *載入原處方案件類別
         */

        private void LoadAdjustCases()
        {
            AdjustCaseCombo.ItemsSource = AdjustCaseDb.GetData();
        }

        /*
         * 藥品Autocomplete like by Id
         */

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineAuto = sender as AutoCompleteBox;
            Debug.Assert(medicineAuto != null, nameof(medicineAuto) + " != null");
            //var tmp = MainWindow.MedicineDataTable.Select("PRO_ID Like '%" + medicineAuto.Text + "%' OR PRO_NAME Like '%" + medicineAuto.Text + "%'");
            //MedicineList.Clear();
            //foreach (var d in tmp.Take(50))
            //{
            //    MedicineList.Add(new Medicine(d));
            //}
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

        private void CountMedicineTotalPrice(DeclareMedicine medicine)
        {
            //if (medicine.PaySelf)
            //    medicine.TotalPrice = medicine.Price * medicine.Amount;
            //medicine.TotalPrice = medicine.HcPrice * medicine.Amount;
        }

        /*
         * 計算處方總藥價
         */

        private void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in Prescription.Medicines)
            {
                //if (!medicine.PaySelf)
                //    medicinesHcCost += medicine.TotalPrice;
                //else
                //{
                //    medicinesSelfCost += medicine.TotalPrice;
                //}
                //purchaseCosts += medicine.Cost*medicine.Amount;
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
            if (times <= 1)
                return free;
            const int grades = 20;
            return (Convert.ToInt16(Math.Floor(times) * grades)).ToString();
        }

        private void CostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox costTextBox) || costTextBox.Text.Length <= 0 || TotalPrice == null) return;
            if (Copayment.Text.Equals(string.Empty) || SelfCost.Text.Equals(string.Empty) || Deposit.Text.Equals(string.Empty)) return;
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
            Prescription.Medicines.Clear();
            InitializeUiElementResource();
        }

        /*
         *確認輸入
         */

        private Prescription CheckPrescriptionInfo()
        {
            CheckMedicalNumber();
            return new Prescription(Prescription.Customer, MainWindow.CurrentUser.Pharmacy, GetTreatment(), GetMedcines());
        }

        /*
         * 將藥品加入處方
         */

        private ObservableCollection<DeclareMedicine> GetMedcines()
        {
            ObservableCollection<DeclareMedicine> medicines = new ObservableCollection<DeclareMedicine>();

            if (Prescription.Medicines.Count == 0)
            {
                AddError("請填寫藥品");
                return medicines;
            }

            foreach (var med in Prescription.Medicines)
            {
                if (CheckMedicine(med))
                    medicines.Add(med);
            }

            return medicines;
        }

        private void AddError(string error)
        {
            _errorList.Add(error);
        }

        /*
        * 檢查藥品規則
        */

        private bool CheckMedicine(DeclareMedicine med)
        {
            //*********************************************
            //add check med rule
            //*********************************************

            //AddError("errrrrrrorrrrrr");

            return true;
        }

        /*
        *確認輸入
        */

        private Treatment GetTreatment()
        {
            return new Treatment(GetMedicalInfo(), CheckPaymentCategory(), CheckCopayment(), CheckAdjustCase(), GetTreatDate(), GetAdjustDate(), GetMedicineDays(), MainWindow.CurrentUser.IcNumber);

            //;
            //CheckMedicalNumber();
            //;
            //;
            //CheckChronicTimes();
        }

        private string GetMedicineDays()
        {
            int days = 0;
            foreach (var med in Prescription.Medicines)
            {
                if (int.Parse(med.Days) > days)
                    days = int.Parse(med.Days);
            }
            return days.ToString();
        }

        /*
         * 設定調劑日期
         */

        private DateTime GetAdjustDate()
        {
            return CheckAdjustDate();
        }

        /*
         * 設定就醫日期
         */

        private DateTime GetTreatDate()
        {
            return CheckTreatDate();
        }

        private MedicalInfo GetMedicalInfo()
        {
            return new MedicalInfo(GetReleaseInstitution(), CheckSpecialCode(), CheckDiseaseCodes(), CheckTreatmentCase());
        }

        /*
         *確認調劑案件D1
         */

        private AdjustCase CheckAdjustCase()
        {
            AdjustCase adjustCase = new AdjustCase();
            var adjustCaseStr = AdjustCaseCombo.Text;
            if (adjustCaseStr == string.Empty)
            {
                AddError("請選擇調劑案件");
                return adjustCase;
            }
            adjustCase.Id = adjustCaseStr.Substring(0, 1);
            adjustCase.Name = adjustCaseStr.Substring(2);
            return adjustCase;
        }

        /*
         * 確認釋出院所D21
         */

        private Hospital GetReleaseInstitution()
        {
            Hospital hospital = new Hospital();
            hospital.FullName = ReleaseInstitution.Text;
            if (hospital.FullName == string.Empty)
            {
                AddError("請選擇釋出院所");
                return hospital;
            }
            if (hospital.FullName.StartsWith("N"))
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    AddError("非藥事居家照護(調劑案件:D).協助辦理門診戒菸計畫(調劑案件:5)者，釋出院所不可為\"N\"");
                hospital.Id = hospital.FullName.Substring(0, 1);
            }
            else
            {
                hospital.Id = hospital.FullName.Substring(0, 10);
                hospital.Name = hospital.FullName.Substring(hospital.FullName.IndexOf(" ") + 1);
                hospital.Division = CheckDivision();
                hospital.Doctor = CheckDoctor();
            }
            return hospital;
        }

        /*
         * 判斷調劑案件為藥是居家照護及協助戒菸計畫
         */

        private bool CheckHomeCareAndSmokingCessation()
        {
            if (AdjustCaseCombo.Text == string.Empty)
            {
                AddError("請選擇調劑案件");
                return false;
            }

            return AdjustCaseCombo.Text.StartsWith("D") || AdjustCaseCombo.Text.StartsWith("5");
        }

        /*
         * 確認就醫科別D13
         */

        private Division CheckDivision()
        {
            Division division = new Division();
            division.FullName = DivisionCombo.Text;
            if (DivisionCombo.Text != string.Empty)
            {
                division.Id = division.FullName.Substring(0, 2);
                division.Name = division.FullName.Substring(division.FullName.IndexOf(" ") + 1);
            }
            else
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    AddError("請選擇就醫科別");
            }
            return division;
        }

        /*
         * 確認診治醫生D24
         */

        private MedicalPersonnel CheckDoctor()
        {
            MedicalPersonnel doctor = new MedicalPersonnel();
            if (DoctorId.Text == string.Empty)
            {
                if (CheckHomeCareAndSmokingCessation())
                    return doctor;
                doctor.Id = ReleaseInstitution.Text.Substring(0, 10);
            }
            doctor.Id = DoctorId.Text;
            return doctor;
        }

        /*
         * 確認就醫序號D7
         */

        private void CheckMedicalNumber()
        {
            var medicalNumber = MedicalNumber.Text;
            if (medicalNumber == string.Empty)
            {
                AddError("請填寫就醫序號");
                return;
            }
            if (!ChronicSequence.Text.Equals(string.Empty))
            {
                if (int.Parse(ChronicSequence.Text) > 1)
                    Prescription.Customer.IcCard.MedicalNumber = "IC0" + ChronicSequence.Text;
            }
            if (CheckHomeCareAndSmokingCessation())
                Prescription.Customer.IcCard.MedicalNumber = "N";
            if (!medicalNumber.Contains("IC") && medicalNumber != "N")
            {
                if (Function.IsNumeric(medicalNumber) == false)
                    AddError("就醫序號輸入格式錯誤");
            }
            Prescription.Customer.IcCard.MedicalNumber = medicalNumber;
        }

        /*
         * 確認就醫日期D14
         */

        private DateTime CheckTreatDate()
        {
            var treatDate = new DateTime();
            if (TreatmentDate.Text == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    AddError("請選擇就醫日期");
                    return treatDate;
                }
            }
            treatDate = Convert.ToDateTime(TreatmentDate.SelectedDate);
            return treatDate;
        }

        /*
         * 確認調劑日期D23
         */

        private DateTime CheckAdjustDate()
        {
            var adjustDate = new DateTime();
            if (AdjustDate.Text == string.Empty)
            {
                AddError("請填寫調劑日期,如為藥事居家照護請填寫訪視日期");
                return adjustDate;
            }
            adjustDate = Convert.ToDateTime(AdjustDate.SelectedDate);
            return adjustDate;
        }

        /*
         * 確認國際疾病代碼D8.D9
         */

        private List<DiseaseCode> CheckDiseaseCodes()
        {
            List<DiseaseCode> diseaseCodes = new List<DiseaseCode>();
            var mainDiagnosis = new DiseaseCode();
            if (MainDiagnosis.Text == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    AddError("請填寫主要診斷代碼");
                    return diseaseCodes;
                }
            }
            mainDiagnosis.Id = MainDiagnosis.Text;
            diseaseCodes.Add(mainDiagnosis);
            if (SeconDiagnosis.Text == string.Empty)
                return diseaseCodes;
            var secondDiagnosis = new DiseaseCode { Id = SeconDiagnosis.Text };
            diseaseCodes.Add(secondDiagnosis);
            return diseaseCodes;
        }

        /*
         *確認處方案件D22
         */

        private TreatmentCase CheckTreatmentCase()
        {
            TreatmentCase treatmentCase = new TreatmentCase();
            var treatmentCaseStr = TreatmentCaseCombo.Text;
            if (TreatmentCaseCombo.Text == string.Empty)
            {
                if (CheckHomeCareAndSmokingCessation() == false)
                    AddError("請選擇處方案件");
                return treatmentCase;
            }
            treatmentCase.Id = treatmentCaseStr.Substring(0, 2);
            treatmentCase.Name = treatmentCaseStr.Substring(treatmentCaseStr.IndexOf(" ") + 1);
            return treatmentCase;
        }

        /*
         * 確認給付類別D5
         */

        private PaymentCategory CheckPaymentCategory()
        {
            PaymentCategory payment = new PaymentCategory();
            var paymentCategory = PaymentCategoryCombo.Text;
            if (paymentCategory == string.Empty)
            {
                if (!AdjustCaseCombo.Text.StartsWith("D"))
                {
                    AddError("請選擇給付類別");
                }
                return payment;
            }
            payment.Id = paymentCategory.Substring(0, 1);
            payment.Name = paymentCategory.Substring(2);
            return payment;
        }

        /*
         * 確認部分負擔代碼D15
         */

        private Copayment CheckCopayment()
        {
            Copayment copayment = new Copayment();
            var copaymentStr = CopaymentCombo.Text;
            if (copaymentStr == string.Empty)
            {
                AddError("請選擇部分負擔");
                return copayment;
            }

            copayment.Id = copaymentStr.Substring(0, 3);
            copayment.Name = copaymentStr.Substring(4);
            copayment.Point = Convert.ToInt32(Copayment.Text);
            if (copayment.Id == "903")
            {
                var newBornBirth = DateTimeExtensions.ToUsDate(Prescription.Customer.IcCard.IcMarks.NewbornsData.Birthday);
                var newBornAge = DateTime.Now - newBornBirth;
                CheckNewBornAge(newBornAge);
            }
            return copayment;
        }

        /*
         * 確認新生兒就醫
         */

        private void CheckNewBornAge(TimeSpan newBornAge)
        {
            if (newBornAge.Days <= 60) return;
            AddError("新生兒依附註記方式就醫者新生兒年齡應小於60日");
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

        private SpecialCode CheckSpecialCode()
        {
            SpecialCode special = new SpecialCode();
            var specialCode = SpecialCode.Text;
            if (specialCode != string.Empty)
            {
                special.Id = specialCode.Substring(0, 2);
                special.Name = string.Empty;
            }
            return special;
        }
    }
}