﻿using GalaSoft.MvvmLight.Command;
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

        private int monthlyNeedWorkingDayCount;

        public int MonthlyNeedWorkingDayCount
        {
            get => monthlyNeedWorkingDayCount;
            set
            {
                Set(() => MonthlyNeedWorkingDayCount, ref monthlyNeedWorkingDayCount, value);
            }
        }

        private MonthlyAccountTarget monthlySelfAccount ;

        public MonthlyAccountTarget MonthlySelfAccount
        {
            get => monthlySelfAccount;
            set
            {
                Set(() => MonthlySelfAccount, ref monthlySelfAccount, value);
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
               
                var sumData = sumRecord.First(_ => _.PharmacyVerifyKey.ToLower() == pharmacy.VerifyKey.ToLower());
                pharmacy.PharmacyName = sumData.PharmacyName;
                pharmacy.MonthlyProfit = sumData.SelfProfit;
                pharmacy.TargetRatio = Math.Round( (double)pharmacy.MonthlyProfit / (double)pharmacy.MonthlyTarget * 100,2).ToString() + "%";
            }

            pharmacyTargetList =  pharmacyTargetList.OrderByDescending(_ => Math.Round((double)_.MonthlyProfit / (double)_.MonthlyTarget * 100, 2)).ToList();
            foreach (var pharmacy in pharmacyTargetList)
            {
                MonthlyAccountTargetCollection.Add(pharmacy); 
            }

            MonthlyAccountTarget sum = new MonthlyAccountTarget() { PharmacyName = "小計"};
            sum.MonthlyTarget = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyTarget);
            sum.MonthlyProfit = MonthlyAccountTargetCollection.Sum(_ => _.MonthlyProfit);
            sum.TargetRatio =  Math.Round((double)sum.MonthlyProfit / (double)sum.MonthlyTarget * 100,2).ToString() + "%";
            //MonthlyAccountTargetCollection.Add(sum);
            MonthlySelfAccount = sum;
            var workedDay = repo.GetGroupClosingAccountRecord().Where(_ => _.ClosingDate >= firstDayOfMonth && _.ClosingDate <= lastDayOfMonth).Select(_ => _.ClosingDate).Distinct().Count();
            var targetratio =  Math.Round((double)workedDay / (double) monthlyNeedWorkingDayCount*100,2);

            MonthlyNeedGetTarget = new MonthlyAccountTarget() { PharmacyName = "應達標準"};

            //MonthlyNeedGetTarget.TargetRatio = Math.Round((double)sum.MonthlyProfit / targetratio * 100, 2).ToString() + "%";
            MonthlyNeedGetTarget.TargetRatio = targetratio.ToString() + "%";
             
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

            var workingSetting = repo.GetWorkingDaySetting();
            if (workingSetting
                .Count(_ => _.Date.Year == DateTime.Today.Year && _.Date.Month == DateTime.Today.Month) == 0)
            {
               var lastMonthData = workingSetting.FirstOrDefault(_ => _.Date.Year == DateTime.Today.AddMonths(-1).Year && _.Date.Month == DateTime.Today.AddMonths(-1).Month);
               repo.UpdateWorkingDaySetting(DateTime.Today, lastMonthData.DayCount);

               MonthlyNeedWorkingDayCount = lastMonthData.DayCount;
            }
            else
            {
                MonthlyNeedWorkingDayCount = workingSetting.FirstOrDefault(_ =>
                    _.Date.Year == (MonthlySearchYear+1911) && _.Date.Month == MonthlySearchMonth).DayCount; 
            }
                
            MainWindow.ServerConnection.CloseConnection();
            
            foreach (var pharmacy in pharmacyList)
            {
                DailyClosingAccount displayDailyClosingAccount = new DailyClosingAccount();
                displayDailyClosingAccount.PharmacyName = pharmacy.Name;
                displayDailyClosingAccount.PharmacyVerifyKey = pharmacy.VerifyKey;
                result.Add(displayDailyClosingAccount);

                var pharmacySearchData =
                    datalist.Where(_ => _.PharmacyVerifyKey.ToLower() == pharmacy.VerifyKey.ToLower()).OrderBy(_ => _.ClosingDate);
                foreach (var pharmacyRecoird in pharmacySearchData.Where(_ => _.ClosingDate >= sDate && _.ClosingDate <= eDate))
                {
                    displayDailyClosingAccount.OTCSaleProfit += pharmacyRecoird.OTCSaleProfit;
                    displayDailyClosingAccount.ChronicAndOtherProfit += pharmacyRecoird.ChronicAndOtherProfit;
                    displayDailyClosingAccount.CooperativeClinicProfit += pharmacyRecoird.CooperativeClinicProfit; 
                    displayDailyClosingAccount.PrescribeProfit += pharmacyRecoird.PrescribeProfit;
                    displayDailyClosingAccount.SelfProfit += pharmacyRecoird.SelfProfit;
                    displayDailyClosingAccount.TotalProfit += pharmacyRecoird.TotalProfit;
                }

              
                if (pharmacySearchData.Count() > 0)
                {
                   
                    int sumDailyAdjustAmount = 0; 
                    var orderData = pharmacySearchData.Where(_ => _.ClosingDate >= sDate && _.ClosingDate <= eDate).ToList();
                   
                    
                    if (orderData.Count == 1) //只查一天
                    {
                        var firstDay = orderData.FirstOrDefault();

                        if(firstDay == null)
                            break;

                        sumDailyAdjustAmount = firstDay.DailyAdjustAmount;

                        int idx = pharmacySearchData.ToList().IndexOf(firstDay);

                        if (idx > 0)
                        {
                            var beforeFirstDay = pharmacySearchData.ToList()[idx - 1];

                            if (beforeFirstDay.ClosingDate.Month == firstDay.ClosingDate.Month)
                            {
                                sumDailyAdjustAmount -= beforeFirstDay.DailyAdjustAmount;
                            }

                        }
                    }
                    else if(orderData.Count > 1)
                    {
                        //計算第一個月的判斷
                        if (orderData[0].ClosingDate.AddMonths(1).Month == orderData[1].ClosingDate.Month) //判斷是否第一筆為該月最後一筆,如果是則當正的
                        {
                            sumDailyAdjustAmount = orderData.First().DailyAdjustAmount;
                        }
                        else //判斷第一筆是否為本月第一筆,若是的話則當作0,若否則往前一筆*-1
                        {
                            int idx = pharmacySearchData.ToList().IndexOf(orderData.First());
                            
                            var beforeFirstDay = pharmacySearchData.ToList()[idx - 1];

                            if (beforeFirstDay.ClosingDate.Month == orderData.First().ClosingDate.Month)
                            {
                                sumDailyAdjustAmount = beforeFirstDay.DailyAdjustAmount*-1;
                            } 
                        }

                        for (int i = 1; i < orderData.Count() - 1; i++)
                        {
                            if (orderData[i].ClosingDate.AddMonths(1).Month == orderData[i + 1].ClosingDate.Month)
                            {
                                sumDailyAdjustAmount += orderData[i].DailyAdjustAmount;
                            }
                        }

                        sumDailyAdjustAmount += orderData.Last().DailyAdjustAmount;
                    }
                        
                   
                    displayDailyClosingAccount.DailyAdjustAmount += sumDailyAdjustAmount;
                }
               

               
            }

            if(result.Count > 0)
                result = result.OrderByDescending(_ => _.SelfProfit).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].OrderNumber = i+1;
            } 
            return result;
        }
    }
}