using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Declare.IcDataUpload
{
    public struct BasicData
    {
        public string CardNumber;
        public string Name;
        public string IcNumber;
        public string Birthday;
        public bool Gender;
        public string CardReleaseDate;
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
            var cardNumberArr = new byte[12];
            Array.Copy(pBuffer, 0, cardNumberArr, 0, 12);
            CardNumber = Encoding.GetEncoding(950).GetString(cardNumberArr);

            var nameArr = new byte[20];
            Array.Copy(pBuffer, 12, nameArr, 0, 20);
            Name = Encoding.GetEncoding(950).GetString(nameArr).Trim();

            var icNumberArr = new byte[10];
            Array.Copy(pBuffer, 32, icNumberArr, 0, 10);
            IcNumber = Encoding.GetEncoding(950).GetString(icNumberArr);

            var birthdayArr = new byte[20];
            Array.Copy(pBuffer, 42, birthdayArr, 0, 7);
            Birthday = Encoding.GetEncoding(950).GetString(birthdayArr);

            var genderArr = new byte[1];
            Array.Copy(pBuffer, 49, genderArr, 0, 1);
            Gender = Encoding.GetEncoding(950).GetString(genderArr).Equals("M");

            var cardReleaseDateArr = new byte[7];
            Array.Copy(pBuffer, 50, cardReleaseDateArr, 0, 7);
            CardReleaseDate = Encoding.GetEncoding(950).GetString(cardReleaseDateArr);

            var cardLogoutMarkArr = new byte[1];
            Array.Copy(pBuffer, 57, genderArr, 0, 1);
            CardLogoutMark = Encoding.GetEncoding(950).GetString(cardLogoutMarkArr);

            var emergencyTelArr = new byte[14];
            Array.Copy(pBuffer, 58, emergencyTelArr, 0, 14);
            EmergencyTel = Encoding.GetEncoding(950).GetString(emergencyTelArr).Trim();
        }
    }
    
}
