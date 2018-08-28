using His_Pos.Service;
using System;
using System.Data;
using System.Xml;

namespace His_Pos.Class.Person
{
    public class Customer : Person, ICloneable
    {
        public Customer()
        {
            IcCard = new IcCard();
        }

        public Customer(IcCard icCard)
        {
            IcCard = icCard;
        }
        public Customer(Customer customer)
        {
            Id = customer.Id;
            IcNumber = customer.IcNumber;
            Birthday = customer.Birthday;
            Name = customer.Name;
            Qname = customer.Qname;
            Gender = customer.Gender;
            GenderName = customer.GenderName;
            IcCard = customer.IcCard;
          
        }
        public Customer(DataRow row,string type)
        {
            Id = row["CUS_ID"].ToString();
            IcNumber = row["CUS_IDNUM"].ToString();
            Birthday = DateTimeExtensions.BirthdayFormatConverter3(row["CUS_BIRTH"].ToString()).ToString();
            Name = row["CUS_NAME"].ToString();
            Qname = row["CUS_QNAME"].ToString();
            Gender = row["CUS_GENDER"].ToString() == "" || Convert.ToBoolean(row["CUS_GENDER"].ToString());
            
            if (type == "fromXml") 
             IcCard = new IcCard(row,DataSource.GetMedicalIcCard);
            if(type == "fromDb")
            {
              GenderName = row["CUS_GENDER"].ToString() == "True" ? "男" : "女";
                IcCard = new IcCard(row, DataSource.InitMedicalIcCard);
            }
             
        }
        public Customer(XmlNode xml) {
            IcCard = new IcCard(xml);
            Name = xml.SelectSingleNode("d20") == null ? null : xml.SelectSingleNode("d20")?.InnerText;
            IcNumber = xml.SelectSingleNode("d3") == null ? null : xml.SelectSingleNode("d3")?.InnerText;
            Gender = IcNumber.Substring(1, 1) == "1" ? true : false;
            Birthday = xml.SelectSingleNode("d6") == null ? null : DateTimeExtensions.BirthdayFormatConverter2(xml.SelectSingleNode("d6")?.InnerText);
        }
        public string Qname { get; set; }
        private bool _gender;
        public bool Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                NotifyPropertyChanged(nameof(Gender));
            }
        }

        private string _genderName;
        public string GenderName
        {
            get => _genderName;
            set
            {
                _genderName = value;
                NotifyPropertyChanged(nameof(Gender));
            }
        }

        private IcCard _icCard;

        public IcCard IcCard
        {
            get => _icCard;
            set
            {
                _icCard = value;
                NotifyPropertyChanged("IcCard");
            }
        }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
        public object Clone()
        {
            return new Customer(this);
        }
    }
}