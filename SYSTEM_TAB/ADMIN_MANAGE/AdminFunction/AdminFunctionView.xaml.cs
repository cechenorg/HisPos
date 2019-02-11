using System.Windows;
using System.Windows.Controls;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;

namespace His_Pos.SYSTEM_TAB.ADMIN_MANAGE.AdminFunction {
    /// <summary>
    /// AdminFunctionView.xaml 的互動邏輯
    /// </summary>
    public partial class AdminFunctionView : UserControl {
        public AdminFunctionView() {
            InitializeComponent();
        }

        private void ButtonPredictChronic_Click(object sender, RoutedEventArgs e) {
            Prescriptions.PredictThreeMonthPrescription();
            MessageWindow.ShowMessage("預約慢箋完成!",MessageType.SUCCESS);
            
        }
    }
}
