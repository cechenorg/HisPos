using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.AccountReport.ClosingAccountReport;
using His_Pos.NewClass.Prescription.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.ClossingCashSelect
{
    public class ClosingCashSelectViwModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private DateTime startDate = DateTime.Today;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime endDate = DateTime.Today;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }

        private DateTime closingAccountMonth = DateTime.Today;

        public DateTime ClosingAccountMonth
        {
            get => closingAccountMonth;
            set
            {
                Set(() => ClosingAccountMonth, ref closingAccountMonth, value);
            }
        }

        private ObservableCollection<DailyClosingAccount> sumDailyClosingAccount = new ObservableCollection<DailyClosingAccount>();

        public ObservableCollection<DailyClosingAccount> SumDailyClosingAccount
        {
            get => sumDailyClosingAccount;
            set
            {
                Set(() => SumDailyClosingAccount, ref sumDailyClosingAccount, value);
            }
        }

        private ObservableCollection<MonthlyAccountTarget> monthlyAccountTargetCollection = new ObservableCollection<MonthlyAccountTarget>();

        public ObservableCollection<MonthlyAccountTarget> MonthlyAccountTargetCollection
        {
            get => monthlyAccountTargetCollection;
            set
            {
                Set(() => MonthlyAccountTargetCollection, ref monthlyAccountTargetCollection, value);
            }
        }
        

        public RelayCommand DailyAccountingSearchCommand { get; set; }
        public RelayCommand MonthlyClosingAccountSearchCommand { get; set; }


        public ClosingCashSelectViwModel() {
            DailyAccountingSearchCommand = new RelayCommand(DailyAccountingSearchAction);
            MonthlyClosingAccountSearchCommand = new RelayCommand(MonthlyClosingAccountSearchAction);
        }

        private void MonthlyClosingAccountSearchAction()
        {
            var firstDayOfMonth = new DateTime(ClosingAccountMonth.Year, ClosingAccountMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            MonthlyAccountTargetCollection.Clear();
            DailyAccountingSearchAction();
            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection(); 
            var pharmacyTargetList = repo.GetMonthTargetByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName)
                .Where(_=>_.Month.Month == firstDayOfMonth.Month).ToList(); 
            MainWindow.ServerConnection.CloseConnection();

        
            var sumRecord = GetSumRecordByDate(firstDayOfMonth, lastDayOfMonth);
            
            foreach (var pharmacy in pharmacyTargetList)
            { 
                MonthlyAccountTargetCollection.Add(pharmacy);
                var sumData = sumRecord.First(_ => _.PharmacyVerifyKey == pharmacy.VerifyKey);
                pharmacy.PharmacyName = sumData.PharmacyName;
                pharmacy.MonthlyProfit = sumData.TotalProfit;
                pharmacy.TargetRatio = pharmacy.MonthlyProfit / pharmacy.MonthlyTarget;
            }

            MonthlyAccountTarget sum = new MonthlyAccountTarget() { PharmacyName = "小計"};
            sum.MonthlyTarget = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyTarget);
            sum.MonthlyProfit = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyProfit);
            sum.TargetRatio = sum.MonthlyProfit / sum.MonthlyTarget;
            MonthlyAccountTargetCollection.Add(sum);
        }

        private void DailyAccountingSearchAction()
        {
            SumDailyClosingAccount.Clear(); 
            foreach(var data in GetSumRecordByDate(StartDate, EndDate))
            {
                SumDailyClosingAccount.Add(data);
            }

        }

        private List<DailyClosingAccount> GetSumRecordByDate(DateTime sDate, DateTime eDate)
        {
            List<DailyClosingAccount> result = new List<DailyClosingAccount>();
            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var datalist = repo.GetGroupClosingAccountRecord();
            var pharmacyList = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);
            MainWindow.ServerConnection.CloseConnection();

            var searchData = datalist.Where(_ => _.ClosingDate >= sDate && _.ClosingDate <= eDate);
            foreach (var pharmacy in pharmacyList)
            {
                DailyClosingAccount displayDailyClosingAccount = new DailyClosingAccount();
                displayDailyClosingAccount.PharmacyName = pharmacy.Name;
                displayDailyClosingAccount.PharmacyVerifyKey = pharmacy.VerifyKey;
                result.Add(displayDailyClosingAccount);

                foreach (var pharmacyRecoird in searchData.Where(_ => _.PharmacyVerifyKey == pharmacy.VerifyKey))
                {
                    displayDailyClosingAccount.OTCSaleProfit += pharmacyRecoird.OTCSaleProfit;
                    displayDailyClosingAccount.ChronicAndOtherProfit += pharmacyRecoird.ChronicAndOtherProfit;
                    displayDailyClosingAccount.CooperativeClinicProfit += pharmacyRecoird.CooperativeClinicProfit;
                    displayDailyClosingAccount.DailyAdjustAmount += pharmacyRecoird.DailyAdjustAmount;
                    displayDailyClosingAccount.PrescribeProfit += pharmacyRecoird.PrescribeProfit;
                    displayDailyClosingAccount.SelfProfit += pharmacyRecoird.SelfProfit;
                }
            }
            return result;
        }
    }
}
