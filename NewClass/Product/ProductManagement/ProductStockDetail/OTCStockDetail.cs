using His_Pos.NewClass.Product.ProductManagement.MedBagDetail;
using His_Pos.NewClass.Product.ProductManagement.OnTheWayDetail;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.ProductManagement.ProductStockDetail
{
    public class OTCStockDetail : ProductStockDetail
    {
        #region ----- Define Variables -----

        private string stockDetail = "";
        private MedBagDetailStructs medBagDetails;
        private OnTheWayDetailStructs onTheWayDetail;

        public double MedBagOnTheWayAmount { get; set; }

        public double TotalOnTheWayAmount
        {
            get { return OnTheWayAmount + MedBagOnTheWayAmount; }
        }

        public double ShelfInventory { get; set; }
        public int TraDet_DepositAmount { get; set; }
        public double AVGVALUE { get; set; }

        public double MedBagInventory { get; set; }
        public double CONSUME_AMOUNT { get; set; }

        public string StockDetail
        {
            get { return stockDetail; }
            set { Set(() => StockDetail, ref stockDetail, value); }
        }

        public OnTheWayDetailStructs OnTheWayDetail
        {
            get { return onTheWayDetail; }
            set { Set(() => OnTheWayDetail, ref onTheWayDetail, value); }
        }

        public IEnumerable<MedBagDetailStruct> MedBagStockDetails
        {
            get { return medBagDetails.Where(d => d.SelfAmount != 0); }
        }

        public IEnumerable<MedBagDetailStruct> MedBagSendDetails
        {
            get { return medBagDetails.Where(d => d.SendAmount != 0); }
        }

        public bool IsInventoryError => MedBagInventory > TotalInventory;

        #endregion ----- Define Variables -----

        public OTCStockDetail(DataRow row) : base(row)
        {
            ShelfInventory = row.Field<double>("SHELF_INV");
            MedBagInventory = row.Field<double>("MEDBAG_INV");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            CONSUME_AMOUNT = row.Field<double>("CONSUME_AMOUNT");
            TraDet_DepositAmount = row.Field<int>("TraDet_DepositAmount");
            AVGVALUE = row.Field<double>("AVGVALUE");
        }

        #region ----- Define Functions -----

        public void GetStockDetailByID(string proID, string wareID)
        {
            DataTable dataTable = ProductDetailDB.GetStockDetailByID(proID, wareID);

            string tempStockDetail = "";

            for (int x = 0; x < dataTable.Rows.Count; x++)
            {
                string amount = dataTable.Rows[x].Field<double>("InvDet_Amount").ToString("0.##").PadLeft(6);
                string price = ((double)dataTable.Rows[x].Field<decimal>("InvDet_Price")).ToString("0.00").PadLeft(8);
                string subtotal = dataTable.Rows[x].Field<double>("SUBTOTAL").ToString("0.00").PadLeft(8);
                string batchNum = dataTable.Rows[x].Field<string>("InvDet_BatchNumber").PadRight(10);

                tempStockDetail += $"數量:{amount} 單價:{price} 小計:{subtotal} 批號: {batchNum}";

                if (x < dataTable.Rows.Count - 1)
                    tempStockDetail += "\n";
            }

            StockDetail = tempStockDetail;
        }

        internal void GetMedBagDetailByID(string proID, string wareID)
        {
            medBagDetails = MedBagDetailStructs.GetOTCMedBagDetailByID(proID, wareID);
            RaisePropertyChanged(nameof(MedBagStockDetails));
            RaisePropertyChanged(nameof(MedBagSendDetails));
        }

        internal void GetOnTheWayDetailByID(string proID, string wareID)
        {
            OnTheWayDetail = OnTheWayDetailStructs.GetOnTheWayDetailByID(proID, wareID);
        }

        #endregion ----- Define Functions -----
    }
}