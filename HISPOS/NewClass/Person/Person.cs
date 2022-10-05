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
                if (string.IsNullOrEmpty(value) == false && value.Length==10)
                    Gender = value.Substring(1, 1) == "1" ? "男" : "女";
                if (string.IsNullOrEmpty(Gender) == true)
                    Gender = "";
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
                case '9':
                case 'B':
                case 'D':
                    Gender = Properties.Resources.Female;
                    break;

                case '1':
                case '8':
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