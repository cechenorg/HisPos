using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using IcCard = His_Pos.NewClass.Prescription.IcCard;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer:Person,ICloneable
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
        private CollectionViewSource historyCollectionViewSource;
        public CollectionViewSource HistoryCollectionViewSource
        {
            get => historyCollectionViewSource;
            set
            {
                Set(() => HistoryCollectionViewSource, ref historyCollectionViewSource, value);
            }
        }

        private ICollectionView historyCollectionView;
        public ICollectionView HistoryCollectionView
        {
            get => historyCollectionView;
            set
            {
                Set(() => HistoryCollectionView, ref historyCollectionView, value);
            }
        }
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
        public Customers Check() {
            var table = CustomerDb.CheckCustomer(this);
            var customers = new Customers();
            if (table.Rows.Count == 0) return customers;
            foreach (DataRow r in table.Rows)
            {
                customers.Add(new Customer(r));
            }
            return customers;
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
        public bool CheckData()
        {
            return (!string.IsNullOrEmpty(IDNumber) && IDNumber.Trim().Length == 10) && Birthday != null && !string.IsNullOrEmpty(Name);
        }
        public object Clone()
        {
            var c = new Customer();
            c.ContactNote = ContactNote;
            c.LastEdit = LastEdit;
            c.Address = Address;
            c.Birthday = Birthday;
            c.CellPhone = CellPhone;
            c.Email = Email;
            c.Gender = Gender;
            c.ID = ID;
            c.IDNumber = IDNumber;
            c.Line = Line;
            c.Name = Name;
            c.Note = Note;
            c.Tel = Tel;
            c.Histories = new CustomerHistories();
            if (Histories != null)
            {
                foreach (var h in Histories)
                    c.Histories.Add(h);
                c.HistoryCollectionViewSource = new CollectionViewSource{Source = c.Histories };
                c.HistoryCollectionView = HistoryCollectionViewSource.View;
            }
            return c;
        }

        public void InsertData()
        {
            var table = CustomerDb.InsertCustomerData(this);
            var c = new Customer(table.Rows[0]);
            ID = c.ID;
            Name = c.Name;
            IDNumber = c.IDNumber;
            Birthday = c.Birthday;
            Tel = c.Tel;
            ContactNote = c.ContactNote;
            LastEdit = c.LastEdit;
            Address = c.Address;
            CellPhone = c.CellPhone;
            Email = c.Email;
            Gender = c.Gender;
            Line = c.Line;
            Note = c.Note;
        }

        public void GetHistories()
        {
            Histories = new CustomerHistories(ID);
            HistoryCollectionViewSource = new CollectionViewSource { Source = Histories };
            HistoryCollectionView =HistoryCollectionViewSource.View;
        }

        public bool CheckIDNumberExist()
        {
            var table = CustomerDb.CheckCustomerIDNumberExist(IDNumber);
            return table.Rows[0].Field<int>("Count") > 0;
        }
    }
}
