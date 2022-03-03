using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.GeneralCustomControl
{
    /// <summary>
    /// MenuListItem.xaml 的互動邏輯
    /// </summary>
    public partial class MenuListItem : UserControl
    {
        public Timer ListMenuTimer = new Timer();
        private double _listMenuHeight;
        private double _listMaxMenuHeight = 0;
        private bool _listMenuHided = true;
        public int _count = 0;

        public MenuListItem()
        {
            InitializeComponent();
            ListMenu.Height = 0;
            ListMenuTimer.Interval = 1;
            ListMenuTimer.Tick += ListMenuTimerTick;
        }

        public ICommand SomeCommand
        {
            get { return (ICommand)GetValue(SomeCommandProperty); }
            set { SetValue(SomeCommandProperty, value); }
        }

        public static readonly DependencyProperty SomeCommandProperty =
            DependencyProperty.Register("SomeCommand", typeof(ICommand), typeof(MenuListItem), new UIPropertyMetadata(null));

        public void SetLabelText(String labelText)
        {
            ListMenuLabel.Content = labelText;
            this.UpdateLayout();
        }

        public void SetLabelImage(String imagePath)
        {
            ListMenuImage.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)); ;
            this.UpdateLayout();
        }

        public void SetLabelColor(String colorCode)
        {
            var color = (Color)ColorConverter.ConvertFromString(colorCode);
            ListMenuLabel.Foreground = new SolidColorBrush(color);
            this.UpdateLayout();
        }

        public void NewMenuItem(String itemName, System.Windows.Controls.MenuItem newItem, RoutedEventHandler e)
        {
            newItem.Name = itemName;
            var color = (Color)ColorConverter.ConvertFromString("#FFe0e0e0");
            var transparent = (Color)ColorConverter.ConvertFromString("#00FFFFFF");
            newItem.Foreground = new SolidColorBrush(color);
            newItem.Background = new SolidColorBrush(transparent);
            newItem.Header = itemName;
            newItem.Height = double.NaN;
            newItem.Padding = new Thickness(60, 6, 0, 6);
            newItem.FontSize = 16;
            newItem.Width = 330;
            newItem.VerticalContentAlignment = VerticalAlignment.Stretch;
            newItem.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            newItem.FlowDirection = System.Windows.FlowDirection.LeftToRight;
            newItem.Click += e;
            ListMenuImage.Margin = new Thickness(10, 10, 6, 0);
            ListMenuLabel.Margin = new Thickness(5, 10, 0, 0);
            ListMenu.Items.Add(newItem);
            _listMenuHeight = ListMenu.Items.Count * 32;
            SetListMenuVisibility(Visibility.Visible);
            _count++;
        }

        public void SetListMenuVisibility(Visibility v)
        {
            ListMenu.Visibility = v;
        }

        public void SetListMenuHeight(double height)
        {
            if (ListMenu.Height != 0)
                ListMenu.Height = height;
            else
                _listMenuHeight = ListMenu.Items.Count * 26;
        }

        public int GetListMenuCount()
        {
            return ListMenu.Items.Count;
        }

        private void ListMenuTimerTick(object sender, EventArgs e)
        {
            if (_listMenuHided)
            {
                ListMenu.Height = ListMenu.Height + 8;
                if (ListMenu.Height >= _listMenuHeight)
                {
                    ListMenuTimer.Stop();
                    _listMenuHided = false;
                    this.UpdateLayout();
                }
            }
            else
            {
                ListMenu.Height = ListMenu.Height - 8;
                if (ListMenu.Height <= 0)
                {
                    ListMenuTimer.Stop();
                    _listMenuHided = true;
                    this.UpdateLayout();
                }
            }
        }

        private void DockPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var color = (Color)ColorConverter.ConvertFromString("#ffa500");
            ListMenuLabel.Foreground = new SolidColorBrush(color);
        }

        private void DockPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var color = (Color)ColorConverter.ConvertFromString("#ffffff");
            ListMenuLabel.Foreground = new SolidColorBrush(color);
        }
    }
}