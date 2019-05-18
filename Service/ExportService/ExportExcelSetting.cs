using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public class ExportExcelSetting
    {
        #region ----- Define Variables -----
        public int Row { get; set; }
        public int Column { get; set; }
        public int FontSize { get; set; }
        public int FontColor { get; set; }
        public int BackGroundColor { get; set; }
        public bool IsBold { get; set; }
        #endregion

        #region ----- Define Functions -----
        internal void InsertDataToSheet(ExcelWorksheet worksheet)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
