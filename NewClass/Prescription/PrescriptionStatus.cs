using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Prescription
{
    public class PrescriptionStatus : ObservableObject
    {
        public PrescriptionStatus()
        {
        }

        public PrescriptionStatus(DataRow r, PrescriptionType type)
        {
            switch (type)
            {
                default:
                    IsGetCard = r.Field<bool>("IsGetCard");
                    IsDeclare = r.Field<bool>("IsDeclare");
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    IsDeposit = r.Field<bool>("IsDeposit");
                    IsAdjust = r.Field<bool>("IsAdjust");
                    break;

                case PrescriptionType.ChronicReserve:
                    IsSendToSingde = r.Field<bool>("IsSendToServer");
                    break;
            }
        }

        #region Properties

        public bool IsSendToSingde
        {
            get;
            set;
        }

        public bool IsAdjust { get; set; }
        public bool IsRead { get; set; }
        public bool IsPrint { get; set; }
        public bool IsVIP { get; set; }
        private bool isGetCard;

        public bool IsGetCard
        {
            get => isGetCard;
            set
            {
                Set(() => IsGetCard, ref isGetCard, value);
            }
        }

        public bool IsDeclare { get; set; }
        public bool IsDeposit { get; set; }
        public bool IsRegister { get; set; }
        public bool? IsCreateSign { get; set; }
        public bool IsSendOrder { get; set; }
        private bool? reserveSend;

        public bool? ReserveSend
        {
            get => reserveSend;
            set
            {
                Set(() => ReserveSend, ref reserveSend, value);
            }
        }

        private string orderStatus;

        public string OrderStatus
        {
            get => orderStatus;
            set
            {
                Set(() => OrderStatus, ref orderStatus, value);
            }
        }

        #endregion Properties

        public void SetPrescribeStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void SetErrorAdjustStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void SetNormalAdjustStatus()
        {
            IsGetCard = true;
            IsAdjust = true;
            IsDeclare = true;
            IsDeposit = false;
        }

        public void Init()
        {
            IsGetCard = false;
            IsRead = false;
            IsAdjust = false;
            IsSendOrder = false;
            IsSendToSingde = false;
            IsRegister = false;
            IsDeclare = true;
            IsVIP = false;
            IsCreateSign = null;
            IsDeposit = false;
        }

        public void SetDepositAdjustStatus()
        {
            IsGetCard = false;
            IsDeposit = true;
            IsDeclare = false;
            IsAdjust = true;
            IsSendOrder = false;
            IsSendToSingde = false;
            IsRegister = false;
            IsCreateSign = null;
        }

        public void SetRegisterStatus()
        {
            IsGetCard = false;
            IsAdjust = false;
            IsDeclare = false;
            IsDeposit = false;
        }

        public void UpdateStatus(int currentID)
        {
            PrescriptionDb.UpdatePrescriptionStatus(this, currentID);
        }
    }
}