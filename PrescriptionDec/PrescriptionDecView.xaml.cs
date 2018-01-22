using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.HisApi;
using His_Pos.PrescriptionInquire;

namespace His_Pos.PrescriptionDec
{
    /// <summary>
    /// 處方資料登錄
    /// </summary>
    public partial class PrescriptionDecView
    {
        private StringBuilder _pBuffer = new StringBuilder(100);
        private int _res = -1;
        private Customer _currentCustomer = new Customer();
        private IcCard _icCard = new IcCard();
        private HisApiFunction _hisApiFunction = new HisApiFunction(); 
        private Function _function = new Function();
        private ObservableCollection<Medicine> MedicineList { get; }
        public ObservableCollection<Medicine> PrescriptionList { get; set; }
        private bool _isDropDownClosed = true;
        public PrescriptionDecView()
        {
            InitializeComponent();
            LoadPatentDataFromIcCard();
            InitializeUiElementData();
            StratClock();
        }
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
            PatientEmergentTel.SetIconSource(new BitmapImage(new Uri(@"..\Images\EmergentPhone.png", UriKind.Relative)));
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
            var divisions = new Divisions();
            divisions.GetData();
            foreach (var division in divisions.DivisionsList)
            {
                DivisionCombo.Items.Add(division.Id + ". " + division.Name);
            }
        }
        /*
         *載入給付類別
         */
        private void LoadPaymentCategories()
        {
            var paymentCategroies = new PaymentCategroies();
            paymentCategroies.GetData();
            foreach (var paymentCategory in paymentCategroies.PaymentCategoryList)
            {
                PaymentCategoryCombo.Items.Add(paymentCategory.Id + ". " + paymentCategory.Name);
            }
        }
        /*
         *載入部分負擔
         */
        private void LoadCopayments()
        {
            var copayments = new Copayments();
            copayments.GetData();
            foreach (var copayment in copayments.CopaymentList)
            {
                CopaymentCombo.Items.Add(copayment.Id + ". " + copayment.Name);
            }
        }
        /*
         *載入原處方案件類別
         */
        private void LoadTreatmentCases()
        {
            var treatmentCases = new TreatmentCases();
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
            var adjustCases = new AdjustCases();
            adjustCases.GetData();
            foreach (var adjustCase in adjustCases.AdjustCaseList)
            {
                AdjustCaseCombo.Items.Add(adjustCase.Id + ". " + adjustCase.Name);
            }
        }
        /*
         *自健保卡讀取病患資料
         */
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
            _currentCustomer.Name = "許文章";
            _currentCustomer.Birthday = "37/10/01";
            _currentCustomer.IdNumber = "S88824769A";
            _currentCustomer.Gender = true;
            _icCard.Customer = _currentCustomer;
            _icCard.AvailableTimes = 5;
            _icCard.ICNumber = "900000000720";
            _icCard.IcMarks.LogOutMark = "1";
            _icCard.SendDate = "91/07/25";
            _icCard.ValidityPeriod = "108/01/01";
            _icCard.IcMarks.InsuranceMark = "3";
            PatientName.SetIconLabel(200, 50, _icCard.Customer.Name);
            PatientId.SetIconLabel(200, 50, _icCard.Customer.IdNumber);
            PatientBirthday.SetIconLabel(200, 50, _icCard.Customer.Birthday);
        }
        /*
         *取得病人基本資料
         */
        private void GetBasicData()
        {
            _hisApiFunction.ErrorDetect(_res);
            var iBufferLen = 72;
            _pBuffer.Clear();
            _res = HisApiBase.hisGetBasicData( _pBuffer, ref iBufferLen);
            if (_res != 0) return;
            SetBasicData();
        }
        /*
         * 設定健保卡資料
         */
        private void SetBasicData()
        {
            _icCard.ICNumber = _hisApiFunction.GetIcData(_pBuffer, 0, 12);
            SetCustomerData();
            _icCard.SendDate = _hisApiFunction.GetIcData(_pBuffer, 50, 7);
            _icCard.IcMarks.LogOutMark = _hisApiFunction.GetIcData(_pBuffer, 57, 1);
        }
        /*
         * 設定健保卡資料(顧客姓名.身分證字號.生日.性別)
         */
        private void SetCustomerData()
        {
            _currentCustomer.Name = _hisApiFunction.GetIcData(_pBuffer, 12, 20);
            _currentCustomer.IdNumber = _hisApiFunction.GetIcData(_pBuffer, 32, 10);
            _currentCustomer.Birthday = _function.BirthdayFormatConverter(_hisApiFunction.GetIcData(_pBuffer, 42, 7));
            if (_hisApiFunction.GetIcData(_pBuffer, 49, 1) == "M")
                _currentCustomer.Gender = true;
            _currentCustomer.Gender = false;
            _icCard.Customer = _currentCustomer;
        }
        /*
         *慢箋領藥DataRow Click事件
         */
        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            row.InputBindings.Add(new MouseBinding(InsertChronicDataCommand,
                new MouseGesture() {MouseAction = MouseAction.LeftDoubleClick}));
        }
        /*
         * 慢箋領藥顯示對話框 Command
         */
        private ICommand _insertChronicDataCommand;
        /*
         *_insertChronicDataCommand Implement
         */
        private ICommand InsertChronicDataCommand
        {
            get
            {
                return _insertChronicDataCommand ?? (_insertChronicDataCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => LoadChronicPrescriptionData()
                });
            }
        }
        /*
         *載入慢箋資料
         */
        private void LoadChronicPrescriptionData()
        {
            
        }
        /*
         *處方登錄時間TimerTick
         */
        private void TickEvent(Object sender, EventArgs e)
        {
            PrescriptionClock.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }
        /*
         *啟動處方登錄時間Timer
         */
        private void StratClock()
        {
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += TickEvent;
            timer.Start();
        }
        private void GetPatientDataButtonClick(object sender, RoutedEventArgs e)
        {
            LoadPatentDataFromIcCard();
        }
        private void Combo_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            Debug.Assert(comboBox != null, nameof(comboBox) + " != null");
            comboBox.Background = Brushes.WhiteSmoke;
            comboBox.IsReadOnly = false;
        }
        private void Combo_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            Debug.Assert(comboBox != null, nameof(comboBox) + " != null");
            comboBox.Background = Brushes.Transparent;
            comboBox.Text = comboBox.SelectedItem?.ToString() ?? "";
            comboBox.IsReadOnly = true;
        }
        /*
         * 載入藥品資料至MedicineCodeAuto
         */
        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineAuto = sender as AutoCompleteBox;
            Debug.Assert(medicineAuto != null, nameof(medicineAuto) + " != null");
            var tmp = MainWindow.MedicineDataTable.Select("HISMED_ID Like '" + medicineAuto.Text + "%' OR PRO_NAME Like '" + medicineAuto.Text + "%'");
            InitialMedicineList(tmp);
            medicineAuto.ItemsSource = MedicineList;
            medicineAuto.PopulateComplete();
        }

        private void InitialMedicineList(DataRow[] tmp)
        {
            MedicineList.Clear();
            foreach (var d in tmp.Take(50))
            {
                var medicate = new Medicate
                {
                    Dosage = double.Parse(d["HISMED_UNIT"].ToString()),
                    Form = d["HISMED_FORM"].ToString()
                };
                var medicine = new Medicine
                {
                    Id = d["HISMED_ID"].ToString(),
                    Name = d["PRO_NAME"].ToString(),
                    HcPrice = double.Parse(d["HISMED_PRICE"].ToString()),
                    Cost = double.Parse(d["HISMED_COST"].ToString()),
                    Price = double.Parse(d["HISMED_SELLPRICE"].ToString()),
                    PaySelf = "0",
                    MedicalCategory = medicate
                };
                MedicineList.Add(medicine);
            }
        }

        private void MedicineCodeAuto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var MedicineAuto = sender as AutoCompleteBox;

            Console.WriteLine(e.GetType().Name);

            if (e.Key == Key.Enter)
            {
                _isDropDownClosed = false;
                AddPrescriptionMedicine(MedicineAuto);
            }
        }
        private void AddPrescriptionMedicine(AutoCompleteBox MedicineAuto)
        {
            Debug.Assert(MedicineAuto != null, nameof(MedicineAuto) + " != null");
            var selected = new Medicine();
            foreach (var med in MedicineList)
            {
                if (med.Id.Contains(MedicineAuto.Text))
                {
                    selected = med;
                    if (!MedicineDays.Text.Equals(string.Empty)) selected.MedicalCategory.Days = int.Parse(MedicineDays.Text);
                }
            }
            if (PrescriptionSet.SelectedIndex == -1)
            {
                return;
            }
            if (PrescriptionList.Count <= PrescriptionSet.SelectedIndex)
                PrescriptionList.Add(selected);
            else
            {
                PrescriptionList[PrescriptionSet.SelectedIndex] = selected;
                return;
            }
            MedicineAuto.Text = "";
        }
        private void MedicineCodeAuto_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var MedicineAuto = sender as AutoCompleteBox;
            Debug.Assert(MedicineAuto != null, nameof(MedicineAuto) + " != null");
            if (MedicineAuto.Text.Length == 10 && _isDropDownClosed)
            {
                AddPrescriptionMedicine(MedicineAuto);
                _isDropDownClosed = false;
            }
        }
        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            _isDropDownClosed = true;
        }
    }
}
