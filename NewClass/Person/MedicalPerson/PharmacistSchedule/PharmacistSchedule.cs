using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class PharmacistSchedule : ObservableCollection<PharmacistScheduleItem>
    {
        public PharmacistSchedule()
        {
        }

        public void GetPharmacistScheduleWithCount(DateTime start, DateTime end)
        {
            Clear();
            var table = PharmacistScheduleDb.GetEmployeeScheduleWithCount(start, end);
            foreach (DataRow r in table.Rows)
            {
                Add(new PharmacistScheduleItem(r));
            }
        }

        public void GetPharmacistSchedule(DateTime start, DateTime end)
        {
            Clear();
            var table = PharmacistScheduleDb.GetEmployeeSchedule(start, end);
            foreach (DataRow r in table.Rows)
            {
                Add(new PharmacistScheduleItem(r));
            }
        }

        public void SaveSchedule(DateTime start, DateTime end)
        {
            PharmacistScheduleDb.InsertSchedule(start, end, this);
        }
    }
}