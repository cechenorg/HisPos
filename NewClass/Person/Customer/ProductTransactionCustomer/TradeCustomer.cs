﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public CustomerGroupTags GroupTags { get; set; }

        public int Old => (Birth is null) ? 0 : DateTime.Now.Subtract((DateTime)Birth).Days / 365;
        public bool HasOwnMoney => OwnMoney > 0;
        #endregion

        public TradeCustomer(DataRow row)
        {

        }

        #region ----- Define Functions -----
        #endregion
    }
}
