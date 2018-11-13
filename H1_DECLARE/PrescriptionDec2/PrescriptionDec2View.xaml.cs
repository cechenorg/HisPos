using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare;
using His_Pos.HisApi;
using Visibility = System.Windows.Visibility;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using His_Pos.AbstractClass;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.Class.DiseaseCode;
using His_Pos.Class.Person;
using His_Pos.Class.SpecialCode;
using His_Pos.Class.StoreOrder;
using His_Pos.Properties;
using His_Pos.Struct.IcData;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataGrid = System.Windows.Controls.DataGrid;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    ///     PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        #region View相關變數
        private string _selectedMedId;
        private readonly bool _isFirst = true;
        private bool _isChanged;
        public bool CustomerSelected;
        private string _cardStatus;
        public string CardStatus
        {
            get => _cardStatus;
            set
            {
                _cardStatus = value;
                NotifyPropertyChanged(nameof(CardStatus));
            }
        }
        public static PrescriptionDec2View Instance;
        public CustomerHistoryMaster CurrentCustomerHistoryMaster { get; set; }
        private SystemType _cusHhistoryFilterCondition = SystemType.ALL;
        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    !((obj as DeclareMedicine)?.Id is null) && (((DeclareMedicine)obj).Id.ToLower().Contains(searchText.ToLower())
                                                                || ((DeclareMedicine)obj).ChiName.ToLower().Contains(searchText.ToLower()) ||
                                                                ((DeclareMedicine)obj).EngName.ToLower().Contains(searchText.ToLower()));
            }
        }
        public IcErrorCodeWindow icErrorWindow;
        #endregion

        #region 健保卡作業相關變數
        public bool IsMedicalNumberGet;//是否取得就醫序號
        public int GetMedicalNumberErrorCode = 0;
        public readonly byte[] BasicDataArr = new byte[72];
        public BasicData CusBasicData;
        public SeqNumber Seq;//取得之就醫序號資料
        private readonly List<string> _prescriptionSignatureList = new List<string>();//處方簽章
        public ObservableCollection<TreatmentDataNoNeedHpc> TreatRecCollection { get; set; } = new ObservableCollection<TreatmentDataNoNeedHpc>();//就醫紀錄
        public ObservableCollection<Customer> CustomerCollection { get; set; } = new ObservableCollection<Customer>();
        #endregion

        #region 處方相關變數
        public static string IndexViewDecMasId = string.Empty;
        private Prescription _currentPrescription = new Prescription();
        private DeclareData _currentDeclareData;
        private string _currentDecMasId = string.Empty;
        private string _firstTimeDecMasId = string.Empty;
        private int _prescriptionCount;

        public int PrescriptionCount
        {
            get => _prescriptionCount;
            set
            {
                _prescriptionCount = value;
            }
        }

        public bool IsSend; 
        public ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData> PrescriptionSendData = new ObservableCollection<ChronicSendToServerWindow.PrescriptionSendData>();
        public Prescription CurrentPrescription
        {
            get => _currentPrescription;
            set
            {
                _currentPrescription = value;
                NotifyPropertyChanged("CurrentPrescription");
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
                CurrentPrescription.Treatment.Copayment.Point = _copayment;
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
        public ObservableCollection<Hospital> Hospitals { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }
        public ObservableCollection<TreatmentCase> TreatmentCases { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategories { get; set; }
        public ObservableCollection<Copayment> Copayments { get; set; }
        public ObservableCollection<AdjustCase> AdjustCases { get; set; }
        public ObservableCollection<MedicalPersonnel> MedicalPersonnels { get; set; }
        public ObservableCollection<Usage> Usages { get; set; }
        public ObservableCollection<Product> DeclareMedicines { get; set; }
        public ObservableCollection<SpecialCode> SpecialCodes { get; set; }
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
            SetCardStatusContent(MainWindow.CardReaderStatus);
        }
        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<Product>();
            TreatmentCases = new ObservableCollection<TreatmentCase>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(Instance);
            loadingWindow.Show();
        }
        #endregion
        
        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (TreatmentDate.Text.Contains(" "))
            {
                var m = new MessageWindow("就醫日期錯誤", MessageType.ERROR, true);
                m.Show();
                return;
            }

            if (AdjustDate.Text.Contains(" "))
            {
                var m = new MessageWindow("調劑日期錯誤", MessageType.ERROR, true);
                m.Show();
                return;
            }
            IsSend = false; 
            ErrorMssageWindow err;
            CurrentPrescription.EList.Error = new List<Error>();
            CurrentPrescription.EList.Error = CurrentPrescription.CheckPrescriptionData();

            var medDays = 0;
            foreach (var med in CurrentPrescription.Medicines)
            {
                if (med is DeclareMedicine)
                {
                    if (string.IsNullOrEmpty(((IProductDeclare)(DeclareMedicine)med).Days))
                    {
                        var messageWindow = new MessageWindow(med.Id + "的給藥日份不可為空", MessageType.ERROR, true);
                        messageWindow.ShowDialog();
                        return;
                    }
                    if (int.Parse(((IProductDeclare)(DeclareMedicine)med).Days) > medDays)
                        medDays = int.Parse(((IProductDeclare)(DeclareMedicine)med).Days);
                }
            }
            CurrentPrescription.Treatment.MedicineDays = medDays.ToString();

            if (CurrentPrescription.EList.Error.Count == 0)
            {
                InsertPrescription();
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
                    InsertPrescription();
                }
            }
        }
        private void InsertPrescription()
        {
            MessageWindow m;
            _currentDeclareData = new DeclareData(CurrentPrescription);
            var declareDb = new DeclareDb();
            string medEntryName;
            string medServiceName;
            switch (CurrentPrescription.Treatment.MedicalInfo.Hospital.Id)
            {
                case "3532016964":
                case "3532052317":
                case "3532071956":
                case "3531066077":
                case "3532082253":
                    medEntryName = "骨科調劑耗用";
                    medServiceName = "骨科藥服費";
                    break;
                default:
                    medEntryName = "調劑耗用";
                    medServiceName = "藥服費";
                    break;
            }

            var declareTrade = new DeclareTrade(MainWindow.CurrentUser.Id, SelfCost.ToString(), Deposit.ToString(), Charge.ToString(), Copayment.ToString(), Pay.ToString(), Change.ToString(), "現金", CurrentPrescription.Customer.Id);
            
            if (CurrentPrescription.Treatment.AdjustCase.Id != "2" && string.IsNullOrEmpty(_currentDecMasId) && DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.TreatmentDate,true) == DateTimeExtensions.ConvertToTaiwanCalender(DateTime.Now, true))
            {//一般處方
                _firstTimeDecMasId = declareDb.InsertDeclareData(_currentDeclareData);

                ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", _firstTimeDecMasId);
                ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", _firstTimeDecMasId);
                ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", _firstTimeDecMasId);
                ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", _firstTimeDecMasId);
                if (medEntryName != "骨科調劑耗用")
                {
                    
                    var medTotalPrice = 0.00;
                    foreach (var med in _currentDeclareData.Prescription.Medicines)
                    {
                        switch (med)
                        {
                            case DeclareMedicine declare:
                                medTotalPrice += double.Parse(ProductDb.GetBucklePrice(declare.Id, ((IProductDeclare)declare).Amount.ToString()));
                                break;
                            case PrescriptionOTC otc:
                                medTotalPrice += double.Parse(ProductDb.GetBucklePrice(med.Id, ((IProductDeclare)otc).Amount.ToString()));
                                break;
                        }
                    }
                    ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _firstTimeDecMasId);
                    declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _firstTimeDecMasId);//庫存扣庫
                }
                declareDb.UpdateDeclareFile(_currentDeclareData);
            }
            else if (CurrentPrescription.Treatment.AdjustCase.Id == "2" && !string.IsNullOrEmpty(_currentDecMasId))
            {//第2次以後的慢性處方
                if (IsSendToServer.IsChecked != null && (bool)IsSendToServer.IsChecked)//選擇傳送藥健康
                { 
                    var chronicSendToServerWindow = new ChronicSendToServerWindow(CurrentPrescription.Medicines, _currentDecMasId);
                    chronicSendToServerWindow.ShowDialog();
                    if (!IsSend) return;
                }
                if (IsSend)
                {//確定傳送
                    var storId = StoreOrderDb.SaveOrderDeclareData(_currentDecMasId, PrescriptionSendData);
                    //送到singde
                    StoreOrderDb.SendDeclareOrderToSingde(_currentDecMasId, storId, _currentDeclareData, declareTrade, PrescriptionSendData);
                }
                if (DeclareSubmit.Content.ToString() == "調劑" && DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.AdjustDate, true) == DateTimeExtensions.ConvertToTaiwanCalender(DateTime.Now,true))
                {
                    ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", _currentDecMasId);
                    if (medEntryName != "骨科調劑耗用")
                    {
                        var medTotalPrice = 0.00;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                            medTotalPrice += double.Parse(ProductDb.GetBucklePrice(med.Id, ((IProductDeclare)(DeclareMedicine)med).Amount.ToString()));
                        }
                        ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _currentDecMasId);
                        declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _currentDecMasId);//庫存扣庫
                    }
                    declareDb.UpdateDeclareFile(_currentDeclareData);
                }

                _currentDeclareData.DecMasId = _currentDecMasId;
                declareDb.UpdateDeclareData(_currentDeclareData);//更新慢箋
                ChronicDb.UpdateChronicData(_currentDecMasId);//重算預約慢箋 
                if (CurrentPrescription.ChronicSequence == CurrentPrescription.ChronicTotal && DeclareSubmit.Content.ToString() == "調劑")
                {//若為最後一次 則再算出下一批慢性
                    declareDb.SetNewGroupChronic(_currentDecMasId);
                }
            }
            else if (CurrentPrescription.Treatment.AdjustCase.Id == "2" && string.IsNullOrEmpty(_currentDecMasId))//第1次的新慢性處方
            {
                if(string.IsNullOrEmpty(_firstTimeDecMasId))
                    _firstTimeDecMasId = declareDb.InsertDeclareData(_currentDeclareData);
                if (IsSendToServer.IsChecked != null && (bool)IsSendToServer.IsChecked)
                { 
                    var chronicSendToServerWindow = new ChronicSendToServerWindow(CurrentPrescription.Medicines, _firstTimeDecMasId);
                    chronicSendToServerWindow.ShowDialog();
                    if (!IsSend) return;
                }
                if (IsSend)
                {
                    var storId = StoreOrderDb.SaveOrderDeclareData(_firstTimeDecMasId, PrescriptionSendData);
                    //送到singde
                    StoreOrderDb.SendDeclareOrderToSingde(_firstTimeDecMasId, storId, _currentDeclareData, declareTrade, PrescriptionSendData);
                }

                if (DeclareSubmit.Content.ToString() == "調劑" && DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.AdjustDate, true) == DateTimeExtensions.ConvertToTaiwanCalender(DateTime.Now,true))
                {
                    ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", _firstTimeDecMasId);
                    ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", _firstTimeDecMasId);
                    if (medEntryName != "骨科調劑耗用")
                    {
                        var medTotalPrice = 0.0;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                            switch (med)
                            {
                                case DeclareMedicine declare:
                                    medTotalPrice += double.Parse(ProductDb.GetBucklePrice(declare.Id, ((IProductDeclare)declare).Amount.ToString()));
                                    break;
                                case PrescriptionOTC otc:
                                    medTotalPrice += double.Parse(ProductDb.GetBucklePrice(otc.Id, ((IProductDeclare)otc).Amount.ToString()));
                                    break;
                            }
                        }
                        ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _firstTimeDecMasId);
                        declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _firstTimeDecMasId);//庫存扣庫
                    }
                    declareDb.UpdateDeclareFile(_currentDeclareData);
                }
               
                    declareDb.CheckPredictChronicExist(_currentDecMasId); //刪除預約慢箋

                var start = Convert.ToInt32(CurrentPrescription.ChronicSequence) + 1;
                var end = Convert.ToInt32(CurrentPrescription.ChronicTotal);
                var intDecMasId = Convert.ToInt32(_firstTimeDecMasId);
                for (var i = start; i <= end; i++)
                {
                    declareDb.SetSameGroupChronic(intDecMasId.ToString(), i.ToString());
                    intDecMasId++;
                }
            }
            else
            {
                m = new MessageWindow("處方登錄失敗 請確認調劑日期是否正確", MessageType.ERROR, true);
                m.ShowDialog();
            }
            if (CurrentPrescription.IsGetIcCard)
            {
                //var loading = new LoadingWindow();
                //loading.LoginIcData(Instance);
                m = new MessageWindow("處方登錄成功", MessageType.SUCCESS, true);
                m.ShowDialog();
            }
            else
            {
                //icErrorWindow = new IcErrorCodeWindow(false, Enum.GetName(typeof(ErrorCode), GetMedicalNumberErrorCode));
                //icErrorWindow.ShowDialog();
                //var loading = new LoadingWindow();
                //if (!string.IsNullOrEmpty(icErrorWindow.SelectedItem.Id))
                //    loading.LoginIcData(Instance);
                m = new MessageWindow("處方登錄成功", MessageType.SUCCESS, true);
                m.ShowDialog();
            } 
            //PrintMedBag(); 印藥袋
            CustomerSelected = false;
            _firstTimeDecMasId = string.Empty;
            ClearPrescription();
            IndexView.IndexView.Instance.InitData();
        }

        #region 每日上傳.讀寫卡相關函數
        public void LogInIcData()
        {
            var cs = new ConvertData();
            List<IcPrescriptData> icPrescripList = new List<IcPrescriptData>();
            foreach (var med in CurrentPrescription.Medicines)
            {
                if (!(med is DeclareMedicine)) continue;
                if (!((DeclareMedicine) med).PaySelf)
                {
                    icPrescripList.Add(new IcPrescriptData((DeclareMedicine)med));
                }
            }
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
                var res = HisApiBase.hisWritePrescriptionSign(pDateTime, pPatientId, pPatientBitrhDay, pDataInput, icData, ref strLength);
                if (res == 0)
                    _prescriptionSignatureList.Add(cs.ByToString(icData, 0, 40));
                else
                    break;
            }
        }

        public void CreatIcUploadData()
        {
            var medicalDatas = new List<MedicalData>();
            var icData = new IcData(Seq, _currentPrescription, CusBasicData, _currentDeclareData);
            var mainMessage = new MainMessage(icData);
            var headerMessage = new Header {DataFormat = "1"};
            var icRecord = new IcRecord(headerMessage, mainMessage);

            for (var i = 0; i < CurrentPrescription.Medicines.Count; i++)
            {
                if (_currentDeclareData.DeclareDetails[i].MedicalOrder.Equals("9"))
                    continue;
                var medicalData = new MedicalData
                {
                    MedicalOrderTreatDateTime = Seq.TreatDateTime,
                    MedicalOrderCategory = _currentDeclareData.DeclareDetails[i].MedicalOrder,
                    TreatmentProjectCode = _currentDeclareData.DeclareDetails[i].MedicalId,
                    Usage = _currentDeclareData.DeclareDetails[i].Usage,
                    Days = _currentDeclareData.DeclareDetails[i].Days.ToString(),
                    TotalAmount = _currentDeclareData.DeclareDetails[i].Total.ToString(),
                    PrescriptionSignature = _prescriptionSignatureList[i],
                };
                if (!string.IsNullOrEmpty(_currentDeclareData.DeclareDetails[i].Position))
                    medicalData.TreatmentPosition = _currentDeclareData.DeclareDetails[i].Position;
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
            }
            icRecord.MainMessage.MedicalMessageList = medicalDatas;
            icRecord.SerializeObject();
            var d = new DeclareDb();
            d.InsertDailyUpload(icRecord.SerializeObject());
        }
        //異常上傳
        public void CreatIcErrorUploadData()
        {
            var medicalDatas = new List<MedicalData>();
            if(icErrorWindow.SelectedItem == null) return;
            var icData = new IcData(CurrentPrescription,icErrorWindow.SelectedItem,_currentDeclareData);
            var mainMessage = new MainMessage(icData);
            var headerMessage = new Header { DataFormat = "2" };
            var icRecord = new IcRecord(headerMessage, mainMessage);

            for (var i = 0; i < CurrentPrescription.Medicines.Count; i++)
            {
                if (_currentDeclareData.DeclareDetails[i].MedicalOrder.Equals("9"))
                    continue;
                var medicalData = new MedicalData
                {
                    MedicalOrderTreatDateTime = icData.TreatmentDateTime,
                    MedicalOrderCategory = _currentDeclareData.DeclareDetails[i].MedicalOrder,
                    TreatmentProjectCode = _currentDeclareData.DeclareDetails[i].MedicalId,
                    Usage = _currentDeclareData.DeclareDetails[i].Usage,
                    Days = _currentDeclareData.DeclareDetails[i].Days.ToString(),
                    TotalAmount = _currentDeclareData.DeclareDetails[i].Total.ToString(),
                };
                if (!string.IsNullOrEmpty(_currentDeclareData.DeclareDetails[i].Position))
                    medicalData.TreatmentPosition = _currentDeclareData.DeclareDetails[i].Position;
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
            }
            icRecord.MainMessage.MedicalMessageList = medicalDatas;
            icRecord.SerializeObject();
            var d = new DeclareDb();
            d.InsertDailyUpload(icRecord.SerializeObject());
        }
        #endregion
        private void PrintMedBag()
        {
            //var messageBoxResult = MessageBox.Show("是否列印一藥一袋?","藥袋列印模式", MessageBoxButton.YesNo);
            //var defaultMedBag = MedBagDb.GetDefaultMedBagData(messageBoxResult == MessageBoxResult.Yes ? MedBagMode.SINGLE : MedBagMode.MULTI);
            //File.WriteAllText(ReportService.ReportPath, string.Empty);
            //File.AppendAllText(ReportService.ReportPath, ReportService.SerializeObject<Report>(ReportService.CreatReport(defaultMedBag, CurrentPrescription)));
            //for (var i = 0; i < CurrentPrescription.Medicines.Count; i++)
            //{
            //    ReportService.CreatePdf(defaultMedBag,i);
            //}
            var rptViewer = new ReportViewer();
            rptViewer.LocalReport.DataSources.Clear();
            var medBagMedicines = new ObservableCollection<MedBagMedicine>();
            foreach (var m in CurrentPrescription.Medicines)
            {
                if(m is DeclareMedicine)
                    medBagMedicines.Add(new MedBagMedicine((DeclareMedicine)m));
                else
                    medBagMedicines.Add(new MedBagMedicine((PrescriptionOTC)m));
            }
            var json = JsonConvert.SerializeObject(medBagMedicines);
            var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("MedicineDataSet", dataTable));
            var parameters = new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id", MainWindow.CurrentPharmacy.Name + "(" + MainWindow.CurrentPharmacy.Id + ")"),
                new ReportParameter("PharmacyAddress", MainWindow.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", MainWindow.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson", CurrentPrescription.Pharmacy.MedicalPersonnel.Name),
                new ReportParameter("PatientName", CurrentPrescription.Customer.Name),
                new ReportParameter("PatientGender_Birthday", CurrentPrescription.Customer.Gender ? "男" : "女" + "/" + DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Customer.Birthday, true)),
                new ReportParameter("TreatmentDate",DateTimeExtensions.ConvertToTaiwanCalender(CurrentPrescription.Treatment.TreatmentDate, true)),
                new ReportParameter("Hospital", CurrentPrescription.Treatment.MedicalInfo.Hospital.Name),
                new ReportParameter("PaySelf", SelfCost.ToString()),
                new ReportParameter("ServicePoint", _currentDeclareData.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", _currentDeclareData.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint", _currentDeclareData.CopaymentPoint.ToString()),
                new ReportParameter("HcPoint", _currentDeclareData.DeclarePoint.ToString()),
                new ReportParameter("MedicinePoint", _currentDeclareData.CopaymentPoint.ToString())
            };
            rptViewer.LocalReport.ReportPath = @"..\..\RDLC\MedBagReport.rdlc";
            rptViewer.LocalReport.SetParameters(parameters);
            rptViewer.LocalReport.Refresh();
            rptViewer.ProcessingMode = ProcessingMode.Local;
            var loadingWindow = new LoadingWindow();
            loadingWindow.Show();
            loadingWindow.PrintMedbag(rptViewer, Instance);
        }

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            if (!(sender is AutoCompleteBox medicineCodeAuto)) return;

            var result = DeclareMedicines.Where(x =>
                x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) ||
                x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
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
                    ((ObservableCollection<object>)medicineCodeAuto.ItemsSource).Count != 0 &&
                    medicineCodeAuto.Text.Length >= 4)
                    medicineCodeAuto.SelectedItem = ((ObservableCollection<object>)medicineCodeAuto.ItemsSource)?[0];
                else
                    return;
            }
            PrescriptionOTC prescriptionOtc;
            DeclareMedicine declareMedicine;
            int currentRow;
            currentRow = GetCurrentRowIndex(sender);
            if (string.IsNullOrEmpty(((IProductDeclare)medicineCodeAuto.SelectedItem).Forms))
            {
                prescriptionOtc = (PrescriptionOTC)((PrescriptionOTC)medicineCodeAuto.SelectedItem)?.Clone();
                if (CurrentPrescription.Medicines.Count > 0)
                {
                    if (CurrentPrescription.Medicines.Count == currentRow)
                    {
                        var m = CurrentPrescription.Medicines[currentRow - 1];
                        Debug.Assert(prescriptionOtc != null, nameof(prescriptionOtc) + " != null");
                        prescriptionOtc.Usage = string.Empty;
                        prescriptionOtc.Dosage = string.Empty;
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
                declareMedicine = (DeclareMedicine)((DeclareMedicine)medicineCodeAuto.SelectedItem)?.Clone();
                if (declareMedicine != null && declareMedicine.Id.EndsWith("00"))
                    declareMedicine.Position = "PO";
                if (CurrentPrescription.Medicines.Count > 0)
                {
                    if (CurrentPrescription.Medicines.Count == currentRow)
                    {
                        var m = CurrentPrescription.Medicines[currentRow - 1];
                        if (m is DeclareMedicine med)
                        {
                            Debug.Assert(declareMedicine != null, nameof(declareMedicine) + " != null");
                            declareMedicine.UsageName = ((IProductDeclare)med).Usage;
                            declareMedicine.Dosage = ((IProductDeclare)med).Dosage;
                            declareMedicine.Days = ((IProductDeclare)med).Days;
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
            PrescriptionMedicines.CurrentCell = new DataGridCellInfo(
                PrescriptionMedicines.Items[currentRow], DosageText);
            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
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
                    if(string.IsNullOrEmpty(t.Text)) continue;
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
                PrescriptionMedicines.Items[currentRow+1], MedicineId);
            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell ?? throw new InvalidOperationException(), 0);
            while (!(firstChild is AutoCompleteBox))
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }
            firstChild.Focus();
            e.Handled = false;
            SendKeys.SendWait("~");
            
        }

        private void MoveFocusNext(object sender)
        {
            if(sender is TextBox box)
                box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (PrescriptionMedicines.CurrentCell.Column is null) return;

            var focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }
                    if ((child is TextBox || child is AutoCompleteBox || child is CheckBox || child is TextBlock))
                        break;
                }
                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                focusedCell = PrescriptionMedicines.CurrentCell.Column.GetCellContent(PrescriptionMedicines.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            while (firstChild is ContentPresenter)
            {
                firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
            }

            if ((firstChild is TextBox || firstChild is AutoCompleteBox ) && firstChild.Focusable )
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
                    ((DeclareMedicine)CurrentPrescription.Medicines[currentRow]).Usage = Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                    if (((IProductDeclare)(DeclareMedicine)CurrentPrescription.Medicines[currentRow]).Usage != null)
                        t.Text = ((IProductDeclare)(DeclareMedicine)CurrentPrescription.Medicines[currentRow]).Usage;
                }
                else
                {
                    ((PrescriptionOTC)CurrentPrescription.Medicines[currentRow]).Usage = Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text)).Name;
                    if (((IProductDeclare)(PrescriptionOTC)CurrentPrescription.Medicines[currentRow]).Usage != null)
                        t.Text = ((IProductDeclare)(PrescriptionOTC)CurrentPrescription.Medicines[currentRow]).Usage;
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
                ClearMedicine((DeclareMedicine)CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
            else if(CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex] is PrescriptionOTC)
                ClearPrescriptionOtc((PrescriptionOTC)CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
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
            med.Dosage = string.Empty;
        }

        private void ClearPrescriptionOtc(PrescriptionOTC med)
        {
            med.PaySelf = false;
            med.Cost = 0;
            med.TotalPrice = 0;
            med.Amount = 0;
            med.CountStatus = string.Empty;
            med.FocusColumn = string.Empty;
            med.Usage = string.Empty;
            med.Days = string.Empty;
            med.Position = string.Empty;
            med.Source = string.Empty;
            med.Dosage = string.Empty;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in CurrentPrescription.Medicines)
            {
                if (medicine is DeclareMedicine declareMedicine)
                {
                    if (!declareMedicine.PaySelf)
                        medicinesHcCost += declareMedicine.TotalPrice;
                    else
                        medicinesSelfCost += declareMedicine.TotalPrice;
                    purchaseCosts += declareMedicine.Cost * declareMedicine.Amount;
                }
                else if(medicine is PrescriptionOTC otc)
                {
                    medicinesSelfCost += otc.TotalPrice;
                    purchaseCosts += otc.Cost * otc.Amount;
                }
                
            }
            SelfCost = Convert.ToInt16(Math.Ceiling(medicinesSelfCost));//自費金額
            Copayment = CountCopaymentCost(medicinesHcCost);//部分負擔
            MedProfit = (medicinesHcCost + medicinesSelfCost - purchaseCosts);//藥品毛利
        }

        private void CountCharge()
        {
            Charge = Deposit + SelfCost + Copayment;
            Change = _pay - Charge;
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

        private void MedTotalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CountMedicinesCost();
        }

        private void NullTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox t && string.IsNullOrEmpty(t.Text))
                t.Text = "0";
        }

        private void LoadCustomerDataButtonClick(object sender, RoutedEventArgs e)
        {
            var loading = new LoadingWindow();
            loading.Show();
            loading.LoadIcData(Instance);
        }

        public void SetCardStatusContent(string content)
        {
            CardStatus = content;
        }

        public void CheckPatientGender()
        {
            if (CurrentPrescription.Customer.IcNumber.IndexOf("1", StringComparison.Ordinal) == 1)
                CurrentPrescription.Customer.Gender = true;
            else if(CurrentPrescription.Customer.IcNumber.IndexOf("2", StringComparison.Ordinal) == 1)
            {
                CurrentPrescription.Customer.Gender = false;
            }
            else
            {
                CurrentPrescription.Customer.Gender = true;
            }
        }

        private bool CusHistoryFilter(object item)
        {
            if (_cusHhistoryFilterCondition == SystemType.ALL)
                return true;
            return ((CustomerHistoryMaster)item).Type == _cusHhistoryFilterCondition;
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

        private void ReleaseHospital_Populating(object sender, PopulatingEventArgs e) {
            if (Hospitals is null) Hospitals = HospitalDb.GetData(); 
            var tempCollection = new ObservableCollection<Hospital>(Hospitals.Where(x => x.Id.Contains(ReleaseHospital.Text)).Take(50).ToList());
            ReleaseHospital.ItemsSource = tempCollection;
            ReleaseHospital.PopulateComplete();
        }

        private void AdjustCaseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(AdjustCaseCombo.SelectedItem == null) return;
            if (((AdjustCase) AdjustCaseCombo.SelectedItem).Id.Equals("0"))
            {
                DeclareSubmit.Visibility = Visibility.Collapsed;
                NotDeclareSubmit.Visibility = Visibility.Visible;
                CurrentPrescription.Declare = false;
            }
            else
            {
                DeclareSubmit.Visibility = Visibility.Visible;
                NotDeclareSubmit.Visibility = Visibility.Collapsed;
                CurrentPrescription.Declare = true;
            }

            if (((AdjustCase) AdjustCaseCombo.SelectedItem).Id.Equals("2"))
                CopaymentCombo.SelectedItem = MainWindow.Copayments.SingleOrDefault(c => c.Id.Equals("I22"));
            else if(((AdjustCase)AdjustCaseCombo.SelectedItem).Id.Equals("1"))
                CopaymentCombo.SelectedItem = MainWindow.Copayments.SingleOrDefault(c => c.Id.Equals("I20"));
            IsSendToServer.IsChecked = false;
            IsSendToServer.IsEnabled = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && AdjustDate.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            IsSendToServer.IsChecked = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && AdjustDate.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
        }

        private void ChronicSequence_TextChanged(object sender, TextChangedEventArgs e) {
            var t = sender as TextBox;
            var tmpMedicalNumber = MedicalNumber.Text;
            if (string.IsNullOrEmpty(t.Text))
            {
                AdjustCaseCombo.SelectedIndex = 0;
                TreatmentCaseCombo.SelectedItem = TreatmentCases.SingleOrDefault(c => c.Name.Equals("一般案件"));
                return;
            }

            AdjustCaseCombo.SelectedItem = AdjustCases.SingleOrDefault(a => a.Name.Contains("慢性病連續處方調劑"));
            TreatmentCaseCombo.SelectedItem = TreatmentCases.SingleOrDefault(c => c.Name.Equals("慢性病"));
            if (!int.TryParse(t.Text, out var seqence)) return;
            if (seqence > 1)
            {
                var myBinding = new Binding("CurrentPrescription.OriginalMedicalNumber");
                BindingOperations.SetBinding(MedicalNumber, TextBox.TextProperty, myBinding);
                MedicalNumber.Text = tmpMedicalNumber;
                CurrentPrescription.Customer.IcCard.MedicalNumber = "IC0" + t.Text;
                CurrentPrescription.OriginalMedicalNumber = tmpMedicalNumber;
            }
            else
            {
                var myBinding = new Binding("CurrentPrescription.Customer.IcCard.MedicalNumber");
                BindingOperations.SetBinding(MedicalNumber, TextBox.TextProperty, myBinding);
                MedicalNumber.Text = tmpMedicalNumber;
            }
        }
         
        public void SetValueByDecMasId(string decMasId) {
            if (decMasId is null) return;
            _currentDecMasId = decMasId;
            var prescription = PrescriptionDB.GetDeclareDataById(decMasId).Prescription;
            CurrentPrescription = prescription;
            DivisionCombo.SelectedItem =
                Divisions.SingleOrDefault(d => d.Id.Equals(prescription.Treatment.MedicalInfo.Hospital.Division.Id));
            TreatmentCaseCombo.SelectedItem =
                TreatmentCases.SingleOrDefault(t => t.Id.Equals(prescription.Treatment.MedicalInfo.TreatmentCase.Id));
            var diseaseCode = CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id;
            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            diseaseCode = CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id;
            if (!string.IsNullOrEmpty(diseaseCode))
                CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(diseaseCode)[0].ICD10;
            if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id))
            {
                foreach (var s in SpecialCodes)
                {
                    if (s.Id.Contains(CurrentPrescription.Treatment.MedicalInfo.SpecialCode.Id))
                        SpecialCodeCombo.SelectedItem = s;
                }
            }

            //}
            //    SpecialCodeCombo.SelectedItem = SpecialCodes.Single(s => s.Id.Equals());
            //DivisionCombo.Text = prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
            //AdjustCaseCombo.Text = prescription.Treatment.AdjustCase.FullName;
            //TreatmentCaseCombo.Text = prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
            //PaymentCategoryCombo.Text = prescription.Treatment.PaymentCategory.FullName;
            //CopaymentCombo.Text = prescription.Treatment.Copayment.FullName;
            //SpecialCodeCombo.Text = prescription.Treatment.MedicalInfo.SpecialCode.Id;
            //ReleaseHospital.Text = prescription.Treatment.MedicalInfo.Hospital.Id;
            //
            //TreatmentDate.Text = DateTimeExtensions.ToSimpleTaiwanDate(prescription.Treatment.TreatmentDate);
            //AdjustDate.Text = DateTimeExtensions.ToSimpleTaiwanDate(prescription.Treatment.AdjustDate); 
            //CurrentPrescription.Medicines = MedicineDb.GetDeclareMedicineByMasId(decMasId);
            //PrescriptionMedicines.ItemsSource = Instance.CurrentPrescription.Medicines;
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

            DependencyObject textBoxDependency = textBox;
            if (textBoxDependency.GetValue(NameProperty) is string  && textBoxDependency.GetValue(NameProperty).Equals("MainDiagnosis"))
            {
                if ((!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id) && textBox.Text.Contains(" "))|| string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    SecondDiagnosis.Focus();
                    return;
                }
            }
            else if(textBoxDependency.GetValue(NameProperty) is string && textBoxDependency.GetValue(NameProperty).Equals("SecondDiagnosis"))
            {
                if (string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    ChronicTotal.Focus();
                    return;
                }
            }
            if (textBox.Text.Length < 3)
            {
                var m = new MessageWindow("請輸入完整疾病代碼", MessageType.WARNING, true);
                m.ShowDialog();
                return;
            }
            if (DiseaseCodeDb.GetDiseaseCodeById(textBox.Text).Count == 1)
            {
                var selectedDiseaseCode = DiseaseCodeDb.GetDiseaseCodeById(textBox.Text)[0].ICD10;
                if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                {
                    var m = new MessageWindow("查無疾病代碼", MessageType.WARNING, true)
                    {
                        Owner = Application.Current.MainWindow
                    };
                    m.ShowDialog();
                    return;
                }
                if (textBoxDependency.GetValue(NameProperty) is string && textBoxDependency.GetValue(NameProperty).Equals("MainDiagnosis"))
                    CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = selectedDiseaseCode;
                else
                {
                    if (selectedDiseaseCode.Id.Equals("查無疾病代碼") && !textBox.Text.Contains(" "))
                    {
                        var m = new MessageWindow("查無疾病代碼", MessageType.WARNING, true)
                        {
                            Owner = Application.Current.MainWindow
                        };
                        m.ShowDialog();
                        return;
                    }
                    if((selectedDiseaseCode.Id.Equals("查無疾病代碼") && textBox.Text.Contains(" ")) || string.IsNullOrEmpty(textBox.Text.Trim()))
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
                var disease = new DiseaseCodeSelectDialog(textBox.Text);
                disease.Show();
            }
        }

        private void Combo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
                            if(!MainWindow.CurrentUser.Id.Equals((HisPerson.SelectedItem as MedicalPersonnel).Id))
                                HisPerson.Focus();
                            else
                                TreatmentCaseCombo.Focus();
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
                            SpecialCodeCombo.Focus();
                            break;
                        case "AdjustCaseCombo":
                            TreatmentCaseCombo.Focus();
                            break;
                        case "SpecialCodeCombo":
                            var nextAutoCompleteBox = new List<AutoCompleteBox>();
                            NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                            nextAutoCompleteBox[0].Focus();
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
                            if (string.IsNullOrEmpty(ChronicTotal.Text) && !int.TryParse(ChronicSequence.Text, out var s))
                            {
                                var m = new MessageWindow("總領藥次數有值，需填寫領藥次數", MessageType.WARNING, true);
                                m.ShowDialog();
                                ChronicTotal.Focus();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ChronicTotal.Text) && int.TryParse(ChronicSequence.Text,out var seq) && seq > 1)
                                {
                                    AdjustCaseCombo.SelectedIndex = 1;
                                    CurrentPrescription.OriginalMedicalNumber = ChronicSequence.Text;
                                    CurrentPrescription.Customer.IcCard.MedicalNumber = "IC0" + ChronicSequence.Text;
                                    SpecialCodeCombo.Focus();
                                }
                            }
                            break;
                        case "ChronicTotal":
                            if (string.IsNullOrEmpty(t.Text))
                                SpecialCodeCombo.Focus();
                            else
                                ChronicSequence.Focus();
                            break;
                        case "MedicalNumber":
                            TreatmentDate.Focus();
                            break;
                        case "TreatmentDate":
                            AdjustDate.Focus();
                            break;
                        case "AdjustDate":
                            MainDiagnosis.Focus();
                            break;
                    }
                    break;
            }
        }

        private void ReleaseHospital_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is AutoCompleteBox a)) return;
            a.Text = a.Text.TrimStart(' ');
            if (e.Key == Key.Enter && a.IsDropDownOpen && a.Text.Length <= 10)
                a.SelectedItem = Hospitals.Where(x => x.Id.Contains(a.Text)).Take(50).ToList()[0];
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
                    ((DeclareMedicine)t).IsBuckle = !((DeclareMedicine)t).IsBuckle;
            }
        }
        private void MedContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (PrescriptionMedicines.SelectedItem == null || CurrentPrescription.Medicines.Count == 0) return;
            var temp = (sender as ContextMenu);
            var menuitem = temp.Items[0] as MenuItem;
            _selectedMedId = ((DeclareMedicine)PrescriptionMedicines.SelectedItem).Id;
            if (((DeclareMedicine)PrescriptionMedicines.SelectedItem).IsBuckle)
                menuitem.Header = "申報不扣庫";
            else
                menuitem.Header = "申報扣庫";
        }

        private void HisPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("MEDICALPERSONNEL", CurrentPrescription.Pharmacy.MedicalPersonnel.IcNumber)
            };
            var t = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetMedicalPersonPrescriptionCount]",parameters);
            PrescriptionCount = int.Parse(t.Rows[0][0].ToString());
        }

        private void NotDeclareSubmit_OnClickSubmit_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageWindow m;
            var declareDb = new DeclareDb();
            _currentDeclareData = new DeclareData(CurrentPrescription);
            var decMasId = declareDb.InsertPrescribeData(_currentDeclareData);
            var totalCost = 0.0;
            var totalPrice = 0;
            foreach (var med in _currentDeclareData.Prescription.Medicines)
            {
                if (med is DeclareMedicine declare)
                {
                    totalCost += double.Parse(ProductDb.GetBucklePrice(((IProductDeclare)declare).ProductId, ((IProductDeclare)declare).Amount.ToString()));
                    totalPrice += int.Parse(Math.Ceiling(((IProductDeclare)declare).TotalPrice).ToString());
                }
                else if (med is PrescriptionOTC otc)
                {
                    totalCost += double.Parse(ProductDb.GetBucklePrice(((IProductDeclare)otc).ProductId, ((IProductDeclare)otc).Amount.ToString()));
                    totalPrice += int.Parse(Math.Ceiling(((IProductDeclare)otc).TotalPrice).ToString());
                }
                
            }
            ProductDb.InsertEntry("配藥收入", totalCost.ToString() , "DecMasId", decMasId);
            ProductDb.InsertEntry("調劑耗用", "-" + totalCost, "DecMasId", decMasId);
            declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", decMasId);//庫存扣庫
            m = new MessageWindow("調劑登錄成功", MessageType.SUCCESS, true);
            m.ShowDialog();
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
            e.Handled = new System.Text.RegularExpressions.Regex("[^0-9]+").IsMatch(e.Text);
        }
        private void AdjustDate_TextChanged(object sender, TextChangedEventArgs e) {
            if (DeclareSubmit == null) return;
            DeclareSubmit.Content = AdjustDate.Text == DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now) ? "調劑" : "預約慢箋";
            IsSendToServer.IsChecked = false;
            if (AdjustCaseCombo.SelectedItem == null) return;
            IsSendToServer.IsEnabled = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && AdjustDate.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            IsSendToServer.IsChecked = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && AdjustDate.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
        }

        private void SelectionStart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (sender)
            {
                case AutoCompleteBox a:
                    var txt = a.Template.FindName("Text", a) as TextBox;
                    if (txt != null)
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
                ShowCustomerSelectionWindow(new CustomerSelectWindow(PatientBirthday.Text.TrimStart('0').Trim().Replace("/",string.Empty), 1));
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
                case Key.Back when CustomerSelected:
                    CustomerSelected = false;
                    CurrentPrescription.Customer = new Customer();
                    break;
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
            if (((Division) c.SelectedItem).Name.Equals("牙科"))
                TreatmentCaseCombo.SelectedItem = TreatmentCases.SingleOrDefault(t => t.Name.Equals("牙醫其他專案"));
            else
                TreatmentCaseCombo.SelectedItem = TreatmentCases.SingleOrDefault(t => t.Name.Equals("一般案件"));
        }

        private void ReloadCardReader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var res = HisApiBase.csVerifySAMDC();
            string cardReaderStatus;
            if (res == 0)
            {
                SetCardStatusContent("安全模組認證成功");
                res = HisApiBase.hisGetCardStatus(2);
                cardReaderStatus = MainWindow.GetEnumDescription((CardStatusReturnCode) res);
                SetCardStatusContent(cardReaderStatus);
                if (cardReaderStatus.Contains("成功"))
                    CurrentPrescription.IsGetIcCard = true;
            }
            else
            {
                res = HisApiBase.hisGetCardStatus(2);
                cardReaderStatus = MainWindow.GetEnumDescription((ErrorCode)res);
                SetCardStatusContent(cardReaderStatus);
            }
        }

        public void ReadTreatRecord()
        {
            var thread = new Thread(() =>
            {
                var strLength = 296;
                var icData = new byte[296];
                var cs = new ConvertData();
                var cTreatItem = cs.StringToBytes("AF\0", 3);
                //新生兒就醫註記,長度兩個char
                var cBabyTreat = TreatRecCollection.Count > 0 ? cs.StringToBytes(TreatRecCollection[0].NewbornTreatmentMark + "\0", 3) : cs.StringToBytes(" ", 2);
                //補卡註記,長度一個char
                var cTreatAfterCheck = new byte[] { 1 };
                var res = HisApiBase.hisGetSeqNumber256(cTreatItem, cBabyTreat, cTreatAfterCheck, icData, ref strLength);
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
                res = HisApiBase.hisGetTreatmentNoNeedHPC(icData, ref strLength);
                if (res == 0)
                {
                    var startIndex = 84;
                    for (var i = 0; i < 6; i++)
                    {
                        if (icData[startIndex + 3] == 32)
                            break;
                        TreatRecCollection.Add(new TreatmentDataNoNeedHpc(icData, startIndex));
                        startIndex += 69;
                    }
                }
                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CheckChronicExist()
        {
            if (!ChronicDb.CheckChronicExistById(CurrentPrescription.Customer.Id)) return;
            var chronicSelectWindow = new ChronicSelectWindow(CurrentPrescription.Customer.Id);
            chronicSelectWindow.ShowDialog();
        }

        private void ClearPrescription()
        {
            CurrentPrescription = new Prescription();
            DivisionCombo.SelectedIndex = -1;
            TreatmentCaseCombo.SelectedIndex = -1;
            var isMedicalPerson = false;
            foreach (var m in MainWindow.CurrentPharmacy.MedicalPersonnelCollection)
            {
                if (!m.Id.Equals(MainWindow.CurrentUser.Id)) continue;
                isMedicalPerson = true;
                break;
            }
            if (isMedicalPerson)
            {
                HisPerson.SelectedItem =
                    MainWindow.CurrentPharmacy.MedicalPersonnelCollection.SingleOrDefault(p =>
                        p.Id.Equals(MainWindow.CurrentUser.Id));
            }
            else
            {
                HisPerson.SelectedIndex = 0;
            }
            CopaymentCombo.SelectedItem =
                MainWindow.Copayments.SingleOrDefault(c => c.Id.Equals("I20"));
            PaymentCategoryCombo.SelectedItem =
                MainWindow.PaymentCategory.SingleOrDefault(p => p.Id.Equals("4"));
            AdjustCaseCombo.SelectedItem =
                MainWindow.AdjustCases.SingleOrDefault(a => a.Id.Equals("1"));
            CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode = new DiseaseCode();
            CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = new DiseaseCode();
            CurrentPrescription.Treatment.TreatmentDate = DateTime.Now;
            CurrentPrescription.Treatment.AdjustDate = DateTime.Now;
            SpecialCodeCombo.SelectedIndex = -1;
            CurrentPrescription.ChronicSequence = string.Empty;
            CurrentPrescription.ChronicTotal = string.Empty;
            CurrentPrescription.Medicines.Clear();
            CopaymentText.Text = string.Empty;
            CustomPayText.Text = string.Empty;
            DepositText.Text = string.Empty;
            SelfCostText.Text = string.Empty;
            PaidText.Text = string.Empty;
            CustomerSelected = false;
        }
    }
}