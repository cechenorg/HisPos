using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.AccountReport.ClosingAccountReport.ClosingAccountTargetSettingWindow
{
    public class ClosingAccountTargetSettingVM : ViewModelBase
    {
        private readonly Action closeAction;
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

        public ClosingAccountTargetSettingVM(Action closeAction)
        {
            this.closeAction = closeAction;

            SearchCommand = new RelayCommand(SearchAction);
            UpdateCommand = new RelayCommand(UpdateAction);

            SearchAction();
        }

        private void UpdateAction()
        {
            var repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();

            foreach (var data in TargetDataCollection)
            {
                repo.UpdateClosingAccountTarget(data);
            }

            repo.UpdateWorkingDaySetting(ClosingAccountMonth, CurrentMonthWorkingDayCount);
            MainWindow.ServerConnection.CloseConnection();

            MessageWindow.ShowMessage("更新完畢!", MessageType.SUCCESS);
            closeAction.Invoke();
        }

        private void SearchAction()
        {
            TargetDataCollection = new ObservableCollection<MonthlyAccountTarget>(GetTargetData());
        }

        private IEnumerable<MonthlyAccountTarget> GetTargetData()
        {
            var result = new List<MonthlyAccountTarget>();

            var repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var gtroupServerInfo = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);
            var pharmacyTargetList = repo.GetMonthTargetByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName)
                .Where(_ => _.Month.Month == ClosingAccountMonth.Month && _.Month.Year == ClosingAccountMonth.Year).ToList();

            foreach (var pharmacyInfo in gtroupServerInfo)
            {
                var target = new MonthlyAccountTarget();
                var data = pharmacyTargetList.FirstOrDefault(_ => _.VerifyKey.ToLower() == pharmacyInfo.VerifyKey.ToLower());

                if (data != null)
                {
                    target.MonthlyTarget = data.MonthlyTarget;
                    target.PrescriptionCountTarget = data.PrescriptionCountTarget;
                    target.OtcProfitTarget = data.OtcProfitTarget;
                    target.OtcTurnoverTarget = data.OtcTurnoverTarget;
                    target.DrugProfitTarget = data.DrugProfitTarget;
                }

                target.VerifyKey = pharmacyInfo.VerifyKey;
                target.PharmacyName = pharmacyInfo.Name;
                target.Month = ClosingAccountMonth;
                result.Add(target);
            }

            var thisMonthWorkingSetting = repo.GetWorkingDaySetting().FirstOrDefault(_ => _.Date.Month == ClosingAccountMonth.Month && _.Date.Year == ClosingAccountMonth.Year);
            CurrentMonthWorkingDayCount = thisMonthWorkingSetting == null ? DateTime.DaysInMonth(ClosingAccountMonth.Year, ClosingAccountMonth.Month) : thisMonthWorkingSetting.DayCount;

            MainWindow.ServerConnection.CloseConnection();

            return result;
        }
    }
}
