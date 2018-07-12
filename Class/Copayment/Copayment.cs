using System.Data;

namespace His_Pos.Class.Copayment
{
    public class Copayment : Selection
    {
        public Copayment()
        {
        }

        public Copayment(DataRow dataRow)
        {
            Id = dataRow["HISCOP_ID"].ToString();
            Name = dataRow["HISCOP_NAME"].ToString();
            FullName = dataRow["FULLNAME"].ToString();
        }

        public int Point { get; set; }
    }
}