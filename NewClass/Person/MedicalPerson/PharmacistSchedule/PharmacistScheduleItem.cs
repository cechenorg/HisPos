using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class PharmacistScheduleItem:ObservableObject
    {
        public PharmacistScheduleItem() { }
        public PharmacistScheduleItem(DataRow r)
        {
            MedicalPersonnel = new DeclareMedicalPersonnel(r);
            Date = r.Field<DateTime>("SchTemp_Date");
        }

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
