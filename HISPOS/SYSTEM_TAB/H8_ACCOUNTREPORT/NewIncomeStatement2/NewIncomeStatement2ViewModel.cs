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

        public ICommand SearchCommand { get; set; }

        public NewIncomeStatement2ViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            Search();
        }

        private void Search()
        {

            var expenseDatas = ReportService.GetIncomeExpense(_year);
            GetExpenseData(expenseDatas);

        }

        private void GetExpenseData(IEnumerable<IncomeStatementRawData> rawData)
        {
            IncomeStatementData = new ObservableCollection<IncomeStatementDisplayData>();
          
            foreach (var raw in rawData.GroupBy(_ => _.Name).Select(_=> _.Key))
            {
                IncomeStatementDisplayData displayData = new IncomeStatementDisplayData(){Name = raw};

                var filterData = rawData.Where(_ => _.Name == raw).OrderBy(_ => _.MM);


                foreach (var fdata in filterData)
                {
                    displayData.MonthlyValues[fdata.MM-1] = fdata.Value;
                }

                displayData.Childs.Add(displayData);

                IncomeStatementData.Add(displayData);
            }

            IncomeStatementDisplayData totalSumData = new IncomeStatementDisplayData() { Name = "總和" };

            for (int i = 0; i < 12; i++)
            {
                totalSumData.MonthlyValues[i] = IncomeStatementData.Select(_ => _.MonthlyValues[i]).Sum();
            }
            IncomeStatementData.Add(totalSumData);

            
        }
    }
}
