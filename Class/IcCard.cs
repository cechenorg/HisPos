using System;
using System.Collections.Generic;
using System.ComponentModel;
using His_Pos.Class.Person;
using System.Data;

namespace His_Pos.Class
{
    public class IcCard : INotifyPropertyChanged
    {
        public IcCard()
        {
            IcMarks = new IcMarks();
            IcNumber = "";
            MedicalNumber = "";
        }

        public IcCard(DataRow row,DataSource source)
        {
            switch (source) {
                case DataSource.InitMedicalIcCard:
                    IcNumber = "";
                    MedicalNumber = "";
                    break;
                case DataSource.GetMedicalIcCard:
                    IcNumber = "";
                    MedicalNumber = row["HISDECMAS_NUMBER"].ToString();
                    break;
            }
          
        }
       
        private string icNumber;

        public string IcNumber
        {
            get { return icNumber; }
            set
            {
                icNumber = value;
                NotifyPropertyChanged("IcNumber");
            }
        }//卡片號碼

        public IcMarks IcMarks { get; set; } = new IcMarks();//卡片註銷註記.保險對象身分註記.新生兒出生日期.新生兒胞胎註記
        public string SendDate { get; set; }//發卡日期
        public string ValidityPeriod { get; set; }//卡片有效期限
        public int AvailableTimes { get; set; }//就醫可用次數
        public IcCardPay IcCardPay { get; set; } //門診醫療費用 住院醫療費用
        public IcCardPrediction IcCardPrediction { get; set; } //預防保健項目
        public Pregnant Pregnant { get; set; } //孕婦檢查項目
        public Vaccination Vaccination { get; set; } //預防接種項目
        private string medicalNumber;

        public string MedicalNumber
        {
            get { return medicalNumber; }
            set
            {
                medicalNumber = value;
                NotifyPropertyChanged("MedicalNumber");
            }
        } //D7就醫序號

        //return 1 : 長度不足 2 : 性別碼錯誤 3 : 首碼錯誤 4 : 檢查碼錯誤
        public string CheckIcNumber(string str)
        {
            if (str.Trim().Length != 10) return "身份證字號長度不足\n";
            var firstEng = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            var icNum = str.ToUpper();
            var chackFirstEnd = false;
            var firstNo = Convert.ToByte(icNum.Trim().Substring(1, 1));
            if (firstNo > 2 || firstNo < 1)
            {
                return "身分證字號性別碼(第一位數字)錯誤，男性為 1 女性為 2\n";
            }
            int x;
            for (x = 0; x < firstEng.Count; x++)
            {
                if (icNum.Substring(0, 1) == firstEng[x])
                {
                    icNum = string.Format("{0}{1}", x + 10, icNum.Substring(1, 9));
                    chackFirstEnd = true;
                    break;
                }
            }
            if (!chackFirstEnd)
                return "3";
            int i = 1;
            int ss = int.Parse(icNum.Substring(0, 1));
            while (icNum.Length > i)
            {
                ss = ss + (int.Parse(icNum.Substring(i, 1)) * (10 - i));
                i++;
            }
            icNum = ss.ToString();
            if (str.Substring(9, 1) == "0")
            {
                if (icNum.Substring(icNum.Length - 1, 1) == "0")
                {
                    return "0";
                }
                return "4";
            }
            if (str.Substring(9, 1) == (10 - int.Parse(icNum.Substring(icNum.Length - 1, 1))).ToString())
            {
                return "0";
            }
            return "4";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}