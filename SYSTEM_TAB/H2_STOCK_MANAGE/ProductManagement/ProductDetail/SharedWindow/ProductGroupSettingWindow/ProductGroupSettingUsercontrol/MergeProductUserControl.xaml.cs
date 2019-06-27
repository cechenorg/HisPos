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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow.ProductGroupSettingUsercontrol {
    /// <summary>
    /// MergeProductUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class MergeProductUserControl : UserControl {
        public MergeProductUserControl() {
            InitializeComponent();
        }

        private void MergeProduct_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if(textBox is null) return;

            if (e.Key == Key.Enter)
            {
                (DataContext as ProductGroupSettingWindowViewModel).SearchMergeProductCommand.Execute(null);
            }
        }
    }
}
