using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    [ZeroFormattable]
    public class MedicalPersonnel : ObservableObject
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee.Employee e)
        {
            ID = e.ID;
            Name = e.Name;
            IdNumber = e.IDNumber;
        }
        public MedicalPersonnel(DataRow r)
        {
            ID = r.Field<int>("Emp_ID");
            Name = r.Field<string>("Emp_Name"); 
            IdNumber = r.Field<string>("Emp_IDNumber");
            IsEnable = r.Field<bool>("Emp_IsEnable");
        }
        [Index(0)]
        public virtual int ID { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }
        [Index(2)]
        public virtual string IdNumber { get; set; }
        [Index(3)]
        public virtual bool IsEnable { get; set; }
    }
}
