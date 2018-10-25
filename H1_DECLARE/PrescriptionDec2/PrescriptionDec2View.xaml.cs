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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using His_Pos.Class.CustomerHistory;
using His_Pos.Class.Declare;
using His_Pos.Class.MedBag;
using His_Pos.HisApi;
using His_Pos.RDLC;
using Visibility = System.Windows.Visibility;
using System.Windows.Data;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.Class.DiseaseCode;
using His_Pos.Class.Person;
using His_Pos.Class.SpecialCode;
using His_Pos.Class.StoreOrder;
using His_Pos.Struct.IcData;

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
        public bool IsSend;
        public bool IsBone;
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
        public ObservableCollection<Hospital> HosiHospitals { get; set; }
        public ObservableCollection<Division> Divisions { get; set; }
        public ObservableCollection<TreatmentCase> TreatmentCases { get; set; }
        public ObservableCollection<PaymentCategory> PaymentCategories { get; set; }
        public ObservableCollection<Copayment> Copayments { get; set; }
        public ObservableCollection<AdjustCase> AdjustCases { get; set; }
        public ObservableCollection<Usage> Usages { get; set; }
        public ObservableCollection<DeclareMedicine> DeclareMedicines { get; set; }
        public ObservableCollection<SpecialCode> SpecialCodes { get; set; }
        #endregion

        public PrescriptionDec2View()
        {
            InitializeComponent(); 
            DataContext = this;
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
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            TreatmentCases = new ObservableCollection<TreatmentCase>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
        }
        #endregion
        
        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            IsSend = false;
            IsBone = false;
            ErrorMssageWindow err;
            CurrentPrescription.EList.Error = new List<Error>();
            CurrentPrescription.EList.Error = CurrentPrescription.CheckPrescriptionData();

            var medDays = 0;
            foreach (var med in CurrentPrescription.Medicines)
            {
                if (string.IsNullOrEmpty(med.Days))
                {
                    var messageWindow = new MessageWindow(med.Id + "的給藥日份不可為空", MessageType.ERROR, true);
                    messageWindow.ShowDialog();
                    return;
                }
                if (int.Parse(med.Days) > medDays)
                    medDays = int.Parse(med.Days);
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
            string decMasId;
            if (CurrentPrescription.Treatment.AdjustCase.Id != "2" && string.IsNullOrEmpty(_currentDecMasId) && CurrentPrescription.Treatment.AdjustDateStr == DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now))
            {//一般處方
                decMasId = declareDb.InsertDeclareData(_currentDeclareData);

                ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", decMasId);
                ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", decMasId);
                ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", decMasId);
                ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", decMasId);
                if (medEntryName != "骨科調劑耗用")
                {
                    var medTotalPrice = 0;
                    foreach (var med in _currentDeclareData.Prescription.Medicines)
                    {
                        medTotalPrice += Convert.ToInt32(ProductDb.GetBucklePrice(med.Id, med.Amount.ToString()));
                    }
                    ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", decMasId);
                    declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", decMasId);//庫存扣庫
                }
            }
            else if (CurrentPrescription.Treatment.AdjustCase.Id == "2" && !string.IsNullOrEmpty(_currentDecMasId))
            {//第2次以後的慢性處方
                if (IsSendToServer.IsChecked != null && (bool)IsSendToServer.IsChecked)//選擇傳送藥健康
                {
                    var chronicSendToServerWindow = new ChronicSendToServerWindow(CurrentPrescription.Medicines);
                    chronicSendToServerWindow.ShowDialog();
                    if (!IsSend) return;
                }
                if (IsSend)
                {//確定傳送
                    var storId = StoreOrderDb.SaveOrderDeclareData(_currentDecMasId, PrescriptionSendData);
                    //送到singde
                    StoreOrderDb.SendDeclareOrderToSingde(_currentDecMasId, storId, _currentDeclareData, declareTrade, PrescriptionSendData);
                }
                if (DeclareSubmit.Content.ToString() == "調劑" && CurrentPrescription.Treatment.AdjustDateStr == DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now))
                {
                    ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", _currentDecMasId);
                    if (medEntryName != "骨科調劑耗用")
                    {
                        var medTotalPrice = 0;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                        {
                            medTotalPrice += Convert.ToInt32(ProductDb.GetBucklePrice(med.Id, med.Amount.ToString()));
                        }
                        ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", _currentDecMasId);
                        declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", _currentDecMasId);//庫存扣庫
                    }
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
                if (IsSendToServer.IsChecked != null && (bool)IsSendToServer.IsChecked)
                {
                    var chronicSendToServerWindow = new ChronicSendToServerWindow(CurrentPrescription.Medicines);
                    chronicSendToServerWindow.ShowDialog();
                    if (!IsSend) return;
                }
                decMasId = declareDb.InsertDeclareData(_currentDeclareData);
                if (IsSend)
                {
                    var storId = StoreOrderDb.SaveOrderDeclareData(decMasId, PrescriptionSendData);
                    //送到singde
                    StoreOrderDb.SendDeclareOrderToSingde(decMasId, storId, _currentDeclareData, declareTrade, PrescriptionSendData);
                }

                if (DeclareSubmit.Content.ToString() == "調劑" && CurrentPrescription.Treatment.AdjustDateStr == DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now))
                {
                    ProductDb.InsertEntry("部分負擔", declareTrade.CopayMent, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("自費", declareTrade.PaySelf, "DecMasId", _currentDecMasId);
                    ProductDb.InsertEntry("押金", declareTrade.Deposit, "DecMasId", decMasId);
                    ProductDb.InsertEntry(medServiceName, _currentDeclareData.MedicalServicePoint.ToString(), "DecMasId", decMasId);
                    if (medEntryName != "骨科調劑耗用")
                    {
                        var medTotalPrice = 0;
                        foreach (var med in _currentDeclareData.Prescription.Medicines)
                            medTotalPrice += Convert.ToInt32(ProductDb.GetBucklePrice(med.Id, med.Amount.ToString()));
                        ProductDb.InsertEntry(medEntryName, "-" + medTotalPrice, "DecMasId", decMasId);
                        declareDb.InsertInventoryDb(_currentDeclareData, "處方登錄", decMasId);//庫存扣庫
                    }
                }
                if (IsBone)//如果來自骨科 刪除預約慢箋
                    declareDb.CheckPredictChronicExist(_currentDecMasId);

                var start = Convert.ToInt32(CurrentPrescription.ChronicSequence) + 1;
                var end = Convert.ToInt32(CurrentPrescription.ChronicTotal);
                var intDecMasId = Convert.ToInt32(decMasId);
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
                var loading = new LoadingWindow();
                loading.LoginIcData(Instance);
                m = new MessageWindow("處方登錄成功", MessageType.SUCCESS, true);
                m.ShowDialog();
            }
            else
            {
                icErrorWindow = new IcErrorCodeWindow(false, Enum.GetName(typeof(ErrorCode), GetMedicalNumberErrorCode));
                icErrorWindow.Show();
                var loading = new LoadingWindow();
                if(!string.IsNullOrEmpty(icErrorWindow.SelectedItem.Id))
                    loading.LoginIcData(Instance);
                m = new MessageWindow("處方登錄成功", MessageType.SUCCESS, true);
                m.ShowDialog();
            }
            declareDb.UpdateDeclareFile(_currentDeclareData);
            //PrintMedBag();
        }

        #region 每日上傳.讀寫卡相關函數
        public void LogInIcData()
        {
            var cs = new ConvertData();
            var icPrescripList = new List<IcPrescriptData>();
            foreach (var med in CurrentPrescription.Medicines)
            {
                icPrescripList.Add(new IcPrescriptData(med));
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

            var declareMedicine = (DeclareMedicine)((DeclareMedicine)medicineCodeAuto.SelectedItem)?.Clone();
            var currentRow = GetCurrentRowIndex(sender);

            if (CurrentPrescription.Medicines.Count > 0)
            {
                if (CurrentPrescription.Medicines.Count == currentRow)
                {
                    var m = CurrentPrescription.Medicines[currentRow - 1];
                    Debug.Assert(declareMedicine != null, nameof(declareMedicine) + " != null");
                    declareMedicine.UsageName = m.UsageName;
                    declareMedicine.Dosage = m.Dosage;
                    declareMedicine.Days = m.Days;
                    CurrentPrescription.Medicines.Add(declareMedicine);
                    medicineCodeAuto.Text = string.Empty;
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

        private void PrescriptionMedicines_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var objectName = (sender as Control)?.Name;
            //按 Enter 下一欄
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var nextTextBox = new List<TextBox>();
                var nextAutoCompleteBox = new List<AutoCompleteBox>();
                var currentRowIndex = GetCurrentRowIndex(sender);

                if (currentRowIndex == -1) return;

                switch (objectName)
                {
                    case "Dosage":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Usage", ref nextTextBox);
                        break;

                    case "Usage":

                        FindUsagesQuickName(sender);
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineDays", ref nextTextBox);
                        break;

                    case "MedicineDays":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineTotal", ref nextTextBox);
                        break;

                    case "MedicineTotal":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "Position", ref nextTextBox);
                        break;

                    case "Position":
                        if (!CurrentPrescription.Medicines[currentRowIndex].PaySelf)
                        {
                            NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                            NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                            nextTextBox[0].Focus();
                            return;
                        }
                        else
                        {
                            NewFunction.FindChildGroup(PrescriptionMedicines, "Price", ref nextTextBox);
                            nextTextBox[currentRowIndex].Focus();
                        }
                        break;

                    case "Price":
                        NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                        NewFunction.FindChildGroup(nextAutoCompleteBox[currentRowIndex + 1], "Text", ref nextTextBox);
                        nextTextBox[0].Focus();
                        return;
                }
                nextTextBox[currentRowIndex].Focus();
                nextTextBox[currentRowIndex].CaretIndex = 0;
            }
            //按 Up Down
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
                var thisTextBox = new List<TextBox>();
                var currentRowIndex = GetCurrentRowIndex(sender);
                if (currentRowIndex == -1) return;

                NewFunction.FindChildGroup(PrescriptionMedicines, objectName, ref thisTextBox);

                var newIndex = (e.Key == Key.Up) ? currentRowIndex - 1 : currentRowIndex + 1;

                if (newIndex < 0)
                    newIndex = 0;
                else if (newIndex >= thisTextBox.Count)
                    newIndex = thisTextBox.Count - 1;

                thisTextBox[newIndex].Focus();
            }
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
                CurrentPrescription.Medicines[currentRow].Usage = Usages.SingleOrDefault(u => u.QuickName.Equals(t.Text));
                if (CurrentPrescription.Medicines[currentRow].Usage != null)
                    t.Text = CurrentPrescription.Medicines[currentRow].Usage.Name;
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
            ClearMedicine(CurrentPrescription.Medicines[PrescriptionMedicines.SelectedIndex]);
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

        private void CountMedicinesCost()
        {
            double medicinesHcCost = 0;//健保給付總藥價
            double medicinesSelfCost = 0;//自費藥總藥價
            double purchaseCosts = 0;//藥品總進貨成本
            foreach (var medicine in CurrentPrescription.Medicines)
            {
                if (!medicine.PaySelf)
                    medicinesHcCost += medicine.TotalPrice;
                else
                {
                    medicinesSelfCost += medicine.TotalPrice;
                }
                purchaseCosts += medicine.Cost * medicine.Amount;
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
            var t1 = new Thread(CheckIcCardStatus);
            SetCardStatusContent("卡片檢查中...");
            t1.Start();
            if (t1.Join(8000))
            {
                if (CurrentPrescription.IsGetIcCard)
                {
                    var loading = new LoadingWindow();
                    loading.Show();
                    loading.LoadIcData(Instance);
                    if (ChronicDb.CheckChronicExistById(CurrentPrescription.Customer.Id))
                    {
                        var chronicSelectWindow = new ChronicSelectWindow(CurrentPrescription.Customer.Id);
                        chronicSelectWindow.ShowDialog();
                    }
                }
                else
                { 
                    if (string.IsNullOrEmpty(PatientName.Text) && string.IsNullOrEmpty(PatientBirthday.Text) &&
                        string.IsNullOrEmpty(PatientId.Text) && CurrentPrescription.Treatment.AdjustCase.Id != "0")
                    {
                        var m = new MessageWindow("無法取得健保卡資料，請輸入病患姓名.生日或身分證字號以查詢病患資料，或選擇自費調劑", MessageType.WARNING, true);
                        m.ShowDialog();
                    }
                    else if (string.IsNullOrEmpty(PatientName.Text) && string.IsNullOrEmpty(PatientBirthday.Text) &&
                             string.IsNullOrEmpty(PatientId.Text) && CurrentPrescription.Treatment.AdjustCase.Id == "0")
                    {
                        CurrentPrescription.Customer.Id = "0";
                        CurrentPrescription.Customer.Name = "匿名";
                    }
                    else
                    {
                        CustomerCollection =
                            CustomerDb.LoadCustomerData(CurrentPrescription.Customer);
                        if (CustomerCollection.Count == 1)
                            CurrentPrescription.Customer = CustomerCollection[0];
                        else
                        {
                            var customerSelect = new CustomerSelectWindow(CustomerCollection);
                            customerSelect.Show();
                        }
                        if (!ChronicDb.CheckChronicExistById(CurrentPrescription.Customer.Id)) return;
                        var chronicSelectWindow = new ChronicSelectWindow(CurrentPrescription.Customer.Id);
                        chronicSelectWindow.ShowDialog();
                    }
                }
            }
            else
            {
                t1.Abort();
                if (string.IsNullOrEmpty(PatientName.Text) && string.IsNullOrEmpty(PatientBirthday.Text) &&
                       string.IsNullOrEmpty(PatientId.Text))
                {
                    var m = new MessageWindow("無法取得健保卡資料，請輸入病患姓名.生日或身分證字號以查詢病患資料，或選擇自費調劑", MessageType.WARNING, true);
                    m.Show();
                    return;
                }
                CurrentPrescription.Customer.Id = CustomerDb.CheckCustomerExist(CurrentPrescription.Customer);
                CustomerCollection =
                    CustomerDb.LoadCustomerData(CurrentPrescription.Customer);
                if (CustomerCollection.Count == 1)
                    CurrentPrescription.Customer = CustomerCollection[0];
                else
                {
                    var customerSelect = new CustomerSelectWindow(CustomerCollection);
                    customerSelect.Show();
                }
                if (!ChronicDb.CheckChronicExistById(CurrentPrescription.Customer.Id)) return;
                var chronicSelectWindow = new ChronicSelectWindow(CurrentPrescription.Customer.Id);
                chronicSelectWindow.ShowDialog();
            }
            CurrentCustomerHistoryMaster = CustomerHistoryDb.GetDataByCUS_ID(CurrentPrescription.Customer.Id);
            CusHistoryMaster.ItemsSource = CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection;
            if (CurrentCustomerHistoryMaster.CustomerHistoryMasterCollection.Count > 0)
                CusHistoryMaster.SelectedIndex = 0;
            NotifyPropertyChanged(nameof(CurrentPrescription));
        }

        private void CheckIcCardStatus()
        {
            var cardStatus = HisApiBase.hisGetCardStatus(2);
            switch (cardStatus)
            {
                case 0:
                    SetCardStatusContent("卡片未置入");
                    break;
                case 1:
                    SetCardStatusContent("尚未與安全模組認證");
                    break;
                case 2:
                    SetCardStatusContent("認證成功");
                    break;
                case 3:
                    SetCardStatusContent("醫事人員卡認證成功");
                    break;
                case 4:
                    SetCardStatusContent("PIN 認證成功");
                    break;
                case 5:
                    SetCardStatusContent("健保局IDC認證成功 ");
                    break;
                case 9:
                    SetCardStatusContent("非健保卡");
                    break;
                case 4000:
                    SetCardStatusContent("讀卡機逾時");
                    break;
            }

            if (cardStatus != 0 && cardStatus != 1 && cardStatus != 9 && cardStatus != 4000)
                CurrentPrescription.IsGetIcCard = true;
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

        private void MedicalNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           InputNumber(e);
        }

        private void InputNumber(KeyEventArgs e)
        {
            var shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            if (shiftKey)  
            {
                e.Handled = true;
            }
            else                        
            {
                if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Enter))
                {
                    e.Handled = true;
                }
            }
        }

        private void ReleaseHospital_Populating(object sender, PopulatingEventArgs e) {
            if (HosiHospitals is null) HosiHospitals = HospitalDb.GetData(); 
            var tempCollection = new ObservableCollection<Hospital>(HosiHospitals.Where(x => x.Id.Contains(ReleaseHospital.Text)).Take(50).ToList());
            ReleaseHospital.ItemsSource = tempCollection;
            ReleaseHospital.PopulateComplete();
        }

        private void AdjustCaseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(AdjustCaseCombo.SelectedItem == null) return;
            if (((AdjustCase) AdjustCaseCombo.SelectedItem).Id.Equals("0"))
            {
                DeclareSubmit.Visibility = Visibility.Collapsed;
                NotDeclareSubmit.Visibility = Visibility.Visible;
            }
            else
            {
                DeclareSubmit.Visibility = Visibility.Visible;
                NotDeclareSubmit.Visibility = Visibility.Collapsed;
            }
            IsSendToServer.IsChecked = false;
            IsSendToServer.IsEnabled = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && DatePickerTreatment.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            IsSendToServer.IsChecked = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && DatePickerTreatment.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
        }

        private void ChronicSequence_TextChanged(object sender, TextChangedEventArgs e) {
            if (Convert.ToInt32(ChronicSequence.Text) > 1)
            {
                var myBinding = new Binding("CurrentPrescription.OriginalMedicalNumber");
                BindingOperations.SetBinding(MedicalNumber, TextBox.TextProperty, myBinding);
            }
            else
            {
                var myBinding = new Binding("CurrentPrescription.Customer.IcCard.MedicalNumber");
                BindingOperations.SetBinding(MedicalNumber, TextBox.TextProperty, myBinding);
            }
        }

        private void DatePickerTreatment_SelectionChanged(object sender, RoutedEventArgs e) {
            DeclareSubmit.Content = DatePickerTreatment.Text == DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now)  ? "調劑" : "預約慢箋";
            IsSendToServer.IsChecked = false;
            if (AdjustCaseCombo.SelectedItem == null) return;
            IsSendToServer.IsEnabled = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && DatePickerTreatment.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
            IsSendToServer.IsChecked = ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "02" || ((AdjustCase)AdjustCaseCombo.SelectedItem).Id == "2" && DatePickerTreatment.Text != DateTimeExtensions.ToSimpleTaiwanDate(DateTime.Now);
        }

        public void SetValueByDecMasId(string decMasId) {
            _currentDecMasId = decMasId;
            var prescription = PrescriptionDB.GetDeclareDataById(decMasId).Prescription;
            CurrentPrescription = prescription;
            DivisionCombo.Text = prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
            AdjustCaseCombo.Text = prescription.Treatment.AdjustCase.FullName;
            TreatmentCaseCombo.Text = prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
            PaymentCategoryCombo.Text = prescription.Treatment.PaymentCategory.FullName;
            CopaymentCombo.Text = prescription.Treatment.Copayment.FullName;
            SpecialCode.Text = prescription.Treatment.MedicalInfo.SpecialCode.Id;
            ReleaseHospital.Text = prescription.Treatment.MedicalInfo.Hospital.Id;

            DatePickerPrecription.Text = DateTimeExtensions.ToSimpleTaiwanDate(prescription.Treatment.TreatmentDate);
            DatePickerTreatment.Text = DateTimeExtensions.ToSimpleTaiwanDate(prescription.Treatment.AdjustDate); 
            CurrentPrescription.Medicines = MedicineDb.GetDeclareMedicineByMasId(decMasId);
            PrescriptionMedicines.ItemsSource = Instance.CurrentPrescription.Medicines;
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
                if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.MainDiseaseCode.Id) && textBox.Text.Contains(" "))
                {
                    SecondDiagnosis.Focus();
                    return;
                }
            }
            else if(textBoxDependency.GetValue(NameProperty) is string && textBoxDependency.GetValue(NameProperty).Equals("SecondDiagnosis"))
            {
                if (!string.IsNullOrEmpty(CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode.Id) && textBox.Text.Contains(" "))
                {
                    PaymentCategoryCombo.Focus();
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
                    CurrentPrescription.Treatment.MedicalInfo.SecondDiseaseCode = selectedDiseaseCode;
                }
            }
            else
            {
                var disease = new DiseaseCodeSelectDialog(textBox.Text);
                disease.Show();
            }
        }

        private void DivisionCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || (search.Length == 1))
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(DivisionCombo.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((Division)o).Id.Contains(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }


        private void DivisionCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (e.Key != Key.Enter)
            {
                if (c.Text.Contains(" "))
                    c.Text = string.Empty;
                return;
            }
            if (c.SelectedItem != null)
            {
                DoctorId.Focus();
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.Items.Count > 0)
                    c.SelectedIndex = 0;
            }
        }

        private void Combo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);
            itemsViewOriginal.Filter = o => true;
            itemsViewOriginal.Refresh();
        }

        private void ReleaseHospital_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is AutoCompleteBox a)) return;
            if (e.Key == Key.Enter && a.IsDropDownOpen && a.Text.Length <= 10)
                a.SelectedItem = HosiHospitals.Where(x => x.Id.Contains(ReleaseHospital.Text)).Take(50).ToList()[0];
        }

        private void ReleaseHospital_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender is AutoCompleteBox a && a.SelectedItem != null)
            {
                DivisionCombo.Focus();
            }
        }

        private void DivisionCombo_OnDropDownClosed(object sender, EventArgs e)
        {
            if(sender is ComboBox c && c.SelectedItem is Division)
                DoctorId.Focus();
        }

        private void DoctorId_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                HisPerson.Focus();
        }

        private void HisPerson_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TreatmentCaseCombo.Focus();
        }

        private void TreatmentCaseCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (e.Key != Key.Enter)
            {
                if (c.Text.Contains(" "))
                    c.Text = string.Empty;
                return;
            }
            if (c.SelectedItem != null)
            {
                MainDiagnosis.Focus();
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.Items.Count > 0)
                    c.SelectedIndex = 0;
            }
        }

        private void TreatmentCaseCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || (search.Length == 1))
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((TreatmentCase)o).Id.Contains(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }

        private void PaymentCategoryCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || string.IsNullOrEmpty(search))
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((PaymentCategory)o).Id.Equals(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }

        private void PaymentCategoryCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (e.Key != Key.Enter)
            {
                if (c.Text.Contains(" "))
                    c.Text = string.Empty;
                return;
            }
            if (c.SelectedItem != null)
            {
                CopaymentCombo.Focus();
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.Items.Count > 0)
                    c.SelectedIndex = 0;
            }
        }

        private void CopaymentCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || search.Length < 3)
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((Copayment)o).Id.Contains(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }

        private void CopaymentCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (e.Key != Key.Enter)
            {
                if(c.Text.Contains(" "))
                    c.Text = string.Empty;
                return;
            }
            if (c.SelectedItem != null)
            {
                AdjustCaseCombo.Focus();
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.Items.Count > 0)
                    c.SelectedIndex = 0;
            }
        }

        private void AdjustCaseCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || string.IsNullOrEmpty(search))
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((AdjustCase)o).Id.Contains(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }

        private void AdjustCaseCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            if (e.Key != Key.Enter)
            {
                if(c.Text.Contains(" "))
                    c.Text = string.Empty;
                return;
            }
            if (c.SelectedItem != null)
            {
                if (CurrentPrescription.Treatment.AdjustCase.Id.Equals("2"))
                    ChronicSequence.Focus();
                else
                {
                    SpecialCode.Focus();
                }
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.Items.Count > 0)
                    c.SelectedIndex = 0;
            }
        }

        private void ChronicSequence_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !(sender is TextBox)) return;
            ChronicTotal.Focus();
        }

        private void ChronicTotal_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !(sender is TextBox)) return;
            SpecialCode.Focus();
        }

        private void SpecialCodeCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox c)) return;
            var search = c.Text.ToUpper();
            if (e.Key == Key.Back)
            {
                c.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter || search.Length < 2)
                return;
            var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(c.ItemsSource);

            itemsViewOriginal.Filter = ((o) =>
            {
                if (string.IsNullOrEmpty(search)) return true;
                if (((SpecialCode)o).Id.Contains(search))
                {
                    c.Text = search;
                    return true;
                }
                c.Text = search;
                return false;
            });
            itemsViewOriginal.Refresh();
        }

        private void SpecialCodeCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var nextAutoCompleteBox = new List<AutoCompleteBox>();
            if (e.Key != Key.Enter || !(sender is ComboBox c)) return;
            if ((c.SelectedItem != null && c.Text.Contains(" ")) || string.IsNullOrEmpty(c.Text))
            {
                NewFunction.FindChildGroup(PrescriptionMedicines, "MedicineCodeAuto", ref nextAutoCompleteBox);
                nextAutoCompleteBox[0].Focus();
            }
            else
            {
                c.IsDropDownOpen = true;
                if (c.SelectedIndex != -1)
                    c.Text = string.Empty;
                if (c.Items.Count > 0 && c.Text.Length > 1)
                    c.SelectedIndex = 0;
            }
        }

        private void IsBuckle_Click(object sender, RoutedEventArgs e)
        {
            foreach (var t in CurrentPrescription.Medicines)
            {
                if (t.Id == _selectedMedId)
                    t.IsBuckle = !t.IsBuckle;
            }
        }
        private void MedContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var temp = (sender as ContextMenu);
            var menuitem = temp.Items[0] as MenuItem;
            _selectedMedId = ((DeclareMedicine)PrescriptionMedicines.SelectedItem).Id;
            if (((DeclareMedicine)PrescriptionMedicines.SelectedItem).IsBuckle)
                menuitem.Header = "申報不扣庫";
            else
                menuitem.Header = "申報扣庫";
        }
        public bool ByPing()
        {
            IPAddress tIP = IPAddress.Parse("10.252.141.53");
            Ping tPingControl = new Ping();
            PingReply tReply = tPingControl.Send(tIP);
            tPingControl.Dispose();
            if (tReply.Status != IPStatus.Success)
                return false;
            else
                return true;
        }
    }
}