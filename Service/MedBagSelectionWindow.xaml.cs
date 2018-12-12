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
        }

        private void MultiMode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SingleMode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
