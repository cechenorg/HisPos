using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public class ExportExcelTemplate
    {
        #region ----- Define Variables -----
        public object Source { get; set; }
        public ExportExcelSettings Settings { get; set; }
        #endregion

        #region ----- Define Functions -----
        public string GetSheetName()
        {
            throw new NotImplementedException();
        }

        internal void SetSheetData(ExcelWorksheet worksheet)
        {
            foreach (var setting in Settings)
                setting.InsertDataToSheet(worksheet);
        }
        #endregion
    }
}
