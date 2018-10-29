using System.Data;

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
