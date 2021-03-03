using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.Service.ExportService;
using System.Data;
using System.Drawing;

namespace His_Pos.NewClass.StoreOrder.ExportOrderRecord
{
    public class ExportOrderRecordTemplate : ExportExcelTemplate
    {
        protected override void CreateExcelSettings()
        {
            if ((Source as StoreOrder).OrderStatus == OrderStatusEnum.SCRAP)
                Settings.Add(new ExportSpecialExcelSetting("已作廢", 1, 1).SetFontColor(Color.Red).SetFontSize(15).SetIsBold(true));

            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).OrderType == OrderTypeEnum.PURCHASE ? "進貨" : "退貨", 2, 2).SetFontColor((Source as StoreOrder).OrderType == OrderTypeEnum.PURCHASE ? Color.Green : Color.Red).SetIsBold(true).SetFontSize(15));
            Settings.Add(new ExportSpecialExcelSetting(((Source as StoreOrder).ReceiveID is null) ? (Source as StoreOrder).ID : (Source as StoreOrder).ReceiveID, 2, 3).SetFontColor(Color.DarkBlue).SetFontSize(15).SetIsBold(true));
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).OrderManufactory.GetName, 2, 4).SetIsBold(true));
            Settings.Add(new ExportSpecialExcelSetting("收貨員工:", 3, 2));
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).ReceiveEmployeeName, 3, 3));
            Settings.Add(new ExportSpecialExcelSetting("收貨時間:", 3, 4));
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).DoneDateTime?.ToString("yyyy/MM/dd hh:mm:ss"), 3, 5));

            DataTable products = PurchaseReturnProductDB.GetProductsByStoreOrderIDForExport((Source as StoreOrder).ID);
            Settings.Add(new ExportDataTableExcelSetting(products, 4, 2));

            int productRows = products.Rows.Count + 1;
            Settings.Add(new ExportSpecialExcelSetting((Source as StoreOrder).TotalPrice.ToString("####"), productRows + 4, 7).SetFontColor(Color.SteelBlue).SetFontSize(15).SetIsBold(true));
        }

        public override string GetSheetName()
        {
            return ((Source as StoreOrder).ReceiveID is null) ? (Source as StoreOrder).ID : (Source as StoreOrder).ReceiveID;
        }
    }
}