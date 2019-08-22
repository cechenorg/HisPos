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

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach.EntryDetailWindow {
    /// <summary>
    /// EntryDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EntryDetailWindow : Window {
        public EntryDetailWindow(DateTime date) {
            InitializeComponent();
            EntryDetailWindowViewModel entryDetailWindowViewModel = new EntryDetailWindowViewModel(date);
            DataContext = entryDetailWindowViewModel;
            ShowDialog();
        }
    }
}
