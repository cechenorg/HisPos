using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
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

        private List<IncomeStatementDisplayData> _incomeStatementData;

        public List<IncomeStatementDisplayData> IncomeStatementData
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

            var rawDatas = ReportService.GetIncomeStatementRawData(_year);

        }
    }
}
