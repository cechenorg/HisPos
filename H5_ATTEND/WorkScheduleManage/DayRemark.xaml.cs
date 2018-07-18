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

namespace His_Pos.H5_ATTEND.WorkScheduleManage
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
            LeaveRecord.Text = leaveRecord;
        }
    }
}
