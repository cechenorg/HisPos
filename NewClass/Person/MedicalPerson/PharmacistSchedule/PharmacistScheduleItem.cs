using System;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class PharmacistScheduleItem:ObservableObject
    {
        private DeclareMedicalPersonnel medicalPersonnel;
        public DeclareMedicalPersonnel MedicalPersonnel
        {
            get => medicalPersonnel;
            set
            {
                Set(() => MedicalPersonnel, ref medicalPersonnel, value);
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set
            {
                Set(() => Date, ref date, value);
            }
        }
    }
}
