using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<LocationDetail> deleteLocationDetails = new ObservableCollection<LocationDetail>();
        public LocationDetailWindow(Location location)
        {
            InitializeComponent();

            locationDetail = location;
            foreach (DataRow row in LocationDb.GetLocationDetail(location.id).Rows) {
                locationDetail.locationDetailCollection.Add(new LocationDetail(row));
            }
            if (locationDetail.locationDetailCollection.Count != 0) {
                Grid newrow = FunctionAddRow(locationDetail.locationDetailCollection[0].name, locationDetail.locationDetailCollection[0].isExist);
                for (int i = 1; i < locationDetail.locationDetailCollection.Count; i++)
                {
                    if (locationDetail.locationDetailCollection[i].locdrow != locationDetail.locationDetailCollection[i - 1].locdrow)
                    {
                        newrow = FunctionAddRow(locationDetail.locationDetailCollection[i].name, locationDetail.locationDetailCollection[i].isExist);
                    }
                    else
                    {
                        FunctionAddColumn(newrow, locationDetail.locationDetailCollection[i].name);
                    }
                }
            }
           
        }
        private void MinusColumns(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid>();
           string name = locationDetail.name + "-" + parent.Tag.ToString() + "-" + (parent.ColumnDefinitions.Count - 2).ToString();
            parent.ColumnDefinitions.RemoveAt(0);

            StackPanel removeItem = null;

            foreach (var obj in parent.Children)
            {
                if (obj is Button)
                {
                    Grid.SetColumn((obj as Button), ((obj as Button).Content.Equals("+")) ? parent.ColumnDefinitions.Count - 1 : parent.ColumnDefinitions.Count - 2);
                }
                else if (obj is StackPanel )
                {
                    foreach( var lab in (obj as StackPanel).FindChildren<Label>())
                    {
                        if((lab as Label).Content.Equals(name))
                        {
                            removeItem = obj as StackPanel;
                            break;
                        }
                    }
                }
            }

            parent.Children.Remove(removeItem);

            if (parent.ColumnDefinitions.Count == 10) (sender as Button).IsEnabled = false;
            LocationDetail newlocationDetail = new LocationDetail(locationDetail.id, name, parent.Tag.ToString(), (parent.ColumnDefinitions.Count - 1).ToString(), "N");
            LocationDb.DeleteLocationDetail(newlocationDetail);
        }
        private void AddColumns(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid> ();
            parent.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);

            Label newLabel = NewLabel();
            newLabel.Content = locationDetail.name + "-" + parent.Tag.ToString() + "-" + (parent.ColumnDefinitions.Count - 2).ToString();
            
            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, parent.ColumnDefinitions.Count - 3);
            parent.Children.Add(newStackPanel);

            foreach( var btn in parent.Children)
            {
                if( btn is Button )
                {
                    Grid.SetColumn((btn as Button), ((btn as Button).Content.Equals("+")) ? parent.ColumnDefinitions.Count - 1 : parent.ColumnDefinitions.Count - 2);
                }
            }

            if (parent.ColumnDefinitions.Count == 11) (sender as Button).IsEnabled = false;
            LocationDetail newlocationDetail = new LocationDetail(locationDetail.id, newLabel.Content.ToString(), parent.Tag.ToString(), (parent.ColumnDefinitions.Count - 1).ToString(),"N");
            LocationDb.UpdateLocationDetail(newlocationDetail);
        }
        private void FunctionAddColumn(Grid parent,string name) {
            parent.ColumnDefinitions.Add(new ColumnDefinition());
            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);

            Label newLabel = NewLabel();
            newLabel.Content = name;
            newStackPanel.Children.Add(newLabel);
            Grid.SetColumn(newStackPanel, parent.ColumnDefinitions.Count - 3);
            parent.Children.Add(newStackPanel);
            foreach (var btn in parent.Children) {
                if (btn is Button) {
                    Grid.SetColumn((btn as Button), ((btn as Button).Content.Equals("+")) ? parent.ColumnDefinitions.Count - 1 : parent.ColumnDefinitions.Count - 2);
                    if (parent.ColumnDefinitions.Count == 11) ((Button)btn).IsEnabled = false;
                }
            }
          
        }
        private Grid FunctionAddRow(string name = null,string isExist = "")
        {
            LocationDetails.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });

            Grid newGrid = new Grid();
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            newGrid.Tag = LocationDetails.RowDefinitions.Count - 2;

            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);

            Label newLabel = NewLabel();
            if (isExist == "Y")
            {

                newLabel.Foreground = Brushes.Yellow;
            }
            else
                newLabel.Foreground = Brushes.DimGray;

            newLabel.Content = name == null ? locationDetail.name + "-" + newGrid.Tag.ToString() + "-" + (newGrid.ColumnDefinitions.Count - 2).ToString() : name;
            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, newGrid.ColumnDefinitions.Count - 3);
            newGrid.Children.Add(newStackPanel);

            Button minusButton = NewButton("-");
            Grid.SetColumn(minusButton, newGrid.ColumnDefinitions.Count - 2);
            newGrid.Children.Add(minusButton);

            Button newButton = NewButton("+");
            Grid.SetColumn(newButton, newGrid.ColumnDefinitions.Count - 1);
            newGrid.Children.Add(newButton);

            Grid.SetRow(newGrid, LocationDetails.RowDefinitions.Count - 2);
            LocationDetails.Children.Add(newGrid);

            Grid.SetRow(ButtonAddRow, LocationDetails.RowDefinitions.Count - 1); 
            if (LocationDetails.RowDefinitions.Count == 12) ButtonAddRow.IsEnabled = false;
            if (name == null) {
                LocationDetail newlocationDetail = new LocationDetail(locationDetail.id, newLabel.Content.ToString(), newGrid.Tag.ToString(), (newGrid.ColumnDefinitions.Count - 1).ToString(), "N");
                LocationDb.UpdateLocationDetail(newlocationDetail);
            }
            return newGrid;
        }
        private void AddRows(object sender, RoutedEventArgs e)
        {
            FunctionAddRow();
        }
        private Label NewLabel() {
            Label newLabel = new Label();
            newLabel.FontFamily = new FontFamily("Segoe UI Semibold");
            newLabel.FontSize = 30;
            newLabel.HorizontalAlignment = HorizontalAlignment.Center;
            newLabel.VerticalAlignment = VerticalAlignment.Center;
            return newLabel;
        }
        private Button NewButton(string content) {
            Button newButton = new Button();
            newButton.Content = content;
            newButton.FontSize = 30;
            newButton.Height = 50;
            newButton.Background = (Brush)FindResource("Shadow");
            newButton.Foreground = Brushes.DimGray;
            newButton.BorderThickness = new Thickness(0);
            newButton.Margin = new Thickness(5);
            if (content == "+")
                newButton.Click += AddColumns;
            else if (content == "-")
                newButton.Click += MinusColumns;
            return newButton;
        }

        
    }
}
