using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Report.IncomeStatement;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.IncomeStatement
{
    public class IncomeStatementViewModel : TabBase
    {
        private int year;
        public int Year
        {
            get => year;
            set
            {
                Set(() => Year, ref year, value);
            }
        }

        private IncomeStatementMatrix incomeStatementMatrix;
        public IncomeStatementMatrix IncomeStatementMatrix
        {
            get => incomeStatementMatrix;
            set
            {
                Set(() => IncomeStatementMatrix, ref incomeStatementMatrix, value);
            }
        }
        public override TabBase getTab()
        {
            return this;
        }

        public IncomeStatementViewModel()
        {
            IncomeStatementMatrix = new IncomeStatementMatrix(2019);
        }
    }
}
