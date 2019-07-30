using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Prescription.EditRecords
{
    public class PrescriptionEditRecord : ObservableObject
    {
        public PrescriptionEditRecord(DataRow r)
        {
            ProductID = r.Field<string>("Pro_ID");
            englishName = r.Field<string>("EnglishName");
            chineseName = r.Field<string>("ChineseName");
            Amount = r.Field<double>("Amount");
            Note = r.Field<string>("Note");
            var tc = new TaiwanCalendar();
            Time = r.Field<DateTime>("RecTime");
            TimeString = $"{tc.GetYear(Time)}/{Time:MM/dd HH:mm}";
        }

        #region Properties
        private string productID;
        public string ProductID
        {
            get => productID;
            set
            {
                Set(() => ProductID, ref productID, value);
            }
        }
        private readonly string englishName;
        private readonly string chineseName;
        public string FullName
        {
            get
            { 
                if(!string.IsNullOrEmpty(englishName))
                    return (englishName.Contains(" ")? englishName.Substring(0, englishName.IndexOf(" ")) : englishName) + chineseName;
                return !string.IsNullOrEmpty(chineseName) ? chineseName : string.Empty;
            }
        }
        private double amount;
        public double Amount
        {
            get => amount;
            set
            {
                Set(() => Amount, ref amount, value);
            }
        }
        private string note;
        public string Note
        {
            get => note;
            set
            {
                Set(() => Note, ref note, value);
            }
        }
        private DateTime time;
        public DateTime Time
        {
            get => time;
            set
            {
                Set(() => Time, ref time, value);
            }
        }
        private string timeString;
        public string TimeString
        {
            get => timeString;
            set
            {
                Set(() => TimeString, ref timeString, value);
            }
        }
        #endregion
    }
}
