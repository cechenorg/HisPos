using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport
{
    public class StockTakingOTCDetailReport : ObservableObject
    {
        public StockTakingOTCDetailReport()
        {
        }

        public StockTakingOTCDetailReport(DataRow r)
        {
            InvRecSourceID = r.Field<string>("StoTakDet_MasterID");
            Price = r.Field<int>("Price");
            Type = r.Field<string>("TypeID");
            Time = r.Field<DateTime>("Time");
        }

        private string invRecSourceID;
        private string type;
        private int price;
        private int count;
        private DateTime time;

        public string InvRecSourceID
        {
            get => invRecSourceID;
            set
            {
                Set(() => InvRecSourceID, ref invRecSourceID, value);
            }
        }

        public string Type
        {
            get => type;
            set
            {
                Set(() => Type, ref type, value);
            }
        }

        public int Price
        {
            get => price;
            set
            {
                Set(() => Price, ref price, value);
            }
        }

        public int Count
        {
            get => count;
            set
            {
                Set(() => Count, ref count, value);
            }
        }
        public DateTime Time
        {
            get => time;
            set
            {
                Set(() => Time, ref time, value);
            }
        }
    }
}