using His_Pos.NewClass.Product.ProductManagement.MedBagDetail;
using His_Pos.NewClass.Product.ProductManagement.OnTheWayDetail;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.ProductManagement.ProductStockDetail
{
    public class MedicineStockDetail : ProductStockDetail
    {
        #region ----- Define Variables -----

        private string stockDetail = "";
        private MedBagDetailStructs medBagDetails;
        private MedBagDetailStructs demandDetails;
        private OnTheWayDetailStructs onTheWayDetail;

        public double MedBagOnTheWayAmount { get; set; }

        public double TotalOnTheWayAmount
        {
            get { return OnTheWayAmount + MedBagOnTheWayAmount; }
        }

        public double InsuffInventory { get; set; }//不足量
        public double ShelfInventory { get; set; }//架上量
        public double MedBagInventory { get; set; }//藥袋量
        public double DemandInventory { get; set; }//需求量
        public double ConsumeIn90Days { get; set; }

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
            //get { return medBagDetails; }
        }
        public IEnumerable<MedBagDetailStruct> DemandStockDetails
        {
            get { return demandDetails.Where(d => d.SelfAmount != 0); }
            //get { return demandDetails; }
        }

        public IEnumerable<MedBagDetailStruct> MedBagSendDetails
        {
            get { return medBagDetails.Where(d => d.SendAmount != 0); }
        }

        public bool IsInventoryError => InsuffInventory > 0;//是否為不足量顯示

        #endregion ----- Define Variables -----

        public MedicineStockDetail(DataRow row) : base(row)
        {
            ShelfInventory = row.Field<double>("SHELF_INV");//架上量
            MedBagInventory = row.Field<double>("Inv_MedBagInventory");//藥袋量
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            ConsumeIn90Days = row.Field<double>("CONSUME_AMOUNT");
            DemandInventory = row.Field<double>("MEDBAG_INV");//需求量
            InsuffInventory = row.Field<double>("InsuffInventory");//不足量  
            //IsInventoryError = InsuffInventory > 0 ? true : false;//不足量顯示
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
            medBagDetails = MedBagDetailStructs.GetMedBagDetailByID(proID, wareID, 0);//藥袋
            demandDetails = MedBagDetailStructs.GetMedBagDetailByID(proID, wareID, 1);//需求
            RaisePropertyChanged(nameof(MedBagStockDetails));
            RaisePropertyChanged(nameof(MedBagSendDetails));
            RaisePropertyChanged(nameof(DemandStockDetails));
        }

        internal void GetOnTheWayDetailByID(string proID, string wareID)
        {
            OnTheWayDetail = OnTheWayDetailStructs.GetOnTheWayDetailByID(proID, wareID);
        }

        #endregion ----- Define Functions -----
    }
}