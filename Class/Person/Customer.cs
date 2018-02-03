using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Person
{
    public class Customer : AbstractClass.Person
    {
        public string Qname { get; set; }
        public bool Gender { get; set; }
        public ContactInfo ContactInfo { get; set; }
        /*
         *檢查顧客是否存在
         */
        public bool CheckCustomerExist(string idCardNumber)
        {
            var listparam1 = new List<SqlParameter>();
            var idnum = new SqlParameter("@CUS_IDNUM", idCardNumber);
            listparam1.Add(idnum);
            var dd = new DbConnection(Settings.Default.SQL_local);
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[CHECKCUSTOMER]", listparam1);
            if (table.Rows.Count == 0) return false;
            Id = table.Rows[0]["CUS_ID"].ToString();
            ContactInfo = new ContactInfo(table.Rows[0]["CUS_ADDR"].ToString(), table.Rows[0]["CUS_EMAIL"].ToString(), table.Rows[0]["CUS_TEL"].ToString());
            return true;
        }

        public void InsertCustomerData(Customer newCustomer)
        {
            var listparam = new List<SqlParameter>();
            var name = new SqlParameter("@NAME", newCustomer.Name);
            var qname = new SqlParameter("@QNAME", newCustomer.Qname);
            var birth = new SqlParameter("@BIRTH", Convert.ToDateTime(newCustomer.Birthday));
            var addr = new SqlParameter("@ADDR", newCustomer.ContactInfo.Address);
            var tel = new SqlParameter("@TEL", newCustomer.ContactInfo.Tel);
            var idnum = new SqlParameter("@IDNUM", newCustomer.IcNumber);
            var email = new SqlParameter("@EMAIL", newCustomer.ContactInfo.Email);
            var gender = new SqlParameter("@GENDER", newCustomer.Gender);
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
    }
}
