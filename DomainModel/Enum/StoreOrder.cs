using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Enum
{
   
    public enum OrderStatusEnum
    {
        ERROR = 0,
        NORMAL_UNPROCESSING = 1,
        SINGDE_UNPROCESSING = 2,
        WAITING = 3,
        NORMAL_PROCESSING = 4,
        SINGDE_PROCESSING = 5,
        DONE = 6,
        SCRAP = 7
    }
}
