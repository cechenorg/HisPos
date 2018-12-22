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
using System.Windows.Shapes;

namespace His_Pos.Service
{
    /// <summary>
    /// MedBagSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagSelectionWindow : Window
    {
        public MedBagSelectionWindow()
        {
            InitializeComponent();
            MultiMode.Focus();
        }

        private void MultiMode_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SingleMode_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void MultiMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                SingleMode.Focus();
            }
            else if(e.Key == Key.Enter)
            {
                DialogResult = false;
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
                DialogResult = true;
                Close();
            }
        }
    }
}
