using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve.IndexReserveDetail
{
   public class IndexReserveDetail : Product.Product {
        public IndexReserveDetail(DataRow r):base(r) { 
            Stock = r.Field<string>("Inventory");
            OntheWay = r.Field<string>("OntheWay");
            Amount = r.Field<double>("TotalAmount");
            IsFrozen = r.Field<bool>("Med_IsFrozen");
            ControlLevel = r.Field<byte?>("Med_Control");
        }
        public string Stock { get; set; }
        public string OntheWay { get; set; }
        public bool IsFrozen { get; set; }
        public byte? ControlLevel { get; set; }
        public double Amount { get; set; }
    }
}
