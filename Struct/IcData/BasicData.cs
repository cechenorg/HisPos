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
            var f = new Function();

            CardNumber = f.ByteArrayToString(12,pBuffer,0);
            Name = f.ByteArrayToString(20, pBuffer, 12).Trim();
            IcNumber = f.ByteArrayToString(10, pBuffer, 32);
            Birthday = f.ByteArrayToString(7, pBuffer, 42);
            Gender = f.ByteArrayToString(1, pBuffer, 49).Equals("M");
            CardReleaseDate = f.ByteArrayToString(7, pBuffer, 50);
            CardLogoutMark = f.ByteArrayToString(1, pBuffer, 57);
            EmergencyTel = f.ByteArrayToString(14, pBuffer, 58);
        }
    }
}
