using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistoryBase:ObservableObject
    {
        public CustomerHistoryBase() { }
        public CustomerHistoryBase(DataRow r, HistoryType t)
        {
            Date = (DateTime) r[""];
            Title = r[""].ToString();
            SubTitle = r[""].ToString();
        }
        public HistoryType Type { get; }
        public DateTime Date { get; set; } //日期
        public string Title { get; set; }//標題
        public string SubTitle { get; set; }//副標題
    }
}
