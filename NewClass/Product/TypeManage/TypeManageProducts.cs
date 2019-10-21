using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.ProductType;

namespace His_Pos.NewClass.Product.TypeManage
{
    public class TypeManageProducts : Collection<TypeManageProduct>
    {
        private TypeManageProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new TypeManageProduct(row));
            }
        }

        internal static TypeManageProducts GetTypeProducts(int iD)
        {
            return new TypeManageProducts(ProductTypeDB.GetTypeProducts(iD));
        }
    }
}
