using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{
    /// <summary>
    /// LeaveTotalControl.xaml 的互動邏輯
    /// </summary>
    public partial class LeaveTotalControl : UserControl
    {
        public Brush TopBackGroundBrush
        {
            get { return (Brush)GetValue(TopBackGroundBrushProperty); }
            set { SetValue(TopBackGroundBrushProperty, value); }
        }

        public int TotalDayCount
        {
            get { return (int)GetValue(TotalDayCountProperty); }
            set { SetValue(TotalDayCountProperty, value); }
        }

        public int DayLeftCount
        {
            get { return (int)GetValue(DayLeftCountProperty); }
            set { SetValue(DayLeftCountProperty, value); }
        }

        public string LeaveType
        {
            get { return (string)GetValue(LeaveTypeProperty); }
            set { SetValue(LeaveTypeProperty, value); }
        }

        public static readonly DependencyProperty TopBackGroundBrushProperty =
            DependencyProperty.Register("TopBackGroundBrush",
                                         typeof(Brush),
                                         typeof(LeaveTotalControl),
                                         new PropertyMetadata(Brushes.LightBlue));

        public static readonly DependencyProperty TotalDayCountProperty =
            DependencyProperty.Register("TotalDayCount",
                                         typeof(int),
                                         typeof(LeaveTotalControl),
                                         new PropertyMetadata(0));

        public static readonly DependencyProperty DayLeftCountProperty =
            DependencyProperty.Register("DayLeftCount",
                                         typeof(int),
                                         typeof(LeaveTotalControl),
                                         new PropertyMetadata(0));

        public static readonly DependencyProperty LeaveTypeProperty =
            DependencyProperty.Register("LeaveType",
                                         typeof(string),
                                         typeof(LeaveTotalControl),
                                         new PropertyMetadata("假別"));

        public LeaveTotalControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}