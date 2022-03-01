using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.MyPharmacyControl
{
    public class StartDateViewModel : ViewModelBase
    {
        #region Properties

        private DateTime? startDate;

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private Pharmacy myPharmacy;

        public Pharmacy MyPharmacy
        {
            get => myPharmacy;
            set
            {
                Set(() => MyPharmacy, ref myPharmacy, value);
            }
        }

        #endregion Properties

        #region Command

        public RelayCommand ConfirmCommand { get; set; }

        #endregion Command

        public StartDateViewModel(Pharmacy pharmacy)
        {
            MyPharmacy = pharmacy;
            ConfirmCommand = new RelayCommand(ConfirmAction);
        }

        private void ConfirmAction()
        {
            if (StartDate is null)
            {
                MessageWindow.ShowMessage("請填寫特約日期。", MessageType.ERROR);
                return;
            }
            Messenger.Default.Send(new NotificationMessage<DateTime>(this, (DateTime)StartDate, "PharmacyStartDateSet"));
            Messenger.Default.Send(new NotificationMessage("CloseStartDateViewModel"));
        }
    }
}