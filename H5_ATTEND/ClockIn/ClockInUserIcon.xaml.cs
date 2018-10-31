using System.Windows.Controls;
using His_Pos.Class.WorkSchedule;

namespace His_Pos.H5_ATTEND.ClockIn
{
    /// <summary>
    /// ClockInUserIcon.xaml 的互動邏輯
    /// </summary>
    public partial class ClockInUserIcon : UserControl
    {
        public string Id { get; set; }

        public ClockInUserIcon(UserIconData userIconData)
        {
            InitializeComponent();

            UserName.Text = userIconData.Name;
            Id = userIconData.Id;
        }
    }
}
