using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Service.ExportService
{
    public class ExportSpecialExcelSetting : ExportExcelSetting
    {
        #region ----- Define Variables -----
        public int FontSize { get; set; } = 12;
        public Color FontColor { get; set; } = Color.Black;
        public Color BackGroundColor { get; set; } = Color.Transparent;
        public bool IsBold { get; set; } = false;
        #endregion

        public ExportSpecialExcelSetting(string data, int row, int column, Color backGroundColor, Color fontColor, bool isBold = false, int fontSize = 12) : base(data, row, column)
        {
            Data = data;
            Row = row;
            Column = column;
            FontSize = fontSize;
            FontColor = fontColor;
            BackGroundColor = backGroundColor;
            IsBold = isBold;
        }

        #region ----- Define Functions -----
        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            worksheet.Cells[Row, Column].Style.Font.Bold = IsBold;
            worksheet.Cells[Row, Column].Style.Font.Size = FontSize;
            worksheet.Cells[Row, Column].Style.Font.Color.SetColor(FontColor);

            if (BackGroundColor != Color.Transparent)
            {
                worksheet.Cells[Row, Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[Row, Column].Style.Fill.BackgroundColor.SetColor(BackGroundColor);
            }

            worksheet.Cells[Row, Column].Value = Data;
        }
        #endregion
    }
}
