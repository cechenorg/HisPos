﻿using His_Pos.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Person;
using System.Xml;
using His_Pos.Class.Product;

namespace His_Pos.Class
{
    public class Prescription
    {
        public Prescription()
        {
            IcCard = new IcCard();
            Pharmacy = new Pharmacy();
            Treatment = new Treatment();
            Medicines = new List<Medicine>();
        }

        public Prescription(IcCard icCard, Pharmacy pharmacy, Treatment treatment, List<Medicine> medicines)
        {
            IcCard = icCard;
            Pharmacy = pharmacy;
            Treatment = treatment;
            Medicines = medicines;
        }

        public IcCard IcCard { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Treatment Treatment { get; set; }
        public List<Medicine> Medicines { get; set; }
        public string OriginalMedicalNumber { get; set; } //D43原處方就醫序號
    }
}
