using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Manufactory
{
    public struct ManufactoryChanged
    {
        public string ManufactoryId;
        public string changedOrderId;
        public ProcedureProcessType ProcessType;
        
        public ManufactoryChanged(ProductDetailManufactory productDetailManufactory, ProcedureProcessType processType)
        {
            ManufactoryId = productDetailManufactory.Id;
            changedOrderId = productDetailManufactory.OrderId;
            ProcessType = processType;
        }
    }
}
