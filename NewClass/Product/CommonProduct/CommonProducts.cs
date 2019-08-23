using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.CommonProduct
{
    public class CommonProducts : ObservableCollection<CommonProduct>
    {
        public CommonProducts(DataTable table) {
            foreach (DataRow r in table.Rows) {
                Add(new CommonProduct(r));
            }
        } 
        public static CommonProducts GetData() {
            return new CommonProducts(CommonProductDb.GetData()); 
        }
    }
}
