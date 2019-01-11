using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StoreOrder
{
    public enum OrderStatusEnum
    {
        UNPROCESSING = 1,
        WAITING = 2,
        NORMAL_PROCESSING = 3,
        SINGDE_PROCESSING = 4,
        DONE = 5,
        SCRAP = 6,
        ERROR = 100
    }
}
