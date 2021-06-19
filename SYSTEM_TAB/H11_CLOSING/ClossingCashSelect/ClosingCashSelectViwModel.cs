using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.AccountReport.ClosingAccountReport;
using His_Pos.NewClass.AccountReport.ClosingAccountReport.ClosingAccountTargetSettingWindow;
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

        private MonthlyAccountTarget monthlyNeedGetTarget;

        public MonthlyAccountTarget MonthlyNeedGetTarget
        {
            get => monthlyNeedGetTarget;
            set
            {
                Set(() => MonthlyNeedGetTarget, ref monthlyNeedGetTarget, value);
            }
        }

        private int monthlySearchMonth = DateTime.Today.Month;

        public int MonthlySearchMonth
        {
            get => monthlySearchMonth;
            set
            {
                Set(() => MonthlySearchMonth, ref monthlySearchMonth, value);
            }
        }

        private int monthlySearchYear = DateTime.Today.Year - 1911;

        public int MonthlySearchYear
        {
            get => monthlySearchYear;
            set
            {
                Set(() => MonthlySearchYear, ref monthlySearchYear, value);
            }
        }



        public RelayCommand DailyAccountingSearchCommand { get; set; }
        public RelayCommand MonthlyClosingAccountSearchCommand { get; set; }
        public RelayCommand MonthlyTargetSettingCommand { get; set; }

        public ClosingCashSelectViwModel()
        { 
            DailyAccountingSearchCommand = new RelayCommand(DailyAccountingSearchAction);
            MonthlyClosingAccountSearchCommand = new RelayCommand(MonthlyClosingAccountSearchAction);
            MonthlyTargetSettingCommand = new RelayCommand(MonthlyTargetSettingAction);

            DailyAccountingSearchAction();
            MonthlyClosingAccountSearchAction();
        }

        private void MonthlyTargetSettingAction()
        {
            ClosingAccountTargetSettingWindow settingWin = new ClosingAccountTargetSettingWindow();
            settingWin.ShowDialog();
        }

        private void MonthlyClosingAccountSearchAction()
        {
            var firstDayOfMonth = new DateTime(MonthlySearchYear + 1911, MonthlySearchMonth, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            MonthlyAccountTargetCollection.Clear();

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
                pharmacy.MonthlyProfit = sumData.SelfProfit;
                pharmacy.TargetRatio = Math.Round( (double)pharmacy.MonthlyProfit / (double)pharmacy.MonthlyTarget * 100,2).ToString() + "%";
            }

            MonthlyAccountTarget sum = new MonthlyAccountTarget() { PharmacyName = "小計"};
            sum.MonthlyTarget = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyTarget);
            sum.MonthlyProfit = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyProfit);
            sum.TargetRatio =  Math.Round((double)sum.MonthlyProfit / (double)sum.MonthlyTarget * 100,2).ToString() + "%";
            MonthlyAccountTargetCollection.Add(sum);

            MonthlyNeedGetTarget = new MonthlyAccountTarget() { PharmacyName = "應達標準"};
            
            int i = 0;
            int workday = 0;
            int untilToday = 1;
            while (firstDayOfMonth.AddDays(i).Month == firstDayOfMonth.Month)
            {
                if (firstDayOfMonth.AddDays(i).DayOfWeek != DayOfWeek.Sunday)
                    workday++;

                if (firstDayOfMonth.AddDays(i) == DateTime.Today)
                    untilToday = workday;

                i++;
            }

            //if (firstDayOfMonth.Month != DateTime.Today.Month)
            //    todayNeedTarget.MonthlyTarget = sum.MonthlyTarget  ;
            //else
            //    todayNeedTarget.MonthlyTarget = sum.MonthlyTarget / workday * untilToday;


            //todayNeedTarget.MonthlyProfit = sum.MonthlyProfit ;
            MonthlyNeedGetTarget.TargetRatio = Math.Round((double)sum.MonthlyProfit / (double)sum.MonthlyTarget * 100, 2).ToString() + "%";
            //MonthlyAccountTargetCollection.Add(todayNeedTarget);

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

            var searchData = datalist.Where(_ => _.ClosingDate >= sDate && _.ClosingDate <= eDate).ToList();
            foreach (var pharmacy in pharmacyList)
            {
                DailyClosingAccount displayDailyClosingAccount = new DailyClosingAccount();
                displayDailyClosingAccount.PharmacyName = pharmacy.Name;
                displayDailyClosingAccount.PharmacyVerifyKey = pharmacy.VerifyKey;
                result.Add(displayDailyClosingAccount); 
                foreach (var pharmacyRecoird in searchData.Where(_ => _.PharmacyVerifyKey.ToLower() == pharmacy.VerifyKey.ToLower()))
                {
                    displayDailyClosingAccount.OTCSaleProfit += pharmacyRecoird.OTCSaleProfit;
                    displayDailyClosingAccount.ChronicAndOtherProfit += pharmacyRecoird.ChronicAndOtherProfit;
                    displayDailyClosingAccount.CooperativeClinicProfit += pharmacyRecoird.CooperativeClinicProfit;
                    displayDailyClosingAccount.DailyAdjustAmount += pharmacyRecoird.DailyAdjustAmount;
                    displayDailyClosingAccount.PrescribeProfit += pharmacyRecoird.PrescribeProfit;
                    displayDailyClosingAccount.SelfProfit += pharmacyRecoird.SelfProfit;
                    displayDailyClosingAccount.TotalProfit += pharmacyRecoird.TotalProfit;
                }
            }

            if(result.Count > 0)
                result.OrderByDescending(_ => _.SelfProfit);

            for (int i = 0; i < result.Count; i++)
            {
                result[i].OrderNumber = i+1;
            } 
            return result;
        }
    }
}
