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
        public static LocationManageView Instance;
        public LocationControl selectItem;
        public ObservableCollection<Location> locationCollection = new ObservableCollection<Location>();
        public static int id = 0;
        public LocationManageView()
        {
            InitializeComponent();
            InitLocation();
            Instance = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NewLocationView newLocationView = new NewLocationView();
            newLocationView.ShowDialog();
        }
        public void NewLocation(string locid = null,string locname = null,double height = 0,double width = 0,double top = 0,double left = 0) {
        
            ContentControl contentControl = new ContentControl();
            contentControl.Template = (ControlTemplate)FindResource("DesignerItemTemplate");
            LocationControl newLocation = null;
            if (locid != null)
            {
                newLocation = new LocationControl(Convert.ToInt32(locid));
                newLocation.Name = locname;
                newLocation.locationName.Content = locname;
                id = Convert.ToInt32(locid);
                id++;
            }
            else {
                newLocation = new LocationControl(id);
                newLocation.Name = locname;
                newLocation.locationName.Content = locname;
                id++;
            }
            contentControl.Height = (height == 0) ? 50 : height;
            contentControl.Width = (width == 0) ? 50 : width;
            contentControl.Content = newLocation;
            LocationCanvus.Children.Add(contentControl);
            Canvas.SetTop(contentControl, top == 0 ? 360 : top);
            Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            SaveLocation();
        }
        public void SaveLocation() {
            locationCollection.Clear();
            foreach (ContentControl contentControl in LocationCanvus.Children) {
                LocationControl locationControl = (LocationControl)contentControl.Content;
                locationCollection.Add(new Location(locationControl.id, locationControl.Name, Canvas.GetLeft(contentControl), Canvas.GetTop(contentControl), contentControl.Width, contentControl.Height));
            }
            LocationDb.SaveLocationData(locationCollection);
        }
        public void InitLocation()
        {
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetLocation(this);
            loadingWindow.Topmost = true;
            loadingWindow.Show();
        }
        private void ShowLocationDetail(object sender, MouseButtonEventArgs e)
        {
            SaveLocation();
            var control = (MoveThumb)sender;
            foreach (ContentControl item in LocationCanvus.Children) {
                if ((LocationControl)item.Content == selectItem)
                {
                    LocationControl locationControl = (LocationControl)item.Content;
                    selectItem = locationControl;
                    LocationDetailWindow locationDetailWindow = new LocationDetailWindow(new Location(locationControl.id, locationControl.Name, Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height));
                   locationDetailWindow.Show();
                    locationDetailWindow.Focus();
                    return;
                }
            }
        }
       
        private void MoveThumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ContentControl contentcontrol in LocationCanvus.Children) {
                var child = VisualTreeHelper.GetChild(contentcontrol,0);
                var thumb = VisualTreeHelper.GetChild(child, 1);
                ((Control)thumb).Visibility = Visibility.Collapsed;
            }
            (((Grid)(sender as MoveThumb).Parent).Children.OfType<Control>().ToList())[1].Visibility = Visibility.Visible;
           
            var grid = VisualTreeHelper.GetChild(((Grid)(sender as MoveThumb).Parent),2);
            var locationcontrol = VisualTreeHelper.GetChild(grid,0);
            selectItem = (LocationControl)locationcontrol;
        }

        private void ButtonDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            ContentControl deleteControl = null;
           foreach (ContentControl contentcontrol in LocationCanvus.Children){
                if ((LocationControl)contentcontrol.Content == selectItem)
                     deleteControl = contentcontrol;
            }
            if (LocationDb.CheckProductExist(selectItem.id.ToString()))
            {
                LocationCanvus.Children.Remove(deleteControl);
                LocationDb.DeleteLocation(selectItem.id.ToString());
            }
            else {
                MessageWindow messageWindow = new MessageWindow("此櫃位尚有商品，無法刪除",MessageType.ERROR);
                messageWindow.ShowDialog();
            }
           
        }

        private void ButtonCopyLocation_Click(object sender, RoutedEventArgs e)
        {
            ContentControl copyControl = null;
            foreach (ContentControl contentcontrol in LocationCanvus.Children)
            {
                if ((LocationControl)contentcontrol.Content == selectItem)
                    copyControl = contentcontrol;
            }
            NewLocation(null, selectItem.Name + "new" + id, copyControl.Height, copyControl.Width, Canvas.GetTop(copyControl)+30, Canvas.GetLeft(copyControl)+30);
        }
    }
}
