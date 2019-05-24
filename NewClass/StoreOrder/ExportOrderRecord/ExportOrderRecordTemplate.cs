using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.NewClass.StoreOrder.ExportOrderRecord
{
    public class ExportOrderRecordTemplate : ExportExcelTemplate
    {
        protected override void CreateExcelSettings()
        {
            if((Source as StoreOrder).OrderStatus == OrderStatusEnum.SCRAP)
                Settings.Add(new ExportSpecialExcelSetting("已作廢", 1, 1, Color.Transparent, Color.Red, true, 15));

            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).OrderType == OrderTypeEnum.PURCHASE ? "進貨" : "退貨", 2, 2, Color.Transparent, (Source as StoreOrder).OrderType == OrderTypeEnum.PURCHASE ? Color.Green : Color.Red, true, 15));
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).ReceiveID, 2, 3, Color.Transparent, Color.DarkBlue, true, 15));
            Settings.Add(new ExportNormalExcelSetting((Source as StoreOrder).OrderManufactory.GetName, 2, 4));
            Settings.Add(new ExportNormalExcelSetting("收貨員工:", 3, 2));
            Settings.Add(new ExportNormalExcelSetting((Source as StoreOrder).ReceiveEmployeeName, 3, 3));
            Settings.Add(new ExportNormalExcelSetting("收貨時間:", 3, 4));
            Settings.Add(new ExportNormalExcelSetting((Source as StoreOrder).DoneDateTime?.ToString("yyyy/MM/dd hh:mm:ss"), 3, 5));

            DataTable products = PurchaseReturnProductDB.GetProductsByStoreOrderIDForExport((Source as StoreOrder).ID);
            Settings.Add(new ExportDataTableExcelSetting(products, 4, 2));

            int productRows = products.Rows.Count + 1;
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).TotalPrice.ToString("####"), productRows + 4, 7, Color.Transparent, Color.SteelBlue, true, 15));
        }

        public override string GetSheetName()
        {
            return (Source as StoreOrder).ReceiveID;
        }
    }
}
