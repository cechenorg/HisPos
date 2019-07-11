using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.PrescriptionRefactoring;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindowRefactoring
{
    public class PrescriptionEditViewModel:ViewModelBase
    {
        private Prescription editedPrescription;
        public Prescription EditedPrescription
        {
            get => editedPrescription;
            set
            {
                Set(() => EditedPrescription, ref editedPrescription, value);
            }
        }

        public RelayCommand<string> ShowMedicineDetail { get; set; }

        public PrescriptionEditViewModel()
        {

        }
        public PrescriptionEditViewModel(Prescription p)
        {

        }
    }
}
