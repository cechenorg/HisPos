using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;

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

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            GetPrescriptionData();
            //LoadPrescriptionData();
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
        }

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;
            LoadPrescriptionData();
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

        private void Usage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var medicineDays = Prescription.Medicines[PrescriptionMedicines.SelectedIndex].Days;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                var usage = textBox.Text;
                CheckUsage(usage, medicineDays);
            }
        }

        public int CheckUsage(string str,int days)
        {
            Regex reg_yWzD = new Regex(@"\d+[Ww]\d+[Dd]");
            Regex reg_MCDxDy = new Regex(@"[Mm][Cc][Dd]\d+[Dd]\d+");
            Regex reg_QxD = new Regex(@"[Qq]\d+[Dd]");
            Regex reg_QxW = new Regex(@"[Qq]\d+[Ww]");
            Regex reg_QxM = new Regex(@"[Qq]\d+[Mm]");
            var count = CheckStableUsage(str, days);
            if (count == 0)
            {
                if (str.StartsWith("QW"))//QW(x,y,z…)：每星期 x，y，z…使用(x,y,z... = 1~7)
                    count = str.Length - 2;
                else if (reg_yWzD.IsMatch(str))//每 y 星期使用 z 天(y、z > 0)
                    count = Case_yWzD(str,days);
                else if (reg_MCDxDy.IsMatch(str))//月經第 x 天至第 y 天使用(x、y > 0,x < y)
                    count = Case_MCDxDy(str);
                else if (reg_QxD.IsMatch(str))//每 x 日 1 次(x >= 2)
                    count = Case_QxD(str, days);
                else if (reg_QxW.IsMatch(str))//每 x 星期 1 次(x > 0)
                    count = Case_QxW(str, days);
                else if (reg_QxM.IsMatch(str))//每 x 月 1 次(x > 0)
                    count = Case_QxM(str, days);
                //else if()
            }
            return count;
        }

        private int Case_QxM(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (30 * x) != 0)
                return -1;
            return days / (30 * x);
        }

        private int Case_QxW(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (7 * x) != 0)
                return -1;
            return days / (7 * x);
        }

        private int Case_QxD(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % x != 0)
                return -1;
            return days / x;
        }

        private int Case_QOD(int days)
        {
            if (days % 2 != 0)
                return -1;
            return days / 2;
        }

        private int CheckStableUsage(string str,int days)
        {
            int count;
            switch (str)
            {
                case "QD"://每日 1 次
                case "QDAM"://每日 1 次上午使用
                case "QDPM"://每日 1 次下午使用
                case "QDHS"://每日 1 次睡前使用
                case "QN"://每晚使用 1 次
                case "HS"://睡前 1 次
                    count = days;
                    break;
                case "BID"://每日 2 次
                case "QAM&HS"://上午使用 1 次且睡前 1 次
                case "QPM&HS"://下午使用 1 次且睡前 1 次
                case "QAM&PM"://每日上下午各使用 1 次
                case "BID&HS"://每日 2 次且睡前 1 次
                    count = days*2;
                    break;
                case "TID"://每日三次
                case "TID&HS"://每日 3 次且睡前 1 次
                    count = days*3;
                    break;
                case "QID":
                    count = days * 4;
                    break;
                case "STAT"://立刻使用
                case "ASORDER"://依照醫師指示使用
                    count = -1;
                    break;
                case "OQD"://隔日使用 1 次
                    count = Case_QOD(days);
                    break;
                case "QW"://每星期 1 次
                    count = Case_QW(days);
                    break;
                case "BIW"://每星期2次
                    count = Case_BIW(days);
                    break;
                default:
                    count = 0;
                    break;
            }
            return count;
        }

        private int Case_BIW(int days)
        {
            if (days % 7 != 0)
                return -1;
            return days / 7 * 2;
        }

        private int Case_QW(int days)
        {
            if (days % 7 != 0)
                return -1;
            return days / 7;
        }

        private int Case_yWzD(string str,int days)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = days / 7 * values[0] * values[1];
            return count;
        }

        private int Case_MCDxDy(string str)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = values[1] - values[0] + 1;
            return count;
        }

        private int[] FindNumberInString(string str)
        {
            int[] values = new int[2];
            int i = 0;
            foreach (Match match in Regex.Matches(str, @"\d+"))
            {
                values[i] = int.Parse(match.Value);
                i++;
            }
            return values;
        }

        private int MatchNumber(string str)
        {
            Match match = Regex.Match(str, @"\d+");
            return int.Parse(match.Value);
        }
    }
}