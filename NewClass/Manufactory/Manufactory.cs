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

        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        #endregion
    }
}
