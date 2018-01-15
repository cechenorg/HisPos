using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class Otc : IProduct
    {
        public Otc(string id, string name, double price, double inventory)
        {
            Id = id;
            Name = name;
            Price = price;
            Inventory = inventory;
        }
        #region -- IProduct --
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Inventory { get; set; }
        #endregion
    }
}
