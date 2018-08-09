using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using His_Pos.Class.MedBagLocation;
using His_Pos.H1_DECLARE.MedBagManage;
using JetBrains.Annotations;

namespace His_Pos.Class.MedBag
{
    public class MedBag : INotifyPropertyChanged
    {

        public MedBag(BitmapImage image,MedBagMode mode)
        {
            SingleMedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            MultiMedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            MedBagImage = image;
            Mode = mode;
        }

        public MedBag(DataRow dataRow)
        {
            SingleMedLocations = MedBagDb.ObservableGetLocationData();
            MultiMedLocations = MedBagDb.ObservableGetLocationData();
        }

        public MedBag(MedBagMode mode)
        {
            SingleMedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            MultiMedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            Id = string.Empty;
            Name = string.Empty;
            BagWidth = 0.0;
            BagHeight = 0.0;
            MedBagImage = null;
            Mode = mode;
        }

        public ObservableCollection<MedBagLocation.MedBagLocation> SingleMedLocations { get; set; }
        public ObservableCollection<MedBagLocation.MedBagLocation> MultiMedLocations { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        private double _bagWidth;

        public double BagWidth
        {
            get => _bagWidth;
            set
            {
                _bagWidth = value;
                OnPropertyChanged(nameof(BagWidth));
            }
        }

        private double _bagHeight;

        public double BagHeight
        {
            get => _bagHeight;
            set
            {
                _bagHeight = value;
                OnPropertyChanged(nameof(BagHeight));
            }
        }

        private BitmapImage _medBagImage;

        public BitmapImage MedBagImage
        {
            get => _medBagImage;
            set
            {
                _medBagImage = value;
                OnPropertyChanged(nameof(MedBagImage));
            }
        }

        private MedBagMode _mode;

        public MedBagMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        private bool _default;

        public bool Default
        {
            get => _default;
            set
            {
                _default = value;
                OnPropertyChanged(nameof(Default));
            }
        }

        public void SetLocationCollection(UIElementCollection locationCollection)
        {
            foreach (var contentControl in locationCollection.OfType<ContentControl>().Where(r => r.Content is RdlLocationControl))
            {
                var rdlLocation = (RdlLocationControl)contentControl.Content;
                var medBagImage = locationCollection.OfType<RdlLocationControl>().Where(r => r.Content is Grid).Single(r =>
                    string.IsNullOrEmpty(r.LabelContent));
                var width = medBagImage.Width;
                var convert = BagWidth / width;
                if (!string.IsNullOrEmpty(rdlLocation.LabelContent))
                {
                    var pathX = convert * (double)rdlLocation.Parent.GetValue(Canvas.LeftProperty);
                    var pathY = convert * (double)rdlLocation.Parent.GetValue(Canvas.TopProperty);
                    var locationWidth = rdlLocation.ActualWidth;
                    var locationHeight = rdlLocation.ActualHeight;
                    var actualWidth = locationWidth * convert;
                    var actualHeight = locationHeight * convert;
                    switch (Mode)
                    {
                        case MedBagMode.SINGLE:
                            SingleMedLocations.Add(new MedBagLocation.MedBagLocation(rdlLocation.id, rdlLocation.LabelName, pathX, pathY, locationWidth, locationHeight, actualWidth, actualHeight));
                            break;
                        case MedBagMode.MULTI:
                            MultiMedLocations.Add(new MedBagLocation.MedBagLocation(rdlLocation.id, rdlLocation.LabelName, pathX, pathY, locationWidth, locationHeight, actualWidth, actualHeight));
                            break;
                    }
                    
                }
            }
        }

        public void DeleteLocation(UIElementCollection locationCollection, string controlName)
        {
            foreach (var contentControl in locationCollection.OfType<ContentControl>().Where(r => r.Content is RdlLocationControl))
            {
                var rdlLocation = (RdlLocationControl)contentControl.Content;
                var medBagImage = locationCollection.OfType<RdlLocationControl>().Where(r => r.Content is Grid).Single(r =>
                    string.IsNullOrEmpty(r.LabelContent));
                var width = medBagImage.Width;
                var convert = BagWidth / width;
                if (!string.IsNullOrEmpty(rdlLocation.LabelContent))
                {
                    var pathX = convert * (double)rdlLocation.Parent.GetValue(Canvas.LeftProperty);
                    var pathY = convert * (double)rdlLocation.Parent.GetValue(Canvas.TopProperty);
                    var locationWidth = rdlLocation.ActualWidth;
                    var locationHeight = rdlLocation.ActualHeight;
                    var actualWidth = locationWidth * convert;
                    var actualHeight = locationHeight * convert;
                    SingleMedLocations.Add(new MedBagLocation.MedBagLocation(rdlLocation.id, rdlLocation.LabelName, pathX, pathY, locationWidth, locationHeight, actualWidth, actualHeight));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}