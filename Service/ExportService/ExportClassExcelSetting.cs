using OfficeOpenXml;
using System.Collections;
using System.Drawing;

namespace His_Pos.Service.ExportService
{
    public class ExportClassExcelSetting : ExportExcelSetting
    {
        #region ----- Define Variables -----

        private new ICollection Data;

        #region ///// Header Variables /////

        private string[] headers = new string[0];
        private int headerFontSize = 12;
        private Color headerFontColor = Color.Black;
        private Color headerBackGroundColor = Color.LightBlue;
        private bool headerIsBold = true;

        #endregion ///// Header Variables /////

        #region ///// Data Variables /////

        private string[] selectedProperties = new string[0];
        private int dataFontSize = 12;
        private Color dataFontColor = Color.Black;
        private Color dataBackGroundColor = Color.Transparent;
        private bool dataIsBold = false;

        #endregion ///// Data Variables /////

        #endregion ----- Define Variables -----

        public ExportClassExcelSetting(ICollection data, int row, int column, string[] properties) : base("", row, column)
        {
            Data = data;
            Row = row;
            Column = column;
            selectedProperties = properties;
        }

        #region ----- Define Fluent Interface Functions -----

        public ExportClassExcelSetting SetHeaderFontSize(int _fontSize)
        {
            headerFontSize = _fontSize;
            return this;
        }

        public ExportClassExcelSetting SetHeaderFontColor(Color _fontColor)
        {
            headerFontColor = _fontColor;
            return this;
        }

        public ExportClassExcelSetting SetHeaderBackGroundColor(Color _backGroundColor)
        {
            headerBackGroundColor = _backGroundColor;
            return this;
        }

        public ExportClassExcelSetting SetHeaderIsBold(bool _isBold)
        {
            headerIsBold = _isBold;
            return this;
        }

        public ExportClassExcelSetting SetDataFontSize(int _fontSize)
        {
            dataFontSize = _fontSize;
            return this;
        }

        public ExportClassExcelSetting SetDataFontColor(Color _fontColor)
        {
            dataFontColor = _fontColor;
            return this;
        }

        public ExportClassExcelSetting SetDataBackGroundColor(Color _backGroundColor)
        {
            dataBackGroundColor = _backGroundColor;
            return this;
        }

        public ExportClassExcelSetting SetDataIsBold(bool _isBold)
        {
            dataIsBold = _isBold;
            return this;
        }

        public ExportClassExcelSetting SetHeaders(string[] _headers)
        {
            headers = _headers;
            return this;
        }

        #endregion ----- Define Fluent Interface Functions -----

        #region ----- Define Functions -----

        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            SetHeader(worksheet);
            SetData(worksheet);
        }

        private void SetData(ExcelWorksheet worksheet)
        {
            int row = 0;

            foreach (var d in Data)
            {
                for (int col = 0; col < selectedProperties.Length; col++)
                {
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Bold = dataIsBold;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Size = dataFontSize;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Color.SetColor(dataFontColor);

                    if (dataBackGroundColor != Color.Transparent)
                    {
                        worksheet.Cells[Row + row + 1, Column + col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[Row + row + 1, Column + col].Style.Fill.BackgroundColor.SetColor(dataBackGroundColor);
                    }

                    worksheet.Cells[Row + row + 1, Column + col].Value = d.GetType().GetProperty(selectedProperties[col]).GetValue(d, null);
                }

                row++;
            }
        }

        private void SetHeader(ExcelWorksheet worksheet)
        {
            for (int col = 0; col < selectedProperties.Length; col++)
            {
                worksheet.Cells[Row, Column + col].Style.Font.Bold = headerIsBold;
                worksheet.Cells[Row, Column + col].Style.Font.Size = headerFontSize;
                worksheet.Cells[Row, Column + col].Style.Font.Color.SetColor(headerFontColor);

                if (headerBackGroundColor != Color.Transparent)
                {
                    worksheet.Cells[Row, Column + col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[Row, Column + col].Style.Fill.BackgroundColor.SetColor(headerBackGroundColor);
                }

                if (headers.Length >= col + 1)
                    worksheet.Cells[Row, Column + col].Value = headers[col];
                else
                    worksheet.Cells[Row, Column + col].Value = selectedProperties[col];
            }
        }

        #endregion ----- Define Functions -----
    }
}