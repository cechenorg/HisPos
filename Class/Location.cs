using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class Location
    {
      public  Location(DataRow row) {
            id = row["LOC_ID"].ToString();
            name = row["LOC_NAME"].ToString();
            pathX = Convert.ToDouble(row["LOC_X"].ToString());
            pathY = Convert.ToDouble(row["LOC_Y"].ToString());
            width = Convert.ToDouble(row["LOC_WIDTH"].ToString());
            heigh = Convert.ToDouble(row["LOC_HEIGHT"].ToString());
        }
        public string id;
        public string name;
        public double pathX;
        public double pathY;
        public double width;
        public double heigh;
        public ObservableCollection<LocationDetail> locationDetailCollection = new ObservableCollection<LocationDetail>();
    }
}
