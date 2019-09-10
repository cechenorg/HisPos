using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Data;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using IcCard = His_Pos.NewClass.Prescription.ICCard.IcCard;

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
        private bool isEnable = true;
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                Set(() => IsEnable, ref isEnable, value); 
            }
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

        public Customer(Cooperative.CooperativeInstitution.Customer customer,int birthYear, int birthMonth, int birthDay)
        {
            IDNumber = customer.IdNumber;
            Name = customer.Name;
            Birthday = new DateTime(birthYear, birthMonth, birthDay);
            Tel = customer.Phone;
        }

        public Customer(CooperativePrescription.Customer customer, int birthYear, int birthMonth, int birthDay)
        {
            IDNumber = customer.IdNumber;
            Name = customer.Name;
            if (birthYear >= 1911)
            {
                Birthday = new DateTime(birthYear, birthMonth, birthDay);
            }
            Tel = customer.Phone;
        }

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
            if (ID == 0 || IDNumber.Equals("A111111111") || Name.Equals("匿名"))
            {
                MessageWindow.ShowMessage("匿名資料不可編輯",MessageType.ERROR);
                return;
            }
            CustomerDb.Save(this);
        }

        public static Customer GetCustomerByCusId(int cusId)
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
            if (Birthday is null) return 100;
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

        public bool CheckData()
        {
            return !string.IsNullOrEmpty(IDNumber) && VerifyService.VerifyIDNumber(IDNumber.Trim()) && Birthday != null && !string.IsNullOrEmpty(Name);
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
            c.IsEnable = IsEnable;
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

        public bool InsertData()
        {
            var table = CustomerDb.InsertCustomerData(this);
            if (table.Rows.Count > 0)
            {
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
                return true;
            }
            MessageWindow.ShowMessage("新增病患資料發生異常，請稍後重試。", MessageType.ERROR);
            return false;
        }

        public void GetHistories()
        {
            Histories = new CustomerHistories(ID);
            HistoryCollectionViewSource = new CollectionViewSource { Source = Histories };
            HistoryCollectionView =HistoryCollectionViewSource.View;
        }

        public void CheckPatientWithCard(Customer patientFromCard)
        {
            if (!IDNumber.Equals(patientFromCard.IDNumber))
                IDNumber = patientFromCard.IDNumber;
            if (!Name.Equals(patientFromCard.Name))
                Name = patientFromCard.Name;
            if (!Birthday.Equals(patientFromCard.Birthday))
                Birthday = patientFromCard.Birthday;
            CheckGender();
        }

        public bool CheckIDNumberEmpty()
        {
            return string.IsNullOrEmpty(IDNumber);
        }

        public bool CheckNameEmpty()
        {
            return string.IsNullOrEmpty(Name);
        }

        public bool CheckBirthdayNull()
        {
            return Birthday is null;
        }

        public bool CheckTelEmpty()
        {
            return string.IsNullOrEmpty(Tel);
        }

        public bool CheckCellPhoneEmpty()
        {
            return string.IsNullOrEmpty(CellPhone);
        }
    }
}
