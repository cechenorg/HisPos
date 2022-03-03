using System.Collections.Generic;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription.ImportDeclareXml
{
    public class ImportDeclareXml
    {
        [XmlRoot(ElementName = "pdata")]
        public class Pdata
        {
            [XmlElement(ElementName = "p1")]
            public string P1 { get; set; }

            [XmlElement(ElementName = "p2")]
            public string P2 { get; set; }

            [XmlElement(ElementName = "p3")]
            public string P3 { get; set; }

            [XmlElement(ElementName = "p4")]
            public string P4 { get; set; }

            [XmlElement(ElementName = "p5")]
            public string P5 { get; set; }

            [XmlElement(ElementName = "p6")]
            public string P6 { get; set; }

            [XmlElement(ElementName = "p7")]
            public string P7 { get; set; }

            [XmlElement(ElementName = "p8")]
            public string P8 { get; set; }

            [XmlElement(ElementName = "p9")]
            public string P9 { get; set; }

            [XmlElement(ElementName = "p10")]
            public string P10 { get; set; }

            [XmlElement(ElementName = "p11")]
            public string P11 { get; set; }

            [XmlElement(ElementName = "p12")]
            public string P12 { get; set; }

            [XmlElement(ElementName = "p13")]
            public string P13 { get; set; }

            [XmlElement(ElementName = "p15")]
            public string P15 { get; set; }
        }

        [XmlRoot(ElementName = "ddata")]
        public class Ddata
        {
            [XmlElement(ElementName = "d1")]
            public string D1 { get; set; }

            [XmlElement(ElementName = "d2")]
            public string D2 { get; set; }

            [XmlElement(ElementName = "d3")]
            public string D3 { get; set; }

            [XmlElement(ElementName = "d4")]
            public string D4 { get; set; }

            [XmlElement(ElementName = "d5")]
            public string D5 { get; set; }

            [XmlElement(ElementName = "d6")]
            public string D6 { get; set; }

            [XmlElement(ElementName = "d7")]
            public string D7 { get; set; }

            [XmlElement(ElementName = "d8")]
            public string D8 { get; set; }

            [XmlElement(ElementName = "d9")]
            public string D9 { get; set; }

            [XmlElement(ElementName = "d13")]
            public string D13 { get; set; }

            [XmlElement(ElementName = "d14")]
            public string D14 { get; set; }

            [XmlElement(ElementName = "d15")]
            public string D15 { get; set; }

            [XmlElement(ElementName = "d16")]
            public string D16 { get; set; }

            [XmlElement(ElementName = "d17")]
            public string D17 { get; set; }

            [XmlElement(ElementName = "d18")]
            public string D18 { get; set; }

            [XmlElement(ElementName = "d20")]
            public string D20 { get; set; }

            [XmlElement(ElementName = "d21")]
            public string D21 { get; set; }

            [XmlElement(ElementName = "d22")]
            public string D22 { get; set; }

            [XmlElement(ElementName = "d23")]
            public string D23 { get; set; }

            [XmlElement(ElementName = "d24")]
            public string D24 { get; set; }

            [XmlElement(ElementName = "d25")]
            public string D25 { get; set; }

            [XmlElement(ElementName = "d26")]
            public string D26 { get; set; }

            [XmlElement(ElementName = "d30")]
            public string D30 { get; set; }

            [XmlElement(ElementName = "d31")]
            public string D31 { get; set; }

            [XmlElement(ElementName = "d32")]
            public string D32 { get; set; }

            [XmlElement(ElementName = "d33")]
            public string D33 { get; set; }

            [XmlElement(ElementName = "d35")]
            public string D35 { get; set; }

            [XmlElement(ElementName = "d36")]
            public string D36 { get; set; }

            [XmlElement(ElementName = "d37")]
            public string D37 { get; set; }

            [XmlElement(ElementName = "d38")]
            public string D38 { get; set; }

            [XmlElement(ElementName = "d43")]
            public string D43 { get; set; }

            [XmlElement(ElementName = "d44")]
            public string D44 { get; set; }

            [XmlElement(ElementName = "pdata")]
            public List<Pdata> Pdatas { get; set; }
        }
    }
}