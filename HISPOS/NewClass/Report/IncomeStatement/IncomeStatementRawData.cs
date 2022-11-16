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
    public class IncomeStatementDisplayData :ObservableObject
    {
        public string Name { get; set; }

        public List<int> MonthlyValues { get; set;}

        public ObservableCollection<IncomeStatementDisplayData> Childs { get; set; }
    }

    public class IncomeStatementRawData
    {
        public IncomeStatementRawData()
        {

        }
        public string Name { get; set; }

        public DateTime DT { get; set; }

        public int Value { get; set; }

        public string Note { get; set; }

        public string Source { get; set; }
    }
}
