using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IcData
{
    public class TreatRecords:Collection<TreatmentDataNoNeedHpc>
    {
        public TreatRecords(byte[] pBuffer)
        {
            Init(pBuffer);
        }

        private void Init(byte[] pBuffer)
        {
            var startIndex = 84;
            for (var i = 0; i < 6; i++)
            {
                Add(new TreatmentDataNoNeedHpc(pBuffer, startIndex, false));
                startIndex += 69;
            }
        }
    }
}
