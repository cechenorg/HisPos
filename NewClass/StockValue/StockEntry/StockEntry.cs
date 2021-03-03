using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach.EntryDetailWindow;
using System;
using System.Data;

namespace His_Pos.NewClass.StockValue.StockEntry
{
    public class StockEntry
    {
        public StockEntry()
        {
        }

        public StockEntry(DataRow r)
        {
            Date = r.Field<DateTime>("Time");
            EntryName = r.Field<string>("Type");
            switch (r.Field<string>("Source"))
            {
                case "PreMasId":
                    Source = EntryDetailEnum.Adjust;
                    break;

                case "StoOrdID":
                    if (EntryName.Contains("進貨"))
                        Source = EntryDetailEnum.Purchase;
                    else
                        Source = EntryDetailEnum.Return;
                    break;

                case "StoTakMasID":
                default:
                    Source = EntryDetailEnum.StockTaking;
                    break;
            }
            SourceID = r.Field<string>("SourceID");
            Value = r.Field<double>("ValueDiff");
        }

        public DateTime Date { get; set; }
        public string EntryName { get; set; }
        public EntryDetailEnum Source { get; set; }
        public string SourceID { get; set; }
        public double Value { get; set; }
    }
}