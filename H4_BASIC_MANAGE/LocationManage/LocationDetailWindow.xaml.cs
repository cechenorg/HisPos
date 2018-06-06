using System;
using System.Collections.Generic;
using System.Data;
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
using His_Pos.Class;
using MahApps.Metro.Controls;

namespace His_Pos.H4_BASIC_MANAGE.LocationManage
{
    /// <summary>
    /// LocationDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LocationDetailWindow : Window
    {
        public Location locationDetail;
        public LocationDetailWindow(Location location)
        {
            InitializeComponent();

            locationDetail = location;
            foreach (DataRow row in LocationDb.GetLocationDetail(location.id).Rows) {
                locationDetail.locationDetailCollection.Add(new LocationDetail(row));
            }

        }

        private void AddColumns(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid>();
            parent.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);

            Label newLabel = new Label();
            newLabel.Content = parent.Tag.ToString() + "-" + (parent.ColumnDefinitions.Count - 1).ToString();
            newLabel.Foreground = Brushes.DimGray;
            newLabel.FontFamily = new FontFamily("Segoe UI Semibold");
            newLabel.FontSize = 30;
            newLabel.HorizontalAlignment = HorizontalAlignment.Center;
            newLabel.VerticalAlignment = VerticalAlignment.Center;

            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, parent.ColumnDefinitions.Count - 2);
            parent.Children.Add(newStackPanel);

            Grid.SetColumn((sender as Button), parent.ColumnDefinitions.Count - 1);

            if (parent.ColumnDefinitions.Count == 11) (sender as Button).IsEnabled = false;
        }
        
        private void AddRows(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid>();
            parent.RowDefinitions.Add(new RowDefinition(){ Height = new GridLength(60)});

            Grid newGrid = new Grid();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.Tag = parent.RowDefinitions.Count - 2;

            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);

            Label newLabel = new Label();
            newLabel.Content = newGrid.Tag.ToString() + "-" + (newGrid.ColumnDefinitions.Count - 1).ToString();
            newLabel.Foreground = Brushes.DimGray;
            newLabel.FontFamily = new FontFamily("Segoe UI Semibold");
            newLabel.FontSize = 30;
            newLabel.HorizontalAlignment = HorizontalAlignment.Center;
            newLabel.VerticalAlignment = VerticalAlignment.Center;

            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, newGrid.ColumnDefinitions.Count - 2);
            newGrid.Children.Add(newStackPanel);
            
            Button newButton = new Button();
            newButton.Content = "+";
            newButton.FontSize = 30;
            newButton.Height = 50;
            newButton.Background = (Brush)FindResource("Shadow");
            newButton.Foreground = Brushes.DimGray;
            newButton.BorderThickness = new Thickness(0);
            newButton.Margin = new Thickness(5);
            newButton.Click += AddColumns;

            Grid.SetColumn(newButton, newGrid.ColumnDefinitions.Count - 1);
            newGrid.Children.Add(newButton);

            Grid.SetRow(newGrid, parent.RowDefinitions.Count - 2);
            parent.Children.Add(newGrid);

            Grid.SetRow((sender as Button), parent.RowDefinitions.Count - 1);

            if (parent.RowDefinitions.Count == 12) (sender as Button).IsEnabled = false;
        }
    }
}
