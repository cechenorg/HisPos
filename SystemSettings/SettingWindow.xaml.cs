using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace His_Pos.SystemSettings
{
    /// <summary>
    /// SettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SettingWindow : Window
    {
        private StackPanel CurrentStack { get; set; }

        public SettingWindow()
        {
            InitializeComponent();

            CurrentStack = MyPharmacyStack;

            UpdateContentControl();
        }

        private void UpdateContentControl()
        {
            SelectTab();

        }
        
        private void SelectTab()
        {
            CurrentStack.Background = Brushes.LightGray;

            Label label = CurrentStack.Children.OfType<Label>().ToList()[0];
            label.Foreground = Brushes.DimGray;
        }

        private void ClearSelectTab()
        {
            CurrentStack.Background = Brushes.Transparent;

            Label label = CurrentStack.Children.OfType<Label>().ToList()[0];
            label.Foreground = Brushes.DarkGray;
        }

        private void SettingTab_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is null) return;

            StackPanel stackPanel = sender as StackPanel;

            if (CurrentStack != null)
                ClearSelectTab();

            CurrentStack = stackPanel;

            UpdateContentControl();
        }
    }
}
