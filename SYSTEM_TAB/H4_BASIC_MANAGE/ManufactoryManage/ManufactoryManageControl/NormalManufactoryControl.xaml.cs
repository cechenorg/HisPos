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
using MahApps.Metro.Controls;

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
