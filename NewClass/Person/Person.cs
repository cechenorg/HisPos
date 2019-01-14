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
            Id = (int)r["Person_Id"];
            Name = r["Person_Name"]?.ToString();
            IDNumber = r["Person_IDNumber"]?.ToString();
            Gender = r["Person_Gender"]?.ToString();
            Birthday = (DateTime?)r["Person_BirthDay"];
            Tel = r["Person_Telephone"]?.ToString();
            CellPhone = r["Person_Cellphone"]?.ToString();
            Address = r["Person_Address"]?.ToString();
            Email = r["Person_Email"]?.ToString();
            Line = r["Person_LINE"]?.ToString();
            Note = r["Person_Note"]?.ToString();
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
