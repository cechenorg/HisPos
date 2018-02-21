using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class IcCardPay
    {
        public string MedicalPay { get; set; } //門診醫療費用(當次)
        public string MedicalCopay { get; set; } //門診部分負擔費用(當次)
        public string HospitalPay { get; set; } //住院醫療費用(當次)
        public string HospitalCopay1 { get; set; } //住診部分負擔費用（當次急性 30 天、慢性 180 天以下）
        public string HospitalCopay2 { get; set; } //住診部分負擔費用（當次急性 31 天、慢性 180 天以上）
    }
}
