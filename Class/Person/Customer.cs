using System;
using System.Data;
using System.Globalization;
using System.Threading;
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
            UrgentContactName = customer.UrgentContactName;
            UrgentContactPhone = customer.UrgentContactPhone; 
            UrgentContactTel = customer.UrgentContactTel;
            Description = customer.Description;
        }
        public Customer(DataRow row,string type)
        {
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
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
                ContactInfo = new ContactInfo();
                ContactInfo.Tel = row["CUS_TEL"].ToString();
                ContactInfo.Phone = row["CUS_PHONE"].ToString();
                ContactInfo.Address = row["CUS_ADDR"].ToString();
                LastEdit = string.IsNullOrEmpty(row["CUS_LASTEDIT"].ToString()) ? new DateTime().ToLocalTime() : Convert.ToDateTime(row["CUS_LASTEDIT"].ToString()).ToLocalTime();
                UrgentContactName = row["CUS_URGENTPERSON"].ToString();
                UrgentContactPhone = row["CUS_URGENTPHONE"].ToString();
                UrgentContactTel = row["CUS_URGENTTEL"].ToString();
                Description = row["CUS_DESC"].ToString();
            }
            
                
        }
        public Customer DeepCopy()
        {
            Customer othercopy = (Customer)this.MemberwiseClone();
            return othercopy;
        }
        public Customer(XmlDocument xml) {
            XmlNode xmlcus = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/profile/person");
            IcCard = new IcCard();
            IcCard.IcNumber = xmlcus.Attributes["id"].Value;
            IcCard.MedicalNumber = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/insurance").Attributes["serial_code"].Value;

            Name = xmlcus.Attributes["name"].Value;
            string birstring = xmlcus.Attributes["birth"].Value.Substring(0, 3) + "-" + xmlcus.Attributes["birth"].Value.Substring(3, 2) + "-" + xmlcus.Attributes["birth"].Value.Substring(5, 2);
            Birthday = Convert.ToDateTime(birstring).AddYears(1911); 
            
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
            Name = !string.IsNullOrEmpty(d.Dhead.D20) ? d.Dhead.D20 : string.Empty;
            IcNumber = !string.IsNullOrEmpty(d.Dhead.D3) ? d.Dhead.D3 : string.Empty;
            Gender = IcNumber.Substring(1, 1) == "1";
            var nodeStr = d.Dhead.D6;
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
        private string urgentContactTel;
        public string UrgentContactTel
        {
            get => urgentContactTel;
            set
            {
                urgentContactTel = value;
                OnPropertyChanged(nameof(UrgentContactTel));
            }
        }
        private string urgentContactName;
        public string UrgentContactName
        {
            get => urgentContactName;
            set
            {
                urgentContactName = value;
                OnPropertyChanged(nameof(UrgentContactName));
            }
        }
        private string urgentContactPhone;
        public string UrgentContactPhone
        {
            get => urgentContactPhone;
            set
            {
                urgentContactPhone = value;
                OnPropertyChanged(nameof(UrgentContactPhone));
            }
        }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
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
                OnPropertyChanged(nameof(EmergentTel));
            }

        }

        private DateTime _lastEdit;
        public DateTime LastEdit
        {
            get => _lastEdit;
            set
            {
                _lastEdit = value;
                OnPropertyChanged(nameof(LastEdit));
            }
        }
    }
}