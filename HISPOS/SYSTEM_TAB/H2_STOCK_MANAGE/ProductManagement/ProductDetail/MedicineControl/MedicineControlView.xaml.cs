using His_Pos.Class;
using His_Pos.FunctionWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    /// <summary>
    /// MedicineControlView.xaml 的互動邏輯
    /// </summary>
    public partial class MedicineControlView : UserControl
    {
        public MedicineControlView()
        {
            InitializeComponent();
        }

        private void IsCommon_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox is null) return;

            if ((bool)checkBox.IsChecked)
            {
                MessageWindow.ShowMessage("架上安全量: 架上量小於安全量時，會在常備採購時轉出採購\r\n\r\n基準量: 架上量小於安全量時，會採購至基準量\r\n\r\n包裝量: 常備採購時，會採購包裝量的倍數", MessageType.TIPS);
            }
        }

        private void TextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.Focus();
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }
    }
}