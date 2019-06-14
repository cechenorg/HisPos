using System;
using System.Collections.ObjectModel;
using System.IO;
using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public class ExportExcelService
    {
        #region ----- Define Variables -----
        private Collection<object> DataSource { get; }
        private ExportExcelTemplate Template { get; }
        #endregion

        public ExportExcelService(Collection<object> dataSource, ExportExcelTemplate template)
        {
            DataSource = dataSource;
            Template = template;
        }

        #region ----- Define Functions -----
        public bool Export(string exportPath)
        {
            try
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    foreach (var data in DataSource)
                    {
                        Template.Source = data;
                        excel.Workbook.Worksheets.Add(Template.GetSheetName());

                        var worksheet = excel.Workbook.Worksheets[Template.GetSheetName()];
                        Template.SetSheetData(worksheet);
                    }

                    FileInfo excelFile = new FileInfo(exportPath);
                    excel.SaveAs(excelFile);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
        #endregion
    }
}
