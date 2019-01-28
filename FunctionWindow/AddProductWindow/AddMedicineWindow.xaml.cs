using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// AddMedicineWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddMedicineWindow : Window
    {
        public AddMedicineWindow(string search)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });
            DataContext = new AddProductViewModel(search, AddProductEnum.AddMedicine);
            SearchStringTextBox.Focus();
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
