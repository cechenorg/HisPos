using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public class ExportExcelService
    {
        #region ----- Define Variables -----
        public ICollection<object> DataSource { get; set; }
        public ExportExcelTemplate Template { get; set; }
        #endregion

        #region ----- Define Functions -----
        public void Export(string exportPath)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                foreach(var data in DataSource)
                {
                    Template.Source = data;
                    excel.Workbook.Worksheets.Add(Template.GetSheetName());

                    var worksheet = excel.Workbook.Worksheets[Template.GetSheetName()];
                    Template.SetSheetData(worksheet);
                }

                FileInfo excelFile = new FileInfo(exportPath);
                excel.SaveAs(excelFile);
            }
        }
        #endregion
    }
}
