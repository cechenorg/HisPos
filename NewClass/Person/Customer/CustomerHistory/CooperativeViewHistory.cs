using System;
using System.Collections.ObjectModel;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CooperativeViewHistory:ObservableObject
    {
        public CooperativeViewHistory()
        {
            Products = new Products();
        }
        public CooperativeViewHistory(DataRow r) { }
        public int PrescriptionID { get; set; }
        public string Institution { get; set; }
        public string Division { get; set; }
        public DateTime AdjustDate { get; set; }
        public int TotalPoint { get; set; }
        public Products Products { get; set; }
    }
}
