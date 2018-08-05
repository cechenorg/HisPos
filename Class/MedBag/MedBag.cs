using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace His_Pos.Class.MedBag
{
    public class MedBag : INotifyPropertyChanged
    {
        public MedBag(BitmapImage image)
        {
            RangeTop = 0;
            RangeBottom = 0;
            RangeLeft = 0;
            RangeRight = 0;
            MedBagLocations = new ObservableCollection<MedBagLocation.MedBagLocation>();
            MedBagImage = image;
        }

        public MedBag(DataRow dataRow)
        {
            RangeTop = double.Parse(dataRow["RANGE_TOP"].ToString());
            RangeBottom = double.Parse(dataRow["RANGE_BOTTOM"].ToString());
            RangeLeft = double.Parse(dataRow["RANGE_LEFT"].ToString());
            RangeRight = double.Parse(dataRow["RANGE_RIGHT"].ToString());
            MedBagLocations = MedBagDb.ObservableGetLocationData();
        }

        public ObservableCollection<MedBagLocation.MedBagLocation> MedBagLocations { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        private double _rangeTop;

        public double RangeTop
        {
            get => _rangeTop;
            set
            {
                _rangeTop = value;
                OnPropertyChanged(nameof(RangeTop));
            }
        }

        private double _rangeLeft;

        public double RangeLeft
        {
            get => _rangeLeft;
            set
            {
                _rangeTop = value;
                OnPropertyChanged(nameof(RangeLeft));
            }
        }

        private double _rangeBottom;

        public double RangeBottom
        {
            get => _rangeBottom;
            set
            {
                _rangeTop = value;
                OnPropertyChanged(nameof(RangeBottom));
            }
        }

        private double _rangeRight;

        public double RangeRight
        {
            get => _rangeRight;
            set
            {
                _rangeTop = value;
                OnPropertyChanged(nameof(RangeRight));
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}