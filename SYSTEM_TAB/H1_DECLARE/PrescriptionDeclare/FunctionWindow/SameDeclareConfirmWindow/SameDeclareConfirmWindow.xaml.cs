using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription.SameDeclarePrescriptions;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.SameDeclareConfirmWindow
{
    /// <summary>
    /// SameDeclareConfirmWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SameDeclareConfirmWindow : Window
    {
        public SameDeclareConfirmWindow()
        {
            InitializeComponent();
        }

        public SameDeclareConfirmWindow(SameDeclarePrescriptions pres)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseSameDeclareConfirmWindow"))
                    Close();
            });
            this.Closing += (sender, e) =>
            {
                DialogResult = ((SameDeclareConfirmViewModel)DataContext).Result;
                Messenger.Default.Unregister(this);
            };
            DataContext = new SameDeclareConfirmViewModel(pres);
            ShowDialog();
        }
    }
}