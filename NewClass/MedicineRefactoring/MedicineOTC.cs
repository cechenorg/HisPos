﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.MedicineRefactoring
{
    public class MedicineOTC : Medicine
    {
        public MedicineOTC() : base() { }
        public MedicineOTC(DataRow r) : base(r)
        {

        }
    }
}
