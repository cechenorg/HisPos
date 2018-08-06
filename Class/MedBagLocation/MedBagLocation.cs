using System;
using System.Data;

namespace His_Pos.Class.MedBagLocation
{
    public class MedBagLocation
    {
        public MedBagLocation(DataRow row)
        {
            Id = row["MEDBAG_ID"].ToString();
            Name = row["MEDBAG_NAME"].ToString();
            PathX = Convert.ToDouble(row["MEDBAG_X"].ToString());
            PathY = Convert.ToDouble(row["MEDBAG_Y"].ToString());
            Width = Convert.ToDouble(row["MEDBAG_WIDTH"].ToString());
            Height = Convert.ToDouble(row["MEDBAG_HEIGHT"].ToString());
            ActualWidth = Convert.ToDouble(row["MEDBAG_ACTUALWIDTH"].ToString());
            ActualHeight = Convert.ToDouble(row["MEDBAG_ACTUALHEIGHT"].ToString());
        }
        public MedBagLocation(int id, string name, double pathX, double pathY, double width, double height, double actualWidth, double actualHeight)
        {
            Id = id.ToString();
            Name = name;
            PathX = pathX;
            PathY = pathY;
            Width = width;
            Height = height;
            ActualWidth = actualWidth;
            ActualHeight = actualHeight;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public double PathX { get; set; }
        public double PathY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double ActualWidth { get; set; }
        public double ActualHeight { get; set; }

    }
}
