using System;
using His_Pos.Service;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.IcData
{
    public struct BasicData
    {
        public string CardNumber;
        public string Name;
        public string IDNumber;
        public DateTime Birthday;
        public string BirthdayStr;
        public string Gender;
        public DateTime CardReleaseDate;
        public string CardLogoutMark;
        public string EmergencyTel;
        /*
         * 卡片號碼(1-12)
         * 姓名(13-32)
         * 身分證號或身分證明文件號碼(33-42)
         * 出生日期(43-49)
         * 性別(50)
         * 發卡日期(51-57)
         * 卡片註銷註記(58)
         * 緊急聯絡電話(59-72) 
         */
        public BasicData(byte[] pBuffer)
        {
            CardNumber = Function.ByteArrayToString(12,pBuffer,0);
            Name = Function.ByteArrayToString(20, pBuffer, 12).Trim();
            IDNumber = Function.ByteArrayToString(10, pBuffer, 32);
            BirthdayStr = Function.ByteArrayToString(7, pBuffer, 42);
            Birthday = DateTimeExtensions.TWDateStringToDateOnly(BirthdayStr);
            Gender = Function.ByteArrayToString(1, pBuffer, 49).Equals("M")? StringRes.Male: StringRes.Female;
            var dateString = Function.ByteArrayToString(7, pBuffer, 50);
            CardReleaseDate = DateTimeExtensions.TWDateStringToDateOnly(dateString);
            CardLogoutMark = Function.ByteArrayToString(1, pBuffer, 57);
            EmergencyTel = Function.ByteArrayToString(14, pBuffer, 58);
        }
    }
}
