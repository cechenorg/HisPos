using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// UserIcon.xaml 的互動邏輯
    /// </summary>
    public partial class UserIcon : UserControl, INotifyPropertyChanged
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

        private bool show;
        public bool Show
        {
            get
            {
                return show;
            }
            set
            {
                show = value;
                NotifyPropertyChanged("Show");
            }
        }

        public UserIcon(UserIconData userIconData)
        {
            InitializeComponent();
            DataContext = this;
            Id = userIconData.Id;
            IsMed = userIconData.IsMed;
            Show = true;
            UserName.Text = userIconData.Name.Substring(0,1);
            Back.Background = userIconData.BackBrush;
        }

        public void HideIcon()
        {
            Show = false;
        }

        public void ShowIcon()
        {
            Show = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class IsMedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 1;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BorderShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 25;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class MarginShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new Thickness(0,0,2,0);
            }

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
