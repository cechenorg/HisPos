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
using His_Pos.H4_BASIC_MANAGE.LocationManage;

namespace His_Pos.LocationManage
{
    /// <summary>
    /// LocationManageView.xaml 的互動邏輯
    /// </summary>
    public partial class LocationManageView : UserControl
    {
        public LocationManageView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ContentControl contentControl = new ContentControl();
            contentControl.Template = (ControlTemplate)FindResource("DesignerItemTemplate");

            LocationControl newLocation = new LocationControl();
            
            contentControl.Height = 50;
            contentControl.Width = 50;
            contentControl.Content = newLocation;

            LocationCanvus.Children.Add(contentControl);
            Canvas.SetTop(contentControl, 360);
            Canvas.SetLeft(contentControl, 648);
        }

        private void ShowLocationDetail(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
