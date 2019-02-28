using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Manufactory.ManufactoryManagement
{
    public class ManufactoryPrincipal
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
        public ManufactoryPrincipal(DataRow row)
        {

        }
    }
}
