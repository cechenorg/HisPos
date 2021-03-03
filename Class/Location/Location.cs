using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.Class.Location
{
    public class Location
    {
        public Location(DataRow row)
        {
            id = row["LOC_ID"].ToString();
            name = row["LOC_NAME"].ToString();
            pathX = Convert.ToDouble(row["LOC_X"].ToString());
            pathY = Convert.ToDouble(row["LOC_Y"].ToString());
            width = Convert.ToDouble(row["LOC_WIDTH"].ToString());
            heigh = Convert.ToDouble(row["LOC_HEIGHT"].ToString());
        }

        public Location(int locId, string locName, double locpathX, double locpathY, double locWidth, double locHeigh)
        {
            id = locId.ToString();
            name = locName;
            pathX = locpathX;
            pathY = locpathY;
            width = locWidth;
            heigh = locHeigh;
        }

        public string id { get; }
        public string name { get; }
        public double pathX { get; }
        public double pathY { get; }
        public double width { get; }
        public double heigh { get; }
        public ObservableCollection<LocationDetail> locationDetailCollection = new ObservableCollection<LocationDetail>();
    }
}