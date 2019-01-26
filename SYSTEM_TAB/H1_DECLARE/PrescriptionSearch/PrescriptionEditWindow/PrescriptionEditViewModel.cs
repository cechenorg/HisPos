using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Interface;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionSearch.PrescriptionEditWindow
{
    public class PrescriptionEditViewModel:ViewModelBase
    {
        private Prescription originalPrescription { get; }
        private Prescription editedPrescription;
        public Prescription EditedPrescription
        {
            get => editedPrescription;
            set
            {
                Set(() => EditedPrescription, ref editedPrescription, value);
            }
        }

        public PrescriptionEditViewModel(Prescription selected)
        {
            originalPrescription = selected;
            EditedPrescription = selected;
        }
    }
}
