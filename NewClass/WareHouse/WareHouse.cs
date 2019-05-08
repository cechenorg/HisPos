using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouse : ObservableObject, ICloneable
    {

        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        #endregion

        private WareHouse() { }
        public WareHouse(DataRow row)
        {
            ID = row.Field<int>("War_ID").ToString();
            Name = row.Field<string>("War_Name");
        }
        
        public object Clone()
        {
            WareHouse wareHouse = new WareHouse();

            wareHouse.ID = ID;
            wareHouse.Name = Name;

            return wareHouse;
        }
    }
}