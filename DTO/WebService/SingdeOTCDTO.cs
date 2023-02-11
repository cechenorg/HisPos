using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDTO
{
    public class SingdeOTCDTO
    {
        public string OTC_Code { get; set; }
        public string OTC_Barcode { get; set; }
        public string OTC_Name { get; set; }
        public string OTC_Type { get; set; }
        public int OTC_BagFact { get; set; }
        public string OTC_StopUsing { get; set; }
        public int OTC_InvQty { get; set; }
        public string OTC_InvMemo { get; set; }
        public int OTC_Price { get; set; }
    }
}
