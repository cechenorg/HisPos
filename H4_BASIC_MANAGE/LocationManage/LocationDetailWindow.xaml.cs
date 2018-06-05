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
using MahApps.Metro.Controls;

namespace His_Pos.H4_BASIC_MANAGE.LocationManage
{
    /// <summary>
    /// LocationDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LocationDetailWindow : Window
    {
        public LocationDetailWindow()
        {
            InitializeComponent();
        }

        private void AddColumns(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid>();
            parent.ColumnDefinitions.Insert(parent.ColumnDefinitions.Count - 2, new ColumnDefinition());

            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(10);

            Label newLabel = new Label();
            newLabel.Content = "1-" + (parent.ColumnDefinitions.Count - 1).ToString();
            newLabel.Foreground = Brushes.DimGray;
            newLabel.FontSize = 50;
            newLabel.HorizontalAlignment = HorizontalAlignment.Center;
            newLabel.VerticalAlignment = VerticalAlignment.Center;

            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, parent.ColumnDefinitions.Count - 2);
            parent.Children.Add(newStackPanel);

            Grid.SetColumn((sender as Button), parent.ColumnDefinitions.Count - 1);
        }
    }
}
