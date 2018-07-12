using System.Data;

namespace His_Pos.Class.Division
{
    public class Division : Selection
    {
        public Division()
        {
        }

        public Division(DataRow dataRow)
        {
            Id = dataRow["HISDIV_ID"].ToString();
            Name = dataRow["HISDIV_NAME"].ToString();
            FullName = dataRow["FULLNAME"].ToString();
        }
    }
}
