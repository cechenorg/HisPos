using ClosingHistoryViewModelHis_Pos.SYSTEM_TAB.H11_CLOSING.Closing;
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

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    /// <summary>
    /// StrikeHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingHistoryWindow : Window
    {
        public ClosingHistoryWindow()
        {
            InitializeComponent();
            DataContext = new ClosingHistoryViewModel();
        }
    }
}
