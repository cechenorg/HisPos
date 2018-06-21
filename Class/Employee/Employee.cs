using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Employee
{
    public class Employee
    {
        public Employee(Employee employee) {
            Id = employee.Id;
            Qname = employee.Qname;
            Name = employee.Name;
            Gender = employee.Gender;
            IdNum = employee.IdNum;
            Position = employee.Position;
            Birth = employee.Birth;
            Address = employee.Address;
            Tel = employee.Tel;
            Email = employee.Email;
            StartDate = employee.StartDate;
        }
        public Employee(DataRow row)
        {
            Id = row["EMP_ID"].ToString();
            Qname = row["EMP_QNAME"].ToString();
            Name = row["EMP_NAME"].ToString();
            Gender = row["EMP_GENDER"].ToString();
            IdNum = row["EMP_IDNUM"].ToString();
            Position = row["EMP_POSITION"].ToString();
            Birth = row["EMP_BIRTH"].ToString();
            Address = row["EMP_ADDR"].ToString();
            Tel = row["EMP_TEL"].ToString();
            Email = row["EMP_EMAIL"].ToString();
            StartDate = row["EMP_STARTDATE"].ToString();
        }
        public string Id { get; set; }
        public string Qname { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string IdNum { get; set; }
        public string Position { get; set; }
        public string Birth { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }
    }
}
