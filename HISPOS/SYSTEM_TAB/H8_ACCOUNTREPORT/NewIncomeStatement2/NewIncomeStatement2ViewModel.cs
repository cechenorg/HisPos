using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.IncomeStatement;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement2
{
    internal class NewIncomeStatement2ViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        private int _year = DateTime.Today.Year;

        public int Year
        {
            get => _year;
            set
            {
                Set(() => Year, ref _year, value);
            }
        }

        private ObservableCollection<IncomeStatementDisplayData> _incomeStatementData;

        public ObservableCollection<IncomeStatementDisplayData> IncomeStatementData
        {
            get => _incomeStatementData;
            set
            {
                Set(() => IncomeStatementData, ref _incomeStatementData, value);
            }
        }

        private IncomeStatementDisplayData _selectedIncomeStatementData;

        public IncomeStatementDisplayData SelectedIncomeStatementData
        {
            get => _selectedIncomeStatementData;
            set
            {
                Set(() => SelectedIncomeStatementData, ref _selectedIncomeStatementData, value);
            }
        }


        public ICommand SearchCommand { get; set; }
        public ICommand YearMinusCommand { get; set; }
        public ICommand YearAddCommand { get; set; }

        public ICommand OpenDetailCommand { get; set; }

        public NewIncomeStatement2ViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            YearAddCommand = new RelayCommand(YearAdd);
            YearMinusCommand = new RelayCommand(YearMinus);
            OpenDetailCommand = new RelayCommand(OpenDetail);

            Search();
        }

        private void OpenDetail()
        {
            if(SelectedIncomeStatementData is null)
                return;

            var expenseDatas = ReportService.GetIncomeStatementDetail(_year, Convert.ToString(_selectedIncomeStatementData.TypeID), Convert.ToString(_selectedIncomeStatementData.AccID)).ToList();


            if (expenseDatas.Count > 0)
            {
                AccountDetailWindow accountDetailWindow = new AccountDetailWindow()
                {
                    DataContext = new AccountDetailViewModel(expenseDatas)
                };
                accountDetailWindow.ShowDialog();
            }
          
        }

        private void YearAdd()
        {
            Year++;
            Search();
        }

        private void YearMinus()
        {
            Year--;
            Search();
        }


        private void Search()
        {

            var expenseDatas = ReportService.GetIncomeStatement(_year);
            GetExpenseData(expenseDatas);

        }

        private void GetExpenseData(IEnumerable<IncomeStatementRawData> rawData)
        {
            IncomeStatementData = new ObservableCollection<IncomeStatementDisplayData>();
          
            foreach (var typeName in rawData.Select(_ => _.ISType).Distinct())
            {

                var typeID = rawData.First(_ => _.ISType == typeName).ISTypeNo;

                IncomeStatementDisplayData typeIncomeData = new IncomeStatementDisplayData() { Name = typeName,TypeID = typeID };
                IncomeStatementData.Add(typeIncomeData);

                foreach (var groupNo in rawData.Where(_ => _.ISType == typeName).Select(_ => _.ISGroupNo).Distinct())
                {

                    if (groupNo == 0)
                    {
                        typeIncomeData.DisplayLayerCount = 2;
                        foreach (var accID in rawData.Where(_ => _.ISType == typeName && _.ISGroupNo == groupNo).Select(_ => _.AcctID).Distinct())
                        {
                            string accName = rawData.First(_ => _.AcctID == accID).AcctName;
                            IncomeStatementDisplayData accIncomeData = new IncomeStatementDisplayData()
                            {
                                Name = accName,AccID = accID, TypeID = typeIncomeData.TypeID

                            };
                            typeIncomeData.Childs.Add(accIncomeData);

                            var accfilterData = rawData.Where(_ => _.AcctID == accID).OrderBy(_ => _.MM);

                            foreach (var fdata in accfilterData)
                            {
                                accIncomeData.MonthlyValues[fdata.MM - 1] = fdata.AcctValue;
                            }
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            typeIncomeData.MonthlyValues[i] = typeIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                        }
                    }
                    else
                    {
                        typeIncomeData.DisplayLayerCount = 3;
                        string groupName = rawData.First(_ => _.ISType == typeName && _.ISGroupNo == groupNo).ISGroup;
                        IncomeStatementDisplayData groupIncomeData = new IncomeStatementDisplayData() { Name = groupName, };
                        typeIncomeData.Childs.Add(groupIncomeData);

                        foreach (var accID in rawData.Where(_ => _.ISType == typeName && _.ISGroupNo == groupNo).Select(_ => _.AcctID).Distinct())
                        {
                            string accName = rawData.First(_ => _.AcctID == accID).AcctName;
                            IncomeStatementDisplayData accIncomeData = new IncomeStatementDisplayData() { Name = accName, AccID = accID, TypeID = typeIncomeData.TypeID };
                            groupIncomeData.Childs.Add(accIncomeData);

                            var accfilterData = rawData.Where(_ => _.AcctID == accID).OrderBy(_ => _.MM);

                            foreach (var fdata in accfilterData)
                            {
                                accIncomeData.MonthlyValues[fdata.MM - 1] = fdata.AcctValue;
                            }
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            groupIncomeData.MonthlyValues[i] = groupIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                        }
                    }

                   

                }
                for (int i = 0; i < 12; i++)
                {
                    typeIncomeData.MonthlyValues[i] = typeIncomeData.Childs.Sum(_ => _.MonthlyValues[i]);
                }
            }

            IncomeStatementDisplayData totalSumData = new IncomeStatementDisplayData() { Name = "總和",DisplayLayerCount = 1};

            for (int i = 0; i < 12; i++)
            {
                totalSumData.MonthlyValues[i] = IncomeStatementData.Select(_ => _.MonthlyValues[i]).Sum();
            }
            IncomeStatementData.Add(totalSumData);

            
        }
    }
}
