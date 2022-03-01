using His_Pos.Service.ExportService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace His_Pos.NewClass.Product.ExportProductRecord
{
    public class ExportProductRecordTemplate : ExportExcelTemplate
    {
        public override string GetSheetName()
        {
            return "商品歷程";
        }

        protected override void CreateExcelSettings()
        {
            List<object> temp = Source as List<object>;

            Settings.Add(new ExportSpecialExcelSetting(temp[0].ToString(), 2, 2).SetFontColor(Color.DarkBlue).SetFontSize(15).SetIsBold(true));
            Settings.Add(new ExportSpecialExcelSetting(((DateTime)temp[1]).ToShortDateString(), 2, 3));
            Settings.Add(new ExportSpecialExcelSetting(((DateTime)temp[2]).ToShortDateString(), 2, 4));

            DataTable products = ProductDB.GetProductInventoryRecordByIDForExport(temp[0].ToString(), (DateTime)temp[1], (DateTime)temp[2], temp[3].ToString());
            Settings.Add(new ExportDataTableExcelSetting(products, 3, 2));
        }
    }
}