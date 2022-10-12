using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using System;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageMedicine : Product, ICloneable
    {
        #region ----- Define Variables -----

        private double selfPayMultiplier;

        public bool Status { get; set; }
        public string Note { get; set; }
        public string Indication { get; set; }
        public string Warnings { get; set; }
        public string SideEffect { get; set; }
        public string OTCMemo { get; set; }
        public string BarCode { get; set; }
        public int? SafeAmount { get; set; }
        public int? BasicAmount { get; set; }
        public int MinOrderAmount { get; set; }
        private string rewardPercent;

        public string RewardPercent
        {
            get { return rewardPercent; }
            set
            {
                rewardPercent = value;
                RaisePropertyChanged(nameof(RewardPercent));
            }
        }

        public SelfPayTypeEnum SelfPayType { get; set; }
        public double? SelfPayPrice { get; set; }

        public double SelfPayMultiplier
        {
            get { return selfPayMultiplier; }
            set
            {
                selfPayMultiplier = value;
                RaisePropertyChanged(nameof(SelfPayMultiplier));
            }
        }

        public bool IsSelfPayTypeDefault
        {
            get { return SelfPayType == SelfPayTypeEnum.Default; }
            set
            {
                SelfPayType = value ? SelfPayTypeEnum.Default : SelfPayTypeEnum.Customize;
                RaisePropertyChanged(nameof(IsSelfPayTypeDefault));
            }
        }

        private bool isReward;//常備品項

        public bool IsReward
        {
            get => isReward;
            set
            {
                if (isReward != value)
                {
                    Set(() => IsReward, ref isReward, value);
                }
            }
        }

        public bool OTCFromSingde { get; set; }

        #endregion ----- Define Variables -----

        public ProductManageMedicine()
        {
        }

        public ProductManageMedicine(DataRow row) : base(row)
        {
            IsReward = row.Field<bool>("Pro_IsReward");
            Status = row.Field<bool>("Pro_IsEnable");
            Note = row.Field<string>("Pro_Note");
            Indication = row.Field<string>("Med_Indication");
            SideEffect = row.Field<string>("Med_SideEffect");
            BarCode = row.Field<string>("Pro_BarCode");
            Warnings = row.Field<string>("Med_Warning");
            SafeAmount = row.Field<int?>("Inv_SafeAmount");
            BasicAmount = row.Field<int?>("Inv_BasicAmount");
            MinOrderAmount = row.Field<int>("Pro_MinOrder");
            SelfPayType = row.Field<string>("Pro_SelfPayType").Equals("D") ? SelfPayTypeEnum.Default : SelfPayTypeEnum.Customize;
            SelfPayPrice = (double?)row.Field<decimal?>("Pro_SelfPayPrice");
            SelfPayMultiplier = row.Field<double>("SysPar_Value");
            RewardPercent = row.Field<double>("Pro_RewardPercent").ToString();
            OTCFromSingde = row.Field<bool>("OTCFromSingde");
            OTCMemo = row.Field<string>("OTC_InvMemo");
        }

        #region ----- Define Functions -----

        public object Clone()
        {
            return this.DeepCloneViaJson();
        }

        public bool Save()
        {
            DataTable dataTable = ProductDetailDB.UpdateMedicineDetailData(this);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                switch (dataTable.Rows[0].Field<string>("RESULT"))
                {
                    case "FAIL-SET":
                        MessageWindow.ShowMessage("藥品組合中包含此品項，請先刪除藥品組合後再停用", MessageType.ERROR);
                        return false;

                    case "FAIL-INV":
                        MessageWindow.ShowMessage("藥品尚有庫存，請歸零後再停用", MessageType.ERROR);
                        return false;
                }
            }

            return true;
        }

        #endregion ----- Define Functions -----
    }
}