using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule;
using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    /// <summary>
    /// AddPharmacistScheduleItemWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddPharmacistScheduleItemWindow : Window
    {
        private AddPharmacistScheduleItemViewModel addPharmacistScheduleItemViewModel;

        public AddPharmacistScheduleItemWindow(Action<PharmacistScheduleItem> saveCallback, DateTime selected)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddPharmacistScheduleItemWindow"))
                    Close();
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            addPharmacistScheduleItemViewModel = new AddPharmacistScheduleItemViewModel(saveCallback, selected);
            DataContext = addPharmacistScheduleItemViewModel;
        }
    }
}