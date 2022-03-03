using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H3_STOCKTAKING.StockTakingPlan.AddNewPlanWindow
{
    /// <summary>
    /// AddNewPlanWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewPlanWindow : Window
    {
        public AddNewPlanWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseAddNewPlanWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}