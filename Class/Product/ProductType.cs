using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class ProductType
    {
        public ProductType() {
            Id = "";
            Rank = "";
            Name = "無";
        }
        public ProductType(DataRow row) {
            Id = row["PROTYP_ID"].ToString();
            Rank = row["PROTYP_PARENT"].ToString();
            Name = row["PROTYP_CHINAME"].ToString();
        }

        public string Id { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
    }
}
