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
        public Manufactory(DataRow dataRow)
        {
            ID = dataRow["Man_ID"].ToString();
            Name = dataRow["Man_NickName"].ToString();
            Telephone = dataRow["Man_Telephone"].ToString();
        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        #endregion
    }
}
