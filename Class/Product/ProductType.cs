using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class ProductType
    {
        public ProductType() { }
        public ProductType(string id,string rank,string name) {
            Id = id;
            Rank = rank;
            Name = name;
        }

        public string Id { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
    }
}
