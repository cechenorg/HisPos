using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.Medicine.ControlMedicineDeclare {
    public class ControlMedicineDeclare : ObservableObject {
        public ControlMedicineDeclare(DataRow r) {
            Id = r.Field<string>("Pro_ID");
            Name = r.Field<string>("Pro_ChineseName");
            InitStock = r.Field<double>("InvRec_OldStock");
            DiffentValue = r.Field<double>("StockDifferent"); 
            FinalValue = r.Field<double>("InvRec_NewStock"); 
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public double InitStock { get; set; }
        public double DiffentValue { get; set; } 
        public double FinalValue { get; set; }
    }
}
