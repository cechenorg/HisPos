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
        public ObservableCollection<MedBagLocation> MedBagLocationCollection = new ObservableCollection<MedBagLocation>();
        private static int id = 0;

        public MedBagManageView()
        {
            InitializeComponent();
            Instance = this;
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
                Image.Source = bitmap;
            }
        }

        private void NewLocationClick(object sender, RoutedEventArgs e)
        {
            string locationName = (sender as System.Windows.Controls.Button).Name;
            Instance.NewLocation(null, locationName);
        }

        public void NewLocation(string locid = null, string parameterName = null, double height = 0, double width = 0, double top = 0, double left = 0)
        {
            ContentControl contentControl = new ContentControl();
            contentControl.Template = (ControlTemplate)FindResource("MedBagDesignerItemTemplate");
            RdlLocationControl newLocation = null;
            if (locid != null)
            {
                newLocation = new RdlLocationControl(Convert.ToInt32(locid));
                newLocation.Name = parameterName;
                newLocation.RdlParameterName.Content = parameterName;
                id = Convert.ToInt32(locid);
                id++;
            }
            else
            {
                newLocation = new RdlLocationControl(id);
                newLocation.Name = parameterName;
                newLocation.RdlParameterName.Content = parameterName;
                id++;
            }
            contentControl.Height = (height == 0) ? 10 : height;
            contentControl.Width = (width == 0) ? 10 : width;
            contentControl.Content = newLocation;
            MedBagCanvas.Children.Add(contentControl);
            Canvas.SetTop(contentControl, top == 0 ? 360 : top);
            Canvas.SetLeft(contentControl, left == 0 ? 648 : left);
            SaveLocation();
        }

        public void SaveLocation()
        {
            MedBagLocationCollection.Clear();
            foreach (ContentControl contentControl in MedBagCanvas.Children)
            {
                RdlLocationControl locationControl = (RdlLocationControl)contentControl.Content;
                MedBagLocationCollection.Add(new MedBagLocation(locationControl.id, locationControl.Name, Canvas.GetLeft(contentControl), Canvas.GetTop(contentControl), contentControl.Width, contentControl.Height));
            }
            MedBagLocationDB.SaveLocationData(MedBagLocationCollection);
        }

        private void MoveThumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ContentControl contentcontrol in MedBagCanvas.Children)
            {
                var child = VisualTreeHelper.GetChild(contentcontrol, 0);
                var thumb = VisualTreeHelper.GetChild(child, 1);
                ((Control)thumb).Visibility = Visibility.Collapsed;
            }
            (((Grid)(sender as RDLLocationMoveThumb).Parent).Children.OfType<Control>().ToList())[1].Visibility = Visibility.Visible;
        }
    }
}