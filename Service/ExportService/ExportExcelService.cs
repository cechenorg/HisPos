using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace His_Pos.Service.ExportService
{
    public class ExportExcelService
    {
        #region ----- Define Variables -----

        private Collection<object> DataSource { get; }
        private ExportExcelTemplate Template { get; }
        private string ExcelBasePath { get; }

        #endregion ----- Define Variables -----

        public ExportExcelService(Collection<object> dataSource, ExportExcelTemplate template, string excelBasePath = "")
        {
            DataSource = dataSource;
            Template = template;
            ExcelBasePath = excelBasePath;
        }

        #region ----- Define Functions -----

        public bool Export(string exportPath)
        {
            try
            {
                ExcelPackage excel;
                FileInfo excelFile = new FileInfo(exportPath);

                if (String.IsNullOrEmpty(ExcelBasePath))
                    excel = new ExcelPackage(excelFile);
                else
                {
                    FileInfo excelBaseFile = new FileInfo(ExcelBasePath);
                    excel = new ExcelPackage(excelFile, excelBaseFile);
                }

                foreach (var data in DataSource)
                {
                    Template.Source = data;

                    if (String.IsNullOrEmpty(ExcelBasePath))
                        excel.Workbook.Worksheets.Add(Template.GetSheetName());

                    var worksheet = excel.Workbook.Worksheets[Template.GetSheetName()];
                    Template.SetSheetData(worksheet);
                }

                excel.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion ----- Define Functions -----
    }
}