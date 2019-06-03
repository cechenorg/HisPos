using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public class MedicalPersonnels:ObservableCollection<MedicalPersonnel>
    {
        public MedicalPersonnels(bool init)
        {
            if(init)
                Init();
        }

        private void Init()
        {
            var table = MedicalPersonnelDb.GetData();
            foreach (DataRow r in table.Rows)
            {
                var pharmacist = new MedicalPersonnel(r);
                if(pharmacist.IsEnable && pharmacist.IsLocal)
                    Add(pharmacist);
            }
            if (!ViewModelMainWindow.CurrentUser.WorkPosition.WorkPositionName.Equals("藥師") ||
                Items.Count(m => m.IDNumber.Equals(ViewModelMainWindow.CurrentUser.IDNumber)) != 0) return;
            var medicalPerson = new MedicalPersonnel(ViewModelMainWindow.CurrentUser);
            Add(medicalPerson);
        }

        public void GetEnablePharmacist(DateTime selectedDate)
        {
            var table = MedicalPersonnelDb.GetEnableMedicalPersonnels(selectedDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicalPersonnel(r));
            }
        }
    }
}
