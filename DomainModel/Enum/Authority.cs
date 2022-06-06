using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Enum
{
    public enum Authority
    {
        [Description("系統管理員")]
        Admin = 1,
        [Description("店長")]
        ShopMaster = 2,
        [Description("店員")]
        ShopEmployee = 3,
        [Description("藥師")]
        Pharmacist = 4 
    }
}
