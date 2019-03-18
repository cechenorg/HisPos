using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class CustomerHistory:ObservableObject
    {
        public CustomerHistory() { }
        public CustomerHistory(DataRow r)
        {
            switch (r.Field<string>("Type")) {
                case "Adjust":
                    Type = HistoryType.AdjustRecord;
                    break;
                case "Register":
                    Type = HistoryType.RegisterRecord;
                    break;
                case "Reserve":
                    Type = HistoryType.ReservedPrescription;
                    break; 
            }
            
            AdjustDate = r.Field<DateTime>("AdjustDate");
            SourceId = r.Field<int>("SourceId");
            InsName = r.Field<string>("Ins_Name");
            DivName = r.Field<string>("Div_Name");
            Title = r.Field<string>("");
            Status = r.Field<bool>("Status");
        } 
        public HistoryType Type { get; }
        public string InsName { get; set; }
        public string DivName { get; set; }
        public int SourceId { get; set; }
        public DateTime AdjustDate { get; set; } //日期
        public string Title { get; set; }//標題
        public bool Status { get; set; }//已調劑處方:是否未過卡 已登錄處方:是否傳送藥健康 預約:無
    }
}
