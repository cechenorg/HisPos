using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Person
{
    public class Person:ObservableObject
    {
        public Person(){}

        public Person(DataRow r)
        {
            Id = r.Field<int>("Person_Id");
            Name = r.Field<string>("Person_Name");
            IDNumber = r.Field<string>("Person_IDNumber");
            Gender = r["Person_Gender"]?.ToString();
            Birthday = r.Field<DateTime?>("Person_BirthDay");
            Tel = r.Field<string>("Person_Telephone");
            CellPhone = r.Field<string>("Person_Cellphone");
            Address = r.Field<string>("Person_Address");
            Email = r.Field<string>("Person_Email");
            Line = r.Field<string>("Person_LINE");
            Note = r.Field<string>("Person_Note");
        }
        public int Id { get; set; }
        public string Name { get; set; }//姓名
        public string Gender { get; set; }//性別
        public string IDNumber { get; set; }//身分證字號
        public DateTime? Birthday { get; set; }//生日
        public string Tel { get; set; }//家電
        public string CellPhone { get; set; }//手機
        public string Address { get; set; }//地址
        public string Email { get; set; }//信箱
        public string Line { get; set; }
        public string Note { get; set; }//備註
    }
}
