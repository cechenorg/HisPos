using System.Data;

namespace His_Pos.NewClass.Medicine.ControlMedicineDeclare
{
    public class ControlMedicineDeclare : Product.Product
    {
        public ControlMedicineDeclare()
        {
        }

        public ControlMedicineDeclare(DataRow r) : base(r)
        {
            WareHouse = ChromeTabViewModel.ViewModelMainWindow.GetWareHouse(r.Field<int>("ProInv_WareHouseID").ToString());
            InitStock = r.Field<double>("InvRec_OldStock");
            PayValue = r.Field<double>("PayAmount");
            GetValue = r.Field<double>("GetAmount");
            FinalValue = r.Field<double>("InvRec_NewStock");
            IsControl = r.Field<byte?>("Med_Control");
        }

        public WareHouse.WareHouse WareHouse { get; set; }
        public double InitStock { get; set; }
        public double GetValue { get; set; }
        public double PayValue { get; set; }
        public double FinalValue { get; set; }
        public int? IsControl { get; set; }
    }
}