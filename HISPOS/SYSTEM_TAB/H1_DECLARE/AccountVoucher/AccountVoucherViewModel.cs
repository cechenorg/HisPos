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
    public class AccountVoucherViewModel: TabBase
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
            FilterCommand = new RelayCommand<string>(FilterAction);
            DeleteDetailCommand = new RelayCommand<string>(DeleteDetailAction);
            AddNewDetailCommand = new RelayCommand<string>(AddNewDetailAction);
            GetData();
            IsProce = true;
        }
        #region Command
        public RelayCommand<string> FilterCommand { get; set; }
        public RelayCommand<string> DeleteDetailCommand { get; set; }
        public RelayCommand<string> AddNewDetailCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand InvalidCommand { get; set; }
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
                    if(CurrentVoucher.JouMas_ID != null)
                    {
                        if(CurrentVoucher.DebitDetails != null && CurrentVoucher.CreditDetails != null && CurrentVoucher.JouMas_IsEnable == 1)
                        {
                            AccountsDb.UpdateJournalData("保存", CurrentVoucher);
                        }
                        GetDetailData(CurrentVoucher.JouMas_ID);
                    }
                }
            }
        }
        #endregion
        #region Function
        private void SubmitAction()
        {
            VoucherCollectionView = CollectionViewSource.GetDefaultView(AccountsDb.GetJournalData(beginDate, endDate, string.IsNullOrEmpty(searchID)? string.Empty : searchID, Account.acctLevel1, Account.acctLevel2, Account.acctLevel3, string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord));
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
                            break;
                        case "1":
                            CurrentVoucher.CreditDetails.Remove(CurrentVoucher.SelectedCreditDetail);
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
                    detail.Accounts = AccountsDb.GetJournalAccount();
                    switch (gridCondition)
                    {
                        case "0":
                            CurrentVoucher.DebitDetails.Add(detail);
                            break;
                        case "1":
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
                    return true;
                }
            }
            else if(IsTemp)
            {
                if (jm.JouMas_Status.Equals("T") && jm.JouMas_IsEnable == 1)
                {
                    BtnName = "新增";
                    return true;
                }
            }
            else
            {
                if (jm.JouMas_Status.Equals("F") && jm.JouMas_IsEnable == 1)
                {
                    BtnName = "修改";
                    return true;
                }
            }
            return false;
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
            string newJouMasID = AccountsDb.InsertTempJournal();
            if(!string.IsNullOrEmpty(newJouMasID))
            {
                MessageWindow.ShowMessage(string.Format("{0}\r\n新增成功", newJouMasID), MessageType.SUCCESS);
                BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                EndDate = DateTime.Today;
                SearchID = string.Empty;
                KeyWord = string.Empty;
                IsTemp = true;
                IEnumerable<JournalMaster> journals = AccountsDb.GetJournalData(BeginDate, EndDate, SearchID, null, null, null, KeyWord);
                VoucherCollectionView = CollectionViewSource.GetDefaultView(journals);
                foreach (var item in journals)
                {
                    if(item.JouMas_ID.Equals(newJouMasID))
                    {
                        CurrentVoucher = item;
                        GetDetailData(newJouMasID);
                        break;
                    }
                }
                VoucherCollectionView.Filter += VoucherFilter;
            }
            else
            {
                MessageWindow.ShowMessage(string.Format("新增失敗"), MessageType.WARNING);
            }
        }
        private void InvalidAction()
        {
            if (CurrentVoucher != null)
            {
                if (CurrentVoucher.JouMas_IsEnable == 1)
                {
                    AccountsDb.InvalidJournalData(CurrentVoucher.JouMas_ID);
                    CurrentVoucher.JouMas_IsEnable = 0;
                    VoucherCollectionView.Filter += VoucherFilter;
                }
            }
        }
        private void GetData()
        {
            Accounts = AccountsDb.GetJournalAccount();
            JournalAccount empty = new JournalAccount();
            Accounts.ToList().Add(empty);
            Account = new JournalAccount();
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
