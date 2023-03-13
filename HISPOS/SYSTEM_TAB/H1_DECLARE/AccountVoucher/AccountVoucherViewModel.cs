using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Report.Accounts;
using His_Pos.NewClass.Report.Accounts.AccountsRecordDetails;
using His_Pos.NewClass.Report.Accounts.AccountsRecords;
using System.Data;
using System.Data.SqlClient;
using His_Pos.NewClass.Accounts;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher.InvalidWindow;
using System.Diagnostics;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AccountVoucher
{
    public class AccountVoucherViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }
        public AccountVoucherViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            ClearCommand = new RelayCommand(ClearAction);
            AddCommand = new RelayCommand(AddAction);
            InvalidCommand = new RelayCommand(InvalidAction);
            SaveCommand = new RelayCommand(SaveAction);
            CopyDataCommand = new RelayCommand(CopyDataAction);
            StrikeCommand = new RelayCommand(StrikeAction);
            VisibilityBtnCommand = new RelayCommand(VisibilityBtnAction);
            ExportCommand = new RelayCommand(ExportAction);
            FilterCommand = new RelayCommand<string>(FilterAction);
            DeleteDetailCommand = new RelayCommand<string>(DeleteDetailAction);
            AddNewDetailCommand = new RelayCommand<string>(AddNewDetailAction);
            ClickDetailCommand = new RelayCommand<string>(ClickDetailAction);
            GetData();
            
        }
        #region Command
        public RelayCommand CopyDataCommand { get; set; }
        public RelayCommand StrikeCommand { get; set; }
        public RelayCommand<string> FilterCommand { get; set; }
        public RelayCommand<string> DeleteDetailCommand { get; set; }
        public RelayCommand<string> AddNewDetailCommand { get; set; }
        public RelayCommand<string> ClickDetailCommand { get; set; }
        public RelayCommand VisibilityBtnCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand InvalidCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        #endregion
        #region
        public ICollectionView VoucherCollectionView
        {
            get => voucherCollectionView;
            set
            {
                Set(() => VoucherCollectionView, ref voucherCollectionView, value);
            }
        }
        private ICollectionView voucherCollectionView;
        public Visibility DisplayVoidReason
        {
            get => displayVoidReason;
            set
            {
                Set(() => DisplayVoidReason, ref displayVoidReason, value);
            }
        }
        private Visibility displayVoidReason = Visibility.Visible;
        public Visibility BtnVisibilty
        {
            get => btnVisibilty;
            set
            {
                Set(() => BtnVisibilty, ref btnVisibilty, value);
            }
        }
        private Visibility btnVisibilty = Visibility.Visible;
        public string BtnName
        {
            get => btnName;
            set
            {
                Set(() => BtnName, ref btnName, value);
            }
        }
        private string btnName = "新增";

        public string EditContent
        {
            get => editContent;
            set
            {
                Set(() => EditContent, ref editContent, value);
            }
        }
        private string editContent = "修改傳票內容";
        public Visibility BtnEditVisibilty
        {
            get => btnEditVisibilty;
            set
            {
                Set(() => BtnEditVisibilty, ref btnEditVisibilty, value);
            }
        }
        private Visibility btnEditVisibilty = Visibility.Visible;//中間修改按鈕
        public Visibility BtnExportVisibilty
        {
            get => btnExportVisibilty;
            set
            {
                Set(() => BtnExportVisibilty, ref btnExportVisibilty, value);
            }
        }
        private Visibility btnExportVisibilty = Visibility.Hidden;//中間匯出按鈕
        public bool IsBtnEnable
        {
            get => isBtnEnable;
            set
            {
                Set(() => IsBtnEnable, ref isBtnEnable, value);
            }
        }
        private bool isBtnEnable = true;//下方按鈕
        public bool IsCanEdit
        {
            get => isCanEdit;
            set
            {
                Set(() => IsCanEdit, ref isCanEdit, value);
            }
        }
        private bool isCanEdit = true;
        public DateTime BeginDate
        {
            get => beginDate;
            set
            {
                Set(() => BeginDate, ref beginDate, value);
            }
        }
        private DateTime beginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private DateTime endDate = DateTime.Today;
        public string SearchID
        {
            get => searchID;
            set
            {
                Set(() => SearchID, ref searchID, value);
            }
        }
        private string searchID;
        public string KeyWord
        {
            get => keyWord;
            set
            {
                Set(() => KeyWord, ref keyWord, value);
            }
        }
        private string keyWord;
        public bool IsProce
        {
            get => isProce;
            set
            {
                Set(() => IsProce, ref isProce, value);
            }
        }
        private bool isProce;
        public bool IsTemp
        {
            get => isTemp;
            set
            {
                Set(() => IsTemp, ref isTemp, value);
            }
        }
        private bool isTemp;
        public bool IsInvalid
        {
            get => isInvalid;
            set
            {
                Set(() => IsInvalid, ref isInvalid, value);
            }
        }
        private bool isInvalid;
        public IEnumerable<JournalAccount> Accounts
        {
            get => accounts;
            set
            {
                Set(() => Accounts, ref accounts, value);
            }
        }
        private IEnumerable<JournalAccount> accounts;
        public JournalAccount Account
        {
            get => account;
            set
            {
                Set(() => Account, ref account, value);
            }
        }
        private JournalAccount account;
        public IEnumerable<JournalType> Types
        {
            get => types;
            set
            {
                Set(() => Types, ref types, value);
            }
        }
        private IEnumerable<JournalType> types;
        public JournalType Type
        {
            get => type;
            set
            {
                Set(() => Type, ref type, value);
            }
        }
        private JournalType type;
        public JournalMaster CurrentVoucher
        {
            get
            {
                return currentVoucher;
            }
            set
            {
                Set(() => CurrentVoucher, ref currentVoucher, value);
                if (CurrentVoucher != null)
                {
                    if (CurrentVoucher.JouMas_ID != null && CurrentVoucher.JouMas_Status.Equals("T"))
                    {
                        if (CurrentVoucher.DebitDetails != null && CurrentVoucher.CreditDetails != null && CurrentVoucher.JouMas_IsEnable == 1)
                        {
                            AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                        }
                    }
                    if (!string.IsNullOrEmpty(CurrentVoucher.JouMas_ID))
                    {
                        GetDetailData(CurrentVoucher.JouMas_ID);
                    }
                    if (CurrentVoucher.JouMas_ID != null && CurrentVoucher.JouMas_Status.Equals("F"))
                    {
                        IsCanEdit = false;
                        BtnVisibilty = Visibility.Hidden;
                    }
                    if (IsProce)
                    {
                        BtnEditVisibilty = Visibility.Visible;
                        BtnExportVisibilty = Visibility.Visible;
                        EditContent = "修改傳票內容";
                    }
                }
                else
                {
                    BtnExportVisibilty = Visibility.Hidden;
                    BtnEditVisibilty = Visibility.Hidden;
                }
            }
        }
        private JournalMaster currentVoucher;
        #endregion
        #region Function
        private void CopyDataAction()
        {
            if (currentVoucher.JouMas_Status.Equals("F") && currentVoucher.JouMas_Source == 1)
            {
                JournalMaster journalMaster = currentVoucher;//紀錄目前傳票
                bool isSuccess = InsertNewJournal();
                if (isSuccess)
                {
                    foreach (JournalDetail item in journalMaster.DebitDetails)
                    {
                        CopyDetailData(CurrentVoucher.JouMas_ID, item, true);
                    }
                    foreach (JournalDetail item in journalMaster.CreditDetails)
                    {
                        CopyDetailData(CurrentVoucher.JouMas_ID, item, false);
                    }
                    AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                }
            }
        }
        private void CopyDetailData(string newJouID, JournalDetail detail, bool isDebit)
        {
            JournalDetail item = new JournalDetail();
            item.JouDet_ID = newJouID;
            item.JouDet_Type = detail.JouDet_Type;
            item.JouDet_Number = detail.JouDet_Number;
            item.JouDet_AcctLvl1 = detail.JouDet_AcctLvl1;
            item.JouDet_AcctLvl1Name = detail.JouDet_AcctLvl1Name;
            item.JouDet_AcctLvl2 = detail.JouDet_AcctLvl2;
            item.JouDet_AcctLvl2Name = detail.JouDet_AcctLvl2Name;
            item.JouDet_AcctLvl3 = detail.JouDet_AcctLvl3;
            item.JouDet_AcctLvl3Name = detail.JouDet_AcctLvl3Name;
            item.Accounts = detail.Accounts;
            item.Account = detail.Account;
            item.JouDet_Memo = detail.JouDet_Memo;
            if (isDebit)
            {
                CurrentVoucher.DebitDetails.Add(item);
            }
            else
            {
                CurrentVoucher.CreditDetails.Add(item);
            }
        }
        private void VisibilityBtnAction()
        {
            if (IsProce)
            {
                if (BtnVisibilty == Visibility.Hidden)
                {
                    EditContent = "取消修改";
                    BtnVisibilty = Visibility.Visible;
                    IsCanEdit = true;
                    IsBtnEnable = false;
                }
                else
                {
                    EditContent = "修改傳票內容";
                    BtnVisibilty = Visibility.Hidden;
                    IsCanEdit = false;
                    IsBtnEnable = true;
                    GetDetailData(CurrentVoucher.JouMas_ID);
                }
            }
        }
        private void StrikeAction()
        {
            if (currentVoucher.JouMas_Status.Equals("F"))
            {
                JournalMaster journalMaster = currentVoucher;//紀錄目前傳票
                bool isSuccess = InsertNewJournal();
                if(isSuccess)
                {
                    CurrentVoucher.JouMas_Status = "F";
                    foreach (JournalDetail item in journalMaster.DebitDetails)
                    {
                        JournalDetail detail = item;
                        detail.JouDet_ID = CurrentVoucher.JouMas_ID;
                        detail.JouDet_Type = "C";
                        detail.JouDet_WriteOffID = journalMaster.JouMas_ID;
                        detail.JouDet_WriteOffNumber = item.JouDet_Number;
                        detail.JouDet_Amount = item.JouDet_Amount;
                        CurrentVoucher.CreditDetails.Add(detail);
                    }
                    foreach (JournalDetail item in journalMaster.CreditDetails)
                    {
                        JournalDetail detail = item;
                        detail.JouDet_ID = CurrentVoucher.JouMas_ID;
                        detail.JouDet_Type = "D";
                        detail.JouDet_WriteOffID = journalMaster.JouMas_ID;
                        detail.JouDet_WriteOffNumber = item.JouDet_Number;
                        detail.JouDet_Amount = item.JouDet_Amount;
                        CurrentVoucher.DebitDetails.Add(detail);
                    }
                    AccountsDb.UpdateJournalData("新增", CurrentVoucher);
                    IsProce = true;
                    VoucherCollectionView.Filter += VoucherFilter;
                }
            }
        }
        private void SubmitAction()
        {
            if (Account is null)
                Account = new JournalAccount();

            VoucherCollectionView = CollectionViewSource.GetDefaultView(AccountsDb.GetJournalData(beginDate, endDate, string.IsNullOrEmpty(searchID)? string.Empty : searchID, Account.acctLevel1, Account.acctLevel2, Account.acctLevel3, string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord, Type.JournalTypeID));
            
            VoucherCollectionView.Filter += VoucherFilter;
            CurrentVoucher = new JournalMaster();
            CurrentVoucher = VoucherCollectionView.CurrentItem as JournalMaster;
            if (CurrentVoucher != null && (CurrentVoucher.JouMas_ID != null || CurrentVoucher.JouMas_ID != ""))
            {
                GetDetailData(CurrentVoucher.JouMas_ID);
            }
        }
        private void ClearAction()
        {
            SearchID = string.Empty;
            KeyWord = string.Empty;
            Account = null;
        }
        private void FilterAction(string filterCondition)
        {
            if (VoucherCollectionView != null)
            {
                if (VoucherCollectionView.CanFilter)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        VoucherCollectionView.Filter += VoucherFilter;
                        if (VoucherCollectionView.CurrentItem != null)
                        {
                            if (!string.IsNullOrEmpty(((JournalMaster)VoucherCollectionView.CurrentItem).JouMas_ID))
                            {
                                CurrentVoucher = (JournalMaster)VoucherCollectionView.CurrentItem;
                                GetDetailData(((JournalMaster)VoucherCollectionView.CurrentItem).JouMas_ID);
                            }
                        }
                    }));
                }
            }
        }
        private void DeleteDetailAction(string gridCondition)
        {
            if (CurrentVoucher != null)
            {
                if(CurrentVoucher.JouMas_IsEnable == 1)
                {
                    switch (gridCondition)
                    {
                        case "0":
                            CurrentVoucher.DebitDetails.Remove(CurrentVoucher.SelectedDebitDetail);
                            CurrentVoucher.DebitTotalAmount = (int)CurrentVoucher.DebitDetails.Sum(s => s.JouDet_Amount);
                            break;
                        case "1":
                            CurrentVoucher.CreditDetails.Remove(CurrentVoucher.SelectedCreditDetail);
                            CurrentVoucher.CreditTotalAmount = (int)CurrentVoucher.CreditDetails.Sum(s => s.JouDet_Amount);
                            break;
                    }
                }
            }
        }
        private void AddNewDetailAction(string gridCondition)
        {
            if(CurrentVoucher != null)
            {
                if(CurrentVoucher.JouMas_IsEnable == 1)
                {
                    JournalDetail detail = new JournalDetail();
                    detail.Accounts = AccountsDb.GetJournalAccount("傳票作業");
                    detail.JouDet_ID = CurrentVoucher.JouMas_ID;
                    detail.JouDet_Type = gridCondition.Equals("0") ? "D" : "C";
                    detail.JouDet_Number = gridCondition.Equals("0") ? CurrentVoucher.DebitDetails.Count + 1 : CurrentVoucher.CreditDetails.Count + 1;
                    switch (gridCondition)
                    {
                        case "0":
                            if (CurrentVoucher.DebitDetails is null)
                                CurrentVoucher.DebitDetails = new JournalDetails();
                            CurrentVoucher.DebitDetails.Add(detail);
                            break;
                        case "1":
                            if (CurrentVoucher.CreditDetails is null)
                                CurrentVoucher.CreditDetails = new JournalDetails();
                            CurrentVoucher.CreditDetails.Add(detail);
                            break;
                    }
                }
            }
        }
        private void ClickDetailAction(string param)
        {
            LedgerWindow.LedgerWindow ledgerWindow = new LedgerWindow.LedgerWindow();
            LedgerWindow.LedgerViewModel viewModel = new LedgerWindow.LedgerViewModel(param);
            ledgerWindow.DataContext = viewModel;
            ledgerWindow.Show();
        }
        private bool VoucherFilter(object journal)
        {
            JournalMaster jm = (JournalMaster)journal;
            if(IsInvalid)
            {
                BtnVisibilty = Visibility.Hidden;
                BtnEditVisibilty = Visibility.Hidden;
                BtnExportVisibilty = Visibility.Hidden;
                DisplayVoidReason = Visibility.Visible;
                if (jm.JouMas_IsEnable == 0)
                {
                    BtnName = "新增";
                    IsBtnEnable = false;
                    IsCanEdit = true;
                    return true;
                }
            }
            if (IsTemp)
            {
                BtnVisibilty = Visibility.Visible;
                BtnEditVisibilty = Visibility.Hidden;
                BtnExportVisibilty = Visibility.Hidden;
                DisplayVoidReason = Visibility.Hidden;
                if (jm.JouMas_Status.Equals("T") && jm.JouMas_IsEnable == 1)
                {
                    BtnName = "新增";
                    IsBtnEnable = true;
                    IsCanEdit = true;
                    return true;
                }
            }
            if (IsProce)
            {
                BtnVisibilty = Visibility.Hidden;
                BtnEditVisibilty = Visibility.Hidden;
                DisplayVoidReason = Visibility.Hidden;
                if (CurrentVoucher != null && !string.IsNullOrEmpty(CurrentVoucher.JouMas_ID))
                {
                    BtnName = "修改";
                    IsBtnEnable = true;
                    IsCanEdit = false;
                    BtnEditVisibilty = Visibility.Visible;
                    BtnExportVisibilty = Visibility.Visible;
                }
                if (jm.JouMas_Status.Equals("F") && jm.JouMas_IsEnable == 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SourceCheck()
        {
            DataTable table = AccountsDb.GetSourceData();

            foreach (JournalDetail item in CurrentVoucher.DebitDetails)
            {
                if (!string.IsNullOrEmpty(item.JouDet_WriteOffID))
                {
                    int count = table.Select(string.Format("JouDet_ID = '{0}' And JouDet_Number = {1} And JouDet_SourceID = '{2}'", item.JouDet_WriteOffID, item.JouDet_WriteOffNumber, item.JouDet_SourceID)).Count();
                    if (count == 0)
                    {
                        MessageWindow.ShowMessage(string.Format("借方項次{0} 沖帳來源不存在\r\n請確認來源", item.JouDet_Number), MessageType.ERROR);
                        return false;
                    }
                    else
                    {
                        DataRow[] drs = table.Select(string.Format("JouDet_ID = '{0}' And JouDet_Number = {1} And JouDet_SourceID = '{2}'", item.JouDet_WriteOffID, item.JouDet_WriteOffNumber, item.JouDet_SourceID));
                        if (drs != null && drs.Length > 0)
                        {
                            int amount = Convert.ToInt32(drs[0]["JouDet_Amount"]);
                            if (item.JouDet_Amount > amount)
                            {
                                MessageWindow.ShowMessage(string.Format("借方項次{0} 沖帳金額超過來源金額\r\n請確認金額", item.JouDet_Number), MessageType.ERROR);
                                return false;
                            }
                        }
                        
                    }
                }
            }
            foreach (JournalDetail item in CurrentVoucher.CreditDetails)
            {
                if (!string.IsNullOrEmpty(item.JouDet_WriteOffID))
                {
                    int count = table.Select(string.Format("JouDet_ID = '{0}' And JouDet_Number = {1} And JouDet_SourceID = '{2}'", item.JouDet_WriteOffID, item.JouDet_WriteOffNumber, item.JouDet_SourceID)).Count();
                    if (count == 0)
                    {
                        MessageWindow.ShowMessage(string.Format("借方項次{0} 沖帳來源不存在\r\n請確認來源", item.JouDet_Number), MessageType.ERROR);
                        return false;
                    }
                    else
                    {
                        DataRow[] drs = table.Select(string.Format("JouDet_ID = '{0}' And JouDet_Number = {1} And JouDet_SourceID = '{2}'", item.JouDet_WriteOffID, item.JouDet_WriteOffNumber, item.JouDet_SourceID));
                        if (drs != null && drs.Length > 0)
                        {
                            int amount = Convert.ToInt32(drs[0]["JouDet_Amount"]);
                            if (item.JouDet_Amount > amount)
                            {
                                MessageWindow.ShowMessage(string.Format("借方項次{0} 沖帳金額超過來源金額\r\n請確認金額", item.JouDet_Number), MessageType.ERROR);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        private void SaveAction()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (CurrentVoucher != null)
                {
                    if (CurrentVoucher.JouMas_IsEnable == 1)
                    {
                        if (CheckData())
                        {
                            if (btnName.Equals("新增"))
                            {
                                if (!SourceCheck())
                                {
                                    return;
                                }
                            }
                            AccountsDb.UpdateJournalData(btnName, CurrentVoucher);
                            if (btnName.Equals("修改"))
                            {
                                MessageWindow.ShowMessage(string.Format("{0}\r\n修改成功", CurrentVoucher.JouMas_ID), MessageType.SUCCESS);
                                IsCanEdit = false;
                                BtnVisibilty = Visibility.Hidden;
                                EditContent = "修改傳票內容";
                                VoucherCollectionView.Filter += VoucherFilter;
                            }
                            else if (btnName.Equals("新增"))//暫存to成立
                            {
                                MessageWindow.ShowMessage(string.Format("{0}已{1}", CurrentVoucher.JouMas_ID, btnName), MessageType.SUCCESS);
                                CurrentVoucher.JouMas_Status = "F";
                                IsProce = true;
                                IsCanEdit = false;
                                BtnEditVisibilty = Visibility.Visible;
                                BtnVisibilty = Visibility.Hidden;
                                VoucherCollectionView.Filter += VoucherFilter;
                            }
                            else
                            {
                                IsCanEdit = true;
                                BtnVisibilty = Visibility.Visible;
                                VoucherCollectionView.Filter += VoucherFilter;
                            }
                            GetMasterData(CurrentVoucher.JouMas_ID);
                            GetDetailData(CurrentVoucher.JouMas_ID);
                        }
                    }
                }
            }));
        }
        private bool CheckData()
        {
            if (CurrentVoucher.DebitDetails is null || CurrentVoucher.DebitDetails.Count == 0)
            {
                MessageWindow.ShowMessage("借方明細沒有資料", MessageType.ERROR);
                return false;
            }

            if (CurrentVoucher.CreditDetails is null || CurrentVoucher.CreditDetails.Count == 0)
            {
                MessageWindow.ShowMessage("貸方明細沒有資料", MessageType.ERROR);
                return false;
            }

            if (CurrentVoucher.DebitTotalAmount != CurrentVoucher.CreditTotalAmount)
            {
                MessageWindow.ShowMessage("借貸金額不一致!", MessageType.ERROR);
                return false;
            }

            foreach (var item in CurrentVoucher.DebitDetails)
            {
                if (item.JouDet_Amount <= 0)
                {
                    MessageWindow.ShowMessage("明細借貸金額必須大於零!", MessageType.ERROR);
                    return false;
                }
                if (item.Account is null)
                {
                    MessageWindow.ShowMessage("明細借貸科目必須填寫!", MessageType.ERROR);
                    return false;
                }
            }

            foreach (var item in CurrentVoucher.CreditDetails)
            {
                if (item.JouDet_Amount <= 0)
                {
                    MessageWindow.ShowMessage("明細借貸金額必須大於零!", MessageType.ERROR);
                    return false;
                }
                if (item.Account is null)
                {
                    MessageWindow.ShowMessage("明細借貸科目必須填寫!", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }
        private void AddAction()
        {
            InsertNewJournal();
        }
        private bool InsertNewJournal()
        {
            string newJouMasID = AccountsDb.InsertTempJournal();
            if (!string.IsNullOrEmpty(newJouMasID))
            {
                MessageWindow.ShowMessage(string.Format("{0}\r\n新增成功", newJouMasID), MessageType.SUCCESS);
                BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                EndDate = DateTime.Today;
                SearchID = string.Empty;
                KeyWord = string.Empty;
                IsTemp = true;
                IEnumerable<JournalMaster> journals = AccountsDb.GetJournalData(BeginDate, EndDate, SearchID, null, null, null, KeyWord, Type.JournalTypeID);
                VoucherCollectionView = CollectionViewSource.GetDefaultView(journals);
                foreach (var item in journals)
                {
                    if (item.JouMas_ID.Equals(newJouMasID))
                    {
                        CurrentVoucher = item;
                        GetDetailData(newJouMasID);
                        break;
                    }
                }
                VoucherCollectionView.Filter += VoucherFilter;
                return true;
            }
            else
            {
                MessageWindow.ShowMessage(string.Format("新增失敗"), MessageType.WARNING);
                return false;
            }
        }
        private void InvalidAction()
        {
            if (CurrentVoucher != null)
            {
                if (CurrentVoucher.JouMas_IsEnable == 1)
                {
                    var invalidWindow = new InvalidWindow.InvalidWindow();
                    invalidWindow.ShowDialog();
                    if ((bool)invalidWindow.DialogResult)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                        {
                            CurrentVoucher.JouMas_VoidReason = ((InvalidViewModel)invalidWindow.DataContext).VoidReason;
                            AccountsDb.UpdateJournalData("作廢", CurrentVoucher);
                            CurrentVoucher.JouMas_IsEnable = 0;
                            CurrentVoucher.JouMas_ModifyTime = DateTime.Now;
                            CurrentVoucher.JouMas_ModifyEmpID = ViewModelMainWindow.CurrentUser.ID;
                            CurrentVoucher.JouMas_ModifyEmpName = ViewModelMainWindow.CurrentUser.Name;
                            VoucherCollectionView.Filter += VoucherFilter;
                        }));
                    }
                }
            }
        }
        private void GetData()
        {
            IsProce = true;

            Accounts = AccountsDb.GetJournalAccount("ALL");
            JournalAccount empty = new JournalAccount();
            Accounts.ToList().Add(empty);
            Account = new JournalAccount();

            JournalType journalType0 = new JournalType(0, "ALL", "1:全部");
            JournalType journalType1 = new JournalType(1, "傳票作業", "2:傳票作業");
            JournalType journalType2 = new JournalType(2, "關班轉入", "3:關班轉入");
            JournalType journalType3 = new JournalType(3, "進退貨轉入", "4:進退貨轉入");

            List<JournalType> types = new List<JournalType>() { journalType0, journalType1, journalType2, journalType3 };
            Types = types;
            Type = journalType1;

            SubmitAction();
        }
        private void GetMasterData(string id)
        {
            IEnumerable<JournalMaster> master = AccountsDb.GetJournalMasterData(id);
            foreach (JournalMaster item in master)
            {
                CurrentVoucher = item;
            }
        }
        private void GetDetailData(string id)
        {
            CurrentVoucher.DebitDetails = new JournalDetails();
            CurrentVoucher.CreditDetails = new JournalDetails();
            IEnumerable<JournalDetail> details = AccountsDb.GetJournalData(id);

            IEnumerable<JournalAccount> orgAccounts = AccountsDb.GetJournalAccount("ALL");

            foreach (JournalDetail item in details)
            {
                string level1 = item.JouDet_AcctLvl1;
                string level2 = item.JouDet_AcctLvl2;
                string level3 = string.IsNullOrEmpty(item.JouDet_AcctLvl3) ? item.JouDet_AcctLvl3 : item.JouDet_AcctLvl3.PadLeft(4, '0');

                IEnumerable<JournalAccount> journal = orgAccounts.Where(s => s.acctLevel1 == level1 && s.acctLevel2 == level2 && s.acctLevel3 == level3);

                item.Accounts = orgAccounts;

                if (journal != null && journal.Count() > 0)
                {
                    foreach (var name in journal)
                    {
                        item.Account = name;
                    }
                }
                if (item.JouDet_Type.Equals("D"))
                {
                    CurrentVoucher.DebitDetails.Add(item);
                }
                else
                {
                    CurrentVoucher.CreditDetails.Add(item);
                }
            }
            CurrentVoucher.DebitTotalAmount = (int)CurrentVoucher.DebitDetails.Sum(s => s.JouDet_Amount);
            CurrentVoucher.CreditTotalAmount = (int)CurrentVoucher.CreditDetails.Sum(s => s.JouDet_Amount);
        }
        private void ExportAction()
        {
            DataTable exportData = AccountsDb.GetJournalToExcel(CurrentVoucher.JouMas_ID);
            if (exportData is null || exportData.Rows.Count == 0)
                return;

            Process myProcess = new Process();
            SaveFileDialog fdlg = new SaveFileDialog
            {
                Title = "傳票",
                InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.DeclareXmlPath) ? @"c:\" : Properties.Settings.Default.DeclareXmlPath,
                Filter = "XLSX檔案|*.xlsx",
                FileName = string.Format("傳票{0}", CurrentVoucher.JouMas_ID),
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                XLWorkbook wb = new XLWorkbook();
                var style = XLWorkbook.DefaultStyle;
                style.Border.DiagonalBorder = XLBorderStyleValues.Thick;

                IXLWorksheet ws = wb.Worksheets.Add("傳票");
                ws.Style.Font.SetFontName("Arial").Font.SetFontSize(14);
                
                ws.Cell("A1").Value = "藥局名稱";
                ws.Cell("B1").Value = "傳票日期";
                ws.Cell("C1").Value = "單據來源";
                ws.Cell("D1").Value = "傳票號碼";
                ws.Cell("E1").Value = "借方 / 貸方";
                ws.Cell("F1").Value = "項次";
                ws.Cell("G1").Value = "會計科目代號1";
                ws.Cell("H1").Value = "會計科目代號2";
                ws.Cell("I1").Value = "會計科目代號3";
                ws.Cell("J1").Value = "會計科目名稱";
                ws.Cell("K1").Value = "金額";
                ws.Cell("L1").Value = "摘要";
                ws.Cell("M1").Value = "來源單號";
                ws.Cell("N1").Value = "備註";
                ws.Cell("O1").Value = "登錄時間";
                ws.Cell("P1").Value = "登錄人";
                ws.Cell("Q1").Value = "最後修改時間";
                ws.Cell("R1").Value = "最後修改人";
                ws.Cell("S1").Value = "單據狀態";
                ws.Cell("T1").Value = "作廢原因";

                DataView dv = exportData.DefaultView;
                dv.Sort = "JouDet_Type, JouDet_Number";

                IXLRange rangeWithData = ws.Cell(2, 1).InsertData(dv.Table.AsEnumerable());
                rangeWithData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rangeWithData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber, XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(" / ", XLHFOccurrence.AllPages);
                ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.NumberOfPages, XLHFOccurrence.AllPages);
                ws.Columns().AdjustToContents();//欄位寬度根據資料調整
                try
                {
                    wb.SaveAs(fdlg.FileName);
                    ConfirmWindow cw = new ConfirmWindow("是否開啟檔案", "確認");
                    if ((bool)cw.DialogResult)
                    {
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = fdlg.FileName;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageWindow.ShowMessage(ex.Message, MessageType.ERROR);
                }
            }
        }
        #endregion
    }
}
