using His_Pos.Interface;
using System.Data;

namespace His_Pos.Class.Product
{
    public class ProductTypeMedicine : AbstractClass.Product, IProductType
    {
        public ProductTypeMedicine(DataRow dataRow) : base(dataRow)
        {
            TypeId = dataRow["PROTYP_ID"].ToString();
        }

        public string TypeId { get; set; }
    }
}