using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    public class MedicineStockViewModel : ViewModelBase
    {
        #region ----- Define Variables -----
        private string medicineID;
        private string wareHouseID;
        private MedicineStockDetail stockDetail;

        public MedicineStockDetail StockDetail
        {
            get { return stockDetail; }
            set { Set(() => StockDetail, ref stockDetail, value); }
        }
        #endregion
        
        #region ----- Define Functions -----
        public void ReloadData(string medID, string wareID)
        {
            medicineID = medID;
            wareHouseID = wareID;

            DataTable stockDataTable = ProductDetailDB.GetMedicineStockDetailByID(medID, wareID);

            if (stockDataTable is null || stockDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            StockDetail = new MedicineStockDetail(stockDataTable.Rows[0]);
        }
        internal void GetMedBagDetailByID()
        {
            StockDetail.GetMedBagDetailByID(medicineID, wareHouseID);
        }
        internal void GetStockDetailByID()
        {
            StockDetail.GetStockDetailByID(medicineID, wareHouseID);
        }
        #endregion
    }
}
