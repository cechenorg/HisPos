using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
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