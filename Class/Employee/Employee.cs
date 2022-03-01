using System;
using System.Data;

namespace His_Pos.Class.Employee
{
    public class Employee : ICloneable
    {
        public Employee()
        {
            Id = "";
            Qname = "";
            Name = "新人";
            NickName = "新人";
            Gender = "";
            IdNum = "";
            Department = "";
            Position = "店員";
            Birth = DateTime.Now.ToString();
            Address = "";
            Tel = "";
            Email = "";
            StartDate = DateTime.Now.ToString();
            UrgentContactPerson = "";
            UrgentContactPhone = "";
            PurchaseLimit = "3000";
            Description = "";
        }

        private Employee(Employee employee)
        {
            Id = employee.Id;
            Qname = employee.Qname;
            Name = employee.Name;
            NickName = employee.NickName;
            Gender = employee.Gender;
            IdNum = employee.IdNum;
            Department = employee.Department;
            Position = employee.Position;
            Birth = employee.Birth;
            Address = employee.Address;
            Tel = employee.Tel;
            Email = employee.Email;
            StartDate = employee.StartDate;
            UrgentContactPerson = employee.UrgentContactPerson;
            UrgentContactPhone = employee.UrgentContactPhone;
            PurchaseLimit = employee.PurchaseLimit;
            Description = employee.Description;
        }

        public Employee(DataRow row)
        {
            Id = row["EMP_ID"].ToString();
            Qname = row["EMP_QNAME"].ToString();
            Name = row["EMP_NAME"].ToString();
            NickName = row["EMP_NICKNAME"].ToString();
            Gender = row["EMP_GENDER"].ToString() == "True" ? "男" : "女";
            IdNum = row["EMP_IDNUM"].ToString();
            Department = row["EMP_DEPARTMENT"].ToString();
            Position = row["EMP_POSITION"].ToString();
            Birth = row["EMP_BIRTH"].ToString();
            Address = row["EMP_ADDR"].ToString();
            Tel = row["EMP_TEL"].ToString();
            Email = row["EMP_EMAIL"].ToString();
            StartDate = row["EMP_STARTDATE"].ToString();
            UrgentContactPerson = row["EMP_URGENTPERSON"].ToString();
            UrgentContactPhone = row["EMP_URGENTPHONE"].ToString();
            PurchaseLimit = row["EMP_PURCHASELIMIT"].ToString();
            Description = row["EMP_DESCRIPTION"].ToString();
        }

        public string Id { get; set; }
        public string Qname { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string IdNum { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Birth { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string UrgentContactPerson { get; set; }
        public string UrgentContactPhone { get; set; }
        public string PurchaseLimit { get; set; }
        public string StartDate { get; set; }
        public string Description { get; set; }

        public object Clone()
        {
            return new Employee(this);
        }
    }
}