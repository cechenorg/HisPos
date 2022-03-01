using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Report.CashFlow.CashFlowRecords
{
    public class CashFlowRecord : ObservableObject
    {
        public CashFlowRecord()
        {
            Details = new CashFlowRecordDetails.CashFlowRecordDetails();
        }

        public CashFlowRecord(DataRow r)
        {
        }

        private decimal totalValue;

        public decimal TotalValue
        {
            get => totalValue;
            set
            {
                Set(() => TotalValue, ref totalValue, value);
            }
        }

        private DateTime? date;

        public DateTime? Date
        {
            get => date;
            set
            {
                Set(() => Date, ref date, value);
            }
        }

        private CashFlowRecordDetails.CashFlowRecordDetail selectedDetail;

        public CashFlowRecordDetails.CashFlowRecordDetail SelectedDetail
        {
            get => selectedDetail;
            set
            {
                if (selectedDetail != null)
                    (selectedDetail).IsSelected = false;

                Set(() => SelectedDetail, ref selectedDetail, value);

                if (selectedDetail != null)
                    (selectedDetail).IsSelected = true;
            }
        }

        private CashFlowRecordDetails.CashFlowRecordDetails details;

        public CashFlowRecordDetails.CashFlowRecordDetails Details
        {
            get => details;
            set
            {
                Set(() => Details, ref details, value);
            }
        }

        private string content;

        public string Content
        {
            get => content;
            set
            {
                Set(() => Content, ref content, value);
            }
        }

        public void CountTotalValue()
        {
            TotalValue = Details.Sum(d => d.CashFlowValue);
        }
    }
}