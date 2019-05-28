using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Service.ExportService
{
    class ExportNormalExcelSetting : ExportExcelSetting
    {
        public ExportNormalExcelSetting(string data, int row, int column) : base(data, row, column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Functions -----
        public override void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            worksheet.Cells[Row, Column].Value = Data;
        }
        #endregion
    }
}
