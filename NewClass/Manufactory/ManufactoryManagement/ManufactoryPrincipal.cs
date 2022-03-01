using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryPrincipal : ObservableObject, ICloneable
    {
        #region ----- Define Variables -----

        private int id;
        private string name;
        private string telephone;
        private string fax;
        private string email;
        private string note;
        private string line;

        public int ID
        {
            get { return id; }
            set { Set(() => ID, ref id, value); }
        }

        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value); }
        }

        public string Telephone
        {
            get { return telephone; }
            set { Set(() => Telephone, ref telephone, value); }
        }

        public string Fax
        {
            get { return fax; }
            set { Set(() => Fax, ref fax, value); }
        }

        public string Email
        {
            get { return email; }
            set { Set(() => Email, ref email, value); }
        }

        public string Line
        {
            get { return line; }
            set { Set(() => Line, ref line, value); }
        }

        public string Note
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }

        #endregion ----- Define Variables -----

        public ManufactoryPrincipal()
        {
            ID = -1;
            Name = "";
            Email = "";
            Fax = "";
            Line = "";
            Note = "";
            Telephone = "";
        }

        public ManufactoryPrincipal(DataRow row)
        {
            ID = row.Field<int>("ManPri_ID");
            Name = row.Field<string>("ManPri_Name");
            Email = row.Field<string>("ManPri_Email");
            Fax = row.Field<string>("ManPri_Fax");
            Line = row.Field<string>("ManPri_LINE");
            Note = row.Field<string>("ManPri_Note");
            Telephone = row.Field<string>("ManPri_Telephone");
        }

        #region ----- Define Functions -----

        public object Clone()
        {
            ManufactoryPrincipal newPrincipal = new ManufactoryPrincipal();

            newPrincipal.ID = ID;
            newPrincipal.Name = Name;
            newPrincipal.Email = Email;
            newPrincipal.Fax = Fax;
            newPrincipal.Line = Line;
            newPrincipal.Note = Note;
            newPrincipal.Telephone = Telephone;

            return newPrincipal;
        }

        #endregion ----- Define Functions -----
    }
}