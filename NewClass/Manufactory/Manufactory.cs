using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactory
    {
        public Manufactory(DataRow row)
        {
            ID = row.Field<int>("Man_ID").ToString();
            Name = row.Field<string>("Man_NickName");
            Telephone = row.Field<string>("Man_Telephone");
        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        #endregion
    }
}
