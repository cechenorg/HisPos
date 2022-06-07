using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.SQLService.SQLServer.Product
{
    public class ProductDBService : SQLServerServiceBase
    {
        public ProductDBService(string connectionString, string dataBaseName) : base(connectionString, dataBaseName)
        {
        }

        private static readonly string[] ProductMasterColumns =
        {
            "Pro_ID",  "Pro_BarCode", "Pro_ChineseName",  "Pro_EnglishName", "Pro_Note",
            "Pro_TypeID",  "Pro_IsEnable", "Pro_LastManufactoryID",  "Pro_MinOrder", "Pro_IsCommon",
            "Pro_SelfPayType",  "Pro_SelfPayPrice", "Pro_Location"
        };

        public string GetProductMasterSelectString()
        {
            return "SELECT * FROM Product.Master ";
        }
    }

    public class dProduct
    {
        public dProduct(){}

        public string Pro_ID { get; set; }

        public string Pro_BarCode { get; set; }

        public string Pro_ChineseName { get; set; }

        public string Pro_EnglishName { get; set; }

        public string Pro_Note { get; set; }

        public int Pro_TypeID { get; set; }

        public bool Pro_IsEnable { get; set; }

        public int Pro_LastManufactoryID { get; set; }

        public int Pro_MinOrder { get; set; }

        public bool Pro_IsCommon { get; set; }

        public string Pro_SelfPayType { get; set; }

        public double Pro_SelfPayPrice { get; set; }

        public int Pro_Location { get; set; }
    }
}
