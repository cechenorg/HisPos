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

        private ObservableCollection<DailyClosingAccount> sumDailyClosingAccount = new ObservableCollection<DailyClosingAccount>();

        public ObservableCollection<DailyClosingAccount> SumDailyClosingAccount
        {
            get => sumDailyClosingAccount;
            set
            {
                Set(() => SumDailyClosingAccount, ref sumDailyClosingAccount, value);
            }
        }

        public RelayCommand DailyAccountingSearchCommand { get; set; }

        public ClosingCashSelectViwModel() {
            DailyAccountingSearchCommand = new RelayCommand(DailyAccountingSearchAction);
        }

        private void DailyAccountingSearchAction()
        {
            SumDailyClosingAccount.Clear();

            ClosingAccountReportRepository repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var datalist = repo.GetGroupClosingAccountRecord();
            var pharmacyList = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);
            MainWindow.ServerConnection.CloseConnection();

            var searchData = datalist.Where(_ => _.ClosingDate >= StartDate && _.ClosingDate <= EndDate);
            foreach (var pharmacy in pharmacyList)
            {
                DailyClosingAccount displayDailyClosingAccount = new DailyClosingAccount();
                displayDailyClosingAccount.PharmacyName = pharmacy.Name;
                displayDailyClosingAccount.PharmacyVerifyKey = pharmacy.VerifyKey;
                SumDailyClosingAccount.Add(displayDailyClosingAccount);

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
        }
    }
}
