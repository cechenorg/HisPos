using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Data;

namespace His_Pos.NewClass.Report.IncomeStatement
{
    public class PrescriptionCount : ObservableObject
    {
        public PrescriptionCount(int month)
        {
            Count = new List<int>();
            Month = $"{month}月";
        }

        public PrescriptionCount(DataRow r)
        {
        }

        private string month;

        public string Month
        {
            get => month;
            set { Set(() => Month, ref month, value); }
        }

        private List<int> count;

        public List<int> Count
        {
            get => count;
            set { Set(() => Count, ref count, value); }
        }
    }
}