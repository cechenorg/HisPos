using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using His_Pos.Properties;

namespace His_Pos.Class
{
    public class Customer
    {
        public string Id { get; set; }
        public string CusotmerId { get; set; }
        public string Name { get; set; }
        public string Qname { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string EmergentTel { get; set; }
        public string IdNumber { get; set; }
        public string Email { get; set; }
        public string ICCardNumber { get; set; }//卡片號碼
        public string ICLogoutMark { get; set; }//卡片註銷註記
        public string MedicalNumber { get; set; }//就醫序號
        public bool Gender { get; set; }
        

        public bool CheckCustomerExist(string customerId)
        {
            var listparam1 = new List<SqlParameter>();
            var idnum = new SqlParameter("@CUS_IDNUM", customerId);
            listparam1.Add(idnum);
            var dd = new DbConnection(Settings.Default.SQL_local);
            var table = dd.ExecuteProc("[HIS_POS_DB].[GET].[CHECKCUSTOMER]", listparam1);

            if (table.Rows.Count != 0)
            {
                Id = table.Rows[0]["CUS_ID"].ToString();
                Address = table.Rows[0]["CUS_ADDR"].ToString();
                Email = table.Rows[0]["CUS_EMAIL"].ToString();
                Tel = table.Rows[0]["CUS_TEL"].ToString();
                return true;
            }
            return false;
        }

        public void InsertCustomerData(Customer newCustomer)
        {
            var listparam = new List<SqlParameter>();
            var name = new SqlParameter("@NAME", newCustomer.Name);
            var qname = new SqlParameter("@QNAME", newCustomer.Qname);
            var birth = new SqlParameter("@BIRTH", Convert.ToDateTime(newCustomer.Birthday));
            var addr = new SqlParameter("@ADDR", newCustomer.Address);
            var tel = new SqlParameter("@TEL", newCustomer.EmergentTel);
            var idnum = new SqlParameter("@IDNUM", newCustomer.CusotmerId);
            var email = new SqlParameter("@EMAIL", newCustomer.Email);
            var Gender = new SqlParameter("@GENDER", newCustomer.Gender);
            listparam.Add(name);
            listparam.Add(qname);
            listparam.Add(birth);
            listparam.Add(addr);
            listparam.Add(tel);
            listparam.Add(idnum);
            listparam.Add(email);
            listparam.Add(Gender);
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
