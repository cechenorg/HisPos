using OfficeOpenXml;
using System.Data;
using System.Drawing;

namespace His_Pos.Service.ExportService
{
    internal class ExportDataTableExcelSetting : ExportExcelSetting
    {
        #region ----- Define Variables -----

        private new DataTable Data;

        #region ///// Header Variables /////

        private string[] headers = new string[0];
        private int headerFontSize = 12;
        private Color headerFontColor = Color.Black;
        private Color headerBackGroundColor = Color.LightBlue;
        private bool headerIsBold = true;
        private bool hasHeader = true;

        #endregion ///// Header Variables /////

        #region ///// Data Variables /////

        private int dataFontSize = 12;
        private Color dataFontColor = Color.Black;
        private Color dataBackGroundColor = Color.Transparent;
        private bool dataIsBold = false;

        #endregion ///// Data Variables /////

        #endregion ----- Define Variables -----

        public ExportDataTableExcelSetting(DataTable data, int row, int column) : base("", row, column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Fluent Interface Functions -----

        public ExportDataTableExcelSetting SetHasHeader(bool _hasHeader)
        {
            hasHeader = _hasHeader;
            return this;
        }

        public ExportDataTableExcelSetting SetHeaderFontSize(int _fontSize)
        {
            headerFontSize = _fontSize;
            return this;
        }

        public ExportDataTableExcelSetting SetHeaderFontColor(Color _fontColor)
        {
            headerFontColor = _fontColor;
            return this;
        }

        public ExportDataTableExcelSetting SetHeaderBackGroundColor(Color _backGroundColor)
        {
            headerBackGroundColor = _backGroundColor;
            return this;
        }

        public ExportDataTableExcelSetting SetHeaderIsBold(bool _isBold)
        {
            headerIsBold = _isBold;
            return this;
        }

        public ExportDataTableExcelSetting SetDataFontSize(int _fontSize)
        {
            dataFontSize = _fontSize;
            return this;
        }

        public ExportDataTableExcelSetting SetDataFontColor(Color _fontColor)
        {
            dataFontColor = _fontColor;
            return this;
        }

        public ExportDataTableExcelSetting SetDataBackGroundColor(Color _backGroundColor)
        {
            dataBackGroundColor = _backGroundColor;
            return this;
        }

        public ExportDataTableExcelSetting SetDataIsBold(bool _isBold)
        {
            dataIsBold = _isBold;
            return this;
        }

        public ExportDataTableExcelSetting SetHeaders(string[] _headers)
        {
            headers = _headers;
            return this;
        }

        #endregion ----- Define Fluent Interface Functions -----

        #region ----- Define Functions -----

        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            if (hasHeader)
                SetHeader(worksheet);
            else
                Row--;

            SetData(worksheet);
        }

        private void SetData(ExcelWorksheet worksheet)
        {
            for (int row = 0; row < Data.Rows.Count; row++)
            {
                for (int col = 0; col < Data.Columns.Count; col++)
                {
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Bold = dataIsBold;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Size = dataFontSize;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Color.SetColor(dataFontColor);

                    if (dataBackGroundColor != Color.Transparent)
                    {
                        worksheet.Cells[Row + row + 1, Column + col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[Row + row + 1, Column + col].Style.Fill.BackgroundColor.SetColor(dataBackGroundColor);
                    }

                    worksheet.Cells[Row + row + 1, Column + col].Value = Data.Rows[row][col];
                }
            }
        }

        private void SetHeader(ExcelWorksheet worksheet)
        {
            for (int col = 0; col < Data.Columns.Count; col++)
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
                    worksheet.Cells[Row, Column + col].Value = Data.Columns[col].ColumnName;
            }
        }

        #endregion ----- Define Functions -----
    }
}