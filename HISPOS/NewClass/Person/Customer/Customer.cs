﻿using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Cooperative.XmlOfPrescription;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using IcCard = His_Pos.NewClass.Prescription.ICCard.IcCard;

namespace His_Pos.NewClass.Person.Customer
{
    public class Customer : Person, ICloneable
    {
        public Customer()
        {
            ID = -1;
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

        private bool isEmp;

        public bool IsEmp
        {
            get
            {
                return isEmp;
            }
            set
            {
                Set(() => IsEmp, ref isEmp, value);
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

        //11.02

        public CustomerRecords Records { get; set; }//處方.自費調劑紀錄
        private CollectionViewSource recordCollectionViewSource;

        public CollectionViewSource RecordCollectionViewSource
        {
            get => recordCollectionViewSource;
            set
            {
                Set(() => RecordCollectionViewSource, ref recordCollectionViewSource, value);
            }
        }

        private ICollectionView recordCollectionView;

        public ICollectionView RecordCollectionView
        {
            get => recordCollectionView;
            set
            {
                Set(() => RecordCollectionView, ref recordCollectionView, value);
            }
        }

        //11.02^^

        public Customer(Cooperative.CooperativeInstitution.Customer customer, int birthYear, int birthMonth, int birthDay)
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
            if (IDNumber != null)
            {
                if (ID == 0 || IDNumber.Equals("A111111111") || Name.Equals("匿名"))
                {
                    MessageWindow.ShowMessage("匿名資料不可編輯", MessageType.ERROR);
                    return;
                }
            }

            if (CustomerDb.Save(this) == false)
            {
                MessageWindow.ShowMessage("顧客資料儲存失敗, 請確認身分證是否重複", MessageType.ERROR);
            }
        }

        public static Customer GetCustomerByCusId(int cusId)
        {
            Customer customer = CustomerDb.GetCustomerByCusId(cusId);
            
            /* 格式化手機 */
            if (!string.IsNullOrEmpty(customer.CellPhone) && customer.CellPhone.Length == 10)
            {
                string FormatCell = customer.CellPhone.Insert(4, "-").Insert(8, "-");
                customer.CellPhone = FormatCell;
            }
            if (!string.IsNullOrEmpty(customer.SecondPhone) && customer.SecondPhone.Length == 10)
            {
                string FormatCell = customer.SecondPhone.Insert(4, "-").Insert(8, "-");
                customer.SecondPhone = FormatCell;
            }
            /* 格式化電話 */
            if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 7)
            {
                string FormatTel = customer.Tel.Insert(3, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 8)
            {
                string FormatTel = customer.Tel.Insert(4, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 9)
            {
                string FormatTel = customer.Tel.Insert(2, "-").Insert(6, "-");
                customer.Tel = FormatTel;
            }
            else if (!string.IsNullOrEmpty(customer.Tel) && customer.Tel.Length == 10)
            {
                string FormatTel = customer.Tel.Insert(2, "-").Insert(7, "-");
                customer.Tel = FormatTel;
            }
            return customer;
        }

        public Customers Check()
        {
            var customerList = CustomerDb.CheckCustomer(this);
            var customers = new Customers();
            if (customerList == null || customerList.Count() == 0) 
                return customers;
            foreach (Customer cus in customerList)
            {
                customers.Add(cus);
            }
            return customers;
        }

        public void UpdateEditTime()
        {
            CustomerDb.UpdateEditTime(ID);
        }

        #endregion Function

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
            c.SecondPhone = SecondPhone;
            c.IsEnable = IsEnable;
            c.Histories = new CustomerHistories();
            if (Histories != null)
            {
                foreach (var h in Histories)
                    c.Histories.Add(h);
                c.HistoryCollectionViewSource = new CollectionViewSource { Source = c.Histories };
                c.HistoryCollectionView = HistoryCollectionViewSource.View;
            }
            return c;
        }

        public bool InsertData()
        {
            var table = CustomerDb.InsertNewCustomerData(this, 0);//新增客戶
            if (table != null && table.Rows.Count > 0)
            {
                int cus_id = Convert.ToInt32(table.Rows[0]["Person_Id"]);
                Customer customer = CustomerDb.GetCustomerByCusId(cus_id);//查詢客戶資料
                if(customer != null)
                {
                    ID = customer.ID;
                    Name = customer.Name;
                    IDNumber = customer.IDNumber;
                    Birthday = customer.Birthday;
                    Tel = customer.Tel;
                    ContactNote = customer.ContactNote;
                    LastEdit = customer.LastEdit;
                    Address = customer.Address;
                    CellPhone = customer.CellPhone;
                    Email = customer.Email;
                    Gender = customer.Gender;
                    Line = customer.Line;
                    Note = customer.Note;
                    SecondPhone = customer.SecondPhone;
                    return true;
                }
            }
            MessageWindow.ShowMessage("新增病患資料發生異常，請稍後重試。", MessageType.ERROR);
            return false;
        }

        public string InsertNewData()
        {
            var table = CustomerDb.InsertNewCustomerData(this, 1);
            if(table != null && table.Rows.Count > 0)
            {
                if (table.Rows[0].Field<string>("RESULT").Equals("IDSAME"))
                {
                    //MessageWindow.ShowMessage("身分證字號已存在！", MessageType.ERROR);
                    return "ID_SAME";
                }
                if (table.Rows[0].Field<string>("RESULT").Equals("PHONESAME"))
                {
                    //MessageWindow.ShowMessage("電話號碼已存在！", MessageType.ERROR);
                    return "PHONE_SAME";
                }
                if (table.Rows[0].Field<string>("RESULT").Equals(string.Empty))
                {
                    ID = Convert.ToInt32(table.Rows[0]["Person_Id"]);
                    return "SUCCESS";
                }
            }
            MessageWindow.ShowMessage("新增顧客資料發生異常，請稍後重試。", MessageType.ERROR);
            return "FAILED";
        }

        public void GetHistories()
        {
            Histories = new CustomerHistories(ID);
            HistoryCollectionViewSource = new CollectionViewSource { Source = Histories };
            HistoryCollectionView = HistoryCollectionViewSource.View;
        }

        public void GetRecord()
        {
            Records = new CustomerRecords(ID);
            RecordCollectionViewSource = new CollectionViewSource { Source = Records };
            RecordCollectionView = RecordCollectionViewSource.View;
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
            return string.IsNullOrEmpty(IDNumber != null ? IDNumber.Trim() : IDNumber);
        }

        public bool CheckNameEmpty()
        {
            return string.IsNullOrEmpty(Name != null ? Name.Trim() : Name);
        }

        public bool CheckBirthdayNull()
        {
            return Birthday is null;
        }

        public bool CheckTelEmpty()
        {
            return string.IsNullOrEmpty(Tel != null ? Tel.Trim() : Tel);
        }

        public bool CheckCellPhoneEmpty()
        {
            return string.IsNullOrEmpty(CellPhone != null ? CellPhone.Trim() : CellPhone);
        }

        public bool IsAnonymous()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(IDNumber) || Birthday is null)
                return false;
            return Name.Equals("匿名") && IDNumber.Equals("A111111111");
        }

        public bool CheckCellFormat()
        {
            var cleared = Regex.Replace(CellPhone, "[^0-9]", "");
            return cleared.Length == 10;
        }

        /*public bool CheckTelFormat() {
            var cleared = Regex.Replace(Tel, "[^0-9]", "");
            return cleared.Length == 7 || cleared.Length == 9;
        }*/
    }
}