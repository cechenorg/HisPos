using System;
using System.Data;
using System.Windows.Controls;
using His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage;

namespace His_Pos.Class.MedBagLocation
{
    public class MedBagLocation
    {
        public MedBagLocation(DataRow row)
        {
            Id = int.Parse(row["MEDBAG_ID"].ToString());
            Name = row["MEDBAG_LOCNAME"].ToString();
            PathX = Convert.ToDouble(row["MEDBAG_X"].ToString());
            PathY = Convert.ToDouble(row["MEDBAG_Y"].ToString());
            Width = Convert.ToDouble(row["MEDBAG_WIDTH"].ToString());
            Height = Convert.ToDouble(row["MEDBAG_HEIGHT"].ToString());
            RealWidth = Convert.ToDouble(row["MEDBAG_REALWIDTH"].ToString());
            RealHeight = Convert.ToDouble(row["MEDBAG_REALHEIGHT"].ToString());
            Content = row["MEDBAG_LOCCONTENT"].ToString().Trim();
            CanvasLeft = Convert.ToDouble(row["MEDBAG_CANVASLEFT"].ToString());
            CanvasTop = Convert.ToDouble(row["MEDBAG_CANVASTOP"].ToString());
        }
        public MedBagLocation(RdlLocationControl r,double convert)
        {
            Id = r.Id;
            Name = r.LabelName;
            Content = r.LabelContent;
            CanvasLeft = (double)r.Parent.GetValue(Canvas.LeftProperty);
            CanvasTop = (double)r.Parent.GetValue(Canvas.TopProperty);
            PathX = convert * CanvasLeft;
            PathY = convert * CanvasTop;
            Width = r.ActualWidth;
            Height = r.ActualHeight;
            RealWidth = convert * Width;
            RealHeight = convert * Height;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public double PathX { get; set; }
        public double PathY { get; set; }
        public double CanvasLeft { get; set; }
        public double CanvasTop { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RealWidth { get; set; }
        public double RealHeight { get; set; }

    }
}
