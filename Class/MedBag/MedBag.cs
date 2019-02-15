using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage;
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
            MedLocations = null;/// MedBagLocationDb.ObservableGetLocationData(Id);
            Mode = dataRow["MEDBAG_MODE"].ToString() == "False" ? MedBagMode.SINGLE : MedBagMode.MULTI;
            Default = dataRow["MEDBAG_DEFAULT"].ToString() == "1";
        }

        public MedBag(MedBagMode m)
        {
            MedLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            Id = string.Empty;
            Name = string.Empty;
            BagWidth = 0.0;
            BagHeight = 0.0;
            MedBagImage = null;
            Mode = m;
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
                var convert = BagWidth / medBagImage.Width;
                if (!string.IsNullOrEmpty(rdlLocation.LabelContent))
                {
                    MedBagLocation.MedBagLocation medLoc = new MedBagLocation.MedBagLocation(rdlLocation, convert);
                    if (!CheckExistLocation(medLoc))
                        MedLocations.Add(medLoc);
                    else
                    {
                        int i = 0;
                        int existIndex = 0;
                        foreach (var m in MedLocations)
                        {
                            if (m.Content.Equals(medLoc.Content))
                                existIndex = i;
                            i++;
                        }
                        MedLocations[existIndex] = medLoc;
                    }
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
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        //檢查MedBagLocation存在
        private bool CheckExistLocation(MedBagLocation.MedBagLocation m)
        {
            foreach (MedBagLocation.MedBagLocation location in MedLocations)
            {
                if (location.Content.Equals(m.Content))
                    return true;
            }
            return false;
        }
    }
}