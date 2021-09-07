﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport.ClosingAccountTargetSettingWindow
{
    public class ClosingAccountTargetSettingVM : ViewModelBase
    {
        private DateTime closingAccountMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        public DateTime ClosingAccountMonth
        {
            get => closingAccountMonth;
            set
            {
                Set(() => ClosingAccountMonth, ref closingAccountMonth, value);
            }
        }

        private ObservableCollection<MonthlyAccountTarget> targetDataCollection = new ObservableCollection<MonthlyAccountTarget>();

        public ObservableCollection<MonthlyAccountTarget> TargetDataCollection
        {
            get => targetDataCollection;
            set
            {
                Set(() => TargetDataCollection, ref targetDataCollection, value);
            }
        }

        private int currentMonthWorkingDayCount;

        public int CurrentMonthWorkingDayCount
        {
            get => currentMonthWorkingDayCount;
            set
            {
                Set(() => CurrentMonthWorkingDayCount, ref currentMonthWorkingDayCount, value);
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }

        public ClosingAccountTargetSettingVM()
        {
            SearchCommand = new RelayCommand(SearchAction);
            UpdateCommand = new RelayCommand(UpdateAction);

            SearchAction();
        }

        private void UpdateAction()
        {
            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();

            foreach (var data in TargetDataCollection)
            {
                repo.UpdateClosingAccountTarget(data);
            }

            repo.UpdateWorkingDaySetting(ClosingAccountMonth, CurrentMonthWorkingDayCount);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void SearchAction()
        {
            TargetDataCollection.Clear();
            var firstDayOfMonth = new DateTime(ClosingAccountMonth.Year, ClosingAccountMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var gtroupServerInfo = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);
            var pharmacyTargetList = repo.GetMonthTargetByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName)
                .Where(_ => _.Month.Month == ClosingAccountMonth.Month).ToList();

            foreach (var data in pharmacyTargetList)
            {
                MonthlyAccountTarget pharmacyTaget = new MonthlyAccountTarget();
                var info = gtroupServerInfo.First(_ => _.VerifyKey == data.VerifyKey);
                pharmacyTaget.VerifyKey = info.VerifyKey;
                pharmacyTaget.PharmacyName = info.Name;
                pharmacyTaget.MonthlyTarget = data.MonthlyTarget;
                pharmacyTaget.Month = ClosingAccountMonth;
                TargetDataCollection.Add(pharmacyTaget);
            }

            var thisMonthWorkingSetting = repo.GetWorkingDaySetting().FirstOrDefault(_ => _.Date.Month == ClosingAccountMonth.Month && _.Date.Year == ClosingAccountMonth.Year);
            CurrentMonthWorkingDayCount = thisMonthWorkingSetting.DayCount;

            MainWindow.ServerConnection.CloseConnection();
        }
    }
}