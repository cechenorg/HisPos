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
        private int _res = -1;
        private int _selectedIindex = -1;
        private IcCard _icCard = new IcCard();
        private Customer _currentCustomer = new Customer();
        private StringBuilder _pBuffer = new StringBuilder(100);
        private readonly Function _function = new Function();
        private readonly HisApiFunction _hisApiFunction = new HisApiFunction();
        private ObservableCollection<Medicine> MedicineList { get; set; }
        public ObservableCollection<Medicine> PrescriptionList { get; set; }
        
        public PrescriptionDecView()
        {
            InitializeComponent();
            DataContext = this;
            SetPrescriptionMedicines();
            LoadPatentDataFromIcCard();
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
        private void SetPrescriptionMedicines()
        {
            PrescriptionList = new ObservableCollection<Medicine>();
            MedicineList = new ObservableCollection<Medicine>();
            PrescriptionMedicines.ItemsSource = PrescriptionList;
        }
        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineAuto = sender as AutoCompleteBox;
            Debug.Assert(medicineAuto != null, nameof(medicineAuto) + " != null");
            var tmp = MainWindow.MedicineDataTable.Select("HISMED_ID Like '" + medicineAuto.Text + "%' OR PRO_NAME Like '" + medicineAuto.Text + "%'");
            MedicineList.Clear();

            foreach (var d in tmp.Take(50))
            {
                var medicine = new Medicine();
                medicine.GetData(d);
                MedicineList.Add(medicine);
            }
            medicineAuto.ItemsSource = MedicineList;
            medicineAuto.PopulateComplete();
        }
        private void MedicineCodeAuto_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            AddPrescriptionMedicine(sender as AutoCompleteBox);
        }
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
    }
}
