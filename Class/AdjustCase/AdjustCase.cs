using System.Data;

namespace His_Pos.Class.AdjustCase
{
    public class AdjustCase : Selection
    {
        public AdjustCase()
        {
        }

        public AdjustCase(DataRow dataRow)
        {
            Id = dataRow["ADJUSTCASE_ID"].ToString();
            Name = dataRow["ADJUSTCASE_NAME"].ToString();
            FullName = dataRow["FULLNAME"].ToString();
        }
    }
}