namespace His_Pos.Class.Product
{
    public class CusOrderOverview
    {
        public CusOrderOverview(string date, string amount, string customer)
        {
            Date = date;
            Amount = amount;
            Customer = customer;
        }

        public string Date { get; }
        public string Amount { get; }
        public string Customer { get; }
    }
}
