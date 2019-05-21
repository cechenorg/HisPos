using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace His_Pos.Service.ExportService
{
    public abstract class ExportExcelSetting
    {
        #region ----- Define Variables -----
        public string Data { get; set; } = "";
        public int Row { get; set; } = 1;
        public int Column { get; set; } = 1;
        #endregion

        protected ExportExcelSetting(string data, int row, int column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Functions -----
        public abstract void InsertDataToSheet(ExcelWorksheet worksheet);
        #endregion
    }
}
