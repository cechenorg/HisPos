using System.Windows;
using System.Windows.Input;
using His_Pos.Class.Declare;

namespace His_Pos.PrescriptionRevise
{
    /// <summary>
    /// PrescriptionReviseOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionReviseOutcome : Window
    {
        public PrescriptionReviseOutcome(DeclareData revised)
        {
            InitializeComponent();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
