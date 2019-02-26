using System.Data;

namespace His_Pos.Class.CustomerHistory
{
    public class CustomerHistoryHis : CustomerHistoryDetail
    {

        public CustomerHistoryHis(DataRow dataRow)
        {
            MedicineName = dataRow["COL0"].ToString();
            Dosage = dataRow["COL1"].ToString();
            Usage = dataRow["COL2"].ToString();
            Days = dataRow["COL3"].ToString();
            Total = dataRow["COL4"].ToString();
            MedId = dataRow["ID"].ToString();
        }

        public string MedicineName { get; }
        public string Usage { get; }
        public string Days { get; }
        public string Dosage { get; }
        public string Total { get; }
        public string MedId { get; }
    }
}