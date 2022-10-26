using System;
using System.Collections.Generic;
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

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.ClossingCashSelect
{
    /// <summary>
    /// ClosingCashDataGrid.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingCashDataGrid : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ClosingCashReportDataList), typeof(ClosingCashDataGrid), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ClosingCashDataGrid), new PropertyMetadata(string.Empty));

        public ClosingCashReportDataList Source
        {
            get { return (ClosingCashReportDataList)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ClosingCashDataGrid()
        {
            InitializeComponent();
            MyGrid.DataContext = this;
        }
    }
}
