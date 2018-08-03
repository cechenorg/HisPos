using His_Pos.Service;
using System;
using System.Data;
using System.Xml;

namespace His_Pos.Class.Person
{
    public class Customer : Person
    {
        public Customer()
        {
            IcCard = new IcCard();
        }

        public Customer(IcCard icCard)
        {
            IcCard = icCard;
        }

        public Customer(DataRow row)
        {
            Id = row["CUS_ID"].ToString();
            IcNumber = row["CUS_IDNUM"].ToString();
            Birthday = row["CUS_BIRTH"].ToString();
            Name = row["CUS_NAME"].ToString();
            Qname = row["CUS_QNAME"].ToString();
            Gender = row["CUS_GENDER"].ToString() == "" || Convert.ToBoolean(row["CUS_GENDER"].ToString());
            IcCard = new IcCard(row,DataSource.GetMedicalIcCard);
        }
        public Customer(XmlNode xml) {
            IcCard = new IcCard(xml);
            Name = xml.SelectSingleNode("d20") == null ? null : xml.SelectSingleNode("d20")?.InnerText;
            IcNumber = xml.SelectSingleNode("d3") == null ? null : xml.SelectSingleNode("d3")?.InnerText;
            Birthday = xml.SelectSingleNode("d6") == null ? null : DateTimeExtensions.BirthdayFormatConverter2(xml.SelectSingleNode("d6")?.InnerText);
        }
        public string Qname { get; set; }
        private bool gender;
        public bool Gender
        {
            get => gender;
            set
            {
                gender = value;
                NotifyPropertyChanged(nameof(Gender));
            }
        }
        public IcCard IcCard { get; set; }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
    }
}