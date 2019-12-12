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
            ID = r.Field<int>("CashFlow_ID");
            Name = r.Field<string>("CashFlow_Name");
            Note = r.Field<string>("CashFlow_Source");
            Value = r.Field<decimal>("CashFlow_Value");
            Date = r.Field<DateTime>("CashFlow_Time");
            EmpName = r.Field<string>("Emp_Name");
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
        private decimal value;
        public decimal Value
        {
            get => value;
            set
            {
                Set(() => Value, ref this.value, value);
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
    }
}
