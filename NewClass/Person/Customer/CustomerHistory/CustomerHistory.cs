using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistory:ObservableObject
    {
        public CustomerHistory() { }
        public CustomerHistory(DataRow r, HistoryType t)
        {
            Type = t;
            AdjustDate = r.Field<DateTime>("AdjustDate");
            Title = r.Field<string>("");
            Status = r.Field<bool>("");
        }
        public HistoryType Type { get; }
        public DateTime AdjustDate { get; set; } //日期
        public string Title { get; set; }//標題
        public bool Status { get; set; }//已調劑處方:是否未過卡 已登錄處方:是否傳送藥健康 預約:無
    }
}
