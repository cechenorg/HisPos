using System;
using His_Pos.Service;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.Struct.IcData
{
    public struct BasicData
    {
        public string CardNumber;
        public string Name;
        public string IDNumber;
        public DateTime Birthday;
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
            var f = new Function();
            CardNumber = f.ByteArrayToString(12,pBuffer,0);
            Name = f.ByteArrayToString(20, pBuffer, 12).Trim();
            IDNumber = f.ByteArrayToString(10, pBuffer, 32);
            var dateString = f.ByteArrayToString(7, pBuffer, 42);
            var year = int.Parse(dateString.Substring(0, 3)) + 1911;
            var month = int.Parse(dateString.Substring(3, 2));
            var day = int.Parse(dateString.Substring(5, 2));
            Birthday = new DateTime(year,month,day);
            Gender = f.ByteArrayToString(1, pBuffer, 49).Equals("M")? StringRes.Male: StringRes.Female;
            dateString = f.ByteArrayToString(7, pBuffer, 50);
            year = int.Parse(dateString.Substring(0, 3)) + 1911;
            month = int.Parse(dateString.Substring(3, 2));
            day = int.Parse(dateString.Substring(5, 2));
            CardReleaseDate = new DateTime(year, month, day);
            CardLogoutMark = f.ByteArrayToString(1, pBuffer, 57);
            EmergencyTel = f.ByteArrayToString(14, pBuffer, 58);
        }
    }
}
