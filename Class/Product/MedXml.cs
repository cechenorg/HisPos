using System.Collections.Generic;
using System.Xml.Serialization;

namespace His_Pos.Class.Product
{
    [XmlRoot(ElementName = "Col2")]
    public class Col2
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col3")]
    public class Col3
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col4")]
    public class Col4
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col5")]
    public class Col5
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col6")]
    public class Col6
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col7")]
    public class Col7
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col8")]
    public class Col8
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col9")]
    public class Col9
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col10")]
    public class Col10
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col11")]
    public class Col11
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col12")]
    public class Col12
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col13")]
    public class Col13
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Col14")]
    public class Col14
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "row")]
    public class Row
    {
        [XmlElement(ElementName = "Col2")]
        public Col2 Col2 { get; set; }
        [XmlElement(ElementName = "Col3")]
        public Col3 Col3 { get; set; }
        [XmlElement(ElementName = "Col4")]
        public Col4 Col4 { get; set; }
        [XmlElement(ElementName = "Col5")]
        public Col5 Col5 { get; set; }
        [XmlElement(ElementName = "Col6")]
        public Col6 Col6 { get; set; }
        [XmlElement(ElementName = "Col7")]
        public Col7 Col7 { get; set; }
        [XmlElement(ElementName = "Col8")]
        public Col8 Col8 { get; set; }
        [XmlElement(ElementName = "Col9")]
        public Col9 Col9 { get; set; }
        [XmlElement(ElementName = "Col10")]
        public Col10 Col10 { get; set; }
        [XmlElement(ElementName = "Col11")]
        public Col11 Col11 { get; set; }
        [XmlElement(ElementName = "Col12")]
        public Col12 Col12 { get; set; }
        [XmlElement(ElementName = "Col13")]
        public Col13 Col13 { get; set; }
        [XmlElement(ElementName = "Col14")]
        public Col14 Col14 { get; set; }
    }

    [XmlRoot(ElementName = "table")]
    public class Table
    {
        [XmlElement(ElementName = "row")]
        public List<Row> Row { get; set; }
    }
}
