using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.StockControl
{
    public class OTCStockViewModel : ViewModelBase
    {
        #region ----- Define Variables -----

        private string medicineID;
        private string wareHouseID;
        private OTCStockDetail stockDetail;

        public OTCStockDetail OTCStockDetail
        {
            get { return stockDetail; }
            set { Set(() => OTCStockDetail, ref stockDetail, value); }
        }

        #endregion ----- Define Variables -----

        #region ----- Define Functions -----

        public void ReloadData(string medID, string wareID)
        {
            medicineID = medID;
            wareHouseID = wareID;

            DataTable stockDataTable = ProductDetailDB.GetOTCStockDetailByID(medID, wareID);

            if (stockDataTable is null || stockDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            OTCStockDetail = new OTCStockDetail(stockDataTable.Rows[0]);
        }

        internal void GetMedBagDetailByID()
        {
            OTCStockDetail.GetMedBagDetailByID(medicineID, wareHouseID);
        }

        internal void GetStockDetailByID()
        {
            OTCStockDetail.GetStockDetailByID(medicineID, wareHouseID);
        }

        internal void GetOnTheWayDetailByID()
        {
            OTCStockDetail.GetOnTheWayDetailByID(medicineID, wareHouseID);
        }

        #endregion ----- Define Functions -----
    }
}