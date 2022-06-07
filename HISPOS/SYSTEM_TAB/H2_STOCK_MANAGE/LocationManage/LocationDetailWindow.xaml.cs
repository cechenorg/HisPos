using His_Pos.Class;
using His_Pos.Class.Location;
using His_Pos.FunctionWindow;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.LocationManage
{
    /// <summary>
    /// LocationDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LocationDetailWindow : Window, INotifyPropertyChanged
    {
        public static bool deactivate = true;

        public Location locationDetail;

        public Location LocationDetail
        {
            get
            {
                return locationDetail;
            }
            set
            {
                locationDetail = value;
                NotifyPropertyChanged("LocationDetail");
            }
        }

        public ObservableCollection<LocationDetail> deleteLocationDetails = new ObservableCollection<LocationDetail>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public LocationDetailWindow(Location location)
        {
            InitializeComponent();

            LocationDetail = location;
            LocationName.Text = LocationDetail.name;
            DataTable table = new DataTable();/// LocationDb.GetLocationDetail(location.id);
            foreach (DataRow row in table.Rows)
            {
                LocationDetail.locationDetailCollection.Add(new LocationDetail(row));
            }
            if (LocationDetail.locationDetailCollection.Count != 0)
            {
                Grid newrow = FunctionAddRow(LocationDetail.locationDetailCollection[0].name, LocationDetail.locationDetailCollection[0].status);
                for (int i = 1; i < LocationDetail.locationDetailCollection.Count; i++)
                {
                    if (LocationDetail.locationDetailCollection[i].locdrow != LocationDetail.locationDetailCollection[i - 1].locdrow)
                    {
                        newrow = FunctionAddRow(LocationDetail.locationDetailCollection[i].name, LocationDetail.locationDetailCollection[i].status);
                    }
                    else
                    {
                        FunctionAddColumn(newrow, LocationDetail.locationDetailCollection[i].name);
                    }
                }
            }
            CheckColumnRule();
            DataContext = this;
        }

        private void CheckColumnRule()
        {
            foreach (var grid in LocationDetails.Children)
                if (grid is Grid)
                {
                    if (((Grid)grid).ColumnDefinitions.Count - 2 == 1 && ((Grid)grid).Tag.ToString() != (LocationDetails.RowDefinitions.Count - 2).ToString())
                        (((Grid)grid).Children.OfType<Button>().ToList())[0].IsEnabled = false;
                    else
                        (((Grid)grid).Children.OfType<Button>().ToList())[0].IsEnabled = true;
                }
        }

        private void MinusColumns(object sender, RoutedEventArgs e)
        {
            Grid parent = (sender as Button).TryFindParent<Grid>();
            foreach (var btn in parent.Children)
            {
                if (btn is Button) ((Button)btn).IsEnabled = true;
            }
            string name = parent.Tag.ToString() + "-" + (parent.ColumnDefinitions.Count - 2).ToString();
            if (parent.ColumnDefinitions.Count - 3 == 1 && parent.Tag.ToString() != (LocationDetails.RowDefinitions.Count - 2).ToString())
                (sender as Button).IsEnabled = false;
            StackPanel removeItem = null;
            List<Button> buttons = new List<Button>();
            foreach (var obj in parent.Children)
            {
                if (obj is Button)
                {
                    buttons.Add(obj as Button);
                }
                else if (obj is StackPanel)
                {
                    foreach (var lab in (obj as StackPanel).FindChildren<Label>())
                    {
                        if ((lab as Label).Content.ToString().Contains(name))
                        {
                            removeItem = obj as StackPanel;
                            break;
                        }
                    }
                }
            }
            foreach (var label in removeItem.Children)
            {
                if (label is Label)
                {
                    if (((Label)label).Foreground == Brushes.Yellow)
                    {
                        deactivate = false;
                        MessageWindow.ShowMessage("此櫃位尚有商品 不可刪除", MessageType.ERROR);

                        deactivate = true;
                        CheckColumnRule();
                        return;
                    }
                }
            }
            parent.ColumnDefinitions.RemoveAt(0);
            foreach (Button button in buttons)
            {
                Grid.SetColumn(button, (button.Content.Equals("+")) ? parent.ColumnDefinitions.Count - 1 : parent.ColumnDefinitions.Count - 2);
            }
            parent.Children.Remove(removeItem);

            // if (parent.ColumnDefinitions.Count == 11) (sender as Button).IsEnabled = false;
            LocationDetail newlocationDetail = new LocationDetail(LocationDetail.id, LocationDetail.name + "-" + name, parent.Tag.ToString(), (parent.ColumnDefinitions.Count - 1).ToString(), "N");
            ///LocationDb.DeleteLocationDetail(newlocationDetail);
            if (parent.ColumnDefinitions.Count - 2 == 0)
            {
                LocationDetails.RowDefinitions.RemoveAt(0);
                LocationDetails.Children.Remove(parent);
                CheckColumnRule();
            }
        }

        private void AddColumns(object sender, RoutedEventArgs e)
        {
            FunctionAddColumn(null, "", sender);
        }

        private void FunctionAddColumn(Grid parent = null, string name = "", object sender = null)
        {
            if (parent == null) parent = (sender as Button).TryFindParent<Grid>();
            foreach (var btn in parent.Children)
            {
                if (btn is Button) ((Button)btn).IsEnabled = true;
            }

            parent.ColumnDefinitions.Add(new ColumnDefinition());
            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Background = (Brush)FindResource("GridSelected");
            newStackPanel.Margin = new Thickness(5);
            MenuItem propertyMenu = new MenuItem();
            propertyMenu.Header = "換櫃";
            propertyMenu.Click += newLabelPropertyMenu_Click;
            newStackPanel.ContextMenu = new ContextMenu();
            newStackPanel.ContextMenu.Items.Add(propertyMenu);

            Label newLabel = NewLabel();

            newLabel.Content = name == "" ? parent.Tag.ToString() + "-" + (parent.ColumnDefinitions.Count - 2).ToString() : name.Replace(LocationDetail.name + "-", "");
            foreach (var locationDetail in LocationDetail.locationDetailCollection)
            {
                if (locationDetail.name.Split('-')[1] + "-" + locationDetail.name.Split('-')[2] == newLabel.Content.ToString() && locationDetail.status == "Y")
                {
                    newLabel.Foreground = Brushes.Yellow;
                    break;
                }
                else
                {
                    newLabel.Foreground = Brushes.DimGray;
                }
            }
            newStackPanel.Children.Add(newLabel);

            Grid.SetColumn(newStackPanel, parent.ColumnDefinitions.Count - 3);
            parent.Children.Add(newStackPanel);
            foreach (var btn in parent.Children)
            {
                if (btn is Button)
                {
                    Grid.SetColumn((btn as Button), ((btn as Button).Content.Equals("+")) ? parent.ColumnDefinitions.Count - 1 : parent.ColumnDefinitions.Count - 2);
                    if (parent.ColumnDefinitions.Count == 11 && ((btn as Button).Content.Equals("+"))) ((Button)btn).IsEnabled = false;
                }
            }
            LocationDetail newlocationDetail = new LocationDetail(LocationDetail.id, LocationDetail.name + "-" + newLabel.Content.ToString(), parent.Tag.ToString(), (parent.ColumnDefinitions.Count - 2).ToString(), "N");
            ///LocationDb.UpdateLocationDetail(newlocationDetail);
        }

        public void newLabelPropertyMenu_Click(object sender, RoutedEventArgs e)
        {
            deactivate = false;
        }

        private Grid FunctionAddRow(string name = null, string isExist = "")
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
            MenuItem propertyMenu = new MenuItem();
            propertyMenu.Header = "換櫃";
            propertyMenu.Click += newLabelPropertyMenu_Click;
            newStackPanel.ContextMenu = new ContextMenu();
            newStackPanel.ContextMenu.Items.Add(propertyMenu);

            Label newLabel = NewLabel();
            if (isExist == "Y")
            {
                newLabel.Foreground = Brushes.Yellow;
            }
            else
                newLabel.Foreground = Brushes.DimGray;

            newLabel.Content = name == null ? newGrid.Tag.ToString() + "-" + (newGrid.ColumnDefinitions.Count - 2).ToString() : name.Replace(LocationDetail.name + "-", "");
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
            if (name == null)
            {
                LocationDetail newlocationDetail = new LocationDetail(LocationDetail.id, LocationDetail.name + "-" + newLabel.Content.ToString(), newGrid.Tag.ToString(), (newGrid.ColumnDefinitions.Count - 2).ToString(), "N");
                ///LocationDb.UpdateLocationDetail(newlocationDetail);
            }
            CheckColumnRule();
            return newGrid;
        }

        private void AddRows(object sender, RoutedEventArgs e)
        {
            FunctionAddRow();
        }

        private Label NewLabel()
        {
            Label newLabel = new Label();
            newLabel.FontFamily = new FontFamily("Segoe UI Semibold");
            newLabel.FontSize = 30;
            newLabel.HorizontalAlignment = HorizontalAlignment.Center;
            newLabel.VerticalAlignment = VerticalAlignment.Center;
            return newLabel;
        }

        private Button NewButton(string content)
        {
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

        private string IsCheck()
        {
            /* int number = 0;
             bool canConvert = int.TryParse(LocationName.Text.Substring(0, 1), out number);
             if (canConvert)
             {
                 return "第一個字不可以為數字";
             }
             foreach (ContentControl contentControl in LocationManageView.Instance.LocationCanvus.Children)
             {
                 LocationControl locationControl = (LocationControl)contentControl.Content;
                 if (locationControl.Name == LocationName.Text && LocationDetail.name != LocationName.Text) return "已有同名櫃位";
             }*/

            return "";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (deactivate)
            {
                string reply = IsCheck();
                if (reply == "")
                {
                    /// LocationDb.UpdateLocationName(LocationDetail.id, LocationName.Text);
                    //LocationManageView.Instance.InitLocation();
                    Close();
                }
                else
                {
                    MessageWindow.ShowMessage(reply, MessageType.ERROR);
                }
            }
        }
    }
}