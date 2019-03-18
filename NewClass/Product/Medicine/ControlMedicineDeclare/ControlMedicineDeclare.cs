using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDeclare {
    public class ControlMedicineDeclare : Product {
        public ControlMedicineDeclare() { }
        public ControlMedicineDeclare(DataRow r):base(r) {  
            InitStock = r.Field<double>("InvRec_OldStock");
            DiffentValue = r.Field<double>("StockDifferent"); 
            FinalValue = r.Field<double>("InvRec_NewStock");
            IsControl = r.Field<byte?>("Med_Control");
        } 
        
        public double InitStock { get; set; }
        public double DiffentValue { get; set; } 
        public double FinalValue { get; set; }
        public int? IsControl { get; set; }
        
    }
}
