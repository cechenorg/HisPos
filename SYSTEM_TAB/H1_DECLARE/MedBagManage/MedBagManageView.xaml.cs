using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using His_Pos.Class;
using His_Pos.Class.MedBag;
using His_Pos.Class.MedBagLocation;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using JetBrains.Annotations;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using DataGrid = System.Windows.Controls.DataGrid;
using RadioButton = System.Windows.Controls.RadioButton;
using Visibility = System.Windows.Visibility;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage
{
    /// <summary>
    ///     MedBagManageView.xaml 的互動邏輯
    /// </summary>
    public partial class MedBagManageView : INotifyPropertyChanged
    {
        public static MedBagManageView Instance;

        private int _id;
        private int _maxMedBagId = -1;
        private MedBagMode _selectedMode;

        public MedBagMode SelectedMode
        {
            get => _selectedMode;
            set
            {
                _selectedMode = value;
                if (_selectedMode == MedBagMode.SINGLE)
                {
                    SingleMode.IsChecked = true;
                    MultiMode.IsChecked = false;
                }
                else
                {
                    MultiMode.IsChecked = true;
                    SingleMode.IsChecked = false;
                }
                SetMedicineCheckBoxVisibility();
                OnPropertyChanged(nameof(SelectedMode));
            }
        }
        private ObservableCollection<MedBag> _medBagCollection;
        private double _medBagImgHeight;
        private double _medBagImgWidth;
        private MedBag _selectedMedBag;

        public MedBagManageView()
        {
            InitializeComponent();
            Instance = this;
            DataContext = this;
            InitializeMedManageViewControl(MedBagMode.SINGLE);
        }

        private bool singleMode;

        public ObservableCollection<MedBag> MedBagCollection
        {
            get => _medBagCollection;
            set
            {
                _medBagCollection = value;
                OnPropertyChanged(nameof(MedBagCollection));
            }
        }

        public MedBag SelectedMedBag
        {
            get => _selectedMedBag;
            set
            {
                _selectedMedBag = value;
                SetMedBagRange();
                OnPropertyChanged(nameof(SelectedMedBag));
            }
        }

        public double MedBagImgWidth
        {
            get => _medBagImgWidth;
            set
            {
                _medBagImgWidth = value;
                OnPropertyChanged(nameof(MedBagImgWidth));
            }
        }

        public double MedBagImgHeight
        {
            get => _medBagImgHeight;
            set
            {
                _medBagImgHeight = value;
                OnPropertyChanged(nameof(MedBagImgHeight));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeMedManageViewControl(MedBagMode mode)
        {
            SelectedMode = mode;
            SelectedMedBag = new MedBag(SelectedMode);
            MedBagCollection = new ObservableCollection<MedBag>();
            var loadingWindow = new LoadingWindow();
            loadingWindow.GetMedBagData(this);
            loadingWindow.Show();
            foreach (var m in MedBagCollection)
            {
                var i = int.Parse(m.Id);
                if (i > _maxMedBagId)
                    _maxMedBagId = i;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageSelector_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty()) return;

            var dlg = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = @"Image files (*.jpg;*.png)|*.jpg|All Files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            SetMedBagImage(dlg);
            SetMedBagRange();
        }

        private void SetMedBagImage(OpenFileDialog dlg)
        {
            var selectedFileName = dlg.FileName;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(selectedFileName);
            bitmap.EndInit();
            SelectedMedBag.MedBagImage = bitmap;
        }

        private void SetMedBagRange()
        {
            if (SelectedMedBag?.MedBagImage == null) return;
            SetImageRangeSize();
            SetImageRangeConrol();
        }

        private void SetImageRangeConrol()
        {
            ImgWrap.Width = MedBagImgWidth;
            ImgWrap.Height = MedBagImgHeight;

            MedBagCanvas.Width = MedBagImgWidth;
            MedBagCanvas.Height = MedBagImgHeight;
            SetMedRangeLocationControl();
        }

        private void SetMedRangeLocationControl()
        {
            MedRangeLocationControl.Template = (ControlTemplate)FindResource("MedBagRangeItemTemplate");
            MedRangeLocationControl.SetValue(Canvas.LeftProperty, 0.0);
            MedRangeLocationControl.SetValue(Canvas.TopProperty, 0.0);
            MedRangeLocationControl.SetValue(WidthProperty, MedBagImgWidth);
            MedRangeLocationControl.SetValue(HeightProperty, MedBagImgHeight);
            MedRangeLocationControl.Visibility = Visibility.Visible;
        }

        private void SetImageRangeSize()
        {
            if (SelectedMedBag.MedBagImage.Width >= SelectedMedBag.MedBagImage.Height)
            {
                MedBagImgWidth = 935;
                MedBagImgHeight = 935 * (SelectedMedBag.MedBagImage.Height / SelectedMedBag.MedBagImage.Width);
                while (MedBagImgHeight > MedBagImg.MaxHeight)
                {
                    MedBagImgWidth *= 0.9;
                    MedBagImgHeight *= 0.9;
                }
            }
            else
            {
                MedBagImgHeight = 835;
                MedBagImgWidth = 835 * (SelectedMedBag.MedBagImage.Width / SelectedMedBag.MedBagImage.Height);
                while (MedBagImgWidth > MedBagImg.MaxWidth)
                {
                    MedBagImgWidth *= 0.9;
                    MedBagImgHeight *= 0.9;
                }
            }
        }

        private void NewLocationClick(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            var c = sender as CheckBox;
            if (c != null && c.IsChecked == true)
            {
                var locationName = ((CheckBox) sender).Content.ToString();
                var controlName = ((CheckBox) sender).Name;
                Instance.NewLocation(null, locationName, controlName);
            }
            else
            {
                MedBagCanvas.Children.Remove(MedBagCanvas.Children.OfType<ContentControl>()
                    .Where(r => r.Content is RdlLocationControl)
                    .Single(r => c != null && ((RdlLocationControl) r.Content).LabelContent.Equals(c.Content)));
                SelectedMedBag.MedLocations.Remove(SelectedMedBag.MedLocations.Where(l => l != null)
                    .Single(l => c != null && l.Content.Equals(c.Content)));
            }
        }

        private void NewLocation(string locid = null, string content = null, string controlName = null,
            double height = 0, double width = 0, double top = 0, double left = 0)
        {
            var contentControl = new ContentControl();
            RdlLocationControl newLocation;
            if (locid != null)
            {
                newLocation = new RdlLocationControl(Convert.ToInt32(locid), content, controlName);
                _id = Convert.ToInt32(locid);
                _id++;
            }
            else
            {
                newLocation = new RdlLocationControl(_id, content, controlName);
                _id++;
            }

            if (string.IsNullOrEmpty(content))
            {
                contentControl.Template = (ControlTemplate) FindResource("MedBagRangeItemTemplate");
                contentControl.Height = Math.Abs(height) < 0.1 ? 150 : height;
                contentControl.Width = Math.Abs(width) < 0.1 ? 150 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, Math.Abs(top) < 0.1 ? 360 : top);
                Canvas.SetLeft(contentControl, Math.Abs(left) < 0.1 ? 648 : left);
            }
            else
            {
                contentControl.Template = (ControlTemplate) FindResource("MedBagDesignerItemTemplate");
                contentControl.Height = Math.Abs(height) < 0.1 ? 15 : height;
                contentControl.Width = Math.Abs(width) < 0.1 ? 60 : width;
                contentControl.Content = newLocation;
                MedBagCanvas.Children.Add(contentControl);
                Canvas.SetTop(contentControl, Math.Abs(top) < 0.1 ? 360 : top);
                Canvas.SetLeft(contentControl, Math.Abs(left) < 0.1 ? 648 : left);
            }
        }

        private void SetLocation(MedBagLocation m)
        {
            if (!string.IsNullOrEmpty(m.Content))
                NewFunction.FindChild<CheckBox>(CheckBoxStack, m.Name).IsChecked = true;
            var contentControl = CreateLocationContentControl(m);
            MedBagCanvas.Children.Add(contentControl);
            Canvas.SetTop(contentControl, m.CanvasTop);
            Canvas.SetLeft(contentControl, m.CanvasLeft);
        }

        private ContentControl CreateLocationContentControl(MedBagLocation m)
        {
            var contentControl = new ContentControl { Template = (ControlTemplate)FindResource("MedBagDesignerItemTemplate") };
            var newLocation = new RdlLocationControl(m.Id, m.Content, m.Name);
            contentControl.Height = m.Height;
            contentControl.Width = m.Width;
            contentControl.Content = newLocation;
            return contentControl;
        }

        public void SaveMedBagData()
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.Mode = SelectedMode;
            SelectedMedBag.SetLocationCollection(MedBagCanvas.Children);
            MedBagDb.SaveMedBagData(SelectedMedBag);
        }

        private void MedBagSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (MedBagCollection[MedBags.SelectedIndex] == null)
                return;
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var loadingWindow = new LoadingWindow();
            loadingWindow.SetMedBagData(this);
            loadingWindow.Show();
            
            SaveMedBagData();
            var m = new MessageWindow("藥袋儲存成功", MessageType.SUCCESS,false);
            m.Show();
        }

        //新增/刪除藥袋
        private void NewMedBagButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button b)) return;
            if (b.Name.Contains("Add"))
            {
                MedBagCollection.Add(new MedBag(SelectedMode));
                MedBags.SelectedItem = MedBagCollection[MedBagCollection.Count - 1];
            }
            else
            {
                if (CheckMedBagCollectionEmpty())
                    return;
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("確認刪除藥袋 " + SelectedMedBag.Name, "刪除確認", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    MedBagDb.DeleteMedBagData(SelectedMedBag);
                    var i = MedBags.SelectedIndex;
                    if (i > 0)
                        MedBags.SelectedItem = MedBagCollection[i - 1];
                    MedBagCollection.Remove(MedBagCollection[i]);
                }
            }
        }

        //設定預設藥袋
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.Default = true;
        }

        //取消預設藥袋
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CheckMedBagCollectionEmpty())
                return;
            SelectedMedBag.Default = false;
        }

        private void MedBags_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ClearMedBagCanvasUiElement();
            SelectedMedBag = ((DataGrid) sender).SelectedItem as MedBag;
            if (SelectedMedBag == null) return;
            var idExist = int.TryParse(SelectedMedBag.Id, out _);
            if (idExist)
            {
                if (SelectedMedBag.MedBagImage != null)
                    SetMedBagRange();
                if (SelectedMedBag.MedLocations.Count > 0)
                    foreach (var location in SelectedMedBag.MedLocations)
                        SetLocation(location);
            }
            else
            {
                SelectedMedBag.Id = (_maxMedBagId + 1).ToString();
            }
        }

        private void CleanButtonClick(object sender, RoutedEventArgs e)
        {
            ClearMedBagCanvasUiElement();
            SelectedMedBag = new MedBag(SelectedMode);
            MedBagCollection[MedBags.SelectedIndex] = SelectedMedBag;
        }

        private bool CheckMedBagCollectionEmpty()
        {
            var m = new MessageWindow("未新增藥袋", MessageType.WARNING, true);
            if (MedBagCollection.Count == 0)
            {
                m.Show();
                return true;
            }
            return false;
        }

        
        

        private void ModeRadioChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton r)
                SelectedMode = (MedBagMode)Convert.ToInt32(r.Tag.ToString());

            MedBags.Items.Filter = m => ((MedBag) m).Mode == SelectedMode;
            if (MedBags.Items.Count > 0)
            {
                MedBags.SelectedIndex = 0;
            }
            else
            {
               SelectedMedBag = new MedBag(SelectedMode);
            }
        }

        private void ClearMedBagCanvasUiElement()
        {
            MedRangeLocationControl.Visibility = Visibility.Hidden;
            if (MedBagCanvas.Children.Count == 1) return;
            foreach (UIElement child in CheckBoxStack.Children)
            {
                if (child is CheckBox box)
                    box.IsChecked = false;
            }
            for (var i = 1; i < MedBagCanvas.Children.Count;)
            {
                MedBagCanvas.Children.Remove(MedBagCanvas.Children[i]);
            }
        }

        private void SetMedicineCheckBoxVisibility()
        {
            if (SelectedMode == MedBagMode.SINGLE)
            {
                foreach (UIElement child in CheckBoxStack.Children)
                {
                    if (child is CheckBox box)
                    {
                        box.Visibility = !box.Name.Equals("MedicineList") ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            else
            {
                foreach (UIElement child in CheckBoxStack.Children)
                {
                    if (!(child is CheckBox box)) continue;
                    switch (box.Name)
                    {
                        case "MedicineList":
                            box.Visibility = Visibility.Visible;
                            break;
                        case "MedicineId":
                        case "EngName":
                        case "ChnName":
                        case "Ingredient":
                        case "Form":
                        case "Usage":
                        case "Dosage":
                        case "Total":
                        case "Days":
                        case "Indication":
                        case "SideEffect":
                        case "Notes":
                            box.Visibility = Visibility.Collapsed;
                            break;
                    }
                }
            }
        }
    }
}