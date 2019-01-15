using System.Windows.Controls;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare
{
    /// <summary>
    /// PrescriptionDeclareView.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDeclareView : UserControl
    {
        public PrescriptionDeclareView()
        {
            InitializeComponent();
            DataContext = new PrescriptionDeclareViewModel();
        }
    }
}
