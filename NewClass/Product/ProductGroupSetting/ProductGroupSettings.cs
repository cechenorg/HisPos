using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSettings :  ObservableCollection<ProductGroupSetting>
    {
        public ProductGroupSettings() { }

        public void GetDataByID(string proID,string warID) {
            Clear();
            DataTable table = ProductGroupSettingDb.GetDataByID(proID,warID);
            foreach (DataRow r in table.Rows) {
                Add(new ProductGroupSetting(r));
            }
        }
        public void MergeProduct(string warID) {
            ProductGroupSettingDb.MergeProductGroup(this,warID);
        }
    }
}
