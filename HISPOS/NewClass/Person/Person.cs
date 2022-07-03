using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Text.RegularExpressions;
using ZeroFormatter;

namespace His_Pos.NewClass.Person
{
    [ZeroFormattable]
    public class Person : ObservableObject
    {
        public Person()
        {
        }

        public Person(DataRow r)
        {
            ID = r.Field<int>("Person_Id");
            Name = r.Field<string>("Person_Name");
            IDNumber = r.Field<string>("Person_IDNumber");
            Gender = r.Field<string>("Person_Gender");
            if (string.IsNullOrEmpty(Gender))
                Gender = CheckGender();
            Birthday = r.Field<DateTime?>("Person_BirthDay");
            Tel = r.Field<string>("Person_Telephone");
            CellPhone = r.Field<string>("Person_Cellphone");
            Address = r.Field<string>("Person_Address");
            Email = r.Field<string>("Person_Email");
            Line = r.Field<string>("Person_LINE");
            Note = r.Field<string>("Person_Note");
            SecondPhone = r.Field<string>("Person_SecondPhone");
        }

        [Index(0)]
        public virtual int ID { get; set; }

        private string name;

        [Index(1)]
        public virtual string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }//姓名

        [Index(2)]
        public virtual string Gender { get; set; }//性別

        private string idNumber;

        [Index(3)]
        public virtual string IDNumber
        {
            get => idNumber;
            set
            {
                Set(() => IDNumber, ref idNumber, value);
                Gender = value.Substring(1, 1) == "1" ? "男" : "女";
            }
        }//身分證字號

        private DateTime? birthday;

        [Index(4)]
        public virtual DateTime? Birthday
        {
            get => birthday;
            set
            {
                Set(() => Birthday, ref birthday, value);
            }
        }//生日

        private string tel;

        [Index(5)]
        public virtual string Tel
        {
            get => tel;
            set
            {
                Set(() => Tel, ref tel, value);
            }
        }//家電

        private string cellPhone;

        [Index(6)]
        public virtual string CellPhone
        {
            get => cellPhone;
            set
            {
                Set(() => CellPhone, ref cellPhone, value);
            }
        }//手機

        private string secondPhone;

        [IgnoreFormat]
        public virtual string SecondPhone
        {
            get => secondPhone;
            set
            {
                Set(() => SecondPhone, ref secondPhone, value);
            }
        }//手機2

        [IgnoreFormat]
        public virtual string FormattedPhoneNumber
        {
            get
            {
                if (CellPhone == null)
                    return string.Empty;

                switch (CellPhone.Length)
                {
                    case 10:
                        return Regex.Replace(CellPhone, @"(\d{4})(\d{3})(\d{3})", "$1-$2-$3");

                    default:
                        return CellPhone;
                }
            }
        }

        [IgnoreFormat]
        public string Address { get; set; }//地址

        [IgnoreFormat]
        public string Email { get; set; }//信箱

        [IgnoreFormat]
        public string Line { get; set; }

        [IgnoreFormat]
        public string Note { get; set; }//備註

        public string CheckGender()
        {
            if (string.IsNullOrEmpty(IDNumber) || IDNumber.Length != 10) return Properties.Resources.Male;
            switch (IDNumber[1])
            {
                case '2':
                case 'B':
                case 'D':
                    Gender = Properties.Resources.Female;
                    break;

                case '1':
                case 'A':
                case 'C':
                    Gender = Properties.Resources.Male;
                    break;

                default:
                    Gender = Properties.Resources.Male;
                    break;
            }
            return Gender;
        }
    }
}