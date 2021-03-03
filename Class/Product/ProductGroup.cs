using System.Data;

namespace His_Pos.Class.Product
{
    public class ProductGroup
    {
        public ProductGroup(DataRow dataRow)
        {
            id = dataRow["PRO_ID"].ToString();
            name = dataRow["PRO_NAME"].ToString().Trim();
            invId = dataRow["PROINV_ID"].ToString();
            warId = dataRow["PROWAR_ID"].ToString();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string invId { get; set; }
        public string warId { get; set; }
    }
}