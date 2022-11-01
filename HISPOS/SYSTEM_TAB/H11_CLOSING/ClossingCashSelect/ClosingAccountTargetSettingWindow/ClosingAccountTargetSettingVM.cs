using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            TargetDataCollection = new ObservableCollection<MonthlyAccountTarget>(GetTargetData()); 
        }

        private IEnumerable<MonthlyAccountTarget> GetTargetData()
        {
            List<MonthlyAccountTarget> result = new List<MonthlyAccountTarget>();

            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var gtroupServerInfo = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);
            var pharmacyTargetList = repo.GetMonthTargetByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName)
                .Where(_ => _.Month.Month == ClosingAccountMonth.Month).ToList();

            foreach (var data in pharmacyTargetList)
            {
                MonthlyAccountTarget pharmacyTaget = new MonthlyAccountTarget();
                var info = gtroupServerInfo.First(_ => _.VerifyKey.ToLower() == data.VerifyKey.ToLower());
                pharmacyTaget.VerifyKey = info.VerifyKey;
                pharmacyTaget.PharmacyName = info.Name;
                pharmacyTaget.MonthlyTarget = data.MonthlyTarget;
                pharmacyTaget.Month = ClosingAccountMonth;
                result.Add(pharmacyTaget);
            }

            var thisMonthWorkingSetting = repo.GetWorkingDaySetting().FirstOrDefault(_ => _.Date.Month == ClosingAccountMonth.Month && _.Date.Year == ClosingAccountMonth.Year);
            CurrentMonthWorkingDayCount = thisMonthWorkingSetting == null ? DateTime.DaysInMonth(ClosingAccountMonth.Year, ClosingAccountMonth.Month) : thisMonthWorkingSetting.DayCount;

            MainWindow.ServerConnection.CloseConnection();

            return result;
        }
    }
}
