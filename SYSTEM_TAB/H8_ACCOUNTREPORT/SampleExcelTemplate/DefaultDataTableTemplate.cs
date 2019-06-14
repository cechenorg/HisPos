using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.SampleExcelTemplate {
    public class DefaultDataTableTemplate : ExportExcelTemplate {
        public override string GetSheetName() {
            return "報表";
        }

        protected override void CreateExcelSettings() {
            DataTable table = (DataTable)Source;
            Settings.Add(new ExportDataTableExcelSetting(table, 1, 1));
        }
    }
}
