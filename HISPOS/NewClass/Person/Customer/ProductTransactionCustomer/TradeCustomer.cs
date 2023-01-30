using System;
using System.Data;

namespace His_Pos.NewClass.Person.Customer.ProductTransactionCustomer
{
    public class TradeCustomer
    {
        #region ----- Define Variables -----

        public GenderEnum Gender { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? Birth { get; set; }
        public string IDNumber { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public int Point { get; set; }
        public int OwnMoney { get; set; }
        public string Note { get; set; }

        public int Old => (Birth is null) ? 0 : DateTime.Now.Subtract((DateTime)Birth).Days / 365;
        public bool HasOwnMoney => OwnMoney > 0;

        #endregion ----- Define Variables -----

        public TradeCustomer(DataRow row)
        {
        }
    }
}