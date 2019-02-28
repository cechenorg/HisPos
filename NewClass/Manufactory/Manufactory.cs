using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactory
    {
        public Manufactory(DataRow row)
        {
            ID = row.Field<int>("Man_ID").ToString();
            Name = row.Field<string>("Man_Name");
            NickName = row.Field<string>("Man_NickName");
            Telephone = row.Field<string>("Man_Telephone");
        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Telephone { get; set; }
        #endregion
    }
}
