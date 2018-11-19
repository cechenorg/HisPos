using System;
using System.Collections.Generic;
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
    public partial class ManReportControl : UserControl
    {

        #region ----- Define Variables -----

        public List<PurchaseReturnReportView.PurchaseReturnRecord> PurchaseReturnRecordList { get; }

        public string ManufactoryName { get; }

        public ScrollViewer OutsideScrollViewer { get; }

        #endregion

        public ManReportControl(List<PurchaseReturnReportView.PurchaseReturnRecord> list, ScrollViewer scrollViewer)
        {
            InitializeComponent();
            DataContext = this;

            PurchaseReturnRecordList = list;
            OutsideScrollViewer = scrollViewer;
            ManufactoryName = list[0].Manufactory;
        }

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
