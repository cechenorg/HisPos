using His_Pos.Service;
using System;
using ZeroFormatter;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.ICCard
{
    [ZeroFormattable]
    public struct BasicData
    {
        [Index(0)]
        public string CardNumber;

        [Index(1)]
        public string Name;

        [Index(2)]
        public string IDNumber;

        [Index(3)]
        public DateTime Birthday;

        [Index(4)]
        public string BirthdayStr;

        [Index(5)]
        public string Gender;

        [Index(6)]
        public DateTime CardReleaseDate;

        [Index(7)]
        public string CardLogoutMark;

        [Index(8)]
        public string Tel;

        /*
         * 卡片號碼(1-12)
         * 姓名(13-32)
         * 身分證號或身分證明文件號碼(33-42)
         * 出生日期(43-49)
         * 性別(50)
         * 發卡日期(51-57)
         * 卡片註銷註記(58)
         * 連絡電話(59-72)
         */

        public BasicData(byte[] pBuffer)
        {
            CardNumber = Function.ByteArrayToString(12, pBuffer, 0);
            Name = Function.ByteArrayToString(20, pBuffer, 12).Trim();
            IDNumber = Function.ByteArrayToString(10, pBuffer, 32);
            BirthdayStr = Function.ByteArrayToString(7, pBuffer, 42);
            Birthday = (DateTime)DateTimeExtensions.TWDateStringToDateOnly(BirthdayStr);
            Gender = Function.ByteArrayToString(1, pBuffer, 49).Equals("M") ? StringRes.Male : StringRes.Female;
            string dateString = Function.ByteArrayToString(7, pBuffer, 50);
            CardReleaseDate = (DateTime)DateTimeExtensions.TWDateStringToDateOnly(dateString);
            CardLogoutMark = Function.ByteArrayToString(1, pBuffer, 57);
            Tel = Function.ByteArrayToString(14, pBuffer, 58);
        }

        public BasicData(string cardNum, string name, string idNum, DateTime birthday, string birth, string gender, DateTime cardRelease, string cardLogout, string tel)
        {
            CardNumber = cardNum;
            Name = name;
            IDNumber = idNum;
            Birthday = birthday;
            BirthdayStr = birth;
            Gender = gender;
            CardReleaseDate = cardRelease;
            CardLogoutMark = cardLogout;
            Tel = tel;
        }
    }
}