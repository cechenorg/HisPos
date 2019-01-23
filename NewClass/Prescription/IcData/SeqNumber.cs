using System;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.IcData
{
    public struct SeqNumber
    {
        public DateTime TreatDateTime;
        public string MedicalNumber;
        public string InstitutionId;
        public string SecuritySignature;
        public string SamId;
        public bool SameDayTreat;

        public SeqNumber(byte[] pBuffer)
        {
            var treatDateTimeString = Function.ByteArrayToString(13, pBuffer, 0);
            TreatDateTime = DateTimeExtensions.TWDateStringToDateTime(treatDateTimeString);
            MedicalNumber = Function.ByteArrayToString(4, pBuffer, 13).Trim();
            InstitutionId = Function.ByteArrayToString(10, pBuffer, 17);
            SecuritySignature = Function.ByteArrayToString(256, pBuffer, 27);
            SamId = Function.ByteArrayToString(12, pBuffer, 283);
            SameDayTreat = Function.ByteArrayToString(1, pBuffer, 295).Equals("Y");
        }
    }
}
