using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public static class CustomerDb
    {
        public static void InsertCustomerData(Customer newCustomer)
        {
            var listparam = new List<SqlParameter>();
            var name = new SqlParameter("NAME", newCustomer.Name);
            var qname = new SqlParameter("QNAME", newCustomer.Qname);
            var birth = new SqlParameter("BIRTH", Convert.ToDateTime(newCustomer.Birthday));
            var addr = new SqlParameter("ADDR", newCustomer.ContactInfo.Address);
            var tel = new SqlParameter("TEL", newCustomer.ContactInfo.Tel);
            var idnum = new SqlParameter("IDNUM", newCustomer.IcNumber);
            var email = new SqlParameter("EMAIL", newCustomer.ContactInfo.Email);
            var gender = new SqlParameter("GENDER", newCustomer.Gender);
            listparam.Add(name);
            listparam.Add(qname);
            listparam.Add(birth);
            listparam.Add(addr);
            listparam.Add(tel);
            listparam.Add(idnum);
            listparam.Add(email);
            listparam.Add(gender);
            var dd = new DbConnection(Settings.Default.SQL_local);
            try
            {
                dd.ExecuteProc("[HIS_POS_DB].[SET].[REGISTERCUSTOMER]", listparam);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        internal static string CheckCustomerExist(Customer customer)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CUS_NAME", customer.Name));
            parameters.Add(new SqlParameter("CUS_BIRTH", customer.Birthday));
            parameters.Add(new SqlParameter("CUS_IDNUM", customer.IcNumber));
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionInquireView].[CheckCustomerExist]", parameters);
            return table.Rows[0][0].ToString();
        }
        internal static ObservableCollection<Customer> GetCustomerData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[CustomerManageView].[GetCustomerData]");
            ObservableCollection<Customer> data = new ObservableCollection<Customer>();
            foreach (DataRow row in table.Rows) {
                data.Add(new Customer(row, "fromDb"));
            }
            return data;
        }
    }
}
