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

namespace His_Pos.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    /// <summary>
    /// ManReportControl.xaml 的互動邏輯
    /// </summary>
    public partial class ManReportControl : UserControl, INotifyPropertyChanged
    {
        #region ----- Define Enum -----
        public enum ControlStatus
        {
            CloseAll = 1,
            OpenOrder = 2,
            OpenAll = 3
        }
        #endregion

        #region ----- Define Variables -----

        public List<PurchaseReturnReportView.PurchaseReturnRecord> PurchaseReturnRecordList { get; }

        public string ManufactoryName { get; private set; }

        public ScrollViewer OutsideScrollViewer { get; }

        public string Total { get; private set; }

        private ControlStatus controlStatus = ControlStatus.CloseAll;
        private ControlStatus ReportControlStatus {
            get { return controlStatus; }
            set
            {
                controlStatus = value;
                NotifyPropertyChanged("IsOpenAll");
            }
        }

        public bool IsOpenAll
        {
            get { return controlStatus == ControlStatus.OpenAll; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public ManReportControl(List<PurchaseReturnReportView.PurchaseReturnRecord> list, ScrollViewer scrollViewer)
        {
            InitializeComponent();
            DataContext = this;

            PurchaseReturnRecordList = list;
            OutsideScrollViewer = scrollViewer;

            InitData();
        }

        #region ----- Init Data -----
        private void InitData()
        {
            ManufactoryName = PurchaseReturnRecordList[0].Manufactory;

            CalculateTotal();
            UpdateControlStatusUi();
        }

        private void CalculateTotal()
        {
            double sum = 0;

            foreach (var record in PurchaseReturnRecordList)
            {
                sum += Double.Parse(record.SubTotal);
            }

            Total = sum.ToString("##,###");
        }
        #endregion

        #region ----- Control Status -----
        private void Icon_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;

            if (border is null) return;

            border.Style = Resources["IconSelectingStyle"] as Style;
        }

        private void Icon_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;

            if (border is null) return;

            UpdateControlStatusUi();
        }

        private void Icon_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;

            if (border is null) return;

            ReportControlStatus = (ControlStatus)Int16.Parse(border.Tag.ToString());

            UpdateControlStatusUi();
        }

        private void UpdateControlStatusUi()
        {
            switch (ReportControlStatus)
            {
                case ControlStatus.CloseAll:
                    CloseAllIcon1.Style = Resources["IconSelectedStyle"] as Style;
                    CloseAllIcon2.Style = Resources["IconSelectedStyle"] as Style;
                    OpenOrderIcon.Style = Resources["IconUnSelectedStyle"] as Style;
                    OpenAllIcon.Style = Resources["IconUnSelectedStyle"] as Style;

                    OrderGrid.RowDefinitions[1].Height = new GridLength(0);
                    break;
                case ControlStatus.OpenOrder:
                    CloseAllIcon1.Style = Resources["IconUnSelectedStyle"] as Style;
                    CloseAllIcon2.Style = Resources["IconUnSelectedStyle"] as Style;
                    OpenOrderIcon.Style = Resources["IconSelectedStyle"] as Style;
                    OpenAllIcon.Style = Resources["IconUnSelectedStyle"] as Style;

                    OrderGrid.RowDefinitions[1].Height = GridLength.Auto;
                    break;
                case ControlStatus.OpenAll:
                    CloseAllIcon1.Style = Resources["IconUnSelectedStyle"] as Style;
                    CloseAllIcon2.Style = Resources["IconUnSelectedStyle"] as Style;
                    OpenOrderIcon.Style = Resources["IconUnSelectedStyle"] as Style;
                    OpenAllIcon.Style = Resources["IconSelectedStyle"] as Style;

                    OrderGrid.RowDefinitions[1].Height = GridLength.Auto;
                    break;
            }
        }

        public void SetControlStatus(ControlStatus status)
        {
            ReportControlStatus = status;

            UpdateControlStatusUi();
        }
        #endregion

        private void DataGrid_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            
            OutsideScrollViewer.ScrollToVerticalOffset(OutsideScrollViewer.VerticalOffset - e.Delta / 3);
        }
    }

    public class CalculateTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CollectionViewGroup group = value as CollectionViewGroup;

            double sum = 0;

            foreach (PurchaseReturnReportView.PurchaseReturnRecord record in group.Items)
            {
                sum += Double.Parse(record.SubTotal);
            }

            return sum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
