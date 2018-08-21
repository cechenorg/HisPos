using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
  public  class WareHouse : Selection
    {
        public WareHouse() {

        }
        public WareHouse(DataRow dataRow) {
            Id = dataRow["PROWAR_ID"].ToString();
            Name = dataRow["PROWAR_NAME"].ToString();
        }
    }
}
