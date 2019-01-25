using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class MedicalPersonnel : ObservableObject
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee.Employee e)
        {
            Id = e.Id;
            Name = e.Name;
            IdNumber = e.IDNumber;
        }
        public MedicalPersonnel(DataRow r)
        {
            Id = r.Field<int>("Emp_ID");
            Name = r.Field<string>("Emp_Name"); 
             IdNumber = r.Field<string>("Emp_IDNumber");
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
    }
}
