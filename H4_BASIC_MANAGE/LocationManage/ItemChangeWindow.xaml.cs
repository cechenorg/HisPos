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
        private ObservableCollection<LocationbData> locationbDatas = new ObservableCollection<LocationbData>();
        public ObservableCollection<LocationbData> LocationbDatas
        {
            get
            {
                return locationbDatas;
            }
            set
            {
                locationbDatas = value;
                NotifyPropertyChanged("LocationbDatas");
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
                    foreach (DataRow row in LocationDb.GetProductLocation().Rows)
                    {
                        LocationbDatas.Add(new LocationbData(row));
                    }
                    break;
            }
        }
        public bool ProductLocationFilter(object item)
        {
            if (((LocationbData)item).locdName.Equals(ComboBoxSourceSmall.SelectedValue.ToString()))
                return true;
            else
                return false;
        }
        public bool LocationDetailFilter(object item)
        {
            if (item.ToString().Split('-')[0].Equals(ComboBoxSourceBig.SelectedValue.ToString()))
                return true;
            else
                return false;
        }
        private void ComboBoxSourceBig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSourceBig.SelectedValue is null) return;
            ComboBoxSourceSmall.Items.Filter = LocationDetailFilter;
            
        }

        private void ComboBoxSourceSmall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSourceSmall.SelectedItem is null) return;
            if ((ComboBoxSourceBig.SelectedValue is null)) 
            ComboBoxSourceBig.Text = ComboBoxSourceSmall.SelectedValue.ToString().Split('-')[0];
            DataGridSource.Items.Filter = ProductLocationFilter;
        }
    }
}
