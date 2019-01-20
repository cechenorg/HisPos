using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProduct : Product
    {
        
        string Note { get; set; }
        double LastPrice { get; set; }
        double OrderAmount { get; set; }
        double FreeAmount { get; set; }
        string Invoice { get; set; }
        string ValidDate { get; set; }
        string BatchNumber { get; set; }

        bool IsFirstBatch { get; set; }

        double PackageAmount { get; }
        double PackagePrice { get; }
        double SingdePrice { get; }

        bool IsSingde { get; set; }

        public PurchaseProduct() : base()
        {
        }
        public PurchaseProduct(DataRow dataRow) : base(dataRow)
        {
        }
        
        
    }
}
