using System.Data;

namespace His_Pos.NewClass.Prescription {
    public class PrescriptionStatus {
        public PrescriptionStatus() { }
        public PrescriptionStatus(DataRow r, PrescriptionSource type) {
            switch (type) {
                case PrescriptionSource.Normal:
                    IsGetCard = r.Field<bool>("IsGetCard");
                    IsDeclare = r.Field<bool>("IsDeclare");
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    break;
                case PrescriptionSource.ChronicReserve:
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    IsRegister = r.Field<bool>("IsRegister");
                    break;
            } 
        }
        public bool IsGetCard { get; set; } //是否讀卡
        public bool IsDeclare { get; set; } //是否申報
        public bool IsRead { get; set; }//是否已讀
        public bool IsSendToSingde { get; set; }//是否傳送過藥健康
        public bool IsAdjust { get; set; }//是否調劑.扣庫
        public bool IsRegister { get; set; } //是否登錄
        public bool IsSendOrder { get; set; } //判斷是否傳送要健康
        public bool IsDeposit { get; set; }//是否押金
    }
}
