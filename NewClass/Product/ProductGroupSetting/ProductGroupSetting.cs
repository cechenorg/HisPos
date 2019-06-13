using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSetting : Product
    {
        public ProductGroupSetting() { }

        public ProductGroupSetting(DataRow r):base(r) {
            IsEditable = true;
            Stock = r.Field<int>("Inv_Inventory");
        }
        public int Stock { get; set; }
        private bool isEditable = false;
        public bool IsEditable
        {
            get { return isEditable; }
            set { Set(() => IsEditable, ref isEditable, value); }
        }
        
    }
}
