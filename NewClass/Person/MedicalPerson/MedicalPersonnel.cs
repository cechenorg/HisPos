using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class MedicalPersonnel : ObservableObject
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee.Employee e)
        {
            Name = e.Name;
            IdNumber = e.IDNumber;
        }
        public MedicalPersonnel(DataRow r)
        {

        }
        public string Name { get; set; }
        public string IdNumber { get; set; }
    }
}
