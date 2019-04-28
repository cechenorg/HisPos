﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.MedicalPerson;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AppointmentViewModel : ViewModelBase
    {
        private Action<Appointment> saveCallback;
        private DateTime selectedDate;
        public MedicalPersonnels MedicalPersonnels { get; set; }
        private MedicalPersonnel selectedMedicalPersonnel;
        public MedicalPersonnel SelectedMedicalPersonnel
        {
            get => selectedMedicalPersonnel;
            set
            {
                Set(() => SelectedMedicalPersonnel, ref selectedMedicalPersonnel, value);
            }
        }
        public RelayCommand Save { get; set; }
        public RelayCommand Cancel { get; set; }
        public AppointmentViewModel(Action<Appointment> saveCallback, DateTime selected)
        {
            MedicalPersonnels = AdjustPharmacistViewModel.MedicalPersonnels;
            selectedDate = selected;
            this.saveCallback = saveCallback;
            Save = new RelayCommand(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }
        private void SaveAction()
        {
            var appointment = new Appointment {MedicalPersonnel = new DeclareMedicalPersonnel(SelectedMedicalPersonnel), Date = selectedDate};
            saveCallback(appointment);
        }
        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAppointmentWindow"));
        }

    }
}
