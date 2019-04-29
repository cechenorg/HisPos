using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.MedicalPerson;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class Appointment:ObservableObject
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
