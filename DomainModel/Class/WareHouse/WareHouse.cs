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
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                Set(() => IsSelected, ref isSelected, value);
            }
        }

        #endregion ----- Define Variables -----

        private WareHouse()
        {
        }

        public WareHouse(DataRow row)
        {
            ID = row.Field<int>("War_ID").ToString();
            Name = row.Field<string>("War_Name");
        }
        public WareHouse(string id, string name)
        {
            WareHouse wareHouse = new WareHouse();
            wareHouse.ID = id;
            wareHouse.Name = name;
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