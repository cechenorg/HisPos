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
            ContentControl c = new ContentControl();
            c.Template = (ControlTemplate)FindResource("DesignerItemTemplate");

            Grid g = new Grid();

            Rectangle r = new Rectangle();
            r.Fill = Brushes.DarkRed;
            r.IsHitTestVisible = false;


            g.Children.Add(r);

            c.Height = 50;
            c.Width = 50;
            c.Content = g;

            LocationCanvus.Children.Add(c);
            Canvas.SetTop(c, 200);
            Canvas.SetLeft(c, 75);
        }
    }
}
