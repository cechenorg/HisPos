using System;
using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Report.CashFlow.CashFlowRecordDetails
{
    public class CashFlowRecordDetail:ObservableObject
    {
        public CashFlowRecordDetail()
        {

        }
        public CashFlowRecordDetail(DataRow r)
        {

        }

        private string cashRecDetName;
        public string CashRecDetName
        {
            get => cashRecDetName;
            set
            {
                Set(() => CashRecDetName, ref cashRecDetName, value);
            }
        }

        private int cashRecDetValue;
        public int CashRecDetValue
        {
            get => cashRecDetValue;
            set
            {
                Set(() => CashRecDetValue, ref cashRecDetValue, value);
            }
        }

        private DateTime? cashRecDate;
        public DateTime? CashRecDate
        {
            get => cashRecDate;
            set
            {
                Set(() => CashRecDate, ref cashRecDate, value);
            }
        }

        private int cashRecDetEmpName;
        public int CashRecDetEmpName
        {
            get => cashRecDetEmpName;
            set
            {
                Set(() => CashRecDetEmpName, ref cashRecDetEmpName, value);
            }
        }

        private string cashRecDetNote;
        public string CashRecDetNote
        {
            get => cashRecDetNote;
            set
            {
                Set(() => CashRecDetNote, ref cashRecDetNote, value);
            }
        }
    }
}
