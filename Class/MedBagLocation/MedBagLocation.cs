using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
        public MedBagLocation(int id, string name, double pathX, double pathY, double width, double height,string content)
        {
            Id = id.ToString();
            Name = name;
            PathX = pathX;
            PathY = pathY;
            Width = width;
            Height = height;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public double PathX { get; set; }
        public double PathY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Content { get; set; }
    }
}
