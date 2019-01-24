using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using His_Pos.Class.Declare.IcDataUpload;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using Prescription = His_Pos.NewClass.Prescription.Prescription;
// ReSharper disable All

namespace His_Pos.HisApi
{
    public class HisApiFunction
    {
        public static List<string> WritePrescriptionData(Prescription p)
        {
            var signList = new List<string>();
            var medList = p.Medicines.Where(m => m is MedicineNHI && !m.PaySelf).ToList();
            var iWriteCount = medList.Count;
            var iBufferLength = 40 * iWriteCount;
            var treatDateTime = DateTimeExtensions.ToStringWithSecond(p.Card.MedicalNumberData.TreatDateTime);
            var pDataWriteStr = p.Medicines.CreateMedicalData(treatDateTime);
            byte[] pDateTime = ConvertData.StringToBytes(treatDateTime+"\0",14);
            byte[] pPatientID = ConvertData.StringToBytes(p.Card.PatientBasicData.IDNumber + "\0", 11);
            byte[] pPatientBirthDay = ConvertData.StringToBytes(p.Card.PatientBasicData.BirthdayStr + "\0", 8);
            byte[] pDataWrite = ConvertData.StringToBytes(pDataWriteStr, 3660);
            byte[] pBuffer = new byte[iBufferLength];
            if (HisApiBase.OpenCom())
            {
                var res = HisApiBase.hisWriteMultiPrescriptSign(pDateTime, pPatientID, pPatientBirthDay, pDataWrite, ref iWriteCount, pBuffer, ref iBufferLength);
                if (res == 0)
                {
                    var startIndex = 0;
                    for (int i = 0; i < iWriteCount; i++)
                    {
                        signList.Add(ConvertData.ByToString(pBuffer, startIndex,40));
                        startIndex += 40;
                    }
                }
            }
            return signList;
        }
        //正常上傳
        public static void CreatDailyUploadData(Prescription p)
        {
            Rec rec = new Rec(p);
            var uploadData = rec.SerializeDailyUploadObject();
            Console.WriteLine(uploadData);
        }

        //異常上傳
        public static void CreatErrorDailyUploadData()
        {
            
        }
    }
}
