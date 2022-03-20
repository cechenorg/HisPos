using System.Data;
using System.Linq;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeManageMaster : ProductType
    {
        #region ----- Define Variables -----

        private ProductTypeManageDetail currentDetailType;
        private ProductTypeManageDetails productTypeDetails;
        private double initTotalStockValue = 0;
        private double initTotalSales = 0;

        public ProductTypeManageDetail CurrentDetailType
        {
            get { return currentDetailType; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetTypeDetailProducts();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentDetailType, ref currentDetailType, value);
            }
        }

        public ProductTypeManageDetails ProductTypeDetails
        {
            get => productTypeDetails;
            set
            {
                Set(() => ProductTypeDetails, ref productTypeDetails, value);
                RaisePropertyChanged(nameof(TotalStockValue));
            }
        }

        public int TypeDetailCount { get; set; }
        public double TotalStockValue { get { return (ProductTypeDetails is null) ? initTotalStockValue : ProductTypeDetails.Sum(d => d.StockValue); } }
        public double TotalSales { get { return (ProductTypeDetails is null) ? initTotalSales : ProductTypeDetails.Sum(d => d.Sales); } }

        #endregion ----- Define Variables -----

        public ProductTypeManageMaster(DataRow row) : base(row)
        {
            TypeDetailCount = row.Field<int>("TYPE_COUNT");
        }

        internal void GetTypeDetails()
        {
            ProductTypeDetails = ProductTypeManageDetails.GetProductTypeDetails(ID);

            if (ProductTypeDetails.Count > 0)
                CurrentDetailType = ProductTypeDetails[0];
        }
    }
}