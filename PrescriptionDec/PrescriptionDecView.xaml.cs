using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare;
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
        private SystemType cusHhistoryFilterCondition = SystemType.ALL;
        private Customer _currentCustomer = new Customer();
        private Prescription prescription = new Prescription();
        private StringBuilder _pBuffer = new StringBuilder(100);
        private readonly HisApiFunction _hisApiFunction = new HisApiFunction();
        private ObservableCollection<Medicine> MedicineList { get; set; }
        private ObservableCollection<Medicine> PrescriptionList { get; set; }
        public ObservableCollection<CustomerHistory> CustomerHistoryList { get; set; }

        private CustomerHistory customerHistory;

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
            _currentCustomer.Name = "林連義進";
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
            _currentCustomer.Id = "1";
            PatientName.SetIconLabel(200, 50, _icCard.Customer.Name);
            PatientId.SetIconLabel(200, 50, _icCard.Customer.IcNumber);
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
            Debug.Assert(sender is Button button, nameof(button) + " != null");
            LoadPatentDataFromIcCard();

            LoadingWindow loadingWindow = new LoadingWindow("Loading Customer Data...");

            customerHistory = CustomerHistoryDb.GetDataByCUS_ID(MainWindow.CurrentUser.Id);

            CusHistoryMaster.ItemsSource = customerHistory.CustomerHistoryMasterCollection;
            
            CusHistoryMaster.SelectedIndex = 0;

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

        private void SetCusHistoryDetail(SystemType type, string customerHistoryDetailId)
        {
            switch (type)
            {
                case SystemType.HIS:
                    CusHistoryDetail.Columns[0].Header = "藥名";
                    CusHistoryDetail.Columns[1].Header = "用法";
                    CusHistoryDetail.Columns[2].Header = "用途";
                    CusHistoryDetail.Columns[3].Header = "天數";
                    break;
                case SystemType.POS:
                    CusHistoryDetail.Columns[0].Header = "商品";
                    CusHistoryDetail.Columns[1].Header = "單價";
                    CusHistoryDetail.Columns[2].Header = "數量";
                    CusHistoryDetail.Columns[3].Header = "價格";
                    break;
            }

            CusHistoryDetail.ItemsSource = customerHistory.getCustomerHistoryDetails(type, customerHistoryDetailId);
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
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex || PrescriptionList[PrescriptionMedicines.SelectedIndex].Total.ToString(CultureInfo.InvariantCulture).Equals(string.Empty)) return;
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
            if (Change == null || !(sender is TextBox)) return;
            Change.Content = CustomPay.Text.Equals(string.Empty) ? (0 - int.Parse(TotalPrice.Text)).ToString() : (int.Parse(CustomPay.Text) - int.Parse(TotalPrice.Text)).ToString();
        }
        /*
         * 清除處方資料
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TraverseVisualTree(this);
        }
        /*
         * 處方申報
         */
        private void DeclareButtonClick(object sender, RoutedEventArgs e)
        {
            CheckPrescriptionInfo();
            AddMedicine();
            prescription.Treatment.MedicalPersonId = "A012345678";
            var declareData = new DeclareData(prescription);
            var declareDb = new DeclareDb();
            declareDb.InsertDb(declareData);
            MessageBox.Show("處方登錄成功");
        }
        /*
         * 將藥品加入處方
         */
        private void AddMedicine()
        {
            foreach (var medicine in PrescriptionList)
            {
                prescription.Medicines.Add(medicine);
            }
        }
        /*
         * 設定藥品用量
         */
        private void Dosage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dosage = sender as TextBox;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex && PrescriptionMedicines.SelectedIndex != 0)
            {
                MessageBox.Show("請選擇藥品");
                return;
            }
            if (dosage == null) return;
            const string regexDouble = "^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$";
            const string regexInt = "^[0-9]*[1-9][0-9]*$";
            var matchDouble = Regex.Match(dosage.Text, regexDouble);
            var matchInt = Regex.Match(dosage.Text, regexInt);
            if(matchDouble.Success || matchInt.Success)
                PrescriptionList[PrescriptionMedicines.SelectedIndex].MedicalCategory.Dosage = dosage.Text;
        }
        /*
         * 設定藥品用法
         */
        private void Usage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var usage = sender as TextBox;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex && PrescriptionMedicines.SelectedIndex != 0)
            {
                MessageBox.Show("請選擇藥品");
                return;
            }
            if (usage != null)
                PrescriptionList[PrescriptionMedicines.SelectedIndex].MedicalCategory.Usage = usage.Text;
        }
        /*
         * 設定藥品天數
         */
        private void MedicineDays_TextChanged(object sender, TextChangedEventArgs e)
        {
            var days = sender as TextBox;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex && PrescriptionMedicines.SelectedIndex != 0)
            {
                MessageBox.Show("請選擇藥品");
                return;
            }
            if (days == null) return;
            const string regex = "^[0-9]*[1-9][0-9]*$";
            var match = Regex.Match(days.Text, regex);
            if(match.Success)
                PrescriptionList[PrescriptionMedicines.SelectedIndex].MedicalCategory.Days = Convert.ToInt32(days.Text);
        }
        /*
         * 設定診治醫師代號
         */
        private void DoctorId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox doctorId)) return;
            prescription.Treatment.MedicalInfo.Hospital.Doctor.Id = doctorId.Text;
        }
        /*
         * 設定藥品用藥途徑
         */
        private void PositionAuto_TextChanged(object sender, RoutedEventArgs e)
        {
            var position = sender as AutoCompleteBox;
            if (PrescriptionList.Count <= PrescriptionMedicines.SelectedIndex && PrescriptionMedicines.SelectedIndex != 0)
            {
                MessageBox.Show("請選擇藥品");
                return;
            }
            if (position != null)
                PrescriptionList[PrescriptionMedicines.SelectedIndex].MedicalCategory.Position = position.Text;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            
            cusHhistoryFilterCondition = (SystemType)Int16.Parse(radioButton.Tag.ToString());

            if (CusHistoryMaster is null) return;
            CusHistoryMaster.Items.Filter = CusHistoryFilter;
        }
        private bool CusHistoryFilter(object item)
        {
            if (cusHhistoryFilterCondition == SystemType.ALL) return true;

            if (((CustomerHistoryMaster)item).Type == cusHhistoryFilterCondition)
                return true;
            return false;
        }
        /*
         * 使用者歷史紀錄選擇事件
         */
        private void Prescription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CusHistoryMaster.SelectedItem is null)
            {
                CusHistoryMaster.SelectedIndex = 0;
                return;
            }
            
            CustomerHistoryMaster selectedItem = (CustomerHistoryMaster)CusHistoryMaster.SelectedItem;

            SetCusHistoryDetail(selectedItem.Type, selectedItem.CustomerHistoryDetailId);
        }
        private void Prescription_MouseEnter(object sender, MouseEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            Debug.Assert(dataGrid != null, nameof(dataGrid) + " != null");
            dataGrid.Focus();
        }
    }
}
