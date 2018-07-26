using His_Pos.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace His_Pos.H4_BASIC_MANAGE.MedFrequencyManage
{
    /// <summary>
    /// MedFrequencyManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedFrequencyManageView : UserControl, INotifyPropertyChanged
    {
        private bool isFirst = true;
        private Usage usageDetail;
        public Usage UsageDetail
        {
            get
            {
                return usageDetail;
            }
            set
            {
                usageDetail = value;
                NotifyPropertyChanged("UsageDetail");
            }
        }
        private ObservableCollection<Usage> usageCollection;

        public ObservableCollection<Usage> UsageCollection {
            get
            {
                return usageCollection;
            }
            set
            {
                usageCollection = value;
                NotifyPropertyChanged("UsageCollection");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public MedFrequencyManageView()
        {
            InitializeComponent();
            UsageCollection = UsageDb.GetUsages();
            DataContext = this;
            DataGridMedFrequency.SelectedIndex = 0;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void TextBox_DataChanged(object sender, EventArgs e)
        {
            DataChanged();
        }

        private void DataChanged()
        {
            if (isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonCancel.IsEnabled = true;
            ButtonSubmit.IsEnabled = true;
        }
        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonCancel.IsEnabled = false;
            ButtonSubmit.IsEnabled = false;
        }
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            isFirst = false;
        }

        private void DataGridMedFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null) return;
            UsageDetail = (Usage)((Usage)(sender as DataGrid).SelectedItem).Clone();
            InitDataChanged();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            UsageDetail = (Usage)UsageCollection[DataGridMedFrequency.SelectedIndex].Clone();
            InitDataChanged();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            UsageDb.SaveUsage(UsageDetail);
            UsageCollection[DataGridMedFrequency.SelectedIndex] = UsageDetail;
            DataGridMedFrequency.SelectedItem = UsageDetail;
            InitDataChanged();
        }

        private void CheckBox_DataChanged(object sender, RoutedEventArgs e)
        {
            DataChanged();
        }
    }
}
