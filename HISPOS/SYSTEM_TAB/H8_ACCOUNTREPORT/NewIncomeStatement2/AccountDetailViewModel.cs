using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Report;
using His_Pos.NewClass.Report.IncomeStatement;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.NewIncomeStatement2
{
    internal class AccountDetailViewModel : ViewModelBase
    {

        private List<IncomeStatementDetailData> _incomeStatementDetailDataList;

        public List<IncomeStatementDetailData> IncomeStatementDetailDataList
        {
            get => _incomeStatementDetailDataList;
            set
            {
                Set(() => IncomeStatementDetailDataList, ref _incomeStatementDetailDataList, value);
            }
        }
        public AccountDetailViewModel(List<IncomeStatementDetailData>  data)
        {
            IncomeStatementDetailDataList = data;
        }

        public int TotalAmount
        {
            get => totalAmount;
            set
            {
                Set(() => TotalAmount, ref totalAmount, value);
            }
        }
        private int totalAmount;
    }
}
