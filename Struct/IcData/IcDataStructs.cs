using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Struct.IcData
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
            Function f = new Function();
            CardNumber = f.ByteArrayToString(12,pBuffer,0);
            //
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
    public struct TreatmentData
    {
        public string TreatmentCategory;
        public string NewbornTreatmentMark;
        public string TreatmentDateTime;
        public string MakeUpMark;
        public string MedicalNumber;
        public string HospitalId;
        public string TreatmentFee;
        public string TreatmentCopaymentFee;
        public string HospitalizationFee;
        public string HospitalizationCopaymentFeeLess;
        public string HospitalizationCopaymentFeeMore;
        /*
         * 就醫類別(85-86)
         * 新生兒就醫註記(87)
         * 就診日期時間(88-100)
         * 補卡註記(101)
         * 就醫序號(102-105)
         * 醫療院所代碼(106-115)
         * 門診醫療費用【當次】(116-123)
         * 門診部分負擔費用【當次】(124-131)
         * 住院醫療費用【當次】(132-139)
         * 住院部分負擔費用【當次急性30天， 慢性180天以下】(140-146)
         * 住院部分負擔費用【當次急性31天， 慢性181天以上】(147-153)
         */
        public TreatmentData(byte[] pBuffer, int startIndex)
        {
            var treatmentCategoryArr = new byte[2];
            Array.Copy(pBuffer, startIndex, treatmentCategoryArr, 0, 2);
            TreatmentCategory = Encoding.GetEncoding(950).GetString(treatmentCategoryArr);

            var newbornTreatmentMarkArr = new byte[1];
            Array.Copy(pBuffer, startIndex + 2, newbornTreatmentMarkArr, 0, 1);
            NewbornTreatmentMark = Encoding.GetEncoding(950).GetString(newbornTreatmentMarkArr);

            var treatmentDateTimeArr = new byte[13];
            Array.Copy(pBuffer, startIndex + 3, treatmentDateTimeArr, 0, 13);
            TreatmentDateTime = Encoding.GetEncoding(950).GetString(treatmentDateTimeArr);

            var makeUpMarkArr = new byte[1];
            Array.Copy(pBuffer, startIndex + 16, makeUpMarkArr, 0, 1);
            MakeUpMark = Encoding.GetEncoding(950).GetString(makeUpMarkArr);

            var medicalNumberArr = new byte[4];
            Array.Copy(pBuffer, startIndex + 17, medicalNumberArr, 0, 4);
            MedicalNumber = Encoding.GetEncoding(950).GetString(medicalNumberArr);

            var hospitalIdArr = new byte[10];
            Array.Copy(pBuffer, startIndex + 21, hospitalIdArr, 0, 10);
            HospitalId = Encoding.GetEncoding(950).GetString(hospitalIdArr);

            var treatmentFeeArr = new byte[8];
            Array.Copy(pBuffer, startIndex + 31, treatmentFeeArr, 0, 1);
            TreatmentFee = Encoding.GetEncoding(950).GetString(treatmentFeeArr);

            var treatmentCopaymentFeeArr = new byte[8];
            Array.Copy(pBuffer, startIndex + 39, treatmentCopaymentFeeArr, 0, 8);
            TreatmentCopaymentFee = Encoding.GetEncoding(950).GetString(treatmentCopaymentFeeArr);

            var hospitalizationFeeArr = new byte[8];
            Array.Copy(pBuffer, startIndex + 47, hospitalizationFeeArr, 0, 8);
            HospitalizationFee = Encoding.GetEncoding(950).GetString(hospitalizationFeeArr);

            var hospitalizationCopaymentFeeLessArr = new byte[8];
            Array.Copy(pBuffer, startIndex + 55, hospitalizationCopaymentFeeLessArr, 0, 8);
            HospitalizationCopaymentFeeLess = Encoding.GetEncoding(950).GetString(hospitalizationCopaymentFeeLessArr);

            var hospitalizationCopaymentFeeMoreArr = new byte[8];
            Array.Copy(pBuffer, startIndex + 39, hospitalizationCopaymentFeeMoreArr, 0, 8);
            HospitalizationCopaymentFeeMore = Encoding.GetEncoding(950).GetString(hospitalizationCopaymentFeeMoreArr);
        }
    }
}
