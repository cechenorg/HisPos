using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class Medicine : IProduct
    {
        #region -- IProduct --
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Inventory { get; set; }
        #endregion

        public double Total { get; set; }
        public bool PaySelf { get; set; }
        public double HcPrice { get; set; }
        public Medicate Medicate { get; set;}
    }
}
