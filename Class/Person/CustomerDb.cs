using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public static class CustomerDb
    {
        internal static string CheckCustomerExist(Customer customer)
        {
            var dd = new DbConnection(Settings.Default.SQL_global);

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CUS_NAME", customer.Name));
            parameters.Add(customer.Birthday.Year!=1
                ? new SqlParameter("CUS_BIRTH", customer.Birthday)
                : new SqlParameter("CUS_BIRTH", DBNull.Value));
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

        internal static ObservableCollection<Customer> LoadCustomerData(Customer c)
        {
            var customerLCollection = new ObservableCollection<Customer>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>
            {
                string.IsNullOrEmpty(c.Id)
                    ? new SqlParameter("CUS_ID", DBNull.Value)
                    : new SqlParameter("CUS_ID", c.Id),
                string.IsNullOrEmpty(c.Name)
                ? new SqlParameter("CUS_NAME", DBNull.Value)
                : new SqlParameter("CUS_NAME", c.Name),
                c.Birthday.Year > 1
                ? new SqlParameter("CUS_BIRTH", c.Birthday)
                : new SqlParameter("CUS_BIRTH", DBNull.Value),
                string.IsNullOrEmpty(c.IcCard.IcNumber)
                    ? new SqlParameter("CUS_IDNUM", DBNull.Value)
                    : new SqlParameter("CUS_IDNUM", c.IcCard.IcNumber)
            };
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[LoadCustomerData]", parameters);
            foreach (DataRow row in table.Rows)
            {
                customerLCollection.Add(new Customer(row, "fromDb"));
            }
            return customerLCollection;
        }
        internal static Collection<string> GetPositionData() {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[EmployeeManageView].[GetPositionData]");
            Collection<string> data = new Collection<string>();
            foreach (DataRow row in table.Rows)
            {
                data.Add(row["AUTHGROUP_NAME"].ToString());
            }
            return data;
        }

        internal static ObservableCollection<Customer> GetData()
        {
            var customerLCollection = new ObservableCollection<Customer>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetAllCustomerData]");
            foreach (DataRow row in table.Rows)
            {
                customerLCollection.Add(new Customer(row, "fromDb"));
            }
            return customerLCollection;
        }
        internal static void UpdateCustomerById(Customer customer) {
            var customerLCollection = new ObservableCollection<Customer>();
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
          
                parameters.Add(new SqlParameter("CUS_ID", customer.Id));
                parameters.Add(new SqlParameter("CUS_NAME", customer.Name));
                parameters.Add(new SqlParameter("CUS_GENDER", customer.Gender));
            if (string.IsNullOrEmpty(customer.Qname))
                parameters.Add(new SqlParameter("CUS_QNAME", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_QNAME", customer.Qname));
            if (string.IsNullOrEmpty(customer.Birthday.ToString()))
                parameters.Add(new SqlParameter("CUS_BIRTH", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_BIRTH", customer.Birthday));
            if (string.IsNullOrEmpty(customer.ContactInfo.Address))
                parameters.Add(new SqlParameter("CUS_ADDR", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_ADDR", customer.ContactInfo.Address));
            if (string.IsNullOrEmpty(customer.ContactInfo.Tel))
                parameters.Add(new SqlParameter("CUS_TEL", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_TEL", customer.ContactInfo.Tel));
            if (string.IsNullOrEmpty(customer.IcNumber))
                parameters.Add(new SqlParameter("CUS_IDNUM", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_IDNUM", customer.IcNumber));
            if (string.IsNullOrEmpty(customer.ContactInfo.Email))
                parameters.Add(new SqlParameter("CUS_EMAIL", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_EMAIL", customer.ContactInfo.Email)); 
            if (string.IsNullOrEmpty(customer.EmergentTel))
                parameters.Add(new SqlParameter("CUS_EMERGENTTEL", DBNull.Value));
            else
                parameters.Add(new SqlParameter("CUS_EMERGENTTEL", customer.EmergentTel)); 

            dd.ExecuteProc("[HIS_POS_DB].[CustomerManageView].[UpdateCustomerById]",parameters);
        }
          
    }
}
