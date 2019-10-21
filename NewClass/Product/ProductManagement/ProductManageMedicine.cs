using System;
using System.Data;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageMedicine : Product, ICloneable
    {
        #region ----- Define Variables -----
        public bool Status { get; set; }
        public string Note { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string BarCode { get; set; }
        public int? SafeAmount { get; set; }
        public int? BasicAmount { get; set; }
        public int MinOrderAmount { get; set; }
        #endregion

        public ProductManageMedicine() { }

        public ProductManageMedicine(DataRow row) : base(row)
        {
            Status = row.Field<bool>("Pro_IsEnable");
            Note = row.Field<string>("Pro_Note");
            Indication = row.Field<string>("Med_Indication");
            SideEffect = row.Field<string>("Med_SideEffect");
            BarCode = row.Field<string>("Pro_BarCode");
            Warnings = row.Field<string>("Med_Warning");
            SafeAmount = row.Field<int?>("Inv_SafeAmount");
            BasicAmount = row.Field<int?>("Inv_BasicAmount");
            MinOrderAmount = row.Field<int>("Pro_MinOrder");
        }

        #region ----- Define Functions -----
        public object Clone()
        {
            return this.DeepCloneViaJson() as ProductManageMedicine;
        }
        public bool Save()
        {
            DataTable dataTable = ProductDetailDB.UpdateMedicineDetailData(this);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                switch (dataTable.Rows[0].Field<string>("RESULT"))
                {
                    case "FAIL-SET":
                        MessageWindow.ShowMessage("藥品組合中包含此品項，請先刪除藥品組合後再停用", MessageType.ERROR);
                        return false;
                    case "FAIL-INV":
                        MessageWindow.ShowMessage("藥品尚有庫存，請歸零後再停用", MessageType.ERROR);
                        return false;
                }
            }

            return true;
        }
        #endregion
    }
}
