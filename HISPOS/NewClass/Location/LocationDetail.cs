using System.Data;

namespace His_Pos.Class.Location
{
    public class LocationDetail
    {
        public LocationDetail(DataRow row)
        {
            id = row["LOC_ID"].ToString();
            name = row["LOCD_NAME"].ToString();
            locdrow = row["LOCD_ROW"].ToString();
            locdcolumn = row["LOCD_COLUMN"].ToString();
            status = row["IsExist"].ToString();
        }

        public LocationDetail(string locid, string locdname, string row, string column, string locdisExist)
        {
            id = locid;
            name = locdname;
            locdrow = row;
            locdcolumn = column;
            status = locdisExist;
        }

        public string id;
        public string name;
        public string locdrow;
        public string locdcolumn;
        public string status;
    }
}