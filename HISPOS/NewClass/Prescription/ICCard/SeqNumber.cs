﻿using His_Pos.Service;
using System;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.ICCard
{
    [ZeroFormattable]
    public struct SeqNumber
    {
        [Index(0)]
        public string TreatDateTime;

        [Index(1)]
        public string MedicalNumber;

        [Index(2)]
        public string InstitutionId;

        [Index(3)]
        public string SecuritySignature;

        [Index(4)]
        public string SamId;

        [Index(5)]
        public bool SameDayTreat;

        [Index(6)]
        public string TreatmentCode;

        public SeqNumber(byte[] pBuffer)
        {
            var treatDateTimeString = Function.ByteArrayToString(13, pBuffer, 0);
            TreatDateTime = treatDateTimeString;//DateTimeExtensions.TWDateStringToDateTime(treatDateTimeString);
            MedicalNumber = Function.ByteArrayToString(4, pBuffer, 13).Trim();
            InstitutionId = Function.ByteArrayToString(10, pBuffer, 17);
            SecuritySignature = Function.ByteArrayToString(256, pBuffer, 27);
            SamId = Function.ByteArrayToString(12, pBuffer, 283);
            SameDayTreat = Function.ByteArrayToString(1, pBuffer, 295).Equals("Y");
            TreatmentCode = Function.ByteArrayToString(20, pBuffer, 296);
        }

        public SeqNumber(string treat, string medicalNum, string insID, string secSig, string samID, bool samDateTreat, string treatmentCode)
        {
            TreatDateTime = treat;
            MedicalNumber = medicalNum;
            InstitutionId = insID;
            SecuritySignature = secSig;
            SamId = samID;
            SameDayTreat = samDateTreat;
            TreatmentCode = treatmentCode;
        }
    }
}