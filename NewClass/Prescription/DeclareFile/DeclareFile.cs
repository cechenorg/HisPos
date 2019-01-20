using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using His_Pos.NewClass.Product.Medicine;
using His_Pos.Service;
using JetBrains.Annotations;
using Int16 = System.Int16;

namespace His_Pos.NewClass.Prescription.DeclareFile
{
    public enum PDataType
    {
        Service = 0,//藥事服務費
        SimpleForm = 1,//日劑藥費
    }

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
        [XmlElement(ElementName = "t13")]
        public string T13 { get; set; }
        [XmlElement(ElementName = "t14")]
        public string T14 { get; set; }
    }

    [XmlRoot(ElementName = "pharmacy")]
    public class Pharmacy
    {
        [XmlElement(ElementName = "tdata")]
        public Class.Declare.Tdata Tdata { get; set; }
        [XmlElement(ElementName = "ddata")]
        public List<Ddata> Ddata { get; set; }
    }

    [XmlRoot(ElementName = "ddata")]
    public class Ddata
    {
        [XmlElement(ElementName = "dhead")]
        public Dhead Dhead { get; set; }
        [XmlElement(ElementName = "dbody")]
        public Dbody Dbody { get; set; }
    }

    [XmlRoot(ElementName = "dhead")]
    public class Dhead
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
    }

    [XmlRoot(ElementName = "dbody")]
    public class Dbody
    {
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
        public List<Pdata> Pdata { get; set; }
    }

    [XmlRoot(ElementName = "pdata")]
    public class Pdata
    {
        public Pdata() { }
        public Pdata(Medicine m,string serial)
        {
            if (m is MedicineNHI && !m.PaySelf)
            {
                P1 = "1";
                P2 = m.ID;
                P7 = $"{m.Amount:00000.0}";
                P8 = $"{m.NHIPrice:0000000.00}";
                P9 = $"{Math.Round(m.NHIPrice * m.Amount, 0, MidpointRounding.AwayFromZero):0000000}";
                P3 = $"{m.Dosage:0000.00}";
                P4 = m.Usage.Name;
                P5 = m.Position.Name;
                P10 = serial;
                P11 = $"{m.Days:00}";
                P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTime(DateTime.Now);
                P13 = P12;
                PaySelf = false;
            }
            else
            {
                P1 = "0";
                P2 = m.ID;
                P7 = m.Amount.ToString();
                var dosage = m.Dosage is null ? string.Empty : m.Dosage.ToString();
                P3 = dosage;
                P4 = m.Usage.Name;
                P5 = m.Position.Name;
                P8 = string.Empty;
                P9 = string.Empty;
                P10 = string.Empty;
                var days = m.Days is null ? string.Empty : m.Days.ToString();
                P11 = days;
                P12 = string.Empty;
                P13 = P12;
                PaySelf = m.PaySelf;
            }
        }

        public Pdata(PDataType type,string code,int percentage,int amount)
        {
            switch (type)
            {
                case PDataType.Service:
                    P1 = "9";
                    P2 = code;
                    P3 = string.Empty;
                    P4 = string.Empty;
                    P5 = string.Empty;
                    P6 = percentage.ToString();
                    P7 = $"{amount:00000.0}";
                    switch (code)
                    {
                        case "05202B":
                        case "05223B":
                            P8 = $"{48:0000000.00}";
                            P9 = $"{48 * percentage * 0.01:00000000}";
                            break;
                        case "05210B":
                            P8 = $"{69:0000000.00}";
                            P9 = $"{69 * percentage * 0.01:00000000}";
                            break;
                        case "05206B":
                            P8 = $"{59:0000000.00}";
                            P9 = $"{59 * percentage * 0.01:00000000}";
                            break;
                    }
                    P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTime(DateTime.Today);
                    P13 = P12;
                    PaySelf = false;
                    break;
                case PDataType.SimpleForm:
                    P1 = "1";
                    switch (code)
                    {
                        case "22":
                            P2 = "MA1";
                            break;
                        case "31":
                            P2 = "MA2";
                            break;
                        case "37":
                            P2 = "MA3";
                            break;
                        case "41":
                            P2 = "MA4";
                            break;
                    }
                    P3 = $"{1.0:0000.00}";
                    P4 = string.Empty;
                    P5 = string.Empty;
                    P6 = percentage.ToString();
                    P7 = $"{amount:00000.0}";
                    P8 = $"{int.Parse(code):00000.0}";
                    P9 = $"{int.Parse(code) * amount:00000000}";
                    P11 = amount.ToString().PadLeft(2,'0');
                    P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTime(DateTime.Today);
                    P13 = DateTimeExtensions.ConvertToTaiwanCalenderWithTime(DateTime.Today.AddDays(amount-1));
                    PaySelf = false;
                    break;
            }
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
        private string _p10;
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
        [XmlIgnore]
        public bool PaySelf { get; set; }
    }
}
