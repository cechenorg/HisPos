using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.ProductType
{
    public class ProductTypeManageMaster : ProductType, INotifyPropertyChanged
    {
        public ProductTypeManageMaster(DataRow dataRow) : base(dataRow)
        {
            StockValue = Double.Parse(dataRow["STOCK"].ToString());
            Sales = Double.Parse(dataRow["TOTAL"].ToString());
            TypeCount = Int32.Parse(dataRow["COUNT"].ToString());
        }

        public ProductTypeManageMaster(string name, string engName) : base("", name, engName)
        {
            StockValue = 0;
            Sales = 0;
            TypeCount = 0;
        }

        public double StockValue { get; set; }
        public double Sales { get; set; }
        private int typeCount;
        public int TypeCount
        {
            get { return typeCount; }
            set
            {
                typeCount = value;
                NotifyPropertyChanged("TypeCount");
            }
        }
    }
}
