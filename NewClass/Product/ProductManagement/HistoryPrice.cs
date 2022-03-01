using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct HistoryPrice
    {
        public HistoryPrice(DataRow dataRow)
        {
            StartDate = dataRow.Field<string>("Med_StartDate").TrimStart('0');
            EndDate = dataRow.Field<string>("Med_EndDate").TrimStart('0');
            Price = (double)dataRow.Field<decimal>("Med_Price");
        }

        public string StartDate { get; }
        public string EndDate { get; }
        public double Price { get; }
    }
}