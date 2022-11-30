using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class IncomeStatementDisplayData : ObservableObject
    {
        public string Name { get; set; }

        public int[] MonthlyValues { get; set; } = new int[12];
        public int MonthlySum { get => MonthlyValues.Sum(); }

        private int _displayLayerCount;
        public int DisplayLayerCount
        {
            get => _displayLayerCount;
            set
            {
                Set(() => _displayLayerCount, ref _displayLayerCount, value);
            }
        }

        public ObservableCollection<IncomeStatementDisplayData> Childs { get; set; } =
            new ObservableCollection<IncomeStatementDisplayData>();

        public int TypeID { get; set; }
        public int AccID { get; set; }
    }

    public class IncomeStatementRawData
    {
        public IncomeStatementRawData()
        {

        }
        public int YYYY { get; set; }
        public int MM { get; set; }

        public int ISTypeNo { get; set; }
        public string ISType { get; set; }
        public int ISGroupNo { get; set; }
        public string ISGroup { get; set; }

        public int AcctID { get; set; }
        public string AcctName { get; set; }

        public int AcctValue { get; set; }

    }

    public class IncomeStatementDetailData
    {
        public IncomeStatementDetailData()
        {

        }

        public string AcctName { get; set; }

        public DateTime AcctDate { get; set; }

        public int AcctValue { get; set; }

        public string AcctNote { get; set; }

        public string AcctSource { get; set; }
    }
}
