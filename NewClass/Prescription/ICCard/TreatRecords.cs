using System.Collections.ObjectModel;

namespace His_Pos.NewClass.Prescription.ICCard
{
    public class TreatRecords : Collection<TreatmentDataNoNeedHpc>
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