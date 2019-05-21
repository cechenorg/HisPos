using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Service.ExportService
{
    class ExportDataTableExcelSetting : ExportExcelSetting
    {
        #region ----- Define Variables -----
        public new DataTable Data { get; set; }

        #region ///// Header Variables /////
        public int HeaderFontSize { get; set; } = 12;
        public Color HeaderFontColor { get; set; } = Color.Black;
        public Color HeaderBackGroundColor { get; set; } = Color.LightSkyBlue;
        public bool HeaderIsBold { get; set; } = true;
        #endregion

        #region ///// Data Variables /////
        public int DataFontSize { get; set; } = 12;
        public Color DataFontColor { get; set; } = Color.Black;
        public Color DataBackGroundColor { get; set; } = Color.Transparent;
        public bool DataIsBold { get; set; } = false;
        #endregion

        #endregion

        public ExportDataTableExcelSetting(DataTable data, int row, int column) : base("", row, column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Functions -----
        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            for (int col = 0; col < Data.Columns.Count; col++)
            {
                worksheet.Cells[Row, Column + col].Style.Font.Bold = HeaderIsBold;
                worksheet.Cells[Row, Column + col].Style.Font.Size = HeaderFontSize;
                worksheet.Cells[Row, Column + col].Style.Font.Color.SetColor(HeaderFontColor);
                worksheet.Cells[Row, Column + col].Style.Fill.BackgroundColor.SetColor(HeaderBackGroundColor);

                worksheet.Cells[Row, Column + col].Value = Data.Columns[col].ColumnName;
            }

            for(int row = 0; row < Data.Rows.Count; row++)
            {
                for (int col = 0; col < Data.Columns.Count; col++)
                {
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Bold = DataIsBold;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Size = DataFontSize;
                    worksheet.Cells[Row + row + 1, Column + col].Style.Font.Color.SetColor(DataFontColor);
                    worksheet.Cells[Row + row + 1, Column + col].Style.Fill.BackgroundColor.SetColor(DataBackGroundColor);

                    worksheet.Cells[Row + row + 1, Column + col].Value = Data.Rows[row][col].ToString();
                }
            }
        }
        #endregion
    }
}
