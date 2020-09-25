using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class.Location;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.ProductLocation;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// LocationManageView.xaml 的互動邏輯
    /// </summary>
    public partial class LocationManageView : UserControl
    {
        public LocationManageView()
        {
            InitializeComponent();
            InitLocation();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            AddLocationWindow addTypeWindow = new AddLocationWindow();
            addTypeWindow.ShowDialog();
            InitLocationLoad();

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditLocationWindow editTypeWindow = new EditLocationWindow((int)ProductLocationDataGrid.SelectedValue);
            editTypeWindow.ShowDialog();
            InitLocationLoad();
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteLocationWindow deleteTypeWindow = new DeleteLocationWindow((int)ProductLocationDataGrid.SelectedValue);
            deleteTypeWindow.ShowDialog();
            InitLocationLoad();
        }
        private void InitLocation()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.GetProductLocationMasters();
            MainWindow.ServerConnection.CloseConnection();
            
            ProductLocationDataGrid.ItemsSource = dataTable.DefaultView;
        }
        private void InitLocationLoad()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductLocationDB.GetProductLocationMasters();
            MainWindow.ServerConnection.CloseConnection();

            ProductLocationDataGrid.ItemsSource = dataTable.DefaultView;
            ProductLocationDataGrid.SelectedIndex = 0;
        }
        private void InitLocationDetail()
        {

            if (ProductLocationDataGrid.SelectedValue == null)
            {
                return;
            }
            else {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = ProductLocationDB.GetProductLocationDetails((int)ProductLocationDataGrid.SelectedValue);
                MainWindow.ServerConnection.CloseConnection();

                ProductLocationDetailDataGrid.ItemsSource = dataTable.DefaultView;
            }
           

        }

        private void ProductLocationDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
            InitLocationDetail();
            InsertButton.Visibility = Visibility.Visible;


        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            InsertLocationDetailWindow insertTypeWindow = new InsertLocationDetailWindow((int)ProductLocationDataGrid.SelectedValue);
            insertTypeWindow.ShowDialog();
            InitLocationDetail();
        }


        /*public static LocationManageView Instance;
        public LocationControl selectItem;
        public ObservableCollection<Location> locationCollection = new ObservableCollection<Location>();
        public static int id = 0;
        public LocationManageView()
        {
            InitializeComponent();
            //InitLocation();
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
            //LocationCanvus.Children.Add(contentControl);
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
            ///LocationDb.SaveLocationData(locationCollection);
        }
        //public void InitLocation()
        //{
        //    LoadingWindow loadingWindow = new LoadingWindow();
        //    loadingWindow.GetLocation(this);
        //    loadingWindow.Topmost = true;
        //    loadingWindow.Show();
        //}
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
           ///if (LocationDb.CheckProductExist(selectItem.id.ToString()))
           ///{
           ///    LocationCanvus.Children.Remove(deleteControl);
           ///    LocationDb.DeleteLocation(selectItem.id.ToString());
           ///}
           ///else {
           ///    MessageWindow.ShowMessage("此櫃位尚有商品，無法刪除",MessageType.ERROR, true);
           ///    
           ///}
           
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

        private void ButtonChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            ItemChangeWindow itemChangeWindow = new ItemChangeWindow("Location");
            itemChangeWindow.ShowDialog();
        }*/
    }
}
