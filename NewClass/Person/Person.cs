using System;
using System.Collections.Generic;
using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Person
{
    [ZeroFormattable]
    public class Person:ObservableObject
    {
        public Person(){}

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
        }
        [Index(0)]
        public virtual int ID { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }//姓名
        [Index(2)]
        public virtual string Gender { get; set; }//性別
        [Index(3)]
        public virtual string IDNumber { get; set; }//身分證字號
        [IgnoreFormat]
        public DateTime? Birthday { get; set; }//生日
        [IgnoreFormat]
        public string Tel { get; set; }//家電
        [IgnoreFormat]
        public string CellPhone { get; set; }//手機
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
