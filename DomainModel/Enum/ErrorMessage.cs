using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Enum
{
    public enum ErrorMessage
    {
        OK,
        [Description("資料為空!")]
        DataEmpty,
        [Description("身分證格式錯誤!")]
        EmployeeIDNumberFormatError,
        [Description("此身分證已經存在!")]
        EmployeeIDNumberExist,
    }
}
