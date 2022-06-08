 using System.ComponentModel; 

namespace DomainModel.Enum
{
    public enum Authority
    {
        [Description("系統管理員")]
        系統管理員 = 1,
        [Description("藥局經理")]
        藥局經理 = 2,
        [Description("會計人員")]
        會計人員 = 3,
        [Description("店長")]
        店長 = 4, 
        [Description("店員")]
        店員 = 5,
        [Description("負責藥師")]
        負責藥師 = 6,
        [Description("執業藥師")]
        執業藥師 = 7,
        [Description("支援藥師")]
        支援藥師 = 8
    }

}
