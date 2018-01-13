using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class MedicalPersonnel : IPerson
    {
        #region --IPerson--
        public string Id { get; set; }
        public string Name { get; set; }
        public string IcNumber { get; set; }
        #endregion
    }
}
