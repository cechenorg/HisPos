﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class Hospital : IInstitution
    {
        #region --IInstitution--
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion
        public MedicalPersonnel Doctor { get; set; }
        public Selection Division { get; set; }
    }
}
