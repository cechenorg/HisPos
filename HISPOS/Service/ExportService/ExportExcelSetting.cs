using OfficeOpenXml;

namespace His_Pos.Service.ExportService
{
    public abstract class ExportExcelSetting
    {
        #region ----- Define Variables -----

        public string Data { get; set; } = "";
        public int Row { get; set; } = 1;
        public int Column { get; set; } = 1;

        #endregion ----- Define Variables -----

        protected ExportExcelSetting(string data, int row, int column)
        {
            Data = data;
            Row = row;
            Column = column;
        }

        #region ----- Define Functions -----

        public abstract void InsertDataToSheet(ExcelWorksheet worksheet);

        #endregion ----- Define Functions -----
    }
}