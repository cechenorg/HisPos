using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public enum MedicalPersonnelInitType
    {
        None = 0,
        Enable = 1,
        All = 2,
    }

    public class MedicalPersonnels:ObservableCollection<MedicalPersonnel>
    {
        public MedicalPersonnels(MedicalPersonnelInitType type)
        {
            switch (type)
            {
                case MedicalPersonnelInitType.All:
                    Init();
                    break;
                case MedicalPersonnelInitType.Enable:
                    foreach (var m in VM.CurrentPharmacy.MedicalPersonnels.Where(e => e.IsEnable))
                    {
                        Add(m);
                    }
                    break;
            }
        }

        private void Init()
        {
            var table = MedicalPersonnelDb.GetData();
            foreach (DataRow r in table.Rows)
            {
                Add(new MedicalPersonnel(r));
            }
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
