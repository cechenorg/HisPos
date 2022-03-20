using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Globalization;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerRecord : ObservableObject
    {
        public CustomerRecord()
        {
        }

        public CustomerRecord(DataRow r)
        {
            TraDet_ProductID = r.Field<string>("TraDet_ProductID");
            Pro_ChineseName = r.Field<string>("Pro_ChineseName");
            TraDet_PriceSum = r.Field<int>("TraDet_PriceSum");
            TraDet_Amount = r.Field<int>("TraDet_Amount");

            DateTime dt = r.Field<DateTime>("TraMas_ChkoutTime");
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            TraMas_ChkoutTime = dt.ToString("yyy/MM/dd HH:mm:ss", culture);
        }

        public string TraDet_ProductID { get; }
        public string Pro_ChineseName { get; }
        public int TraDet_PriceSum { get; }
        public int TraDet_Amount { get; }

        public string TraMas_ChkoutTime { get; } //日期
    }
}