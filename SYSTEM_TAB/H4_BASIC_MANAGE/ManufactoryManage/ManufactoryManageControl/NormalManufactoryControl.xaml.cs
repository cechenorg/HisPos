using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage.ManufactoryManageControl
{
    /// <summary>
    /// NormalManufactoryControl.xaml 的互動邏輯
    /// </summary>
    public partial class NormalManufactoryControl : UserControl
    {
        public NormalManufactoryControl()
        {
            InitializeComponent();
        }

        private void DataGrid_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = ((DependencyObject)sender).FindChild<ScrollViewer>("DG_ScrollViewer");
            scrollViewer.ScrollToHorizontalOffset(-e.Delta);
        }
    }
}