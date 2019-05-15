using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public RelayCommand<object> Save { get; set; }
        public RelayCommand Cancel { get; set; }
        public AddPharmacistScheduleItemViewModel(Action<PharmacistScheduleItem> saveCallback, DateTime selected)
        {
            MedicalPersonnels = new MedicalPersonnels(MedicalPersonnelInitType.None);
            MainWindow.ServerConnection.OpenConnection();
            MedicalPersonnels.GetEnablePharmacist(selected);
            MainWindow.ServerConnection.CloseConnection();
            selectedDate = selected;
            this.saveCallback = saveCallback;
            Save = new RelayCommand<object>(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }
        private void SaveAction(object selectedItems)
        {
            var selectPharmacists = (selectedItems as ObservableCollection<object>).Cast<MedicalPersonnel>().ToList();
            if (selectPharmacists.Count == 0)
            {
                MessageWindow.ShowMessage("請選擇新增藥師", MessageType.WARNING);
                return;
            }

            foreach (var selected in selectPharmacists)
            {
                var item = new PharmacistScheduleItem { MedicalPersonnel = new DeclareMedicalPersonnel(selected), Date = selectedDate, RegisterTime = DateTime.Now };
                saveCallback(item);
            }
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }
        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseAddPharmacistScheduleItemWindow"));
        }
    }
}
