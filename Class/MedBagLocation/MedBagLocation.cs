using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public MedBagLocation(int id, string name, double pathX, double pathY, double width, double height)
        {
            Id = id.ToString();
            Name = name;
            PathX = pathX;
            PathY = pathY;
            Width = width;
            Height = height;
        }
        public string Id { get; }
        public string Name { get; }
        public double PathX { get; }
        public double PathY { get; }
        public double Width { get; }
        public double Height { get; }
    }
}
