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

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
    }
}
