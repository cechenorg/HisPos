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
        public int YYYY { get; set; }
        public int MM { get; set; }

        public int AccID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

    }
}
