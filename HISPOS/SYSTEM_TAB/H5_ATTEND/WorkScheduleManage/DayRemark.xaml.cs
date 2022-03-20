using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// DayRemark.xaml 的互動邏輯
    /// </summary>
    public partial class DayRemark : UserControl
    {
        public DayRemark(string message, string leaveRecord)
        {
            InitializeComponent();

            Remark.Text = message;
            LeaveRecord.Text = leaveRecord.Replace('#', '\n');
        }
    }
}