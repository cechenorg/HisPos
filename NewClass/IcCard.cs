using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass
{
    public class IcCard
    {
        public IcCard() { }
        public string CardNumber { get;}//卡片號碼
        public string IcNumber { get; set; }//身分證字號
        public DateTime CardReleaseDate { get;}//發卡日期
        public DateTime ValidityPeriod { get;}//卡片有效期限
        public int AvailableTimes { get;}//就醫可用次數

    }
}
