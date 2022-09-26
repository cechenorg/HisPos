 using System.ComponentModel; 

namespace DomainModel.Enum
{
    public enum Authority
    {
        [Description("系統管理員")]
        Admin = 1,
        [Description("藥局經理")]
        PharmacyManager = 2,
        [Description("會計人員")]
        AccountingStaff = 3,
        [Description("店長")]
        StoreManager = 4, 
        [Description("店員")]
        StoreEmployee = 5,
        [Description("負責藥師")]
        MasterPharmacist = 6,
        [Description("執業藥師")]
        NormalPharmacist = 7,
        [Description("支援藥師")]
        SupportPharmacist = 8
    }


    public class EnumFactory
    {
        public static Authority TranValueToAuthority(int autVal)
        {
            switch (autVal)
            {
                case 1:
                    return Authority.Admin;
                case 2:
                    return Authority.PharmacyManager;
                case 3:
                    return Authority.AccountingStaff;
                case 4:
                    return Authority.StoreManager;
                case 5:
                    return Authority.StoreEmployee;
                case 6:
                    return Authority.MasterPharmacist;
                case 7:
                    return Authority.NormalPharmacist;
                case 8:
                    return Authority.SupportPharmacist;
            }
            return Authority.StoreEmployee;
        }
    }
    

}
