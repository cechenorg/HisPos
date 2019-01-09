using System.Windows.Controls;

namespace His_Pos.H4_BASIC_MANAGE.LocationManage
{
    /// <summary>
    /// LocationControl.xaml 的互動邏輯
    /// </summary>
    public partial class LocationControl : UserControl
    {
        public int id;
        public LocationControl(int locId)
        {
            InitializeComponent();
            id = locId;
        }
    }
}
