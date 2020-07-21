using System.Data;
using System.Windows;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// TradeAddProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class TradeAddProductWindow : Window
    {
        public DataRow SelectedProduct { get; set; }

        public TradeAddProductWindow(DataTable dt)
        {
            InitializeComponent();

            ResultGrid.ItemsSource = dt.DefaultView;


            /*Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });

            if (searchString.Equals(""))
                DataContext = new AddProductViewModel(AddProductEnum.PrescriptionDeclare, "0");
            else
                DataContext = new AddProductViewModel(searchString, AddProductEnum.PrescriptionDeclare, "0");

            SearchStringTextBox.Focus();

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);*/
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = (DataRowView)ResultGrid.SelectedItem;
            SelectedProduct = drv.Row;
            Close();
        }
    }
}
