using System;
using System.Data;

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
            Gender = Convert.ToBoolean(row["CUS_GENDER"].ToString());
            IcCard = new IcCard(row,DataSource.GetMedicalIcCard);
        }

        public string Qname { get; set; }
        public bool Gender { get; set; }
        public IcCard IcCard { get; set; }
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();
    }
}