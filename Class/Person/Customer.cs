using System;
using System.Data;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Struct.IcData;

namespace His_Pos.Class.Person
{
    public class Customer : Person, ICloneable
    {
        public Customer()
        {
            IcCard = new IcCard();
        }

        public Customer(BasicData basicData)
        {
            var year = int.Parse(basicData.Birthday.Substring(0, 3)) + 1911;
            var month = int.Parse(basicData.Birthday.Substring(3, 2));
            var day = int.Parse(basicData.Birthday.Substring(5, 2));
            Name = basicData.Name;
            Birthday = new DateTime(year,month,day);
            IcNumber = basicData.IcNumber;
            Gender = basicData.Gender;
            IcCard = new IcCard(basicData);
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
            ContactInfo = customer.ContactInfo;
        }
        public Customer(DataRow row,string type)
        {
            Id = row["CUS_ID"].ToString();
            IcNumber = row["CUS_IDNUM"].ToString();
            Birthday = string.IsNullOrEmpty(row["CUS_BIRTH"].ToString()) ? new DateTime() : Convert.ToDateTime(row["CUS_BIRTH"].ToString());
            Name = row["CUS_NAME"].ToString();
            Qname = row["CUS_QNAME"].ToString();
            Gender = string.IsNullOrEmpty(row["CUS_GENDER"].ToString()) || Convert.ToBoolean(row["CUS_GENDER"].ToString());
            
            if (type.Equals("fromXml"))
                IcCard = new IcCard(row,DataSource.GetMedicalIcCard);
            if(type.Equals("fromDb"))
            {
                GenderName = row["CUS_GENDER"].ToString() == "True" ? "男" : "女";
                IcCard = new IcCard(row, DataSource.InitMedicalIcCard);
            }

            if (!string.IsNullOrEmpty(row["CUS_TEL"].ToString()))
            {
                ContactInfo = new ContactInfo();
                ContactInfo.Tel = row["CUS_TEL"].ToString();
            }
                
        }
        public Customer(XmlNode xml) {
            IcCard = new IcCard(xml);
            Name = xml.SelectSingleNode("d20") == null ? null : xml.SelectSingleNode("d20")?.InnerText;
            IcNumber = xml.SelectSingleNode("d3") == null ? null : xml.SelectSingleNode("d3")?.InnerText;
            Gender = IcNumber.Substring(1, 1) == "1" ? true : false;
            var nodeStr = xml.SelectSingleNode("d6")?.InnerText;
            if (!string.IsNullOrEmpty(nodeStr))
            {
                var year = int.Parse(nodeStr.Substring(0, 3))+1911;
                var month = int.Parse(nodeStr.Substring(3, 2));
                var day = int.Parse(nodeStr.Substring(5, 2));
                Birthday = new DateTime(year,month,day);
            }
            else
            {
                Birthday = new DateTime();
            }
        }

        public Customer(DeclareFileDdata d)
        {
            IcCard = new IcCard(d);
            Name = !string.IsNullOrEmpty(d.Dbody.D20) ? d.Dbody.D20 : string.Empty;
            IcNumber = !string.IsNullOrEmpty(d.Dbody.D3) ? d.Dbody.D3 : string.Empty;
            Gender = IcNumber.Substring(1, 1) == "1";
            var nodeStr = d.Dbody.D6;
            if (!string.IsNullOrEmpty(nodeStr))
            {
                var year = int.Parse(nodeStr.Substring(0, 3)) + 1911;
                var month = int.Parse(nodeStr.Substring(3, 2));
                var day = int.Parse(nodeStr.Substring(5, 2));
                Birthday = new DateTime(year, month, day);
            }
            else
            {
                Birthday = new DateTime();
            }
        }

        public string Qname { get; set; }
        private bool _gender;
        public bool Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private string _genderName;
        public string GenderName
        {
            get => _genderName;
            set
            {
                _genderName = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        private IcCard _icCard;

        public IcCard IcCard
        {
            get => _icCard;
            set
            {
                _icCard = value;
                OnPropertyChanged(nameof(IcCard));
            }
        }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
        public object Clone()
        {
            return new Customer(this);
        }
        private string emergentTel;
        public string EmergentTel {
            get => emergentTel;
            set
            {
                emergentTel = value;
                OnPropertyChanged("EmergentTel");
            }

        } 
    }
}