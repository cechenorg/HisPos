using His_Pos.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;

namespace His_Pos.H4_BASIC_MANAGE.LocationManage
{
    /// <summary>
    /// ItemChangeWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ItemChangeWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

       public class LocationbData {
           public LocationbData(DataRow row) {
                proid = row["PRO_ID"].ToString();
                proname = row["PRO_NAME"].ToString();
                locdName = row["LOCD_NAME"].ToString();
            }
           public string proid { get; set; }
            public string proname { get; set; }
            public string locdName { get; set; }
        }
        private ObservableCollection<LocationbData> locationSourceDatas = new ObservableCollection<LocationbData>();
        public ObservableCollection<LocationbData> LocationSourceDatas
        {
            get
            {
                return locationSourceDatas;
            }
            set
            {
                locationSourceDatas = value;
                NotifyPropertyChanged("LocationSourceDatas");
            }
        }
        private ObservableCollection<LocationbData> locationTargetDatas = new ObservableCollection<LocationbData>();
        public ObservableCollection<LocationbData> LocationTargetDatas
        {
            get
            {
                return locationTargetDatas;
            }
            set
            {
                locationTargetDatas = value;
                NotifyPropertyChanged("LocationTargetDatas");
            }
        }
        private ObservableCollection<string> sourceBig = new ObservableCollection<string>();
        private ObservableCollection<string> sourceSmall = new ObservableCollection<string>();
        public ItemChangeWindow(string view)
        {
            InitializeComponent();
            DataContext = this;
            switch (view) {
                case "Location":
                    ComboBoxSourceBig.ItemsSource = LocationDb.ObservableGetLocationData();
                    ComboBoxSourceSmall.ItemsSource = LocationDb.ObservableGetLocationDetail();
                    ComboBoxTargetBig.ItemsSource = LocationDb.ObservableGetLocationData();
                    ComboBoxTargetSmall.ItemsSource = LocationDb.ObservableGetLocationDetail();
                    foreach (DataRow row in LocationDb.GetProductLocation().Rows)
                    {
                        LocationSourceDatas.Add(new LocationbData(row));
                        LocationTargetDatas.Add(new LocationbData(row));
                    }
                    DataGridTarget.Items.Filter = ProductLocationTargetFilter;
                    DataGridSource.Items.Filter = ProductLocationSourceFilter;
                    break;
            }
        }
   
        private void ComboBoxSourceBig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSourceBig.SelectedValue is null) return;
            ComboBoxSourceSmall.SelectedItem = null;
            ComboBoxSourceSmall.Items.Filter = LocationDetailSourceFilter;
            
        }

        private void ComboBoxSourceSmall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxSourceBig.SelectedValue is null)) 
                 ComboBoxSourceBig.Text = ComboBoxSourceSmall.SelectedValue.ToString().Split('-')[0];
            DataGridSource.Items.Filter = ProductLocationSourceFilter;
           
        }
        private void ComboBoxTargetBig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTargetBig.SelectedValue is null) return;
            ComboBoxTargetSmall.SelectedItem = null;
            ComboBoxTargetSmall.Items.Filter = LocationDetailTargetFilter;
        }

        private void ComboBoxTargetSmall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxTargetBig.SelectedValue is null))
                ComboBoxTargetBig.Text = ComboBoxTargetSmall.SelectedValue.ToString().Split('-')[0];
            DataGridTarget.Items.Filter = ProductLocationTargetFilter;
        }
        private void ButtonPlus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridSource.SelectedItem is null) {
                MessageWindow messageWindow = new MessageWindow("請選擇項目",MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            if (ComboBoxSourceSmall.SelectedItem is null) {
                MessageWindow messageWindow = new MessageWindow("請選擇來源", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            if (ComboBoxTargetSmall.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇目的", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            (DataGridSource.SelectedItem as LocationbData).locdName = ComboBoxTargetSmall.SelectedValue.ToString();
            LocationTargetDatas.Single(product => product.proname == (DataGridSource.SelectedItem as LocationbData).proname).locdName = ComboBoxTargetSmall.SelectedValue.ToString();
            DataGridTarget.Items.Filter = ProductLocationTargetFilter;
            DataGridSource.Items.Filter = ProductLocationSourceFilter;
        }

        private void ButtonBalance_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridTarget.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇項目", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            if (ComboBoxTargetSmall.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇來源", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
            if (ComboBoxSourceSmall.SelectedItem is null)
            {
                MessageWindow messageWindow = new MessageWindow("請選擇目的", MessageType.ERROR);
                messageWindow.ShowDialog();
                return;
            }
              (DataGridTarget.SelectedItem as LocationbData).locdName = ComboBoxSourceSmall.SelectedValue.ToString();
            LocationSourceDatas.Single(product => product.proname == (DataGridTarget.SelectedItem as LocationbData).proname).locdName = ComboBoxSourceSmall.SelectedValue.ToString();
            DataGridTarget.Items.Filter = ProductLocationTargetFilter;
            DataGridSource.Items.Filter = ProductLocationSourceFilter;
            DataGridTarget.SelectedIndex = 0;
            DataGridSource.SelectedIndex = 0;
        }
        public bool ProductLocationSourceFilter(object item)
        {
            if (ComboBoxSourceSmall.SelectedItem is null) return false;
            if (((LocationbData)item).locdName.Equals(ComboBoxSourceSmall.SelectedValue.ToString()))
                return true;
            else
                return false;
        }
        public bool ProductLocationTargetFilter(object item)
        {
            if (ComboBoxTargetSmall.SelectedItem is null) return false;
            if (((LocationbData)item).locdName.Equals(ComboBoxTargetSmall.SelectedValue.ToString()))
                return true;
            else
                return false;
        }
        public bool LocationDetailSourceFilter(object item)
        {
            if (item.ToString().Split('-')[0].Equals(ComboBoxSourceBig.SelectedValue.ToString()))
                return true;
            else
                return false;
        }
        public bool LocationDetailTargetFilter(object item)
        {
            if (item.ToString().Split('-')[0].Equals(ComboBoxTargetBig.SelectedValue.ToString()))
                return true;
            else
                return false;
        }

    }
}
