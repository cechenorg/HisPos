﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.Institution
{
    public class PrescriptionSearchInstitution : Institution
    {
        public PrescriptionSearchInstitution(DataRow r)
        {
            ID = r.Field<string>("Ins_ID");
            Name = r.Field<string>("Ins_Name");
            FullName = ID + " " + Name;
            Selected = true;
        }

        private bool selected;

        public bool Selected
        {
            get => selected;
            set
            {
                Set(() => Selected, ref selected, value);
            }
        }
    }
}
