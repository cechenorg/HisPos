using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder
{
    public class ReturnOrder: StoreOrder
    {
        public ReturnOrder(DataRow row) : base(row)
        {
        }
    }
}
