using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryPrincipal : ICloneable
    {
        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Line { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        #endregion

        public ManufactoryPrincipal()
        {
            ID = "";
            Name = "";
            Email = "";
            Fax = "";
            Line = "";
            Note = "";
            Telephone = "";
        }
        public ManufactoryPrincipal(DataRow row)
        {

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
        #endregion
    }
}
