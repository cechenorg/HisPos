using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public abstract class ExportExcelTemplate : ObservableObject
    {
        #region ----- Define Variables -----
        private object source;

        public object Source
        {
            get => source;
            set
            {
                Set(() => Source, ref source, value);

                Settings.Clear();
                CreateExcelSettings();
            }
        }
        public ExportExcelSettings Settings { get; set; } = new ExportExcelSettings();
        #endregion

        #region ----- Define Functions -----
        public abstract string GetSheetName();
        protected abstract void CreateExcelSettings();

        public void SetSheetData(ExcelWorksheet worksheet)
        {
            foreach (var setting in Settings)
                setting.InsertDataToSheet(worksheet);
        }
        #endregion
    }
}
