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
            ClickSourceCommand = new RelayCommand(ClickSourceAction);
            CopyDataCommand = new RelayCommand(CopyDataAction);
            StrikeCommand = new RelayCommand(StrikeAction);
            FilterCommand = new RelayCommand<string>(FilterAction);
            DeleteDetailCommand = new RelayCommand<string>(DeleteDetailAction);
            AddNewDetailCommand = new RelayCommand<string>(AddNewDetailAction);
            GetData();
            IsProce = true;
        }
        #region Command
        public RelayCommand CopyDataCommand { get; set; }
        public RelayCommand StrikeCommand { get; set; }
        public RelayCommand<string> FilterCommand { get; set; }
        public RelayCommand<string> DeleteDetailCommand { get; set; }
        public RelayCommand<string> AddNewDetailCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand InvalidCommand { get; set; }
        public RelayCommand ClickSourceCommand { get; set; }
        #endregion
        #region
        private ICollectionView voucherCollectionView;
        public ICollectionView VoucherCollectionView
        {
            get => voucherCollectionView;
            set
            {
                Set(() => VoucherCollectionView, ref voucherCollectionView, value);
            }
        }
        public string BtnName
        {
            get => btnName;
            set
            {
                Set(() => BtnName, ref btnName, value);
            }
        }
        private string btnName = "新增";
        public bool IsBtnEnable
        {
            get => isBtnEnable;
            set
            {
                Set(() => IsBtnEnable, ref isBtnEnable, value);
            }
        }
        private bool isBtnEnable = true;
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
        private bool isProce;
        public bool IsProce
        {
            get => isProce;
            set
            {
                Set(() => IsProce, ref isProce, value);
            }
        }
        private bool isTemp;
        public bool IsTemp
        {
            get => isTemp;
            set
            {
                Set(() => IsTemp, ref isTemp, value);
            }
        }
        private bool isInvalid;
        public bool IsInvalid
        {
            get => isInvalid;
            set
            {
                Set(() => IsInvalid, ref isInvalid, value);
            }
        }
        public IEnumerable<JournalAccount> Accounts { get; set; }
        public JournalAccount Account { get; set; }

        public IEnumerable<JournalType> Types { get; set; }
        public JournalType Type { get; set; }

        private JournalMaster currentVoucher;
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
                    if(CurrentVoucher.JouMas_ID != null && CurrentVoucher.JouMas_Status.Equals("T"))
                    {
                        if(CurrentVoucher.DebitDetails != null && CurrentVoucher.CreditDetails != null && CurrentVoucher.JouMas_IsEnable == 1)
                        {
                            AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                        }
                    }
                    if (!string.IsNullOrEmpty(CurrentVoucher.JouMas_ID))
                    {
                        GetDetailData(CurrentVoucher.JouMas_ID);
                    }
                }
            }
        }
        #endregion
        #region Function
        private void CopyDataAction()
        {
            if (currentVoucher.JouMas_Status.Equals("F"))
            {
                JournalMaster journalMaster = currentVoucher;//紀錄目前傳票
                bool isSuccess = InsertNewJournal();
                if (isSuccess)
                {
                    foreach (JournalDetail item in journalMaster.DebitDetails)
                    {
                        CopyDetailData(item, true);
                    }
                    foreach (JournalDetail item in journalMaster.CreditDetails)
                    {
                        CopyDetailData(item, false);
                    }
                    AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                }
            }
        }
        private void CopyDetailData(JournalDetail detail, bool isDebit)
        {
            JournalDetail item = new JournalDetail();
            item.JouDet_ID = detail.JouDet_ID;
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
                    AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                }
            }
        }
        private void SubmitAction()
        {
            VoucherCollectionView = CollectionViewSource.GetDefaultView(AccountsDb.GetJournalData(beginDate, endDate, string.IsNullOrEmpty(searchID)? string.Empty : searchID, Account.acctLevel1, Account.acctLevel2, Account.acctLevel3, string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord, Type.JournalTypeID));
            CurrentVoucher = new JournalMaster();
            CurrentVoucher = VoucherCollectionView.CurrentItem as JournalMaster;
            if (CurrentVoucher != null && (CurrentVoucher.JouMas_ID != null || CurrentVoucher.JouMas_ID != ""))
            {
                GetDetailData(CurrentVoucher.JouMas_ID);
            }
            VoucherCollectionView.Filter += VoucherFilter;
        }
        private void ClearAction()
        {
            SearchID = string.Empty;
            KeyWord = string.Empty;
            Account = new JournalAccount();
        }
        private void FilterAction(string filterCondition)
        {
            if (VoucherCollectionView != null)
            {
                if (VoucherCollectionView.CanFilter)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(delegate
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
        private bool VoucherFilter(object journal)
        {
            JournalMaster jm = (JournalMaster)journal;
            if(IsInvalid)
            {
                if(jm.JouMas_IsEnable == 0)
                {
                    BtnName = "新增";
                    IsBtnEnable = false;
                    return true;
                }
            }
            else if(IsTemp)
            {
                if (jm.JouMas_Status.Equals("T") && jm.JouMas_IsEnable == 1)
                {
                    BtnName = "新增";
                    IsBtnEnable = true;
                    return true;
                }
            }
            else
            {
                if (jm.JouMas_Status.Equals("F") && jm.JouMas_IsEnable == 1)
                {
                    BtnName = "修改";
                    IsBtnEnable = true;
                    return true;
                }
            }
            return false;
        }
        private void ClickSourceAction()
        {

        }
        private void SaveAction()
        {
            if (CurrentVoucher != null)
            {
                if (CurrentVoucher.JouMas_IsEnable == 1)
                {
                    if (CheckData())
                    {
                        AccountsDb.UpdateJournalData(btnName, CurrentVoucher);
                        MessageWindow.ShowMessage(string.Format("{0}已{1}", CurrentVoucher.JouMas_ID, btnName), MessageType.SUCCESS);
                        CurrentVoucher.JouMas_Status = "F";
                        VoucherCollectionView.Filter += VoucherFilter;
                    }
                }
            }
        }
        private bool CheckData()
        {
            if(CurrentVoucher.DebitTotalAmount != CurrentVoucher.CreditTotalAmount)
            {
                MessageWindow.ShowMessage("借貸金額不一致!", MessageType.ERROR);
                return false;
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
                    AccountsDb.UpdateJournalData("作廢", CurrentVoucher);
                    CurrentVoucher.JouMas_IsEnable = 0;
                    VoucherCollectionView.Filter += VoucherFilter;
                }
            }
        }
        private void GetData()
        {
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
        }
        private void GetDetailData(string id)
        {
            CurrentVoucher.DebitDetails = new JournalDetails();
            CurrentVoucher.CreditDetails = new JournalDetails();
            var details = AccountsDb.GetJournalData(id);
            foreach (var item in details)
            {
                var journal = Accounts.Where(s => s.acctLevel1 == item.JouDet_AcctLvl1 && s.acctLevel2 == item.JouDet_AcctLvl2 && s.acctLevel3 == (string.IsNullOrEmpty(item.JouDet_AcctLvl3) ? item.JouDet_AcctLvl3 : item.JouDet_AcctLvl3.PadLeft(4, '0')));

                item.Accounts = Accounts;

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
        
        #endregion
    }
}
