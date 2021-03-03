using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow.ProductGroupSettingUsercontrol
{
    /// <summary>
    /// MergeProductUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class MergeProductUserControl : UserControl
    {
        public MergeProductUserControl()
        {
            InitializeComponent();
        }

        private void MergeProduct_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                (DataContext as ProductGroupSettingWindowViewModel).SearchMergeProductCommand.Execute(null);
            }
        }
    }
}