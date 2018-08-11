using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using His_Pos.Class.MedBagLocation;
using His_Pos.H1_DECLARE.MedBagManage;
using JetBrains.Annotations;

namespace His_Pos.Class.MedBag
{
    public class MedBag : INotifyPropertyChanged
    {
        public MedBag(BitmapImage image)
        {
            MedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            MedBagImage = image;
        }

        public MedBag(DataRow dataRow)
        {
            Id = dataRow["MEDBAG_ID"].ToString();
            Name = dataRow["MEDBAG_NAME"].ToString();
            BagWidth = double.Parse(dataRow["MEDBAG_SIZEX"].ToString());
            BagHeight = double.Parse(dataRow["MEDBAG_SIZEY"].ToString());
            MedBagImage = ToImage((byte[])dataRow["MEDBAG_IMAGE"]);
            MedLocations = MedBagDb.ObservableGetLocationData(Id);
            Mode = dataRow["MEDBAG_MODE"].ToString() == "0" ? MedBagMode.SINGLE : MedBagMode.MULTI;
            Default = dataRow["MEDBAG_DEFAULT"].ToString() != "0";
        }

        public MedBag(MedBagMode mode)
        {
            MedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            Id = string.Empty;
            Name = string.Empty;
            BagWidth = 0.0;
            BagHeight = 0.0;
            MedBagImage = null;
            Mode = mode;
        }

        public ObservableCollection<MedBagLocation.MedBagLocation> MedLocations { get; set; }
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
                    MedLocations.Add(new MedBagLocation.MedBagLocation(rdlLocation.id, rdlLocation.LabelName, pathX, pathY, locationWidth, locationHeight, actualWidth, actualHeight));
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
                    MedLocations.Add(new MedBagLocation.MedBagLocation(rdlLocation.id, rdlLocation.LabelName, pathX, pathY, locationWidth, locationHeight, actualWidth, actualHeight));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}