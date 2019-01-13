using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Person.Customer;

namespace His_Pos.NewClass.Prescription
{
    public class Prescriptions:Collection<Prescription>
    {
        public Prescriptions()
        {
        }

        public void GetCooperativePrescriptions()
        {
            Prescriptions prescriptions = PrescriptionDb.GetCooperaPrescriptionsData();
            //foreach (var p in prescriptions)
            //{
            //    Add(p);
            //}
            Prescription p1 = new Prescription();
            p1.Treatment.TreatDate = new DateTime(2019, 1, 1);
            p1.IsRead = false;
            p1.Patient.IDNumber = "A123456789";
            p1.Patient.Name = "AAA";
            p1.Treatment.Institution.Name = "翰群骨科診所";
            p1.Treatment.Division.Name = "骨科";
            Prescription p2 = new Prescription();
            p2.Treatment.TreatDate = new DateTime(2019, 1, 14);
            p2.IsRead = true;
            p2.Patient.IDNumber = "B123456789";
            p2.Patient.Name = "BBB";
            p2.Treatment.Institution.Name = "翰群骨科診所";
            p2.Treatment.Division.Name = "骨科";
            Prescription p3 = new Prescription();
            p3.Treatment.TreatDate = new DateTime(2019, 1, 14);
            p3.IsRead = false;
            p3.Patient.IDNumber = "C123456789";
            p3.Patient.Name = "CCC";
            p3.Treatment.Institution.Name = "翰群骨科診所";
            p3.Treatment.Division.Name = "骨科";
            Add(p1);
            Add(p2);
            Add(p3);
        }
    }
}
