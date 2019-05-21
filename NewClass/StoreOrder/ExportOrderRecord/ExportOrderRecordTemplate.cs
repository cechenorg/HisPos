using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder.ExportOrderRecord
{
    public class ExportOrderRecordTemplate : ExportExcelTemplate
    {
        protected override void CreateExcelSettings()
        {
            Settings.Add(new ExportNormalExcelSetting((Source as StoreOrder).ReceiveID, 1, 1));
            Settings.Add(new ExportNormalExcelSetting((Source as StoreOrder).DoneDateTime?.ToLongTimeString(), 2, 10));

            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).ReceiveID, 5, 1, Color.LightSkyBlue, Color.Pink));
        }

        public override string GetSheetName()
        {
            return (Source as StoreOrder).ReceiveID;
        }
    }
}
