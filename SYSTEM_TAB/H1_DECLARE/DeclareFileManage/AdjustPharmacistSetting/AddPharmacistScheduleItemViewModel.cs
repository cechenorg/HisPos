using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.MedicalPerson;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AddPharmacistScheduleItemViewModel:ViewModelBase
    {
        private Action<PharmacistScheduleItem> saveCallback;
        private DateTime selectedDate;
        private MedicalPersonnels medicalPersonnels;
        public MedicalPersonnels MedicalPersonnels
        {
            get => medicalPersonnels;
            set
            {
                Set(() => MedicalPersonnels, ref medicalPersonnels, value);
            }
        }
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
        public AddPharmacistScheduleItemViewModel(Action<PharmacistScheduleItem> saveCallback, DateTime selected)
        {
            MedicalPersonnels = AdjustPharmacistViewModel.MedicalPersonnels;
            selectedDate = selected;
            this.saveCallback = saveCallback;
            Save = new RelayCommand(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }
        private void SaveAction()
        {
            if (SelectedMedicalPersonnel is null)
            {
                MessageWindow.ShowMessage("請選擇新增藥師", MessageType.WARNING);
                return;
            }
            var item = new PharmacistScheduleItem { MedicalPersonnel = new DeclareMedicalPersonnel(SelectedMedicalPersonnel), Date = selectedDate ,RegisterTime = DateTime.Now};
            saveCallback(item);
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }
        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }
    }
}
