using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow
{
    /// <summary>
    /// CommonHospitalsWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CommonHospitalsWindow : Window
    {
        public CommonHospitalsWindow(ViewModelEnum vm)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCommonHospitalsWindow"))
                    Close();
            });
            this.DataContext = new CommonHospitalsViewModel(vm);
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
