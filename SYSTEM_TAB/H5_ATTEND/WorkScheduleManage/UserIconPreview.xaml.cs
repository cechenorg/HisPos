using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// UserIconPreview.xaml 的互動邏輯
    /// </summary>
    public partial class UserIconPreview : UserControl, INotifyPropertyChanged
    {
        public string Id { get; }

        private bool isMed;

        public bool IsMed
        {
            get
            {
                return isMed;
            }
            set
            {
                isMed = value;
                NotifyPropertyChanged("IsMed");
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        //public UserIconPreview(UserIconData userIconData)
        //{
        //    InitializeComponent();
        //    DataContext = this;
        //    Id = userIconData.Id;
        //    IsSelected = false;
        //    UserName.Text = userIconData.Name;
        //    UserColor.Fill = userIconData.BackBrush;
        //    IsMed = userIconData.IsMed;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class IsSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Application.Current.FindResource("Shadow") as Brush;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}