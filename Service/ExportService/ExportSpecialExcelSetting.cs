using OfficeOpenXml;
using System.Drawing;

namespace His_Pos.Service.ExportService
{
    public class ExportSpecialExcelSetting : ExportExcelSetting
    {
        #region ----- Define Variables -----

        private int fontSize = 12;
        private Color fontColor = Color.Black;
        private Color backGroundColor = Color.Transparent;
        private bool isBold = false;

        #endregion ----- Define Variables -----

        public ExportSpecialExcelSetting(string data, int row, int column) : base(data, row, column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Fluent Interface Functions -----

        public ExportSpecialExcelSetting SetFontSize(int _fontSize)
        {
            fontSize = _fontSize;
            return this;
        }

        public ExportSpecialExcelSetting SetFontColor(Color _fontColor)
        {
            fontColor = _fontColor;
            return this;
        }

        public ExportSpecialExcelSetting SetBackGroundColor(Color _backGroundColor)
        {
            backGroundColor = _backGroundColor;
            return this;
        }

        public ExportSpecialExcelSetting SetIsBold(bool _isBold)
        {
            isBold = _isBold;
            return this;
        }

        #endregion ----- Define Fluent Interface Functions -----

        #region ----- Define Functions -----

        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            worksheet.Cells[Row, Column].Style.Font.Bold = isBold;
            worksheet.Cells[Row, Column].Style.Font.Size = fontSize;
            worksheet.Cells[Row, Column].Style.Font.Color.SetColor(fontColor);

            if (backGroundColor != Color.Transparent)
            {
                worksheet.Cells[Row, Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[Row, Column].Style.Fill.BackgroundColor.SetColor(backGroundColor);
            }

            worksheet.Cells[Row, Column].Value = Data;
        }

        #endregion ----- Define Functions -----
    }
}