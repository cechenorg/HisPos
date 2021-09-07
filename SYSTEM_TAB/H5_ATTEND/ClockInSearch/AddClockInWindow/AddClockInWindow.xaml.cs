using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.ClockIn.AddClockInWindow
{
    /// <summary>
    /// AddClockInWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddClockInWindow : Window
    {
        public AddClockInWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                string tt = notificationMessage.Notification;
                //string[] aa[] = tt.Split(",");

                if (tt.Contains("CloseAddClockInWindow") && notificationMessage.Sender is AddClockInWindowViewModel)
                {
                    tt = tt.Replace("CloseAddClockInWindow", "");
                    Messenger.Default.Send(new NotificationMessage(this, tt));
                    Close();
                }
            });

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}