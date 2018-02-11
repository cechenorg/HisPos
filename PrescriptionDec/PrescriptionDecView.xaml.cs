using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.HisApi;
using His_Pos.PrescriptionInquire;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionDec
{
    /// <summary>
    /// 處方資料登錄
    /// </summary>
    public partial class PrescriptionDecView
    {
        private int _res = -1;
        private IcCard _icCard = new IcCard();
        private Customer _currentCustomer = new Customer();
        private Prescription prescription = new Prescription();
        private StringBuilder _pBuffer = new StringBuilder(100);
        private readonly HisApiFunction _hisApiFunction = new HisApiFunction();
        private ObservableCollection<Medicine> MedicineList { get; set; }
        private ObservableCollection<Medicine> PrescriptionList { get; set; }
        public ObservableCollection<CustomerHistory> CustomerHistoryList { get; set; }
        public PrescriptionDecView()
        {
            InitializeComponent();
            DataContext = this;
            InitializeLists();
            InitializeUiElementData();
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
            _currentCustomer.IcNumber = "S88824769A";
            _currentCustomer.Gender = true;
            _icCard.Customer = _currentCustomer;
            _icCard.AvailableTimes = 5;
            _icCard.ICNumber = "900000000720";
            _icCard.IcMarks.LogOutMark = "1";
            _icCard.SendDate = "91/07/25";
            _icCard.ValidityPeriod = "108/01/01";
            _icCard.IcMarks.InsuranceMark = "3";
            PatientName.SetIconLabel(200, 50, _icCard.Customer.Name);
            PatientId.SetIconLabel(200, 50, _icCard.Customer.IcNumber);
            PatientBirthday.SetIconLabel(200, 50, _icCard.Customer.Birthday.ToString());
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
            DateTimeExtensions dateTimeExtensions = new DateTimeExtensions();
            _currentCustomer.Name = _hisApiFunction.GetIcData(_pBuffer, 12, 20);
            _currentCustomer.IcNumber = _hisApiFunction.GetIcData(_pBuffer, 32, 10);
            _currentCustomer.Birthday = dateTimeExtensions.BirthdayFormatConverter(_hisApiFunction.GetIcData(_pBuffer, 42, 7));
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
            Debug.Assert(row != null, nameof(row) + " != null");
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
        private void GetPatientDataButtonClick(object sender, RoutedEventArgs e)
        {
            
            Button button = sender as Button;
            Debug.Assert(button != null, nameof(button) + " != null");
            LoadPatentDataFromIcCard();

            LoadingWindow loadingWindow = new LoadingWindow("Loading Customer Data...");

            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            var sqlParameter = new SqlParameter("CUS_ID", "1");
            parameters.Add(sqlParameter);
            Prescription.ItemsSource = null;
            CustomerHistoryList.Clear();
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[CUSHISTORY]", parameters);
            foreach (DataRow d in table.Rows)
            {
                CustomerHistoryList.Add(new CustomerHistory((int)d["TYPE"], d["DATE"].ToString(), d["HISTORY_ID"].ToString(), d["HISTORY_TITLE"].ToString()));
            }

            Prescription.ItemsSource = CustomerHistoryList;
            Prescription.SelectedItem = CustomerHistoryList[0];

            loadingWindow.backgroundWorker.CancelAsync();

            //List<SqlParameter> customerId = new List<SqlParameter>();
            //customerId.Add(new SqlParameter("IDNUM", "S88824769A"));
            //table = dd.ExecuteProc("[HIS_POS_DB].[GET].[CHRONICINGDATA]", customerId);
            //if (table.Rows.Count > 0)
            //{
            //    ChronicList.ItemsSource = null;
            //    ChronicPrescriptionList.Clear();
            //    button.Command = DialogHost.OpenDialogCommand;
            //    foreach (DataRow d in table.Rows)
            //    {
            //        ChronicPrescription c = new ChronicPrescription(d["CHRONICING_ID"].ToString(), d["HISDECMAS_ID"].ToString(), d["USER_IDNUM"].ToString(), d["HISDIV_ID"].ToString(), d["PRESCRIPTION_DATE"].ToString(), Parse(d["COUNT"].ToString()), d["INS_NAME"].ToString(), d["HISDIV_NAME"].ToString());
            //        ChronicPrescriptionList.Add(c);
            //    }
            //    ChronicList.ItemsSource = ChronicPrescriptionList;
            //    ChronicDialog.DataContext = this;
            //}
            //else
            //    button.Command = null;
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
         *初始化所有List
         */
        private void InitializeLists()
        {
            PrescriptionList = new ObservableCollection<Medicine>();
            MedicineList = new ObservableCollection<Medicine>();
            CustomerHistoryList = new ObservableCollection<CustomerHistory>();
            PrescriptionMedicines.ItemsSource = PrescriptionList;
        }
        private void MedicineCodeAuto_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            AddPrescriptionMedicine(sender as AutoCompleteBox);
        }
        /*
         * 新增處方藥品
         */
        private void AddPrescriptionMedicine(AutoCompleteBox medicineAuto)
        {
            Debug.Assert(medicineAuto != null, nameof(medicineAuto) + " != null");
            if (medicineAuto.SelectedItem is null) return;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex)
                PrescriptionList.Add((Medicine)medicineAuto.SelectedItem);
            else
            {
                PrescriptionList[PrescriptionMedicines.SelectedIndex] = (Medicine)medicineAuto.SelectedItem;
                return;
            }
            medicineAuto.Text = "";
        }
        /*
         * 藥品自費狀態
         */
        private void PaySelf_CheckedEvent(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox selfPay)) return;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex || PrescriptionList[PrescriptionMedicines.SelectedIndex].Total.ToString().Equals(string.Empty)) return;
            var medicine = PrescriptionList[PrescriptionMedicines.SelectedIndex];
            var selfCost = medicine.Price * medicine.Total;
            if (selfPay.IsChecked == true)
            {
                medicine.PaySelf = true;
                medicine.TotalPrice = medicine.Price * medicine.Total;
                SelfCost.Text = (int.Parse(SelfCost.Text) + PriceConvert(selfCost)).ToString();
            }
            else
            {
                medicine.PaySelf = false;
                medicine.TotalPrice = medicine.HcPrice * medicine.Total;
                SelfCost.Text = (int.Parse(SelfCost.Text) - PriceConvert(selfCost)).ToString();
            }
            CountMedicinesCost();
        }
        /*
         * 應付金額改變
         */
        private void MedicineTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PrescriptionMedicines.SelectedIndex >= PrescriptionList.Count || !(sender is TextBox medicineTotal)) return;
            if (medicineTotal.Text.Equals(string.Empty)) return;
            var medicine = PrescriptionList[PrescriptionMedicines.SelectedIndex];
            medicine.Total = double.Parse(medicineTotal.Text);
            CountMedicineTotalPrice(medicine);
            CountMedicinesCost();
        }
        /*
         * 輸入顧客付款金額
         */
        private void CustomPay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Change == null || !(sender is TextBox customPay)) return;
            if (CustomPay.Text.Equals(string.Empty))
                Change.Content = (0 - int.Parse(TotalPrice.Text)).ToString();
            else
            {
                Change.Content = (int.Parse(CustomPay.Text) - int.Parse(TotalPrice.Text)).ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TraverseVisualTree(this);
        }

        private void DeclareButtonClick(object sender, RoutedEventArgs e)
        {
            CheckPrescriptionInfo();
        }
    }
}
