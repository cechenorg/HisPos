using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Class.MedBag;
using His_Pos.Class.MedBagLocation;
using His_Pos.H4_BASIC_MANAGE.LocationManage;
using His_Pos.LocationManage;
using JetBrains.Annotations;
using Control = System.Windows.Controls.Control;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.H4_BASIC_MANAGE.MedBagManage
{
    /// <summary>
    /// MedBagManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagManageView : UserControl, INotifyPropertyChanged
    {
        public static MedBagManageView Instance;
        private MedBag selectedMedBag;

        public MedBag SelectedMedBag
        {
            get { return selectedMedBag; }
            set
            {
                selectedMedBag = value;
                OnPropertyChanged("SelectedMedBag");
            }
        }

        private double medBagImgWidth;

        public double MedBagImgWidth
        {
            get { return medBagImgWidth; }
            set
            {
                medBagImgWidth = value;
                OnPropertyChanged("MedBagImgWidth");
            }
        }

        private double medBagImgHeight;

        public double MedBagImgHeight
        {
            get { return medBagImgHeight; }
            set
            {
                medBagImgHeight = value;
                OnPropertyChanged("MedBagImgHeight");
            }
        }

        private static int id = 0;

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
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dlg.Filter = "Image files (*.jpg;*.png)|*.jpg|All Files (*.*)|*.*";

            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                ImgWrap.Width = bitmap.Width;
                MedBagImgWidth = (bitmap.Width / bitmap.Height) * 700;
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

        private void NewLocationClick(object sender, RoutedEventArgs e)
        {
            string locationName = (sender as System.Windows.Controls.CheckBox).Content.ToString();
            Instance.NewLocation(null, locationName);
        }

        public void NewLocation(string locid = null, string parameterName = null, double height = 0, double width = 0, double top = 0, double left = 0)
        {
            ContentControl contentControl = new ContentControl();
            if (string.IsNullOrEmpty(parameterName))
                contentControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            else
                contentControl.Template = (ControlTemplate)FindResource("MedBagDesignerItemTemplate");
            RdlLocationControl newLocation = null;
            if (locid != null)
            {
                newLocation = new RdlLocationControl(Convert.ToInt32(locid),parameterName);
                newLocation.RdlParameterName.Content = parameterName;
                id = Convert.ToInt32(locid);
                id++;
            }
            else
            {
                newLocation = new RdlLocationControl(id, parameterName);
                newLocation.RdlParameterName.Content = parameterName;
                id++;
            }
            if (string.IsNullOrEmpty(parameterName))
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
            SaveLocation();
        }

        public void SaveLocation()
        {

        }
    }
}