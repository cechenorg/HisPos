using System.Collections.Generic;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFile.CooperativeFtpFile
{
    [XmlRoot(ElementName = "outpatient")]
    public class CooperativeFtpFile
    {
        public CooperativeFtpFile()
        {
        }

        [XmlElement(ElementName = "tdata")]
        public Tdata Tdata { get; set; }

        [XmlElement(ElementName = "ddata")]
        public List<Ddata> Ddata { get; set; }
    }

    [XmlRoot(ElementName = "tdata")]
    public class Tdata
    {
        public Tdata()
        {
        }

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

        [XmlElement(ElementName = "t13")]
        public string T13 { get; set; }

        [XmlElement(ElementName = "t14")]
        public string T14 { get; set; }

        [XmlElement(ElementName = "t15")]
        public string T15 { get; set; }

        [XmlElement(ElementName = "t16")]
        public string T16 { get; set; }

        [XmlElement(ElementName = "t17")]
        public string T17 { get; set; }

        [XmlElement(ElementName = "t18")]
        public string T18 { get; set; }

        [XmlElement(ElementName = "t19")]
        public string T19 { get; set; }

        [XmlElement(ElementName = "t20")]
        public string T20 { get; set; }

        [XmlElement(ElementName = "t21")]
        public string T21 { get; set; }

        [XmlElement(ElementName = "t22")]
        public string T22 { get; set; }

        [XmlElement(ElementName = "t23")]
        public string T23 { get; set; }

        [XmlElement(ElementName = "t24")]
        public string T24 { get; set; }

        [XmlElement(ElementName = "t25")]
        public string T25 { get; set; }

        [XmlElement(ElementName = "t26")]
        public string T26 { get; set; }

        [XmlElement(ElementName = "t27")]
        public string T27 { get; set; }

        [XmlElement(ElementName = "t28")]
        public string T28 { get; set; }

        [XmlElement(ElementName = "t29")]
        public string T29 { get; set; }

        [XmlElement(ElementName = "t30")]
        public string T30 { get; set; }

        [XmlElement(ElementName = "t31")]
        public string T31 { get; set; }

        [XmlElement(ElementName = "t32")]
        public string T32 { get; set; }

        [XmlElement(ElementName = "t33")]
        public string T33 { get; set; }

        [XmlElement(ElementName = "t34")]
        public string T34 { get; set; }

        [XmlElement(ElementName = "t35")]
        public string T35 { get; set; }

        [XmlElement(ElementName = "t36")]
        public string T36 { get; set; }

        [XmlElement(ElementName = "t37")]
        public string T37 { get; set; }

        [XmlElement(ElementName = "t38")]
        public string T38 { get; set; }

        [XmlElement(ElementName = "t39")]
        public string T39 { get; set; }

        [XmlElement(ElementName = "t40")]
        public string T40 { get; set; }

        [XmlElement(ElementName = "t41")]
        public string T41 { get; set; }

        [XmlElement(ElementName = "t42")]
        public string T42 { get; set; }
    }

    [XmlRoot(ElementName = "ddata")]
    public class Ddata
    {
        public Ddata()
        {
        }

        [XmlElement(ElementName = "dhead")]
        public Dhead Dhead { get; set; }

        [XmlElement(ElementName = "dbody")]
        public Dbody Dbody { get; set; }
    }

    [XmlRoot(ElementName = "dhead")]
    public class Dhead
    {
        public Dhead()
        {
        }

        [XmlElement(ElementName = "d1")]
        public string D1 { get; set; }

        [XmlElement(ElementName = "d2")]
        public string D2 { get; set; }
    }

    [XmlRoot(ElementName = "dbody")]
    public class Dbody
    {
        public Dbody()
        {
        }

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

        [XmlElement(ElementName = "d10")]
        public string D10 { get; set; }

        [XmlElement(ElementName = "d11")]
        public string D11 { get; set; }

        [XmlElement(ElementName = "d12")]
        public string D12 { get; set; }

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

        [XmlElement(ElementName = "d19")]
        public string D19 { get; set; }

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

        [XmlElement(ElementName = "d27")]
        public string D27 { get; set; }

        [XmlElement(ElementName = "d28")]
        public string D28 { get; set; }

        [XmlElement(ElementName = "d29")]
        public string D29 { get; set; }

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

        [XmlElement(ElementName = "d39")]
        public string D39 { get; set; }

        [XmlElement(ElementName = "d40")]
        public string D40 { get; set; }

        [XmlElement(ElementName = "d41")]
        public string D41 { get; set; }

        [XmlElement(ElementName = "d42")]
        public string D42 { get; set; }

        [XmlElement(ElementName = "d43")]
        public string D43 { get; set; }

        [XmlElement(ElementName = "d44")]
        public string D44 { get; set; }

        [XmlElement(ElementName = "d45")]
        public string D45 { get; set; }

        [XmlElement(ElementName = "d46")]
        public string D46 { get; set; }

        [XmlElement(ElementName = "d47")]
        public string D47 { get; set; }

        [XmlElement(ElementName = "d48")]
        public string D48 { get; set; }

        [XmlElement(ElementName = "d49")]
        public string D49 { get; set; }

        [XmlElement(ElementName = "d50")]
        public string D50 { get; set; }

        [XmlElement(ElementName = "pdata")]
        public List<Pdata> Pdata { get; set; }
    }

    [XmlRoot(ElementName = "pdata")]
    public class Pdata
    {
        public Pdata()
        {
        }

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

        [XmlElement(ElementName = "p14")]
        public string P14 { get; set; }

        [XmlElement(ElementName = "p15")]
        public string P15 { get; set; }

        [XmlElement(ElementName = "p16")]
        public string P16 { get; set; }

        [XmlElement(ElementName = "p17")]
        public string P17 { get; set; }

        [XmlElement(ElementName = "p18")]
        public string P18 { get; set; }

        [XmlElement(ElementName = "p19")]
        public string P19 { get; set; }

        [XmlElement(ElementName = "p20")]
        public string P20 { get; set; }

        [XmlElement(ElementName = "p21")]
        public string P21 { get; set; }
    }
}