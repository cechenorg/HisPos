using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Report.Accounts.AccountsRecords
{
    public class AccountsRecord : ObservableObject
    {
        public AccountsRecord()
        {
            Details = new AccountsRecordDetails.AccountsRecordDetails();
        }

        public AccountsRecord(DataRow r)
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

        private AccountsRecordDetails.AccountsRecordDetail selectedDetail;

        public AccountsRecordDetails.AccountsRecordDetail SelectedDetail
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

        private AccountsRecordDetails.AccountsRecordDetails details;

        public AccountsRecordDetails.AccountsRecordDetails Details
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