using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass {
    public class PrescriptionStatus {
        public bool IsGetCard { get; set; } //是否讀卡
        public bool IsDeclare { get; set; } //是否申報
        public bool IsRead { get; set; }//是否已讀
        public bool IsSendToSingde { get; set; }//是否傳送藥健康
        public bool IsAdjust { get; set; }//是否調劑.扣庫
        public bool IsRegister { get; set; } //是否登錄
    }
}
