using System;
using System.Data;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouse : ICloneable
    {
        private WareHouse() { }
        public WareHouse(DataRow row)
        {
            ID = row.Field<int>("War_ID").ToString();
            Name = row.Field<string>("War_Name");
        }

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        #endregion

        public object Clone()
        {
            WareHouse wareHouse = new WareHouse();

            wareHouse.ID = ID;
            wareHouse.Name = Name;

            return wareHouse;
        }
    }
}