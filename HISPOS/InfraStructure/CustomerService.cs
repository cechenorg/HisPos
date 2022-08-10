using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.InfraStructure
{
    public class CustomerService
    {

        public static Customer GetCustomerByCusId(int cusId)
        {
            DataTable table = CustomerDb.GetCustomerByCusId(cusId);
            var customer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            /* 格式化手機 */

            if (!string.IsNullOrEmpty(customer.CellPhone))
            {
                if(customer.CellPhone.Length == 10)
                    customer.CellPhone = customer.CellPhone.Insert(4, "-").Insert(8, "-");

                if(customer.SecondPhone.Length == 10)
                    customer.SecondPhone = customer.SecondPhone.Insert(4, "-").Insert(8, "-");
            }
            /* 格式化電話 */
            if (!string.IsNullOrEmpty(customer.Tel))
            {
                var telLen = customer.Tel.Length;
                string FormatTel = default;
                switch (telLen)
                {
                    case 7:
                        FormatTel = customer.Tel.Insert(3, "-");
                        break;
                    case 8:
                        FormatTel = customer.Tel.Insert(4, "-");
                        break;
                    case 9:
                        FormatTel = customer.Tel.Insert(2, "-").Insert(6, "-");
                        break;
                    case 10:
                        FormatTel = customer.Tel.Insert(2, "-").Insert(7, "-");
                        break;

                }
                customer.Tel = FormatTel;
            }

            return customer;
        }

        public static bool InsertData(Customer cus)
        {
            var table = CustomerDb.InsertNewCustomerData(cus, 0);//新增客戶
            if (table != null && table.Rows.Count > 0)
            {
                int cus_id = Convert.ToInt32(table.Rows[0]["Person_Id"]);
                table = CustomerDb.GetCustomerByCusId(cus_id);//查詢客戶資料
                if (table != null && table.Rows.Count > 0)
                {
                    cus = new Customer(table.Rows[0]);
                    
                    return true;
                }
            }
            MessageWindow.ShowMessage("新增病患資料發生異常，請稍後重試。", MessageType.ERROR);
            return false;
        }

        public static string InsertNewData(Customer cus)
        {
            var table = CustomerDb.InsertNewCustomerData(cus, 1);
            if (table != null && table.Rows.Count > 0)
            {
                string result = table.Rows[0].Field<string>("RESULT");
                switch (result)
                {
                    case "IDSAME":
                        return "ID_SAME";
                    case "PHONESAME":
                        return "PHONE_SAME";
                    case "":
                        cus.ID = Convert.ToInt32(table.Rows[0]["Person_Id"]);
                        return "SUCCESS";
                }
            }
            MessageWindow.ShowMessage("新增顧客資料發生異常，請稍後重試。", MessageType.ERROR);
            return "FAILED";
        }
    }
}
