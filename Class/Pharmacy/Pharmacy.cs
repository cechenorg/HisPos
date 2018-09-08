﻿using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.Person;
using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.Pharmacy
{
    public class Pharmacy : Selection
    {
        public Pharmacy()
        {
            MedicalPersonnel = new MedicalPersonnel();
        }
        public Pharmacy(DataRow row) {
            MedicalPersonnel = new MedicalPersonnel(row);
        }
        public Pharmacy(string id, string name,string address,string tel)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
            Address = address;
            Tel = tel;
            MedicalPersonnel = new MedicalPersonnel();
        }
        public Pharmacy(XmlNode xml) {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = xml.SelectSingleNode("d25") == null ? null : xml.SelectSingleNode("d25").InnerText;
        }

        public Pharmacy(Ddata d)
        {
            MedicalPersonnel = new MedicalPersonnel();
            MedicalPersonnel.IcNumber = !string.IsNullOrEmpty(d.Dbody.D25) ? d.Dbody.D25 : string.Empty;
        }

        public MedicalPersonnel MedicalPersonnel { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
    }
}
