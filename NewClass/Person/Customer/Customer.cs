using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using IcCard = His_Pos.NewClass.Prescription.IcCard;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person
    {
        public Customer() {}

        public Customer(DataRow r) : base(r)
        {
            ContactNote = r.Field<string>("Cus_UrgentNote");
            LastEdit = r.Field<DateTime?>("Cus_EditTime");
        }

        public Customer(IcCard card)
        {
            Name = card.Name;
            IDNumber = card.IDNumber;
            Birthday = card.Birthday;
            Gender = card.Gender;
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
        public void Check() {
            DataTable table = CustomerDb.CheckCustomer(this);
            Customer newCustomer = table.Rows.Count == 0 ? null : new Customer(table.Rows[0]);
            if (newCustomer is null)
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MessageWindow.ShowMessage("取得顧客資料發生異常，請重試。", MessageType.WARNING);
                }));
                return;
            }
            ID = newCustomer.ID;
            Name = newCustomer.Name;
            Birthday = newCustomer.Birthday;
            IDNumber = newCustomer.IDNumber;
            Tel = newCustomer.Tel;
            ContactNote = newCustomer.ContactNote;
            CellPhone = newCustomer.CellPhone;
            Line = newCustomer.Line;
            Note = newCustomer.Note;
            Address = newCustomer.Address;
            Email = newCustomer.Email;
            Gender = newCustomer.Gender;
            LastEdit = newCustomer.LastEdit;
        }
        public void UpdateEditTime() {
            CustomerDb.UpdateEditTime(ID);
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

        private DateTimeExtensions.Age CountAgeToMonth()
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

        private string CheckBirthday()
        {
            if (Birthday is null) return "請填寫病患出生年月日\r\n";
            return DateTime.Compare((DateTime) Birthday, DateTime.Today) > 0 ? "出生年月日不可大於今天\r\n" : string.Empty;
        }

        private string CheckIDNumber()
        {
            return string.IsNullOrEmpty(IDNumber) ? "請填寫病患身分證字號\r\n" : string.Empty;
        }
        private string CheckName()
        {
            return string.IsNullOrEmpty(Name) ? "請填寫病患姓名\r\n" : string.Empty;
        }

        public string CheckBasicData()
        {
            return
            CheckBirthday()+
            CheckIDNumber()+
            CheckName();
        }
        public int Count()
        {
            var count = (CustomerDb.GetCustomerCountByCustomer(this).Rows[0]).Field<int>("Count");
            return count;
        }
    }
}
