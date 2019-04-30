using System;
using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    [ZeroFormattable]
    public class MedicalPersonnel : Employee.Employee
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee.Employee e)
        {
            ID = e.ID;
            Name = e.Name;
            IDNumber = e.IDNumber;
        }
        public MedicalPersonnel(DataRow r)
        {
            ID = r.Field<int>("Emp_ID");
            Name = r.Field<string>("Emp_Name");
            IDNumber = r.Field<string>("Emp_IDNumber");
            IsEnable = r.Field<bool>("Emp_IsEnable");
            //StartDate = r.Field<DateTime>("Emp_StartDate");
            //LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
        }
        [Index(12)]
        public virtual bool IsEnable { get; set; }
        [Index(13)]
        public virtual DateTime StartDate { get; set; }
        [Index(14)]
        public virtual DateTime? LeaveDate { get; set; }
    }
}
