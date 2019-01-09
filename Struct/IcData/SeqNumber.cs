using His_Pos.Service;

namespace His_Pos.Struct.IcData
{
    public struct SeqNumber
    {
        public string TreatDateTime;
        public string MedicalNumber;
        public string InstitutionId;
        public string SecuritySignature;
        public string SamId;
        public bool SameDayTreat;

        public SeqNumber(byte[] pBuffer)
        {
            var f = new Function();
            TreatDateTime = f.ByteArrayToString(13, pBuffer, 0);
            MedicalNumber = f.ByteArrayToString(4, pBuffer, 13).Trim();
            InstitutionId = f.ByteArrayToString(10, pBuffer, 17);
            SecuritySignature = f.ByteArrayToString(256, pBuffer, 27);
            SamId = f.ByteArrayToString(12, pBuffer, 283);
            SameDayTreat = f.ByteArrayToString(1, pBuffer, 295).Equals("Y");
        }
    }
}
