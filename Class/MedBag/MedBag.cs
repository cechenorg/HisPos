using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        private double rangeTop;

        public double RangeTop
        {
            get { return rangeTop; }
            set
            {
                rangeTop = value;
                OnPropertyChanged("RangeTop");
            }
        }

        private double rangeLeft;

        public double RangeLeft
        {
            get { return rangeLeft; }
            set
            {
                rangeTop = value;
                OnPropertyChanged("RangeLeft");
            }
        }

        private double rangeBottom;

        public double RangeBottom
        {
            get { return rangeBottom; }
            set
            {
                rangeTop = value;
                OnPropertyChanged("RangeBottom");
            }
        }

        private double rangeRight;

        public double RangeRight
        {
            get { return rangeRight; }
            set
            {
                rangeTop = value;
                OnPropertyChanged("RangeRight");
            }
        }

        private double bagWidth;

        public double BagWidth
        {
            get { return bagWidth; }
            set
            {
                bagWidth = value;
                OnPropertyChanged("BagWidth");
            }
        }

        private double bagHeight;

        public double BagHeight
        {
            get { return bagHeight; }
            set
            {
                bagWidth = value;
                OnPropertyChanged("BagHeight");
            }
        }

        private BitmapImage medBagImage;

        public BitmapImage MedBagImage
        {
            get { return medBagImage; }
            set
            {
                medBagImage = value;
                OnPropertyChanged("MedBagImage");
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