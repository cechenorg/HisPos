using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouse
    {
        public WareHouse(DataRow row)
        {
            ID = row["War_ID"].ToString();
            Name = row["War_Name"].ToString();
        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        #endregion
    }
}