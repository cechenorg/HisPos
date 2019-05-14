using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    public interface IPrescriptionService
    {
        void GetMasterData();
        void GetDetailData();
        bool Adjust();
        bool ErrorAdjust();
        bool NoCardAdjust();
        List<bool> AskPrint();
        bool Register();
    }
}
