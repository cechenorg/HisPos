using System;
using His_Pos.Service;
using ZeroFormatter;
using StringRes = His_Pos.Properties.Resources;

namespace His_Pos.NewClass.Prescription.IcData
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
        public string InsurerID;
        [Index(9)]
        public string InsuranceMark;
        [Index(10)]
        public DateTime ValidityPeriod;
        [Index(11)]
        public int AvailableTimes;
        [Index(12)]
        public string NewBornBirthday;
        [Index(13)]
        public string NewBornMark;
        /*
         * 卡片號碼(1-12)
         * 姓名(13-32)
         * 身分證號或身分證明文件號碼(33-42)
         * 出生日期(43-49)
         * 性別(50)
         * 發卡日期(51-57)
         * 卡片註銷註記(58)
         * 保險人代碼(59-60)
         * 保險對象身份註記(61)
         * 卡片有效期限(62-68)
         * 就醫可用次數(69-70)
         * 新生兒依附註記之新生兒出生日期(71-77)
         * 新生兒依附註記之新生兒胞胎註記(78)
         */
        public BasicData(byte[] pBuffer)
        {
            CardNumber = Function.ByteArrayToString(12, pBuffer, 0);
            Name = Function.ByteArrayToString(20, pBuffer, 12).Trim();
            IDNumber = Function.ByteArrayToString(10, pBuffer, 32);
            BirthdayStr = Function.ByteArrayToString(7, pBuffer, 42);
            Birthday = DateTimeExtensions.TWDateStringToDateOnly(BirthdayStr);
            Gender = Function.ByteArrayToString(1, pBuffer, 49).Equals("M") ? StringRes.Male : StringRes.Female;
            string dateString = Function.ByteArrayToString(7, pBuffer, 50);
            CardReleaseDate = DateTimeExtensions.TWDateStringToDateOnly(dateString);
            CardLogoutMark = Function.ByteArrayToString(1, pBuffer, 57);
            InsurerID = Function.ByteArrayToString(2, pBuffer, 58);
            InsuranceMark = Function.ByteArrayToString(1, pBuffer, 60);
            ValidityPeriod = DateTimeExtensions.TWDateStringToDateOnly(Function.ByteArrayToString(7, pBuffer, 61));
            AvailableTimes = int.Parse(Function.ByteArrayToString(2, pBuffer, 68));
            NewBornBirthday = Function.ByteArrayToString(7, pBuffer, 70);
            NewBornMark = Function.ByteArrayToString(1, pBuffer, 77);
        }
        public BasicData(string cardNum, string name, string idNum, DateTime birthday, string birth, string gender, DateTime cardRelease, string cardLogout, string insurerID, string insuranceMark, DateTime validityPeriod, int availableTimes, string newBornBirthday, string newBornMark)
        {
            CardNumber = cardNum;
            Name = name;
            IDNumber = idNum;
            Birthday = birthday;
            BirthdayStr = birth;
            Gender = gender;
            CardReleaseDate = cardRelease;
            CardLogoutMark = cardLogout;
            InsurerID = insurerID;
            InsuranceMark = insuranceMark;
            ValidityPeriod = validityPeriod;
            AvailableTimes = availableTimes;
            NewBornBirthday = newBornBirthday;
            NewBornMark = newBornMark;
        }
    }
}
