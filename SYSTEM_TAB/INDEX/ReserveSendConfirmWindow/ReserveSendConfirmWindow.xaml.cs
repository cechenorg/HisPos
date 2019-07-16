using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription.IndexReserve;
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
using His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.INDEX.ReserveSendConfirmWindow
{
    /// <summary>
    /// ReserveSendConfirmWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ReserveSendConfirmWindow : Window
    {
        public ReserveSendConfirmWindow(IndexReserves indexReserves)
        {
            InitializeComponent();
            ReserveSendConfirmViewModel reserveSendConfirmViewModel = new ReserveSendConfirmViewModel(indexReserves);
            DataContext = reserveSendConfirmViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseReserveSendConfirmWindow")
                    Close();
            });
            ShowDialog();
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is IndexReserveDetail)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((IndexReserveDetail)cell.DataContext).ID, "0" }, "ShowProductDetail"));
        }
    }
}
