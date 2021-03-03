using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.Accounts.AccountsRecordDetails
{
    public class AccountsRecordDetail : ObservableObject
    {
        public AccountsRecordDetail()
        {
        }

        public AccountsRecordDetail(DataRow r)
        {
            ID = r.Field<int>("CashFlow_ID");
            Name = r.Field<string>("CashFlow_Name");
            Note = r.Field<string>("CashFlow_Note");
            CashFlowValue = decimal.ToInt32(r.Field<decimal>("CashFlow_Value"));
            Date = r.Field<DateTime>("CashFlow_Time");
            EmpName = r.Field<string>("Emp_Name");
            CanEdit = DateTime.Compare(Date, DateTime.Today) >= 0 || ViewModelMainWindow.CurrentUser.ID == 1;
        }

        private int id;

        public int ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }

        private string note;

        public string Note
        {
            get => note;
            set
            {
                Set(() => Note, ref note, value);
            }
        }

        private int cashFlowValue;

        public int CashFlowValue
        {
            get => cashFlowValue;
            set
            {
                Set(() => CashFlowValue, ref this.cashFlowValue, value);
            }
        }

        private DateTime date;

        public DateTime Date
        {
            get => date;
            set
            {
                Set(() => Date, ref date, value);
            }
        }

        private string empName;

        public string EmpName
        {
            get => empName;
            set
            {
                Set(() => EmpName, ref empName, value);
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                Set(() => IsSelected, ref isSelected, value);
            }
        }

        private bool canEdit;

        public bool CanEdit
        {
            get => canEdit;
            set
            {
                Set(() => CanEdit, ref canEdit, value);
            }
        }
    }
}