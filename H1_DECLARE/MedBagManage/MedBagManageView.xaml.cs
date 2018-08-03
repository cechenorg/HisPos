using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using His_Pos.Class.MedBag;
using JetBrains.Annotations;
using CheckBox = System.Windows.Controls.CheckBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.H1_DECLARE.MedBagManage
{
    /// <summary>
    /// MedBagManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagManageView : UserControl, INotifyPropertyChanged
    {
        public static MedBagManageView Instance;
        private MedBag _selectedMedBag;

        public MedBag SelectedMedBag
        {
            get => _selectedMedBag;
            set
            {
                _selectedMedBag = value;
                OnPropertyChanged(nameof(SelectedMedBag));
            }
        }

        private double _medBagImgWidth;

        public double MedBagImgWidth
        {
            get => _medBagImgWidth;
            set
            {
                _medBagImgWidth = value;
                OnPropertyChanged(nameof(MedBagImgWidth));
            }
        }

        private double _medBagImgHeight;

        public double MedBagImgHeight
        {
            get => _medBagImgHeight;
            set
            {
                _medBagImgHeight = value;
                OnPropertyChanged(nameof(MedBagImgHeight));
            }
        }

        private static int id;

        public MedBagManageView()
        {
            InitializeComponent();
            Instance = this;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = @"Image files (*.jpg;*.png)|*.jpg|All Files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var selectedFileName = dlg.FileName;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                ImgWrap.Width = bitmap.Width;
                MedBagImgWidth = (bitmap.Width / bitmap.Height) * 850;
                SelectedMedBag = new MedBag(bitmap);
                SetMedBagRange();
            }
        }

        private void SetMedBagRange()
        {
            MedRangeLocationControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            MedRangeLocationControl.SetValue(Canvas.LeftProperty, 0.0);
            MedRangeLocationControl.SetValue(Canvas.TopProperty, 0.0);
            MedRangeLocationControl.SetValue(WidthProperty, MedBagImgWidth);
            MedRangeLocationControl.SetValue(HeightProperty, MedBagImg.Height);
            MedBagCanvas.Width = MedBagImgWidth;
            MedBagCanvas.Height = MedBagImg.Height;
        }

        private void NewLocationChecked(object sender, RoutedEventArgs e)
        {
            var locationName = (sender as CheckBox)?.Content.ToString();
            var controlName = (sender as CheckBox)?.Name;
            Instance.NewLocation(null, locationName, controlName);
        }

        public void NewLocation(string locid = null, string content = null, string controlName = null, double height = 0, double width = 0, double top = 0, double left = 0)
        {
            var contentControl = new ContentControl();
            if (string.IsNullOrEmpty(content))
                contentControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            else
                contentControl.Template = (ControlTemplate)FindResource("MedBagDesignerItemTemplate");
            RdlLocationControl newLocation = null;
            if (locid != null)
            {
                newLocation = new RdlLocationControl(Convert.ToInt32(locid), content);
                id = Convert.ToInt32(locid);
                id++;
            }
            else
            {
                newLocation = new RdlLocationControl(id, content);
                id++;
            }
            if (string.IsNullOrEmpty(content))
            {
                contentControl.Height = (height == 0) ? 150 : height;
                contentControl.Width = (width == 0) ? 150 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
            else
            {
                contentControl.Height = (height == 0) ? 15 : height;
                contentControl.Width = (width == 0) ? 60 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, top == 0 ? 360 : top);
                Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            }
        }

        public void SaveLocation()
        {
        }

        private void DeleteLocation(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            MedBagCanvas.Children.Remove(MedBagCanvas.Children.OfType<ContentControl>().Where(r => r.Content is RdlLocationControl).Single(r => (r.Content as RdlLocationControl).LabelContent.Equals(checkBox.Content)));
        }
    }
}