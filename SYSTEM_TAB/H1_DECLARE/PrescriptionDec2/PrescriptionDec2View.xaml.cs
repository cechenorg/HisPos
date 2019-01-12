using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using His_Pos.AbstractClass;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.Class.Copayment;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.Class.DiseaseCode;
using His_Pos.Class.Person;
using His_Pos.Class.Product;
using His_Pos.Class.SpecialCode;
using His_Pos.Class.StoreOrder;
using His_Pos.Class.TreatmentCase;
using His_Pos.FunctionWindow;
using His_Pos.HisApi;
using His_Pos.Interface;
using His_Pos.NewClass.Prescription.Position;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using His_Pos.NewClass.Prescription.Usage;
using His_Pos.NewClass.Usage;
using His_Pos.Service;
using His_Pos.Struct.IcData;
using AdjustCase = His_Pos.Class.AdjustCase.AdjustCase;
using Visibility = System.Windows.Visibility;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataGrid = System.Windows.Controls.DataGrid;
using Division = His_Pos.Class.Division.Division;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using PaymentCategory = His_Pos.Class.PaymentCategory.PaymentCategory;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    ///     PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        #region View相關變數

        private string _tempMedicalNumber;

        public string TempMedicalNumber
        {
            get => _tempMedicalNumber;
            set
            {
                _tempMedicalNumber = value;
                NotifyPropertyChanged(nameof(TempMedicalNumber));
            }
        }
        private string _selectedMedId;
        private readonly bool _isFirst = true;
        private bool _isChanged;
        public bool CustomerSelected;
        private bool _isPrescribe;

        public static PrescriptionDec2View Instance;
        public CustomerHistoryMaster CurrentCustomerHistoryMaster { get; set; }
        private SystemType _cusHhistoryFilterCondition = SystemType.ALL;

        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    !((obj as DeclareMedicine)?.Id is null) &&
                    (((DeclareMedicine) obj).Id.ToLower().Contains(searchText.ToLower())
                     || ((DeclareMedicine) obj).ChiName.ToLower().Contains(searchText.ToLower()) ||
                     ((DeclareMedicine) obj).EngName.ToLower().Contains(searchText.ToLower()));
            }
        }

        private IcErrorCodeWindow _icErrorWindow;
        private IcErrorCodeWindow.IcErrorCode SelectedErrorCode { get; set; }
        #endregion

        #region 健保卡作業相關變數
        public bool IsMedicalNumberGet; //是否取得就醫序號
        public int GetMedicalNumberErrorCode = -1;
        public readonly byte[] BasicDataArr = new byte[72];
        public BasicData CusBasicData;
        public SeqNumber Seq; //取得之就醫序號資料
        private readonly List<string> _prescriptionSignatureList = new List<string>(); //處方簽章

        public ObservableCollection<TreatmentDataNoNeedHpc> TreatRecCollection { get; set; } =
            new ObservableCollection<TreatmentDataNoNeedHpc>(); //就醫紀錄

        public ObservableCollection<Customer> CustomerCollection { get; set; } = new ObservableCollection<Customer>();

        #endregion

        #region 處方相關變數

        private static bool _prescriptionCountWarning = true;
        public static string IndexViewDecMasId = string.Empty;
        private Prescription _currentPrescription = new Prescription();
        private DeclareData _currentDeclareData;
        private string _currentDecMasId = string.Empty;
        private string _firstTimeDecMasId = string.Empty;
        private XmlDocument _clinicXml = new XmlDocument();
        private string _clinicDeclareId = string.Empty;
        private bool _isNotReceiveCopayment = true;

        public bool IsNotReceiveCopayment
        {
            get => _isNotReceiveCopayment;
            set
            {
                _isNotReceiveCopayment = value;
                NotifyPropertyChanged(nameof(IsNotReceiveCopayment));
            }
        }
        private int _prescriptionCount;

        public int PrescriptionCount
        {
            get => _prescriptionCount;
            set
            {
                _prescriptionCount = value;
                if (_prescriptionCount >= 80 && _prescriptionCountWarning)
                {
                    YesNoMessageWindow m = new YesNoMessageWindow("調劑處方已超過80件，是否繼續提醒?","調劑上限提醒");
                    _prescriptionCountWarning = (bool) m.ShowDialog();
                }
                NotifyPropertyChanged(nameof(PrescriptionCount));
            }
        }

        public bool IsSend;

        public ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData> PrescriptionSendData =
            new ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData>();

        public Prescription CurrentPrescription
        {
            get => _currentPrescription;
            set
            {
                _currentPrescription = value;
                NotifyPropertyChanged(nameof(CurrentPrescription));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo.Hospital));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo.Hospital.Division));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo.Hospital.Doctor));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.TreatmentDate));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.AdjustDate));
                NotifyPropertyChanged(nameof(CurrentPrescription.Customer));
                NotifyPropertyChanged(nameof(CurrentPrescription.ChronicSequence));
                NotifyPropertyChanged(nameof(CurrentPrescription.ChronicTotal));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.AdjustCase));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.Copayment));
                NotifyPropertyChanged(nameof(CurrentPrescription.Treatment.PaymentCategory));
                NotifyPropertyChanged(nameof(CurrentPrescription.MedicinePoint));
                NotifyPropertyChanged(nameof(CurrentPrescription.CopaymentPoint));
                NotifyPropertyChanged(nameof(CurrentPrescription.Pharmacy.MedicalPersonnel));
                NotifyPropertyChanged(nameof(CurrentPrescription.Pharmacy));
            }
        }

        #endregion

        #region 調劑結帳相關變數

        private int _selfCost;

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
                Pay = _charge;
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

        #endregion

        #region ItemsSourceCollection

        private ObservableCollection<object> _medicines;
        public Institutions Hospitals { get; set; }
        public Divisions Divisions { get; set; }
        public PrescriptionCases PrescriptionCases { get; set; }
        public PaymentCategories PaymentCategories { get; set; }
        public Copayments Copayments { get; set; }
        public AdjustCases AdjustCases { get; set; }
        public ObservableCollection<MedicalPersonnel> MedicalPersonnels { get; set; }
        public Usages Usages { get; set; }
        public ObservableCollection<Product> DeclareMedicines { get; set; }
        public SpecialTreats SpecialCodes { get; set; }
        public ObservableCollection<Position> Positions { get; set; }

        #endregion

        public PrescriptionDec2View()
        {
            InitializeComponent();
            Instance = this;
            GetPrescriptionData();
            SetDefaultFieldsValue();
            if (string.IsNullOrEmpty(IndexViewDecMasId)) return;
            SetValueByDecMasId(IndexViewDecMasId);
            IndexViewDecMasId = string.Empty;
        }

        #region InitializeFunction

        private void SetDefaultFieldsValue()
        {
            SelfCost = 0;
            Copayment = 0;
            MedProfit = 0;
            Deposit = 0;
            Pay = 0;
            Change = 0;
            IsNotReceiveCopayment = false;
        }

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<Product>();
            PrescriptionCases = new PrescriptionCases();
            ///CurrentPrescription.Pharmacy = MainWindow.CurrentPharmacy.DeepCloneViaJson();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(Instance);
            loadingWindow.Show();
        }

        #endregion

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageWindow m;
            if (!IsMedicalNumberGet)
            {
                _icErrorWindow =
                    new IcErrorCodeWindow(false, Enum.GetName(typeof(ErrorCode), GetMedicalNumberErrorCode));
                _icErrorWindow.ShowDialog();
                if (_icErrorWindow.SelectedItem is null || string.IsNullOrEmpty(_icErrorWindow.SelectedItem.Id))
                {
                    var y = new YesNoMessageWindow("尚未選擇異常代碼，是否自費押金", "是否押金");
                    CurrentPrescription.IsDeposit = (bool)y.ShowDialog();
                    if(!CurrentPrescription.IsDeposit)
                        return;
                }
                if (!CurrentPrescription.IsDeposit)
                {
                    SelectedErrorCode = new IcErrorCodeWindow.IcErrorCode();
                    SelectedErrorCode = _icErrorWindow.SelectedItem;
                }
            }
            if (TreatmentDate.Text.Contains(" "))
            {
                MessageWindow.ShowMessage("就醫日期錯誤", MessageType.ERROR);
                
                return;
            }

            if (AdjustDate.Text.Contains(" "))
            {
                MessageWindow.ShowMessage("調劑日期錯誤", MessageType.ERROR);
                
                return;
            }

            IsSend = false;
            ErrorMssageWindow err;
            CurrentPrescription.EList.Error = new List<Error>();
            CurrentPrescription.Customer.IcCard.MedicalNumber = TempMedicalNumber;
            CurrentPrescription.EList.Error = CurrentPrescription.CheckPrescriptionData();

            var medDays = 0;
            foreach (var med in CurrentPrescription.Medicines)
            {
                if (!(med is DeclareMedicine declare)) continue;
                if (string.IsNullOrEmpty(((IProductDeclare) declare).Days))
                {
                    MessageWindow.ShowMessage(declare.Id + "的給藥日份不可為空", MessageType.ERROR);
                    
                    return;
                }

                if (int.Parse(((IProductDeclare) declare).Days) > medDays)
                    medDays = int.Parse(((IProductDeclare) declare).Days);
            }

            CurrentPrescription.Treatment.MedicineDays = medDays.ToString();

            if (CurrentPrescription.EList.Error.Count == 0)
            {
                InsertPrescription("Adjustment");
            }
            else
            {
                var errorMessage = "";
                foreach (var error in CurrentPrescription.EList.Error)
                {
                    errorMessage += error.Content + "\n";
                }

                err = new ErrorMssageWindow(errorMessage);
                err.Show();
                if (err.ErrorLogIn)
                {
                    InsertPrescription("Adjustment");
                }
            }
        }

        private void ButtonDeclareRegister_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentDate.Text.Contains(" "))
            {
                MessageWindow.ShowMessage("就醫日期錯誤", MessageType.ERROR);
                
                return;
            }

            if (AdjustDate.Text.Contains(" "))
            {
                MessageWindow.ShowMessage("調劑日期錯誤", MessageType.ERROR);
                return;
            }

            IsSend = false;
            ErrorMssageWindow err;
            CurrentPrescription.EList.Error = new List<Error>();
            CurrentPrescription.Customer.IcCard.MedicalNumber = TempMedicalNumber;
            CurrentPrescription.EList.Error = CurrentPrescription.CheckPrescriptionData();
            
            if (CurrentPrescription.Treatment.AdjustCase.Id.Equals("D") ||
                CurrentPrescription.Treatment.AdjustCase.Id.Equals("5"))
            {
                CurrentPrescription.Treatment.MedicineDays = "0";
            }
            else
            {
                var medDays = 0;
                foreach (var med in CurrentPrescription.Medicines)
                {
                    if (!(med is DeclareMedicine declare)) continue;
                    if (string.IsNullOrEmpty(((IProductDeclare)declare).Days))
                    {
                        MessageWindow.ShowMessage(declare.Id + "的給藥日份不可為空", MessageType.ERROR);
                        return;
                    }

                    if (int.Parse(((IProductDeclare)declare).Days) > medDays)
                        medDays = int.Parse(((IProductDeclare)declare).Days);
                }

                CurrentPrescription.Treatment.MedicineDays = medDays.ToString();
            }

            if (CurrentPrescription.EList.Error.Count == 0)
            {
                InsertPrescription("Register");
            }
            else
            {
                var errorMessage = "";
                foreach (var error in CurrentPrescription.EList.Error)
                {
                    errorMessage += error.Content + "\n";
                }

                err = new ErrorMssageWindow(errorMessage);
                err.Show();
                if (err.ErrorLogIn)
                {
                    InsertPrescription("Register");
                }
            }
        }
        private void SetEntryType(ref string medEntryName, ref string medServiceName, ref string medCopayName,ref string medPaySelf) {
            switch (CurrentPrescription.Treatment.MedicalInfo.Hospital.Id)
            {
                case "3532072408":
                case "3532071956":
                case "3531066077":
                case "3532082253":
                case "3532016964":
                    medEntryName = "合作診所調劑耗用";
                    medServiceName = "合作診所藥服費";
                    medCopayName = IsNotReceiveCopayment ? "合作診所應付負擔" : "合作診所部分負擔";
                    medPaySelf = "合作診所自費";
                    break;
                default:
                    medEntryName = "調劑耗用";
                    medServiceName = "藥服費";
                    medCopayName = "部分負擔";
                    medPaySelf = "自費";
                    break;
            }
        }

        private void InsertPrescription(string type)
        {
            //MedHistoryWebWindow medHistoryWebWindow = new MedHistoryWebWindow();
            //medHistoryWebWindow.ShowDialog();
            MessageWindow m;
            CurrentPrescription.Customer.Id = CurrentPrescription.Customer.Id;

            _currentDeclareData = new DeclareData(CurrentPrescription); 
            var declareDb = new DeclareDb();
            string medEntryName = string.Empty;
            string medServiceName = string.Empty;
            string medCopayName = string.Empty;
            string medPaySelf = string.Empty;
            SetEntryType( ref medEntryName, ref medServiceName, ref medCopayName,ref medPaySelf);
            var buckleCondition = type == "Adjustment" && medEntryName == "調劑耗用" && CurrentPrescription.Treatment.AdjustDate.Date == DateTime.Now.Date; //扣庫條件
            var declareTrade = new DeclareTrade(MainWindow.CurrentUser.Id.ToString(), SelfCost.ToString(), Deposit.ToString(), Charge.ToString(), Copayment.ToString(), Pay.ToString(), Change.ToString(), "現金", CurrentPrescription.Customer.Id);
            int caseType; 
            if (string.IsNullOrEmpty(CurrentPrescription.ChronicTotal) &&
                string.IsNullOrEmpty(CurrentPrescription.ChronicSequence) && string.IsNullOrEmpty(_currentDecMasId) &&
                CurrentPrescription.Treatment.AdjustDate.Date == DateTime.Now.Date)
                caseType = 1;//一般處方調劑
            else if (!string.IsNullOrEmpty(CurrentPrescription.ChronicTotal) &&
                     !string.IsNullOrEmpty(CurrentPrescription.ChronicSequence) &&
                     !string.IsNullOrEmpty(_currentDecMasId))
                caseType = 2;//帶出預約慢箋
            else if (!string.IsNullOrEmpty(CurrentPrescription.ChronicTotal) &&
                     !string.IsNullOrEmpty(CurrentPrescription.ChronicSequence) &&
                     string.IsNullOrEmpty(_currentDecMasId))
                caseType = 3;//第一次慢箋
            else
                caseType = -1; //Fail

            switch (caseType)
            {
                case 1: //一般處方調劑
                    _firstTimeDecMasId = "";/// declareDb.InsertDeclareData(_currentDeclareData);
                                            /// ProductDb.InsertCashFow(medCopayName, declareTrade.CopayMent, "DecMasId", _firstTimeDecMasId);
                                            /// ProductDb.InsertCashFow(medPaySelf, declareTrade.PaySelf, "DecMasId", _firstTimeDecMasId);
                    if (!CurrentPrescription.IsGetIcCard && CurrentPrescription.IsDeposit)
                        ;/// ProductDb.InsertCashFow("押金", declareTrade.Deposit, "DecMasId", _firstTimeDecMasId);
                    ///ProductDb.InsertCashFow(medServiceName, _currentDeclareData.D38MedicalServicePoint.ToString(), "DecMasId", _firstTimeDecMasId);
                    if (buckleCondition)
                    {
                        var medTotalPrice = 0.00;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                            switch (med)
                            {
                                case DeclareMedicine declare:
                                    medTotalPrice += 0;/// double.Parse(ProductDb.GetBucklePrice(declare.Id,((IProductDeclare) declare).Amount.ToString()));
                                    break;
                                case PrescriptionOTC otc:
                                    medTotalPrice += 0;/// double.Parse(ProductDb.GetBucklePrice(med.Id, ((IProductDeclare) otc).Amount.ToString()));
                                    break;
                            }
                        }

                        ///ProductDb.InsertEntry(medEntryName, (medTotalPrice * -1).ToString(), "DecMasId", _firstTimeDecMasId);
                        ///declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _firstTimeDecMasId); //庫存扣庫
                      
                        ///declareDb.InsertDeclareTrade(_firstTimeDecMasId,declareTrade);//Insert Trade
                        var medorder = CurrentPrescription.Medicines.Where(med => ((IProductDeclare)med).Amount > ((IProductDeclare)med).Stock.Inventory).ToList();
                        if (medorder.Count > 0) {
                            bool isCheck = false;
                            YesNoMessageWindow medorderCheck = new YesNoMessageWindow("發現負庫存是否直接向\r\n調劑中心訂貨?", "負庫訂單確認");
                            isCheck = (bool)medorderCheck.ShowDialog();
                            if (isCheck)
                                ;/// StoreOrderDb.AddDeclareOrder(medorder); //缺藥直接訂貨
                        }
                           
                    }
                    if (!CurrentPrescription.IsGetIcCard && CurrentPrescription.IsDeposit)
                        ;///declareDb.InsertDeclareRegister(_firstTimeDecMasId, false, true, CurrentPrescription.IsGetIcCard, false, false, true); //押金
                    else
                    {
                        ;/// declareDb.InsertDeclareRegister(_firstTimeDecMasId, false, true, CurrentPrescription.IsGetIcCard, true, false, true); //處方登錄
                    }
                    ;/// declareDb.UpdateDeclareFile(_currentDeclareData);
                    break;
                case 2: //帶出預約慢箋
                    if (IsSendToServer.IsChecked != null && (bool) IsSendToServer.IsChecked) //選擇傳送藥健康
                    {
                        var chronicSendToServerWindow =
                            new ChronicSendToServerWindow(CurrentPrescription.Medicines, _currentDecMasId);
                        chronicSendToServerWindow.ShowDialog();
                        if (!IsSend) return;
                    }

                    if (IsSend)
                    {
                        //確定傳送
                        var storId = "";/// StoreOrderDb.SaveOrderDeclareData(_currentDecMasId, PrescriptionSendData);
                        ///StoreOrderDb.SendDeclareOrderToSingde(_currentDecMasId, storId, _currentDeclareData,declareTrade, PrescriptionSendData); //送到singde
                    }

                    if (buckleCondition)
                    {
                        ///ProductDb.InsertEntry(medCopayName, declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                        ///ProductDb.InsertCashFow(medPaySelf, declareTrade.PaySelf, "DecMasId", _currentDecMasId);
                        if (CurrentPrescription.IsDeposit)
                            ;/// ProductDb.InsertCashFow("押金", declareTrade.Deposit, "DecMasId", _currentDecMasId);
                        ///ProductDb.InsertCashFow(medServiceName, _currentDeclareData.D38MedicalServicePoint.ToString(), DecMasId", _currentDecMasId);
                        var medTotalPrice = 0.00;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                          ///  medTotalPrice += double.Parse(ProductDb.GetBucklePrice(med.Id,((IProductDeclare) (DeclareMedicine) med).Amount.ToString()));
                        }
                        ///declareDb.AdjustChronicById(_currentDecMasId);
                        ///ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _currentDecMasId);
                        ///declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _currentDecMasId); //庫存扣庫
                        ///declareDb.InsertDeclareTrade(_currentDecMasId, declareTrade);//Insert Trade
                        ///declareDb.UpdateDeclareFile(_currentDeclareData);
                        var medorder = CurrentPrescription.Medicines.Where(med => ((IProductDeclare)med).Amount > ((IProductDeclare)med).Stock.Inventory).ToList();
                        if (medorder.Count > 0)
                        {
                            bool isCheck = false;
                            YesNoMessageWindow medorderCheck = new YesNoMessageWindow("發現負庫存是否直接向\r\n調劑中心訂貨?", "負庫訂單確認");
                            isCheck = (bool)medorderCheck.ShowDialog();
                            if (isCheck)
                                ;/// StoreOrderDb.AddDeclareOrder(medorder); //缺藥直接訂貨
                        }
                    }
                    if (!CurrentPrescription.IsGetIcCard && CurrentPrescription.IsDeposit)
                        ;/// declareDb.InsertDeclareRegister(_firstTimeDecMasId, false, true, CurrentPrescription.IsGetIcCard, false, false, true); //押金
                    else
                    {
                        ///declareDb.InsertDeclareRegister(_currentDecMasId, IsSend, true, CurrentPrescription.IsGetIcCard, true, false, true); //處方登錄
                    }
                    _currentDeclareData.DecMasId = _currentDecMasId;
                    ///declareDb.UpdateDeclareData(_currentDeclareData); //更新慢箋
                    ///ChronicDb.UpdateChronicData(_currentDecMasId); //重算預約慢箋  
                    break;
                case 3: //第一次慢箋
                    YesNoMessageWindow prescriptionAdjust;
                    var isAdjust = false;
                    if (CurrentPrescription.Treatment.AdjustDate.Date == DateTime.Today.Date) {
                         prescriptionAdjust = new YesNoMessageWindow("是否調劑處方?", "處方調劑確認");
                         isAdjust = (bool)prescriptionAdjust.ShowDialog();
                    }
                    if (isAdjust)
                    {
                        type = "Adjustment";
                        if (!IsMedicalNumberGet)
                        {
                            _icErrorWindow = new IcErrorCodeWindow(false, Enum.GetName(typeof(ErrorCode), GetMedicalNumberErrorCode));
                            _icErrorWindow.ShowDialog();
                            if (_icErrorWindow.SelectedItem is null || string.IsNullOrEmpty(_icErrorWindow.SelectedItem.Id))
                            {
                                var y = new YesNoMessageWindow("尚未選擇異常代碼，是否自費押金", "是否押金");
                                CurrentPrescription.IsDeposit = (bool)y.ShowDialog();
                                if (!CurrentPrescription.IsDeposit)
                                    return;
                            }
                            if (!CurrentPrescription.IsDeposit)
                            {
                                SelectedErrorCode = new IcErrorCodeWindow.IcErrorCode();
                                SelectedErrorCode = _icErrorWindow.SelectedItem;
                            }
                        }
                        if (string.IsNullOrEmpty(_firstTimeDecMasId))
                            ;/// _firstTimeDecMasId = declareDb.InsertDeclareData(_currentDeclareData);
                        //ProductDb.InsertEntry(medCopayName, declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                        ///ProductDb.InsertCashFow(medPaySelf, declareTrade.PaySelf, "DecMasId", _firstTimeDecMasId);
                        if (!CurrentPrescription.IsGetIcCard && CurrentPrescription.IsDeposit)
                        {
                            ///declareDb.InsertDeclareRegister(_firstTimeDecMasId, false, true, CurrentPrescription.IsGetIcCard, false, false, true); //押金
                            ///ProductDb.InsertCashFow("押金", declareTrade.Deposit, "DecMasId", _firstTimeDecMasId);
                        }
                        else
                        {
                            ///declareDb.InsertDeclareRegister(_firstTimeDecMasId, IsSend, true, CurrentPrescription.IsGetIcCard, true, false, true); //處方登錄
                        }
                        if(!CurrentPrescription.IsDeposit)
                            ///ProductDb.InsertCashFow(medServiceName, _currentDeclareData.D38MedicalServicePoint.ToString(),"DecMasId", _firstTimeDecMasId);
                        
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                           /// medTotalPrice += double.Parse(ProductDb.GetBucklePrice(med.Id,((IProductDeclare)(DeclareMedicine)med).Amount.ToString()));
                        }
                       ///declareDb.AdjustChronicById(_firstTimeDecMasId);
                       ///ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _firstTimeDecMasId);
                       ///declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _firstTimeDecMasId); //庫存扣庫
                       ///declareDb.InsertDeclareTrade(_firstTimeDecMasId, declareTrade);//Insert Trade
                       ///declareDb.UpdateDeclareFile(_currentDeclareData);
                        var medorder = CurrentPrescription.Medicines.Where(med => ((IProductDeclare)med).Amount > ((IProductDeclare)med).Stock.Inventory).ToList();
                        if (medorder.Count > 0)
                        {
                            bool isCheck = false;
                            YesNoMessageWindow medorderCheck = new YesNoMessageWindow("發現負庫存是否直接向\r\n調劑中心訂貨?", "負庫訂單確認");
                            isCheck = (bool)medorderCheck.ShowDialog();
                            if (isCheck)
                                ;/// StoreOrderDb.AddDeclareOrder(medorder); //缺藥直接訂貨
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_firstTimeDecMasId))
                            ;/// _firstTimeDecMasId = declareDb.InsertDeclareData(_currentDeclareData);

                        ///declareDb.InsertDeclareRegister(_firstTimeDecMasId, IsSend, true, false, false, false, false); //處方登錄
                    }
                    if (IsSendToServer.IsChecked != null && (bool) IsSendToServer.IsChecked)
                    {
                        var chronicSendToServerWindow =
                            new ChronicSendToServerWindow(CurrentPrescription.Medicines, _firstTimeDecMasId);
                        chronicSendToServerWindow.ShowDialog();
                        if (!IsSend) return;
                    }

                    if (IsSend)
                    {
                        ///var storId = StoreOrderDb.SaveOrderDeclareData(_firstTimeDecMasId, PrescriptionSendData); //出進貨單
                        ///StoreOrderDb.SendDeclareOrderToSingde(_firstTimeDecMasId, storId, _currentDeclareData,declareTrade, PrescriptionSendData); //送到singde
                    }

                    //declareDb.CheckPredictChronicExist(_firstTimeDecMasId); //刪除同人同科別預約慢箋

                    var intDecMasId = Convert.ToInt32(_firstTimeDecMasId);
                    for (var i = Convert.ToInt32(CurrentPrescription.ChronicSequence) + 1;
                        i <= Convert.ToInt32(CurrentPrescription.ChronicTotal);
                        i++)
                    {
                        ///declareDb.SetSameGroupChronic(intDecMasId.ToString(), i.ToString()); //預約慢箋 
                        intDecMasId++;
                        ///declareDb.InsertDeclareRegister(intDecMasId.ToString(), false, false, false, false, false,false); //處方登錄
                    }

                    ChronicRegisterWindow chronicRegisterWindow = new ChronicRegisterWindow(_firstTimeDecMasId);
                    
                    break;
                case -1:
                    MessageWindow.ShowMessage("處方登錄失敗 請確認調劑日期是否正確", MessageType.ERROR);
                    
                    return;
            }

            if (type.Equals("Adjustment"))
            {
                var medBagPrint = new YesNoMessageWindow("是否列印藥袋", "列印確認");
                var print = (bool)medBagPrint.ShowDialog();
                ///CustomerDb.UpdateCustomerBasicDataByCusId(CurrentPrescription.Customer);
                if (print)
                    NewFunction.PrintMedBag(CurrentPrescription, _currentDeclareData, CurrentPrescription.MedicinePoint, SelfCost, Pay, "登錄", Charge, Instance);
                if (IsMedicalNumberGet)
                {
                    var loading = new LoadingWindow();
                    loading.LoginIcData(Instance);
                    loading.ShowDialog();
                    MessageWindow.ShowMessage("處方登錄成功", MessageType.SUCCESS);
                    
                }
                else
                {
                    var loading = new LoadingWindow();
                    loading.LoginIcData(Instance, SelectedErrorCode);
                    loading.ShowDialog();
                    MessageWindow.ShowMessage("處方登錄成功", MessageType.SUCCESS);
                    
                }
            }
            else
            {
                MessageWindow.ShowMessage("處方登錄成功", MessageType.SUCCESS);
                
            }

            if (MainWindow.CurrentPharmacy.MedicalPersonnelCollection != null && MainWindow.CurrentPharmacy.MedicalPersonnelCollection.Count > 0)
            {
                foreach (var medicalPerson in MainWindow.CurrentPharmacy.MedicalPersonnelCollection)
                {
                    if (!medicalPerson.IdNumber.Equals(CurrentPrescription.Pharmacy.MedicalPersonnel.IcNumber)) continue;
                    ///medicalPerson.PrescriptionCount++;
                  ///  PrescriptionCount = medicalPerson.PrescriptionCount;
                    break;
                }
            }
            ///MedicalPersonnels = new ObservableCollection<MedicalPersonnel>(MainWindow.CurrentPharmacy.MedicalPersonnelCollection);
            HisPerson.ItemsSource = MedicalPersonnels;
            if (!string.IsNullOrEmpty(_clinicDeclareId))
            {
                ///declareDb.SaveCooperClinicDeclare(_clinicDeclareId, _clinicXml);
                WebApi.UpdateXmlStatus(_clinicDeclareId);
            }

            CustomerSelected = false;
            _firstTimeDecMasId = string.Empty;
            ClearPrescription();
            INDEX.IndexView.Instance.InitData();
        }

        #region 每日上傳.讀寫卡相關函數

        public void LogInIcData()
        {
            try
            {
                var cs = new ConvertData();
                var icPrescripList = new List<IcPrescriptData>();
                foreach (var med in CurrentPrescription.Medicines)
                {
                    if (!(med is DeclareMedicine)) continue;
                    if (!((DeclareMedicine)med).PaySelf)
                    {
                        icPrescripList.Add(new IcPrescriptData((DeclareMedicine)med));
                    }
                }
                if (!CurrentPrescription.IsGetIcCard) return;
                var pPatientId = new byte[10];
                var pPatientBitrhDay = new byte[10];
                Array.Copy(BasicDataArr, 32, pPatientId, 0, 10);
                Array.Copy(BasicDataArr, 42, pPatientBitrhDay, 0, 7);
                var pDateTime = cs.StringToBytes(Seq.TreatDateTime + " ", 14);
                foreach (var icPrescript in icPrescripList)
                {
                    var pData = cs.StringToBytes(icPrescript.DataStr, 61);
                    var pDataInput = pDateTime.Concat(pData).ToArray();
                    var strLength = 40;
                    var icData = new byte[40];
                    var res = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
                    if (res == 0)
                    {
                        res = HisApiBase.hisWritePrescriptionSign(pDateTime, pPatientId, pPatientBitrhDay, pDataInput,
                            icData, ref strLength);
                        if (res == 0)
                            _prescriptionSignatureList.Add(cs.ByToString(icData, 0, 40));
                        HisApiBase.csCloseCom();
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageWindow.ShowMessage("LogInIcData()", MessageType.ERROR);
            }
        }

        public void CreatIcUploadData(){
            try
            {
                var medicalDatas = new List<MedicalData>();
                var icData = new IcData(Seq, _currentPrescription, CusBasicData, _currentDeclareData);
                var mainMessage = new MainMessage(icData);
                var headerMessage = new Header { DataFormat = "1" };
                var icRecord = new REC(headerMessage, mainMessage);
                int sigCount = 0;
                for (var i = 0; i < CurrentPrescription.Medicines.Count(m => (m is DeclareMedicine med) && !med.PaySelf); i++)
                {
                    if (_currentDeclareData.DeclareDetails[i].P1MedicalOrder.Equals("9"))
                        continue;
                    bool isMedicine = false;
                    foreach (var m in CurrentPrescription.Medicines)
                    {
                        if (_currentDeclareData.DeclareDetails[i].P2MedicalId.Equals(m.Id))
                        {
                            isMedicine = true;
                            break;
                        }
                    }
                    if(!isMedicine)
                        continue;
                    var medicalData = new MedicalData
                    {
                        MedicalOrderTreatDateTime = Seq.TreatDateTime,
                        MedicalOrderCategory = _currentDeclareData.DeclareDetails[i].P1MedicalOrder,
                        TreatmentProjectCode = _currentDeclareData.DeclareDetails[i].P2MedicalId,
                        Usage = _currentDeclareData.DeclareDetails[i].P4Usage,
                        Days = _currentDeclareData.DeclareDetails[i].P11Days.ToString(),
                        TotalAmount = _currentDeclareData.DeclareDetails[i].P7Total.ToString(),
                        PrescriptionSignature = _prescriptionSignatureList[sigCount],
                    };
                    if (!string.IsNullOrEmpty(_currentDeclareData.DeclareDetails[i].P5Position))
                        medicalData.TreatmentPosition = _currentDeclareData.DeclareDetails[i].P5Position;
                    switch (medicalData.MedicalOrderCategory)
                    {
                        case "1":
                        case "A":
                            medicalData.PrescriptionDeliveryMark = "02";
                            break;
                        case "2":
                        case "B":
                            medicalData.PrescriptionDeliveryMark = "06";
                            break;
                        case "3":
                        case "C":
                        case "4":
                        case "D":
                        case "5":
                        case "E":
                            medicalData.PrescriptionDeliveryMark = "04";
                            break;
                    }
                    medicalDatas.Add(medicalData);
                    sigCount++;
                }
                icRecord.MainMessage.MedicalMessageList = medicalDatas;
                icRecord.SerializeDailyUploadObject();
                var d = new DeclareDb();
                ///d.InsertDailyUpload(icRecord.SerializeDailyUploadObject());
            }
            catch (Exception ex)
            {
                Action creatIcUploadDataDelegate = delegate ()
                {
                    MessageWindow.ShowMessage("產生健保資料:"+ex.Message, MessageType.ERROR);
                    
                };
                Instance.Dispatcher.BeginInvoke(creatIcUploadDataDelegate);
            }
        }

        //異常上傳
        public void CreatIcErrorUploadData(IcErrorCodeWindow.IcErrorCode errorCode)
        {
            Action creatIcUploadDataDelegate = delegate ()
            {
                try
                {
                    var medicalDatas = new List<MedicalData>();
                    if (_icErrorWindow.SelectedItem == null) return;
                    var icData = new IcData(CurrentPrescription, errorCode, _currentDeclareData);
                    var mainMessage = new MainMessage(icData);
                    var headerMessage = new Header { DataFormat = "2" };
                    var icRecord = new REC(headerMessage, mainMessage);

                    for (var i = 0; i < CurrentPrescription.Medicines.Count(m=>(m is DeclareMedicine med) && !med.PaySelf); i++)
                    {
                        if (_currentDeclareData.DeclareDetails[i].P1MedicalOrder.Equals("9"))
                            continue;
                        var medicalData = new MedicalData
                        {
                            MedicalOrderTreatDateTime = icData.TreatmentDateTime,
                            MedicalOrderCategory = _currentDeclareData.DeclareDetails[i].P1MedicalOrder,
                            TreatmentProjectCode = _currentDeclareData.DeclareDetails[i].P2MedicalId,
                            Usage = _currentDeclareData.DeclareDetails[i].P4Usage,
                            Days = _currentDeclareData.DeclareDetails[i].P11Days.ToString(),
                            TotalAmount = _currentDeclareData.DeclareDetails[i].P7Total.ToString(),
                        };
                        if (!string.IsNullOrEmpty(_currentDeclareData.DeclareDetails[i].P5Position))
                            medicalData.TreatmentPosition = _currentDeclareData.DeclareDetails[i].P5Position;
                        switch (medicalData.MedicalOrderCategory)
                        {
                            case "1":
                            case "A":
                                medicalData.PrescriptionDeliveryMark = "01";
                                break;
                            case "2":
                            case "B":
                                medicalData.PrescriptionDeliveryMark = "06";
                                break;
                            case "3":
                            case "C":
                            case "4":
                            case "D":
                            case "5":
                            case "E":
                                medicalData.PrescriptionDeliveryMark = "04";
                                break;
                        }
                        medicalDatas.Add(medicalData);
                    }

                    icRecord.MainMessage.MedicalMessageList = medicalDatas;
                    icRecord.SerializeDailyUploadObject();
                    var d = new DeclareDb();
                    ///d.InsertDailyUpload(icRecord.SerializeDailyUploadObject());
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                    
                }
            };
            Instance.Dispatcher.BeginInvoke(creatIcUploadDataDelegate);
        }

        #endregion

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            if (!(sender is AutoCompleteBox medicineCodeAuto)) return;

            var result = DeclareMedicines.Where(x =>
                (x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower()))
                ).Take(50).Select(x => x);
            _medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = _medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }

        #region MedicineDataGridEventFunction

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!(sender is AutoCompleteBox medicineCodeAuto)) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty &&
                    ((ObservableCollection<object>) medicineCodeAuto.ItemsSource).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                    medicineCodeAuto.SelectedItem = ((ObservableCollection<object>) medicineCodeAuto.ItemsSource)?[0];
                else
                    return;
            }

            var currentRow = GetCurrentRowIndex(sender);
            if (string.IsNullOrEmpty(((IProductDeclare) medicineCodeAuto.SelectedItem)?.Forms))
            {
                var prescriptionOtc = (PrescriptionOTC) ((PrescriptionOTC) medicineCodeAuto.SelectedItem)?.Clone();
                if (CurrentPrescription.Medicines.Count > 0)
                {
                    if (CurrentPrescription.Medicines.Count == currentRow)
                    {
                        var m = CurrentPrescription.Medicines[currentRow - 1];
                        Debug.Assert(prescriptionOtc != null, nameof(prescriptionOtc) + " != null");
                        prescriptionOtc.Usage = new Usage();
                        prescriptionOtc.Dosage = 0;
                        prescriptionOtc.Days = string.Empty;
                        CurrentPrescription.Medicines.Add(prescriptionOtc);
                        medicineCodeAuto.Text = string.Empty;
                    }
                    else
                    {
                        CurrentPrescription.Medicines[currentRow] = prescriptionOtc;
                    }
                }
                else
                {
                    CurrentPrescription.Medicines.Add(prescriptionOtc);
                    medicineCodeAuto.Text = string.Empty;
                }
            }
            else
            {
                var declareMedicine = ((DeclareMedicine) medicineCodeAuto.SelectedItem).DeepCloneViaJson();
                if (declareMedicine != null && (declareMedicine.Id.EndsWith("00") || declareMedicine.Id.EndsWith("G0")))
                    declareMedicine.Position = Positions.SingleOrDefault(p => p.Id.Contains("PO"))?.Id;
                if (_isPrescribe)
                    if (declareMedicine != null)
                        declareMedicine.PaySelf = true;
                if (CurrentPrescription.Medicines.Count > 0)
                {
                    if (CurrentPrescription.Medicines.Count == currentRow)
                    {
                        var m = CurrentPrescription.Medicines[currentRow - 1];
                        if (m is DeclareMedicine med)
                        {
                            Debug.Assert(declareMedicine != null, nameof(declareMedicine) + " != null");
                            declareMedicine.UsageName = ((IProductDeclare) med).Usage;
                            declareMedicine.Dosage = ((IProductDeclare) med).Dosage;
                            declareMedicine.Days = ((IProductDeclare) med).Days;
                            CurrentPrescription.Medicines.Add(declareMedicine);
                            medicineCodeAuto.Text = string.Empty;
                        }
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

            MedicineCodeMoveFocus(currentRow);
        }

        private void MedicineCodeMoveFocus(int currentRow)
        {
            if(currentRow == -1) return;
            PrescriptionMedicines.CurrentCell = new DataGridCellInfo(
                PrescriptionMedicines.Items[currentRow], DosageText);
            if (PrescriptionMedicines.CurrentCell.Item is null) return;
            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column?.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;
            var firstChild =
                (UIElement) VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
            }

            firstChild.Focus();
        }

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter) return;
            TextBox t = sender as TextBox;
            DependencyObject dpobj = sender as DependencyObject;
            string name = dpobj.GetValue(FrameworkElement.NameProperty) as string;
            if (name.Equals("Usage"))
            {
                foreach (var u in MainWindow.Usages)
                {
                    if (string.IsNullOrEmpty(t.Text)) continue;
                    if (t.Text.Equals(u.QuickName))
                        t.Text = u.Name;
                }
            }

            e.Handled = true;
            MoveFocusNext(sender);
        }

        private void Position_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //按 Enter 下一欄
            if (e.Key != Key.Enter) return;
            e.Handled = true;
            var currentRow = GetCurrentRowIndex(sender);
            PrescriptionMedicines.CurrentCell = new DataGridCellInfo(
                PrescriptionMedicines.Items[currentRow + 1], MedicineId);
            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            var firstChild =
                (UIElement) VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
            while (!(firstChild is AutoCompleteBox))
            {
                firstChild = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
            }

            firstChild.Focus();
            e.Handled = false;
            SendKeys.SendWait("~");

        }

        private void MoveFocusNext(object sender)
        {
            if (sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionMedicines.CurrentCell.Column is null) return;

            var focusedCell =
                PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
                    }

                    if ((child is TextBox || child is AutoCompleteBox || child is CheckBox || child is TextBlock))
                        break;
                }

                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell =
                    PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement) VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is AutoCompleteBox) && firstChild.Focusable)
                firstChild.Focus();
        }

        private int GetCurrentRowIndex(object sender)
        {
            switch (sender)
            {
                case TextBox textBox:
                {
                    var temp = new List<TextBox>();
                    NewFunction.FindChildGroup(PrescriptionMedicines, textBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(textBox))
                            return x;
                    }

                    break;
                }
                case CheckBox checkBox:
                {
                    var temp = new List<CheckBox>();
                    NewFunction.FindChildGroup(PrescriptionMedicines, checkBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(checkBox))
                            return x;
                    }

                    break;
                }
                case AutoCompleteBox autoCompleteBox:
                {
                    var temp = new List<AutoCompleteBox>();
                    NewFunction.FindChildGroup(PrescriptionMedicines, autoCompleteBox.Name, ref temp);
                    for (var x = 0; x < temp.Count; x++)
                    {
                        if (temp[x].Equals(autoCompleteBox))
                            return x;
                    }

                    break;
                }
            }

            return -1;
        }

        private void MedicineTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectAll();
        }

        private void FindUsagesQuickName(object sender)
        {
            var currentRow = GetCurrentRowIndex(sender);
            if (!(sender is TextBox t) || string.IsNullOrEmpty(t.Text)) return;
            if (Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text)) == null) return;
            {
                if (CurrentPrescription.Medicines[currentRow] is DeclareMedicine)
                {
                    ((DeclareMedicine) CurrentPrescription.Medicines[currentRow]).Usage =
                        Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                    if (((IProductDeclare) (DeclareMedicine) CurrentPrescription.Medicines[currentRow]).Usage != null)
                        t.Text = ((IProductDeclare) (DeclareMedicine) CurrentPrescription.Medicines[currentRow]).Usage;
                }
                else
                {
                    ((PrescriptionOTC) CurrentPrescription.Medicines[currentRow]).Usage =
                        Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                    if (((IProductDeclare) (PrescriptionOTC) CurrentPrescription.Medicines[currentRow]).Usage != null)
                        t.Text = ((IProductDeclare) (PrescriptionOTC) CurrentPrescription.Medicines[currentRow]).Usage;
                }
            }
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;

            if (selectedItem is IDeletable deletable)
            {
                if (CurrentPrescription.Medicines.Contains(selectedItem))
                    deletable.Source = "/Images/DeleteDot.png";

                PrescriptionMedicines.SelectedItem = deletable;
                return;
            }

            PrescriptionMedicines.SelectedIndex = CurrentPrescription.Medicines.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;

            if (leaveItem is IDeletable deletable) deletable.Source = string.Empty;
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex] is DeclareMedicine)
                ClearMedicine((DeclareMedicine) CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
            else if (CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex] is PrescriptionOTC)
                ClearPrescriptionOtc(
                    (PrescriptionOTC) CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
            SetChanged();
            CurrentPrescription.Medicines.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }

        private void ClearMedicine(DeclareMedicine med)
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
            med.Dosage = 0;
        }

        private void ClearPrescriptionOtc(PrescriptionOTC med)
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
            med.Dosage = 0;
        }

        private void SetChanged()
        {
            if (_isFirst) return;
            _isChanged = true;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CountMedicinesCost()
        {
            double medicinesHcCost = 0; //健保給付總藥價
            double medicinesSelfCost = 0; //自費藥總藥價
            double purchaseCosts = 0; //藥品總進貨成本
            foreach (var medicine in CurrentPrescription.Medicines)
            {
                switch (medicine)
                {
                    case DeclareMedicine declareMedicine:
                        {
                            if (!declareMedicine.PaySelf)
                                medicinesHcCost += declareMedicine.HcPrice * declareMedicine.Amount;
                            else
                                medicinesSelfCost += declareMedicine.TotalPrice;
                            purchaseCosts += declareMedicine.Cost * declareMedicine.Amount;
                            break;
                        }
                    case PrescriptionOTC otc:
                        medicinesSelfCost += otc.TotalPrice;
                        purchaseCosts += otc.Cost * otc.Amount;
                        break;
                }
            }

            CurrentPrescription.MedicinePoint = Math.Round(medicinesHcCost, 2, MidpointRounding.AwayFromZero);
            SelfCost = Convert.ToInt16(Math.Ceiling(medicinesSelfCost)); //自費金額
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.Copayment.Id))
            {
                if (CurrentPrescription.Treatment.AdjustCase != null)
                {
                    if (CurrentPrescription.Treatment.AdjustCase.Id.Equals("2") && CurrentPrescription.Treatment.AdjustCase.Id.Equals("D"))
                        CurrentPrescription.Treatment.Copayment = Copayments.SingleOrDefault(c => c.Id.Equals("I22")).DeepCloneViaJson();
                }

                if (CurrentPrescription.Treatment.MedicalInfo.Hospital.Division != null)
                {
                    if (CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Name.Equals("牙科"))
                        CurrentPrescription.Treatment.Copayment = Copayments.SingleOrDefault(c => c.Id.Equals("I22")).DeepCloneViaJson();
                }

                CurrentPrescription.Treatment.Copayment = CurrentPrescription.MedicinePoint >= 101 ? Copayments.SingleOrDefault(c => c.Id.Equals("I20")) : Copayments.SingleOrDefault(c => c.Id.Equals("I21"));

                if (CurrentPrescription.Treatment.Copayment != null && !NewFunction.CheckCopaymentFreeProject(CurrentPrescription.Treatment.Copayment.Id))
                {
                    Copayment = CountCopaymentCost(CurrentPrescription.MedicinePoint); //部分負擔
                    CurrentPrescription.CopaymentPoint = CountCopaymentCost(CurrentPrescription.MedicinePoint); //部分負擔
                }

                if (IsNotReceiveCopayment)
                    Copayment = 0;
            }
            MedProfit = (CurrentPrescription.MedicinePoint + medicinesSelfCost - purchaseCosts); //藥品毛利
        }

        private void CountCharge()
        {
            if (Charge != Deposit + SelfCost + Copayment)
                Charge = Deposit + SelfCost + Copayment;
            if(Change != Pay - Charge)
                Change = Pay - Charge;
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

        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox t && string.IsNullOrEmpty(t.Text))
            {
                t.Text = "0";
                t.SelectAll();
            }
        }

        private void LoadCustomerDataButtonClick(object sender, RoutedEventArgs e)
        {
            var loading = new LoadingWindow();
            loading.LoadIcData(Instance);
            loading.ShowDialog();
            ReadCustomerTreatRecord();
        }

        private bool CusHistoryFilter(object item)
        {
            if (_cusHhistoryFilterCondition == SystemType.ALL)
                return true;
            return ((CustomerHistoryMaster) item).Type == _cusHhistoryFilterCondition;
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
            if (selectedItem is null) return;
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

        private void ReleaseHospital_Populating(object sender, PopulatingEventArgs e)
        {
            var a = sender as AutoCompleteBox;
            if (Hospitals is null) ;/// Hospitals = HospitalDb.GetData();
            var tempCollection = new Institutions(true);
            if (tempCollection.Count == 1)
            {
                if (a != null) a.SelectedItem = tempCollection[0];
                ReleaseHospital.PopulateComplete();
                DivisionCombo.Focus();
                return;
            }
            ReleaseHospital.ItemsSource = tempCollection;
            ReleaseHospital.PopulateComplete();
        }

        private void AdjustCaseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AdjustCaseCombo.SelectedItem == null) return;
            ButtonDeclareRegister.IsEnabled = false;
            if (((AdjustCase) AdjustCaseCombo.SelectedItem).Id.Equals("0"))
            {
                DeclareSubmit.Visibility = Visibility.Collapsed;
                NotDeclareSubmit.Visibility = Visibility.Visible;
                CurrentPrescription.Declare = false;
                _isPrescribe = true;
            }
            else
            {
                DeclareSubmit.Visibility = Visibility.Visible;
                NotDeclareSubmit.Visibility = Visibility.Collapsed;
                CurrentPrescription.Declare = true;
                _isPrescribe = false;
            }

            switch (((AdjustCase)AdjustCaseCombo.SelectedItem).Id)
            {
                case "2":
                    CopaymentCombo.SelectedItem = Copayments.SingleOrDefault(c => c.Id.Equals("I22"));
                    ButtonDeclareRegister.IsEnabled = true;
                    break;
                case "D":
                    CopaymentCombo.SelectedItem = Copayments.SingleOrDefault(c => c.Id.Equals("009"));
                    ///CurrentPrescription.Treatment.MedicalInfo.Hospital = Hospitals.SingleOrDefault(h => h.Id.Equals("N")).DeepCloneViaJson();
                    CurrentPrescription.Treatment.MedicalInfo.Hospital.Division = null;
                    CurrentPrescription.Treatment.MedicalInfo.Hospital.Doctor = null;
                    CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = null;
                    CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = null;
                    break;
                default:
                    CopaymentCombo.SelectedItem = CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Name.Equals("牙科") ? Copayments.SingleOrDefault(c => c.Id.Equals("I22")) : Copayments.SingleOrDefault(c => c.Id.Equals("I20"));
                    break;
            }

            if (CurrentPrescription.Treatment.AdjustCase.Id.Equals("2") || CurrentPrescription.Treatment.AdjustCase.Id.Equals("D"))
            {
                PaymentCategoryCombo.SelectedIndex = -1;
            }
            else
            {
                PaymentCategoryCombo.SelectedIndex = 3;
            }

            SetSubmmitButton();
        }

        private void ChronicSequence_TextChanged(object sender, TextChangedEventArgs e)
        {
            var t = sender as TextBox; 
            if (string.IsNullOrEmpty(t.Text))
            {
                AdjustCaseCombo.SelectedIndex = 0;
                TreatmentCaseCombo.SelectedItem = PrescriptionCases.SingleOrDefault(c => c.Name.Equals("一般案件"));
                return;
            } 
            AdjustCaseCombo.SelectedItem = AdjustCases.SingleOrDefault(a => a.Name.Contains("慢性病連續處方調劑"));
            TreatmentCaseCombo.SelectedItem = PrescriptionCases.SingleOrDefault(c => c.Name.Equals("慢性病"));
          
            SetSubmmitButton();
        }

        public void SetValueByPrescription(CooperativeClinic cooperativeClinic)
        {
            IsNotReceiveCopayment = cooperativeClinic.Remark.Substring(16, 1).Equals("Y");
            _clinicDeclareId = cooperativeClinic.DeclareId;
            _clinicXml = cooperativeClinic.Xml;
            for (int i = 0; i < cooperativeClinic.Prescription.Medicines.Count; i++)
            {
                if (DeclareMedicines.Count(med => med.Id == cooperativeClinic.Prescription.Medicines[i].Id) == 0)
                {
                    var tempMedicine = "";/// MedicineDb.GetMedicinesDataByMedId(cooperativeClinic.Prescription.Medicines[i].Id);
                    if (tempMedicine is null)  {
                        ///ProductDb.InsertMedicine(cooperativeClinic.Prescription.Medicines[i].Id, cooperativeClinic.Prescription.Medicines[i].Name);
                        DeclareMedicines.Add((PrescriptionOTC)cooperativeClinic.Prescription.Medicines[i]);
                    }
                    else {
                        ;/// DeclareMedicines.Add(tempMedicine);
                    }
                }
            }

            foreach (Product declareMedicine in cooperativeClinic.Prescription.Medicines)
            {
                declareMedicine.Name = DeclareMedicines.SingleOrDefault(med => med.Id == declareMedicine.Id).Name;
                if (declareMedicine is DeclareMedicine)
                {
                    var tempMed = ((DeclareMedicine)DeclareMedicines.SingleOrDefault(med => med.Id == declareMedicine.Id));
                    ((DeclareMedicine) declareMedicine).HcPrice = tempMed.HcPrice;
                    ((DeclareMedicine) declareMedicine).Stock = tempMed.Stock;
                    if( !((DeclareMedicine)declareMedicine).PaySelf)
                        ((DeclareMedicine)declareMedicine).TotalPrice = tempMed.HcPrice * ((DeclareMedicine)declareMedicine).Amount;
                }
            }

            bool tmpGetCard = CurrentPrescription.IsGetIcCard;
            CurrentPrescription = cooperativeClinic.Prescription;
            CurrentPrescription.IsGetIcCard = tmpGetCard;
            string id = null;/// CustomerDb.CheckCustomerExist(CurrentPrescription.Customer);
            string medicalNum = CurrentPrescription.Customer.IcCard.MedicalNumber;
            CurrentPrescription.Customer = null;//CustomerDb.GetCustomerDataById(id);  
            CurrentPrescription.Customer.IcCard.MedicalNumber = medicalNum;
            if (!string.IsNullOrEmpty(CurrentPrescription.ChronicSequence) && int.Parse(CurrentPrescription.ChronicSequence) > 1)
            {
                CurrentPrescription.Customer.IcCard.MedicalNumber = "IC0" + CurrentPrescription.ChronicSequence;
                TempMedicalNumber = CurrentPrescription.OriginalMedicalNumber;
            }
            else
            {
                TempMedicalNumber = CurrentPrescription.Customer.IcCard.MedicalNumber;
            }
            string tempTreatmentId = CurrentPrescription.Treatment.MedicalInfo.TreatmentCase.Id;

            DivisionCombo.SelectedItem = Divisions.SingleOrDefault(d =>
                d.Id.Equals(CurrentPrescription.Treatment.MedicalInfo.Hospital.Division.Id));

            CurrentPrescription.Treatment.AdjustCase =
                AdjustCases.SingleOrDefault(t => t.Id == CurrentPrescription.Treatment.AdjustCase.Id);
            AdjustCaseCombo.SelectedItem =
                AdjustCases.SingleOrDefault(t => t.Id == CurrentPrescription.Treatment.AdjustCase.Id);

            CurrentPrescription.Treatment.MedicalInfo.TreatmentCase =
                PrescriptionCases.SingleOrDefault(t => t.Id.Equals(tempTreatmentId));
            TreatmentCaseCombo.SelectedItem = PrescriptionCases.SingleOrDefault(t => t.Id.Equals(tempTreatmentId));

            var diseaseCode = CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = null;/// DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            diseaseCode = CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = null;/// DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id))
            {
                foreach (var s in SpecialCodes)
                {
                    if (!s.Id.Contains(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id)) continue;
                    CurrentPrescription.Treatment.MedicalInfo.SpecialCode = s.DeepCloneViaJson();
                    break;
                }
            }

            CountMedicinesCost();
            SetSubmmitButton();
        }

        public void SetValueByDecMasId(string decMasId)
        {
            if (decMasId is null) return;
            _currentDecMasId = decMasId;
            var prescription = new Prescription();/// PrescriptionDB.GetDeclareDataById(decMasId).Prescription;
            var tempHospital = prescription.Treatment.MedicalInfo.Hospital.DeepCloneViaJson();
            CurrentPrescription = prescription;
            var diseaseCode = CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id;

            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = null;/// DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            diseaseCode = CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;

            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = null;/// DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id))
            {
                foreach (var s in SpecialCodes)
                {
                    if (!s.Id.Contains(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id)) continue;
                    CurrentPrescription.Treatment.MedicalInfo.SpecialCode = s.DeepCloneViaJson();
                    break;
                }
            }
            if (!string.IsNullOrEmpty(CurrentPrescription.ChronicSequence) && int.Parse(CurrentPrescription.ChronicSequence) > 1)
            {
                CurrentPrescription.Customer.IcCard.MedicalNumber = "IC0" + CurrentPrescription.ChronicSequence;
                TempMedicalNumber = CurrentPrescription.OriginalMedicalNumber;
            }
            else
            {
                TempMedicalNumber = CurrentPrescription.Customer.IcCard.MedicalNumber;
            }
            CountMedicinesCost();
            SetSubmmitButton();
        }

        private void DiseaseCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is null) return;
            if (!(sender is TextBox textBox)) return;
            if (e.Key != Key.Enter)
            {
                if (textBox.Text.Contains(" "))
                    textBox.Text = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.Id) && CurrentPrescription.Treatment.AdjustCase.Id.Equals("D"))
            {
                textBox.Text = string.Empty;
                MessageWindow.ShowMessage("調劑案件為慢性病連續處方箋調劑/藥事居家照護，本欄免填。", MessageType.WARNING);
                
                CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = null;
                CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = null;
                return;
            }

            DependencyObject textBoxDependency = textBox;
            var name = textBoxDependency.GetValue(NameProperty);
            if (name.Equals("MainDiagnosis"))
            {
                if ((!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id) &&
                     textBox.Text.Contains(" ")) || string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    SecondDiagnosis.Focus();
                    return;
                }
            }
            else if (name.Equals("SecondDiagnosis"))
            {
                if (string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    ChronicTotal.Focus();
                    return;
                }
            }

            if (textBox.Text.Length < 3)
            {
                MessageWindow.ShowMessage("請輸入完整疾病代碼", MessageType.WARNING);
                
                return;
            }

            if (true)///DiseaseCodeDb.GetDiseaseCodeById(textBox.Text).DistinctBy(d => d.ICD10.Id).DistinctBy(d => d.ICD9.Id).ToList().Count == 1)
            {
                var selectedDiseaseCode = new DiseaseCode();/// DiseaseCodeDb.GetDiseaseCodeById(textBox.Text)[0].ICD10;
                if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                {
                    MessageWindow.ShowMessage("查無疾病代碼", MessageType.WARNING);
                    
                    return;
                }

                if (textBoxDependency.GetValue(NameProperty) is string &&
                    textBoxDependency.GetValue(NameProperty).Equals("MainDiagnosis"))
                    CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = selectedDiseaseCode;
                else
                {
                    if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                    {
                        MessageWindow.ShowMessage("查無疾病代碼", MessageType.WARNING);
                        
                        return;
                    }

                    if ((selectedDiseaseCode.Id.Equals("查無疾病代碼") && textBox.Text.Contains(" ")) ||
                        string.IsNullOrEmpty(textBox.Text.Trim()))
                    {
                        ChronicTotal.Focus();
                    }
                    else
                    {
                        CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = selectedDiseaseCode;
                    }
                }
            }
            else
            {
                var disease = new DiseaseCodeSelectDialog(textBox.Text, (string) name);
                if (disease.DiseaseCollection.Count > 1)
                    disease.Show();
            }
        }

        private void Combo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (HisPerson.SelectedItem is null) {
                MessageWindow.ShowMessage("目前沒有藥師 請新增藥師",MessageType.WARNING);
                
                return;
            }
            switch (sender)
            {
                case ComboBox _ when e.Key != Key.Enter:
                    return;
                case ComboBox c:
                    DependencyObject combo = c;
                    var cName = combo.GetValue(NameProperty) as string;
                    switch (cName)
                    {
                        case "DivisionCombo":
                            if (!MainWindow.CurrentUser.Id.Equals(((MedicalPersonnel) HisPerson.SelectedItem).Id))
                                HisPerson.Focus();
                            else
                                MedicalNumber.Focus();
                            break;
                        case "HisPerson":
                            if (string.IsNullOrEmpty(MedicalNumber.Text.Trim()))
                                MedicalNumber.Focus();
                            else
                                MainDiagnosis.Focus();
                            break;
                        case "TreatmentCaseCombo":
                            CopaymentCombo.Focus();
                            break;
                        case "PaymentCategoryCombo":
                            AdjustCaseCombo.Focus();
                            break;
                        case "CopaymentCombo":
                            SpecialCodeText.Focus();
                            break;
                        case "AdjustCaseCombo":
                            TreatmentCaseCombo.Focus();
                            break;
                    }

                    break;
                case TextBox _ when e.Key != Key.Enter:
                    return;
                case TextBox t:
                    DependencyObject text = t;
                    var tName = text.GetValue(NameProperty) as string;
                    switch (tName)
                    {
                        case "DoctorId":
                            MedicalNumber.Focus();
                            break;
                        case "PaymentCategoryCombo":
                            AdjustCaseCombo.Focus();
                            break;
                        case "ChronicSequence":
                            if (string.IsNullOrEmpty(ChronicTotal.Text) &&
                                !int.TryParse(ChronicSequence.Text, out _))
                            {
                                MessageWindow.ShowMessage("總領藥次數有值，需填寫領藥次數", MessageType.WARNING);
                                
                                ChronicTotal.Focus();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ChronicTotal.Text) &&
                                    int.TryParse(ChronicSequence.Text, out var seq) && seq >= 1)
                                {
                                    AdjustCaseCombo.SelectedIndex = 1;
                                    SpecialCodeText.Focus();
                                }
                            }

                            break;
                        case "ChronicTotal":
                            if (string.IsNullOrEmpty(t.Text))
                                SpecialCodeText.Focus();
                            else
                                ChronicSequence.Focus();
                            break;
                        case "MedicalNumber":
                            TreatmentDate.Focus();
                            TreatmentDate.SelectionStart = 0;
                            break;
                        case "TreatmentDate":
                            AdjustDate.Focus();
                            AdjustDate.SelectionStart = 0;
                            break;
                        case "AdjustDate":
                            MainDiagnosis.Focus();
                            break;
                        case "SpecialCodeText":
                            if (t != null && t.Text.Length > 0)
                            {
                                foreach (var specialCode in SpecialCodes)
                                {
                                    if (!specialCode.Id.Equals(t.Text.ToUpper())) continue;
                                    CurrentPrescription.Treatment.MedicalInfo.SpecialCode =
                                        specialCode.DeepCloneViaJson();
                                    t.CaretIndex = 0;
                                    break;
                                }
                            }
                            var nextAutoCompleteBox = new List<AutoCompleteBox>();
                            NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto",
                                ref nextAutoCompleteBox);
                            nextAutoCompleteBox[0].Focus();
                            break;
                    }

                    break;
            }
        }

        private void ReleaseHospital_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is AutoCompleteBox a)) return;
            a.Text = a.Text.TrimStart(' ');
            if (e.Key != Key.Enter) return;
            if (CurrentPrescription.Treatment.MedicalInfo.Hospital == null) return;
            if (string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.Hospital.Id)) return;
            CurrentPrescription.Treatment.MedicalInfo.Hospital.Division = NewFunction.CheckHospitalNameContainsDivision(CurrentPrescription.Treatment.MedicalInfo.Hospital.Name);
            e.Handled = true;
            DivisionCombo.Focus();
        }

        private void ReleaseHospital_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!(sender is AutoCompleteBox a) || a.SelectedItem == null) return;
            CurrentPrescription.Treatment.MedicalInfo.Hospital.Doctor.IcNumber =
                CurrentPrescription.Treatment.MedicalInfo.Hospital.Id;
            DivisionCombo.Focus();
        }

        private void IsBuckle_Click(object sender, RoutedEventArgs e)
        {
            foreach (var t in CurrentPrescription.Medicines)
            {
                if (!(t is DeclareMedicine)) continue;
                if (t.Id == _selectedMedId)
                    ((DeclareMedicine) t).IsBuckle = !((DeclareMedicine) t).IsBuckle;
            }
        }

        private void MedContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (PrescriptionMedicines.SelectedItem == null || CurrentPrescription.Medicines.Count == 0) return;
            var temp = (sender as ContextMenu);
            var menuitem = temp.Items[0] as MenuItem;
            if (!(PrescriptionMedicines.SelectedItem is DeclareMedicine)) return;
            _selectedMedId = ((DeclareMedicine) PrescriptionMedicines.SelectedItem).Id;
            if (((DeclareMedicine) PrescriptionMedicines.SelectedItem).IsBuckle)
                menuitem.Header = "申報不扣庫";
            else
                menuitem.Header = "申報扣庫";
        }

        private void HisPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var c = sender as ComboBox;
          ///  PrescriptionCount = MainWindow.CurrentPharmacy.MedicalPersonnelCollection.SingleOrDefault(p => p.IdNumber.Equals((c.SelectedItem as MedicalPersonnel).IDNumber)).PrescriptionCount;
        }

        private void NotDeclareSubmit_OnClickSubmit_ButtonClick(object sender, RoutedEventArgs e)
        {
            var declareDb = new DeclareDb();
            _currentDeclareData = new DeclareData(CurrentPrescription);
            var decMasId = "";/// declareDb.InsertPrescribeData(_currentDeclareData);
            var totalCost = 0.0;
            var totalPrice = 0;
            foreach (var med in _currentDeclareData.Prescription.Medicines)
            {
                switch (med)
                {
                    case DeclareMedicine declare:
                        totalCost += 0;/// double.Parse(ProductDb.GetBucklePrice(((IProductDeclare) declare).ProductId,((IProductDeclare) declare).Amount.ToString()));
                        totalPrice += int.Parse(Math.Ceiling(((IProductDeclare) declare).TotalPrice).ToString());
                        break;
                    case PrescriptionOTC otc:
                        totalCost += 0;/// double.Parse(ProductDb.GetBucklePrice(((IProductDeclare) otc).ProductId,((IProductDeclare) otc).Amount.ToString()));
                        totalPrice += int.Parse(Math.Ceiling(((IProductDeclare) otc).TotalPrice).ToString());
                        break;
                }
            }

            ///ProductDb.InsertEntry("配藥收入", totalCost.ToString(), "DecMasId", decMasId);
            ///ProductDb.InsertEntry("調劑耗用", "-" + totalCost, "DecMasId", decMasId);
            ///declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", decMasId); //庫存扣庫
            MessageWindow.ShowMessage("調劑登錄成功", MessageType.SUCCESS);
            
            ClearPrescription();
        }

        private void MedTotalPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            textBox.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearPrescription();
        }

        private void ChronicSequence_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new System.Text.RegularExpressions.Regex("[^0-9.]+").IsMatch(e.Text);
        }

        private void AdjustDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSubmmitButton();
        }

        private void SelectionStart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (sender)
            {
                case AutoCompleteBox a:
                    if (a.Template.FindName("Text", a) is TextBox txt)
                    {
                        txt.SelectionStart = 0;
                        txt.SelectionLength = txt.Text.Length;
                    }

                    break;
                case TextBox t:
                    t.SelectionStart = 0;
                    t.SelectionLength = t.Text.Length;
                    break;
            }
        }

        public void SearchCustomer()
        {
            if (!string.IsNullOrEmpty(PatientId.Text))
            {
                ShowCustomerSelectionWindow(new CustomerSelectWindow(PatientId.Text, 3));
                return;
            }

            if (!string.IsNullOrEmpty(PatientName.Text))
            {
                ShowCustomerSelectionWindow(new CustomerSelectWindow(PatientName.Text, 2));
                return;
            }

            if (!string.IsNullOrEmpty(PatientTel.Text))
            {
                ShowCustomerSelectionWindow(new CustomerSelectWindow(PatientTel.Text, 4));
                return;
            }

            if (!PatientBirthday.Text.Equals("   /  /  "))
            {
                ShowCustomerSelectionWindow(
                    new CustomerSelectWindow(PatientBirthday.Text.TrimStart('0').Trim().Replace("/", string.Empty), 1));
                return;
            }

            if (string.IsNullOrEmpty(PatientName.Text) && string.IsNullOrEmpty(PatientBirthday.Text) &&
                string.IsNullOrEmpty(PatientId.Text) && CurrentPrescription.Treatment.AdjustCase.Id == "0")
                ShowCustomerSelectionWindow(new CustomerSelectWindow(string.Empty, 0));
            else
                ShowCustomerSelectionWindow(new CustomerSelectWindow(string.Empty, -1));
        }

        private void PatientId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SearchCustomer();
                    break;
            }
        }

        private void ShowCustomerSelectionWindow(CustomerSelectWindow selectionWindow)
        {
            if (!CustomerSelected)
                selectionWindow.ShowDialog();
            CheckChronicExist();
        }

        private void DivisionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (c.SelectedItem is null) return;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.Id) && CurrentPrescription.Treatment.AdjustCase.Id.Equals("D"))
            {
                if (DivisionCombo.SelectedIndex == -1)
                {
                    CurrentPrescription.Treatment.MedicalInfo.Hospital.Division = null;
                    return;
                }
                MessageWindow.ShowMessage("調劑案件為藥事居家照護，本欄免填。", MessageType.WARNING);
                
                DivisionCombo.SelectedIndex = -1;
                return;
            }
            if (((Division) c.SelectedItem).Name.Equals("牙科"))
            {
                TreatmentCaseCombo.SelectedItem = PrescriptionCases.SingleOrDefault(t => t.Name.Equals("牙醫其他專案"));
                CopaymentCombo.SelectedItem = Copayments.SingleOrDefault(p => p.Name.Equals("其他免收"));
            }
            else
            {
                TreatmentCaseCombo.SelectedItem = PrescriptionCases.SingleOrDefault(t => t.Name.Equals("一般案件"));
                CopaymentCombo.SelectedItem = Copayments.SingleOrDefault(p => p.Name.Equals("加收部分負擔"));
            }
        }

        private void ReloadCardReader_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ReloadCardReader();
        }

        public void ReadTreatRecord()
        {
            var strLength = 296;
            var icData = new byte[296];
            var cs = new ConvertData();
            var cTreatItem = cs.StringToBytes("AF\0", 3);
            //新生兒就醫註記,長度兩個char
            var cBabyTreat = TreatRecCollection.Count > 0
                ? cs.StringToBytes(TreatRecCollection[0].NewbornTreatmentMark + "\0", 3)
                : cs.StringToBytes(" ", 2);
            //補卡註記,長度一個char
            var cTreatAfterCheck = new byte[] {1};
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
            var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, icData, ref strLength);
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csCloseCom();
            //取得就醫序號
            if (res == 0)
            {
                IsMedicalNumberGet = true;
                Seq = new SeqNumber(icData);
                CurrentPrescription.Customer.IcCard.MedicalNumber = Seq.MedicalNumber;
                NotifyPropertyChanged(nameof(CurrentPrescription.Customer.IcCard));
            }
            //未取得就醫序號
            else
            {
                GetMedicalNumberErrorCode = res;
            }

            //取得就醫紀錄
            strLength = 498;
            icData = new byte[498];
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csOpenCom(MainWindow.CurrentPharmacy.ReaderCom);
            res = HisApiBase.hisGetTreatmentNoNeedHPC(icData, ref strLength);
            MainWindow.Instance.HisApiErrorCode = HisApiBase.csCloseCom();
            if (res == 0)
            {
                var startIndex = 84;
                for (var i = 0; i < 6; i++)
                {
                    if (icData[startIndex + 3] == 32) break;
                    TreatRecCollection.Add(new TreatmentDataNoNeedHpc(icData, startIndex,false));
                    startIndex += 69;
                }
            }
        }

        public void CheckChronicExist()
        {
            if (string.IsNullOrEmpty(CurrentPrescription.Customer.Id)) return;
            var chronicSelectWindow =
                new ChronicSelectWindow(CurrentPrescription.Customer.Id, CurrentPrescription.Customer.IcNumber);
            if (chronicSelectWindow.ChronicCollection.Count == 0 &&  chronicSelectWindow.CooperativeClinicCollection.Count == 0)
                chronicSelectWindow.Close();
            else
                chronicSelectWindow.ShowDialog();

        }

        public void ClearPrescription()
        {
            _firstTimeDecMasId = string.Empty;
            _currentDecMasId = string.Empty;
            _clinicDeclareId = string.Empty;
            IsNotReceiveCopayment = false;
            _isPrescribe = false;
            _clinicXml = new XmlDocument();
            CurrentPrescription = new Prescription();
            DivisionCombo.SelectedIndex = -1;
            TreatmentCaseCombo.SelectedIndex = -1;
            var isMedicalPerson = false;
            foreach (var m in MainWindow.CurrentPharmacy.MedicalPersonnelCollection)
            {
                ///if (!m.Id.Equals(MainWindow.CurrentUser.Id)) continue;
                isMedicalPerson = true;
                break;
            }

            if (isMedicalPerson)
            {
                CurrentPrescription.Pharmacy.MedicalPersonnel = MedicalPersonnels.SingleOrDefault(p => p.Id.Equals(MainWindow.CurrentUser.Id));
            }
            else
            {
                CurrentPrescription.Pharmacy.MedicalPersonnel = MedicalPersonnels[0];
            }

            CurrentPrescription.Treatment.Copayment = Copayments.SingleOrDefault(c => c.Name.Equals("加收部分負擔"));
            CurrentPrescription.Treatment.PaymentCategory =
                PaymentCategories.SingleOrDefault(p => p.Name.Equals("普通疾病"));
            CurrentPrescription.Treatment.AdjustCase = AdjustCases.SingleOrDefault(a => a.Name.Equals("一般處方調劑"));
            CurrentPrescription.Treatment.MedicalInfo.TreatmentCase =
                PrescriptionCases.SingleOrDefault(c => c.Name.Equals("一般案件"));
            CurrentPrescription.Treatment.MedicalInfo.SpecialCode = new SpecialTreat();
            CurrentPrescription.ChronicTotal = string.Empty;
            CurrentPrescription.Medicines.Clear();
            CopaymentText.Text = string.Empty;
            CustomPayText.Text = string.Empty;
            DepositText.Text = string.Empty;
            SelfCostText.Text = string.Empty;
            PaidText.Text = string.Empty;
            CustomerSelected = false;
            _isPrescribe = false;
            ((ViewModelMainWindow) MainWindow.Instance.DataContext).IsIcCardValid = false;
            CurrentCustomerHistoryMaster?.CustomerHistoryMasterCollection.Clear();
            CurrentCustomerHistoryMaster = new CustomerHistoryMaster();
            CusHistoryMaster.ItemsSource = null;
            CusHistoryDetailPos.ItemsSource = null;
            CusHistoryDetailHis.ItemsSource = null;
            TempMedicalNumber = string.Empty;
        }

        private void ReloadCardReader()
        {
            LoadingWindow loading = new LoadingWindow();
            loading.ResetCardReader(Instance);
            loading.ShowDialog();
        }

        public void LoadPatentDataFromIcCard()
        {
            if (HisApiBase.GetStatus(2))
            {
                var strLength = 72;
                var icData = new byte[72];
                HisApiBase.OpenCom();
                if (MainWindow.Instance.HisApiErrorCode == 0)
                {
                    MainWindow.Instance.SetCardReaderStatus("健保卡資料讀取中...");
                    MainWindow.Instance.HisApiErrorCode = HisApiBase.hisGetBasicData(icData, ref strLength);
                    if (MainWindow.Instance.HisApiErrorCode == 0)
                    {
                        MainWindow.Instance.SetCardReaderStatus("健保卡讀取成功");
                        CurrentPrescription.IsGetIcCard = true;
                        icData.CopyTo(BasicDataArr, 0);
                        CusBasicData = new BasicData(icData);
                        HisApiBase.CloseCom();
                        CurrentPrescription.Customer = new Customer(CusBasicData);
                        ///CustomerDb.LoadCustomerData(CurrentPrescription.Customer);
                    }
                    HisApiBase.CloseCom();
                }
            }
            else
            {
                MainWindow.Instance.SetCardReaderStatus("安全模組未認證");
            }
        }

        private void MedicalNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox t && CurrentPrescription.IsGetIcCard) Seq.MedicalNumber = t.Text;
        }

        private void ReadCustomerTreatRecord()
        {
            if (CurrentPrescription.IsGetIcCard)
            {
                var c = new CustomerSelectWindow(CurrentPrescription.Customer.IcCard.IcNumber, 3);
                if (string.IsNullOrEmpty(CurrentPrescription.Customer.IcCard.MedicalNumber) &&
                    !string.IsNullOrEmpty(MedicalNumber.Text.Trim()))
                    CurrentPrescription.Customer.IcCard.MedicalNumber = MedicalNumber.Text;
                if (!string.IsNullOrEmpty(CurrentPrescription.Customer.IcCard.MedicalNumber) &&
                    string.IsNullOrEmpty(MedicalNumber.Text.Trim()))
                    MedicalNumber.Text = CurrentPrescription.Customer.IcCard.MedicalNumber;
                CheckChronicExist();
            }
            else
            {
                ShowCustomerSelectionWindow(new CustomerSelectWindow(string.Empty, 0));
            }
        }

        private void PrescriptionCopy(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGridRow)?.Item;
            if (CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection.Contains(selectedItem))
            {
                if (((CustomerHistoryMaster) selectedItem).Type.Equals(SystemType.HIS))
                {
                    var tmpCustomer = CurrentPrescription.Customer.DeepCloneViaJson();
                    var declareCopied = new DeclareData(); ///PrescriptionDB.GetDeclareDataById(((CustomerHistoryMaster)selectedItem).CustomerHistoryDetailId);
                    if (declareCopied is null)
                    {
                        MessageWindow.ShowMessage("處方資料異常，複製失敗。",MessageType.ERROR);
                        
                        return;
                    }
                    var isGetIcCard = CurrentPrescription.IsGetIcCard;
                    CurrentPrescription = declareCopied.Prescription;
                    CurrentPrescription.IsGetIcCard = isGetIcCard;
                    CurrentPrescription.Customer = tmpCustomer;
                    CurrentPrescription.Treatment.AdjustDate = DateTime.Today;
                    if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id))
                    {
                        ///CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id)[0].ICD10;
                        if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id))
                        {
                            ///CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id)[0].ICD10;
                        }
                    }
                }
            }
        }
        private void SetSubmmitButton() {
            if (IsSendToServer is null || ButtonDeclareRegister is null || DeclareSubmit is null)
                return;
            int caseType = 0;
            bool isAdjustDayToday = CurrentPrescription.Treatment.AdjustDate.Date == DateTime.Today.Date;
            bool isNormalPres = string.IsNullOrEmpty(CurrentPrescription.ChronicTotal) && string.IsNullOrEmpty(CurrentPrescription.ChronicSequence);
            if ( string.IsNullOrEmpty(_currentDecMasId) && isAdjustDayToday && isNormalPres)
                caseType = 1; //一般處方
            else if (!string.IsNullOrEmpty(_currentDecMasId) && isAdjustDayToday && !isNormalPres)
                caseType = 2; //調劑日今天 預約慢箋
            else if (!string.IsNullOrEmpty(_currentDecMasId) && !isAdjustDayToday && !isNormalPres)
                caseType = 3; //調劑日明天以後 預約慢箋
            else if (string.IsNullOrEmpty(_currentDecMasId) && isAdjustDayToday && !isNormalPres)
                caseType = 4; //調劑日今天 第一次慢箋
            else if (string.IsNullOrEmpty(_currentDecMasId) && !isAdjustDayToday && !isNormalPres)
                caseType = 5; //調劑日明天以後 第一次慢箋

            if (!string.IsNullOrEmpty(_currentDecMasId) && isAdjustDayToday && isNormalPres) {
                MessageWindow.ShowMessage("不可將預約慢箋改為一般箋\r\n請取消後重新登錄處方",MessageType.ERROR);
                
            }
            switch (caseType) {
                case 1:
                    IsSendToServer.IsEnabled = false;
                    IsSendToServer.IsChecked = false;
                    ButtonDeclareRegister.IsEnabled = false;
                    DeclareSubmit.IsEnabled = true; 
                    break;
                case 2:
                    IsSendToServer.IsEnabled = true;
                    IsSendToServer.IsChecked = false; 
                    ButtonDeclareRegister.IsEnabled = false;
                    DeclareSubmit.IsEnabled = true; 
                    break;
                case 3:
                    IsSendToServer.IsEnabled = true;
                    IsSendToServer.IsChecked = true;
                    ButtonDeclareRegister.IsEnabled = true;
                    DeclareSubmit.IsEnabled = false; 
                    break;
                case 4:
                    IsSendToServer.IsEnabled = true;
                    IsSendToServer.IsChecked = false; 
                    ButtonDeclareRegister.IsEnabled = true;
                    DeclareSubmit.IsEnabled = false; 
                    break;
                case 5:
                    IsSendToServer.IsEnabled = true;
                    IsSendToServer.IsChecked = true;
                    ButtonDeclareRegister.IsEnabled = true;
                    DeclareSubmit.IsEnabled = false;
                    break; 
            }

        }

        private void ButtonCooperativeClinic_Click(object sender, RoutedEventArgs e)
        {
            CooperativePrescriptSelectWindow cooperativePrescriptSelectWindow = new CooperativePrescriptSelectWindow();
            cooperativePrescriptSelectWindow.ShowDialog();
        }

        private void ChronicTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSubmmitButton();
        }

        private void ShowMedicineInformation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PrescriptionMedicines.SelectedIndex == -1 || CurrentPrescription.Medicines.Count == 0) return;
            if (!(CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex] is DeclareMedicine med)) return;
           /// var m = new MedicineInfoWindow(MedicineDb.GetMedicalInfoById(med.Id));
           /// m.Show();
        }

        private void HistoryDetail_ShowMedicineInformation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CusHistoryDetailHis.SelectedIndex == -1 || ((CustomerHistoryMaster) CusHistoryMaster.SelectedItem).HistoryCollection.Count ==0) return;
            if (((CustomerHistoryMaster)CusHistoryMaster.SelectedItem).HistoryCollection[CusHistoryDetailHis.SelectedIndex] is null) return;
            string medicineId = (((CustomerHistoryMaster) CusHistoryMaster.SelectedItem).HistoryCollection[
                CusHistoryDetailHis.SelectedIndex] as CustomerHistoryHis)?.MedId;
           /// var m = new MedicineInfoWindow(MedicineDb.GetMedicalInfoById(medicineId));
           /// m.Show();
        }

        private void SpecialCodeText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t) t.CaretIndex = 0;
        }

        private void PaymentCategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.AdjustCase.Id) && (CurrentPrescription.Treatment.AdjustCase.Id.Equals("2") || CurrentPrescription.Treatment.AdjustCase.Id.Equals("D")))
            {
                if (PaymentCategoryCombo.SelectedIndex == -1)
                {
                    CurrentPrescription.Treatment.PaymentCategory = new NewClass.Prescription.Treatment.PaymentCategory.PaymentCategory();
                    return;
                }
                MessageWindow.ShowMessage("調劑案件為慢性病連續處方箋調劑/藥事居家照護，本欄免填。",MessageType.WARNING);
                
                CurrentPrescription.Treatment.PaymentCategory = new NewClass.Prescription.Treatment.PaymentCategory.PaymentCategory();
                PaymentCategoryCombo.SelectedIndex = -1;
            }
            else
            {
                if (PaymentCategoryCombo.SelectedIndex > -1 && PaymentCategoryCombo.SelectedIndex < PaymentCategories.Count)
                {
                    CurrentPrescription.Treatment.PaymentCategory =
                        PaymentCategories[PaymentCategoryCombo.SelectedIndex];
                }
            }
        }

        private void CommonHospitalList_Click(object sender, RoutedEventArgs e)
        {
            CommonHospitalsWindow c = new CommonHospitalsWindow();
            c.ShowDialog();
            DivisionCombo.Focus();
        }

        private void IsSendToServer_Checked(object sender, RoutedEventArgs e) {
            if (CurrentPrescription.Treatment.AdjustDate.Date == DateTime.Today.Date) {
                MessageWindow.ShowMessage("新慢箋調劑日為今日時\r\n不可傳送藥健康\r\n調劑日自動幫您加一天",MessageType.WARNING);
                
                CurrentPrescription.Treatment.AdjustDate = CurrentPrescription.Treatment.AdjustDate.AddDays(1);
            }
              
        }

        private void ReleaseHospital_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = sender as AutoCompleteBox;
            if (a?.SelectedItem == null) return;
            var selectedHospital =
                MainWindow.Institutions.SingleOrDefault(h => h.Id.Equals(((Hospital) a.SelectedItem).Id));
            if (selectedHospital == null) return;
            selectedHospital.Common = true;
           /// HospitalDb.UpdateCommonHospitalById(selectedHospital.Id, true);
        }

        private void DivisionCombo_GotFocus(object sender, RoutedEventArgs e)
        {
            //AutomationPeer button1AP = UIElementAutomationPeer.CreatePeerForElement(DivisionCombo);
            //(button1AP.GetPattern(PatternInterface.Invoke) as IInvokeProvider).Invoke();
        }

        private void ReleaseHospital_KeyDown(object sender, KeyEventArgs e)
        {
            if (ReleaseHospital.Text.Length <= 10) return;
            //CurrentPrescription.Treatment.MedicalInfo.Hospital.Division = NewFunction.CheckHospitalNameContainsDivision(CurrentPrescription.Treatment.MedicalInfo.Hospital.Name);
        }
    }
}