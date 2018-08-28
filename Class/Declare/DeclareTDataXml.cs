using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace His_Pos.Class.Declare
{
    [XmlRoot(ElementName = "tdata")]
    public class Tdata
    {
        [XmlElement(ElementName = "t1")]
        public string T1 { get; set; }
        [XmlElement(ElementName = "t2")]
        public string T2 { get; set; }
        [XmlElement(ElementName = "t3")]
        public string T3 { get; set; }
        [XmlElement(ElementName = "t4")]
        public string T4 { get; set; }
        [XmlElement(ElementName = "t5")]
        public string T5 { get; set; }
        [XmlElement(ElementName = "t6")]
        public string T6 { get; set; }
        [XmlElement(ElementName = "t7")]
        public string T7 { get; set; }
        [XmlElement(ElementName = "t8")]
        public string T8 { get; set; }
        [XmlElement(ElementName = "t9")]
        public string T9 { get; set; }
        [XmlElement(ElementName = "t10")]
        public string T10 { get; set; }
        [XmlElement(ElementName = "t11")]
        public string T11 { get; set; }
        [XmlElement(ElementName = "t12")]
        public string T12 { get; set; }
    }

    [XmlRoot(ElementName = "pharmacy")]
    public class Pharmacy
    {
        [XmlElement(ElementName = "tdata")]
        public Tdata Tdata { get; set; }
        [XmlElement(ElementName = "ddata")]
        public List<Ddata> Ddata { get; set; }
    }

}
