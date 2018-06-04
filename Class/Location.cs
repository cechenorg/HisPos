using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Location
    {
        public string id;
        public string name;
        public double pathX;
        public double pathY;
        public double width;
        public double height;
        public ObservableCollection<LocationDetail> locationDetailCollection = new ObservableCollection<LocationDetail>();
    }
}
