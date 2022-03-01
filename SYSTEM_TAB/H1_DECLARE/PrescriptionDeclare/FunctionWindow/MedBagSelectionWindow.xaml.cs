using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow
{
    /// <summary>
    /// MedBagSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagSelectionWindow : Window
    {
        public bool? result { get; set; }

        public MedBagSelectionWindow()
        {
            InitializeComponent();
            MultiMode.Focus();
        }

        private void MultiMode_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }

        private void SingleMode_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            Close();
        }

        private void MultiMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                SingleMode.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                result = false;
                Close();
            }
        }

        private void SingleMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                MultiMode.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                result = true;
                Close();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                result = null;
                Close();
            }
        }
    }
}