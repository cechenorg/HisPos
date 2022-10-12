using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.CooperativeClinicControl
{
    /// <summary>
    /// CooperativeClinicControl.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeClinicControl : UserControl
    {
        public CooperativeClinicControl()
        {
            InitializeComponent();
            DataContext = new CooperativeClinicControlViewModel();
        }
    }
}