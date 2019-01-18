using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person
    { 
        public Customer() {}

        public Customer(DataRow r) : base(r)
        {
            ContactNote = r.Field<string>("Cus_UrgentNote");
            LastEdit = r.Field<DateTime>("Cus_EditTime");
        } 
        public string ContactNote { get; set; }//連絡備註
        public DateTime? LastEdit { get; set; }//最後編輯時間
        public CustomerHistories Histories { get; set; }//處方.自費調劑紀錄
        #region Function
        public void Save()
        {
            CustomerDb.Save(this);
        }
        public void Delete()
        {
        }
        public Customer GetCustomerByCusId(int cusId)
        {
            DataTable table = CustomerDb.GetCustomerByCusId(cusId);
            var customer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            return customer;
        }
        public Customer Check() {
            DataTable table = CustomerDb.CheckCustomer(this);
            Customer newcustomer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            return newcustomer;
        }
        public void UpdateEditTime() {
            CustomerDb.UpdateEditTime(Id);
        }

        #endregion

        public int CountAge()
        {
            var today = DateTime.Today;
            Debug.Assert(Birthday != null, nameof(Birthday) + " != null");
            var birthdate = (DateTime)Birthday;
            var age = today.Year - birthdate.Year;
            if (birthdate > today.AddYears(-age)) age--;
            return age;
        }

        public DateTimeExtensions.Age CountAgeToMonth()
        {
            return DateTimeExtensions.CalculateAgeToMonth((DateTime)Birthday);
        }

        public int CheckAgePercentage()
        {
            var cusAge = CountAgeToMonth();
            if (cusAge.Years == 0 && cusAge.Months < 6)
            {
                return 160;
            }
            if (cusAge.Years > 0 && cusAge.Years < 2)
            {
                return 130;
            }
            if (cusAge.Years == 2)
            {
                if (cusAge.Months == 0 && cusAge.Days == 0)
                    return 130;
                return 120;
            }
            if (cusAge.Years > 2 && cusAge.Years <= 6)
            {
                if (cusAge.Years == 6)
                {
                    if (cusAge.Months == 0 && cusAge.Days == 0)
                        return 120;
                    return 100;
                }
                return 120;
            }
            return 100;
        }
    }
}
