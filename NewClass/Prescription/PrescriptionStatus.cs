using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Prescription {
    public class PrescriptionStatus : ObservableObject {
        public PrescriptionStatus() { }
        public PrescriptionStatus(DataRow r, PrescriptionSource type) {
            switch (type) {
                case PrescriptionSource.Normal:
                    IsGetCard = r.Field<bool>("IsGetCard");
                    IsDeclare = r.Field<bool>("IsDeclare");
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    IsDeposit = r.Field<bool>("IsDeposit");
                    IsAdjust = r.Field<bool>("IsAdjust");
                    break;
                case PrescriptionSource.ChronicReserve:
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    break;
            } 
        }
        private bool isGetCard;//是否讀卡
        public bool IsGetCard
        {
            get => isGetCard;
            set
            {
                Set(() => IsGetCard, ref isGetCard, value);
            }
        } 
        public bool IsDeclare { get; set; } //是否申報
        public bool IsRead { get; set; }//是否已讀
        public bool IsSendToSingde { get; set; }//是否傳送過藥健康
        public bool IsAdjust { get; set; }//是否調劑.扣庫
        public bool IsRegister { get; set; } //是否登錄 
        private bool isSendOrder;
        public bool IsSendOrder  //判斷是否傳送要健康
        {
            get => isSendOrder;
            set
            {
                Set(() => IsSendOrder, ref isSendOrder, value);
            }
        }
        public bool IsDeposit { get; set; } //是否押金
        public bool? IsCreateSign { get; set; }
        private bool isCooperativeVIP;
        public bool IsCooperativeVIP
        {
            get => isCooperativeVIP;
            set
            {
                Set(() => IsCooperativeVIP, ref isCooperativeVIP, value);
            }
        }
        private bool isCooperative;
        public bool IsCooperative
        {
            get => isCooperative;
            set
            {
                Set(() => IsCooperative, ref isCooperative, value);
            }
        }
        public bool IsPrescribe { get; set; }
        public void Init()
        {
            IsGetCard = false;
            IsRead = false;
            IsAdjust = false;
            IsSendOrder = false;
            IsSendToSingde = false;
            IsRegister = false;
            IsDeclare = true;
            IsCooperativeVIP = false;
            IsCreateSign = null;
            IsPrescribe = false;
            IsDeposit = false;
            IsCooperative = false;
        }

        public void UpdateStatus(int id)
        {
            PrescriptionDb.UpdatePrescriptionStatus(this,id);
        }
        
        //未過卡 已調劑 不申報 押金
        public void SetNoCardSatus()
        {
            IsGetCard = false;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = true;
        }
        //已過卡 已調劑 正常申報 不押金
        public void SetNormalAdjustStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = true;
            IsDeposit = false;
        }
        //已過卡 已調劑 不申報 不押金
        public void SetCooperativePrescribeStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }
        // 未過卡 未調劑 不申報 不押金
        public void SetRegisterStatus()
        {
            IsGetCard = false;
            IsAdjust = false;
            IsDeclare = false;
            IsDeposit = false;
        }
        // 未過卡 已調劑 不申報 不押金
        public void SetPrescribeStatus()
        {
            IsGetCard = false;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }
    }
}
