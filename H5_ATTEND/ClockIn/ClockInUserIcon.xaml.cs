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
