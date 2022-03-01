using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach.EntryDetailWindow
{
    /// <summary>
    /// EntryDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EntryDetailWindow : Window
    {
        public EntryDetailWindow(DateTime date)
        {
            InitializeComponent();
            EntryDetailWindowViewModel entryDetailWindowViewModel = new EntryDetailWindowViewModel(date);
            DataContext = entryDetailWindowViewModel;
            ShowDialog();
        }
    }
}