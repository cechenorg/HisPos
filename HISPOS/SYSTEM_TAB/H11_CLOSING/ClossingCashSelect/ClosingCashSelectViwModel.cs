using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.AccountReport.ClosingAccountReport;
using His_Pos.NewClass.AccountReport.ClosingAccountReport.ClosingAccountTargetSettingWindow;
using His_Pos.NewClass.Prescription.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private ClosingCashReportDataList _prescriptionCountList = new ClosingCashReportDataList();

        public ClosingCashReportDataList PrescriptionCountList
        {
            get => _prescriptionCountList;
            set
            {
                Set(() => PrescriptionCountList, ref _prescriptionCountList, value);
            }
        }

        private ClosingCashReportDataList _prescriptionProfitList = new ClosingCashReportDataList();

        public ClosingCashReportDataList PrescriptionProfitList
        {
            get => _prescriptionProfitList;
            set
            {
                Set(() => PrescriptionProfitList, ref _prescriptionProfitList, value);
            }
        }

        private ClosingCashReportDataList _otcTurnoverList = new ClosingCashReportDataList();

        public ClosingCashReportDataList OtcTurnoverList
        {
            get => _otcTurnoverList;
            set
            {
                Set(() => OtcTurnoverList, ref _otcTurnoverList, value);
            }
        }

        private ClosingCashReportDataList _otcProfitList = new ClosingCashReportDataList();

        public ClosingCashReportDataList OtcProfitList
        {
            get => _otcProfitList;
            set
            {
                Set(() => OtcProfitList, ref _otcProfitList, value);
            }
        }

        public RelayCommand DailyAccountingSearchCommand { get; set; }
        public RelayCommand MonthlyTargetSettingCommand { get; set; }

        public ClosingCashSelectViwModel()
        { 
            DailyAccountingSearchCommand = new RelayCommand(DailyAccountingSearchAction);
            MonthlyTargetSettingCommand = new RelayCommand(MonthlyTargetSettingAction);

            DailyAccountingSearchAction();
        }

        private void MonthlyTargetSettingAction()
        {
            var settingWin = new ClosingAccountTargetSettingWindow();
            settingWin.ShowDialog();
        }

        private void DailyAccountingSearchAction()
        {
            SumDailyClosingAccount.Clear(); 

            _prescriptionProfitList.Clear();
            _prescriptionCountList.Clear();
            _otcProfitList.Clear();
            _otcTurnoverList.Clear();

            var tempPrescriptionProfitList = new List<ClosingCashReportData>();
            var tempPrescriptionCountList = new List<ClosingCashReportData>();
            var tempOtcProfitList = new List<ClosingCashReportData>();
            var tempOtcTurnoverList = new List<ClosingCashReportData>();

            foreach (var data in GetSumRecordByDate(StartDate, EndDate))
            {
                tempPrescriptionProfitList.Add(new ClosingCashReportData()
                {
                    Name = data.PharmacyName,
                    Actual = data.ChronicAndOtherProfit,
                });

                tempOtcProfitList.Add(new ClosingCashReportData()
                {
                    Name = data.PharmacyName,
                    Actual = data.OTCSaleProfit,
                });

                tempOtcTurnoverList.Add(new ClosingCashReportData()
                {
                    Name = data.PharmacyName,
                    Actual = data.OTCSaleProfit,
                });

                tempPrescriptionCountList.Add(new ClosingCashReportData()
                {
                    Name = data.PharmacyName,
                    Actual = data.OTCSaleProfit,
                });

                SumDailyClosingAccount.Add(data);
            }

            OrderRecordListByPercent(_prescriptionProfitList, tempPrescriptionProfitList);
            OrderRecordListByPercent(_prescriptionCountList, tempPrescriptionCountList);
            OrderRecordListByPercent(_otcProfitList, tempOtcProfitList);
            OrderRecordListByPercent(_otcTurnoverList, tempOtcTurnoverList);
        }

        private void OrderRecordListByPercent(ClosingCashReportDataList targetList, List<ClosingCashReportData> sourceList)
        {
            var temp = sourceList.OrderBy(_ => _.Percent).ThenBy(_ => _.Name).ToList();
            for (var index = 0; index < temp.Count; index++)
            {
                temp[index].Order = index + 1;
                targetList.Add(temp[index]);
            }
        }

        private List<DailyClosingAccount> GetSumRecordByDate(DateTime sDate, DateTime eDate)
        {
            var result = new List<DailyClosingAccount>();
            var repo = new ClosingAccountReportRepository();
            MainWindow.ServerConnection.OpenConnection();
            var datalist = repo.GetGroupClosingAccountRecord();
            var pharmacyList = repo.GetPharmacyInfosByGroupServerName(ViewModelMainWindow.CurrentPharmacy.GroupServerName);

            MainWindow.ServerConnection.CloseConnection();
            
            foreach (var pharmacy in pharmacyList)
            {
                var displayDailyClosingAccount = new DailyClosingAccount();
                displayDailyClosingAccount.PharmacyName = pharmacy.Name.Replace("藥師藥局", string.Empty).Replace("藥局", string.Empty);
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

                if (pharmacySearchData.Any())
                {
                    var sumDailyAdjustAmount = 0; 
                    var orderData = pharmacySearchData.Where(_ => _.ClosingDate >= sDate && _.ClosingDate <= eDate).ToList();
                    
                    if (orderData.Count == 1) //只查一天
                    {
                        var firstDay = orderData.FirstOrDefault();

                        if(firstDay == null)
                            break;

                        sumDailyAdjustAmount = firstDay.DailyAdjustAmount;

                        var idx = pharmacySearchData.ToList().IndexOf(firstDay);

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
                            var idx = pharmacySearchData.ToList().IndexOf(orderData.First());
                            
                            var beforeFirstDay = pharmacySearchData.ToList()[idx - 1];

                            if (beforeFirstDay.ClosingDate.Month == orderData.First().ClosingDate.Month)
                            {
                                sumDailyAdjustAmount = beforeFirstDay.DailyAdjustAmount*-1;
                            } 
                        }

                        for (var i = 1; i < orderData.Count() - 1; i++)
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

            for (var i = 0; i < result.Count; i++)
            {
                result[i].OrderNumber = i+1;
            } 
            return result;
        }
    }

    public class ClosingCashReportDataList : ObservableCollection<ClosingCashReportData> { }

    public class ClosingCashReportData
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public int Target { get; set; }
        public int Actual { get; set; }
        public int Percent => Target == 0 ? 0 : Actual / Target;
    }
}
