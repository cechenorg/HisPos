using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person;
using His_Pos.NewClass.Person.Employee;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Treatment
{
    public class MedicalPersonnel : ObservableObject
    {
        public MedicalPersonnel(){}

        public MedicalPersonnel(Employee e)
        {
            Name = e.Name;
            IdNumber = e.IDNumber;
        }
        public string Name { get; set; }
        public string IdNumber { get; set; }
    }
}
