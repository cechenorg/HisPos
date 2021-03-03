using His_Pos.Service.ExportService;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.SampleExcelTemplate
{
    public class DefaultDataTableTemplate : ExportExcelTemplate
    {
        public override string GetSheetName()
        {
            return "報表";
        }

        protected override void CreateExcelSettings()
        {
            DataTable table = (DataTable)Source;
            Settings.Add(new ExportDataTableExcelSetting(table, 1, 1));
        }
    }
}