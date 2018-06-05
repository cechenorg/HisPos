using His_Pos.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Location> locationCollection = new ObservableCollection<Location>();
        public LocationManageView()
        {
            InitializeComponent();
            InitLocation();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NewLocation();
        }
        public void NewLocation(double height = 0,double width = 0,double top = 0,double left = 0) {
            ContentControl c = new ContentControl();
            c.Template = (ControlTemplate)FindResource("DesignerItemTemplate");

            Grid g = new Grid();

            Rectangle r = new Rectangle();
            r.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#33009DF7"));
            r.IsHitTestVisible = false;

            g.Children.Add(r);

            c.Height = height == 0? 50 : height;
            c.Width = width == 0 ? 50 : width;
            c.Content = g;
            LocationCanvus.Children.Add(c);
            Canvas.SetTop(c,top == 0 ? 360 : top);
            Canvas.SetLeft(c, left == 0 ? 648 : left );
        }
        public void SaveLocation() {
            foreach (var location in LocationCanvus.Children) {

            }

        }
        public void InitLocation() {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetLocation(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        private void ShowLocationDetail(object sender, MouseButtonEventArgs e)
        {
            
        }

    }
}
