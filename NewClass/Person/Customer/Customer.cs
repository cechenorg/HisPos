using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using His_Pos.NewClass.CooperativeInstitution;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person
    {
        public Customer() {}

        public Customer(DataRow r) : base(r)
        {
            ContactNote = r.Field<string>("Cus_UrgentNote");
        } 
        public string ContactNote { get; set; }//連絡備註
        public DateTime? LastEdit { get; set; }//最後編輯時間
        public CustomerHistories Histories { get; set; }//處方.自費調劑紀錄
        #region Function
        public void Save()
        {
        }
        public void Delete()
        {
        }
        public Customer GetCustomerByCusId(int cusId)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = CustomerDb.GetCustomerByCusId(cusId);
            var customer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            MainWindow.ServerConnection.CloseConnection();
            return customer;
        }
        public Customer Check() {
            MainWindow.ServerConnection.OpenConnection();
            DataTable table = CustomerDb.CheckCustomer(this);
            var result = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
        #endregion
    }
}
