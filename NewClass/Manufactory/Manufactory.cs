using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Manufactory
{
    public class Manufactory : ObservableObject, ICloneable
    {
        protected Manufactory()
        {
        }

        public Manufactory(DataRow row)
        {
            ID = row.Field<int>("Man_ID").ToString();
            Name = row.Field<string>("Man_Name");
            NickName = row.Field<string>("Man_NickName");
            Telephone = row.Field<string>("Man_Telephone");
        }

        #region ----- Define Variables -----

        private string id;
        private string name;
        private string nickName;
        private string telephone;

        public string ID
        {
            get { return id; }
            set { Set(() => ID, ref id, value); }
        }

        public string Name
        {
            get { return name; }
            set
            {
                Set(() => Name, ref name, value);
                RaisePropertyChanged(nameof(GetName));
            }
        }

        public string NickName
        {
            get { return nickName; }
            set
            {
                Set(() => NickName, ref nickName, value);
                RaisePropertyChanged(nameof(GetName));
            }
        }

        public string Telephone
        {
            get { return telephone; }
            set { Set(() => Telephone, ref telephone, value); }
        }

        public string GetName
        {
            get { return string.IsNullOrEmpty(NickName) ? Name : NickName; }
        }

        #endregion ----- Define Variables -----

        public object Clone()
        {
            Manufactory manufactory = new Manufactory();

            manufactory.ID = ID;
            manufactory.Name = Name;
            manufactory.NickName = NickName;
            manufactory.Telephone = Telephone;

            return manufactory;
        }
    }
}