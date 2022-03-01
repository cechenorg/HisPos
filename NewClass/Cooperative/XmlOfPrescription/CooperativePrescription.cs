using System.Collections.Generic;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Cooperative.XmlOfPrescription
{
    public class CooperativePrescription
    {
        [XmlRoot(ElementName = "person")]
        public class Customer
        {
            [XmlElement(ElementName = "addr")]
            public string Address { get; set; }

            [XmlElement(ElementName = "remark")]
            public string Remark { get; set; }

            [XmlElement(ElementName = "allergy")]
            public string Allergy { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string IdNumber { get; set; }

            [XmlAttribute(AttributeName = "foreigner")]
            public string Foreigner { get; set; }

            [XmlAttribute(AttributeName = "sex")]
            public string Gender { get; set; }

            [XmlAttribute(AttributeName = "birth")]
            public string Birth { get; set; }

            [XmlAttribute(AttributeName = "birth_order")]
            public string Birth_order { get; set; }

            [XmlAttribute(AttributeName = "phone")]
            public string Phone { get; set; }

            [XmlAttribute(AttributeName = "family")]
            public string Family { get; set; }

            [XmlAttribute(AttributeName = "mobile")]
            public string Mobile { get; set; }

            [XmlAttribute(AttributeName = "email")]
            public string Email { get; set; }

            [XmlAttribute(AttributeName = "blood")]
            public string Blood { get; set; }

            [XmlAttribute(AttributeName = "blood_rh")]
            public string Blood_rh { get; set; }
        }

        [XmlRoot(ElementName = "profile")]
        public class CustomerProfile
        {
            [XmlElement(ElementName = "person")]
            public Customer Customer { get; set; }
        }

        [XmlRoot(ElementName = "insurance")]
        public class Insurance
        {
            [XmlAttribute(AttributeName = "insurance_type")]
            public string Insurance_type { get; set; }

            [XmlAttribute(AttributeName = "serial_code")]
            public string MedicalNumber { get; set; }

            [XmlAttribute(AttributeName = "except_code")]
            public string IcErrorCode { get; set; } //例外就醫

            [XmlAttribute(AttributeName = "copayment_code")]
            public string CopaymentCode { get; set; }

            [XmlAttribute(AttributeName = "case_type")]
            public string PrescriptionCase { get; set; }

            [XmlAttribute(AttributeName = "pay_type")]
            public string Pay_type { get; set; }

            [XmlAttribute(AttributeName = "release_type")]
            public string Release_type { get; set; }
        }

        [XmlRoot(ElementName = "item")]
        public class Item
        {
            [XmlAttribute(AttributeName = "code")]
            public string Code { get; set; }

            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "desc")]
            public string Desc { get; set; }

            [XmlAttribute(AttributeName = "remark")]
            public string Remark { get; set; }

            [XmlAttribute(AttributeName = "local_code")]
            public string Local_code { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlAttribute(AttributeName = "divided_dose")]
            public string Divided_dose { get; set; }

            [XmlAttribute(AttributeName = "daily_dose")]
            public string Daily_dose { get; set; }

            [XmlAttribute(AttributeName = "total_dose")]
            public string Total_dose { get; set; }

            [XmlAttribute(AttributeName = "freq")]
            public string Freq { get; set; }

            [XmlAttribute(AttributeName = "days")]
            public string Days { get; set; }

            [XmlAttribute(AttributeName = "way")]
            public string Way { get; set; }

            [XmlAttribute(AttributeName = "price")]
            public string Price { get; set; }

            [XmlAttribute(AttributeName = "multiplier")]
            public string Multiplier { get; set; }

            [XmlAttribute(AttributeName = "memo")]
            public string Memo { get; set; }
        }

        [XmlRoot(ElementName = "diseases")]
        public class Diseases
        {
            [XmlElement(ElementName = "item")]
            public List<Item> Disease { get; set; }
        }

        [XmlRoot(ElementName = "treatments")]
        public class Treatments
        {
            [XmlElement(ElementName = "item")]
            public List<Item> Disease { get; set; }
        }

        [XmlRoot(ElementName = "study")]
        public class Study
        {
            [XmlElement(ElementName = "diseases")]
            public Diseases Diseases { get; set; }

            [XmlElement(ElementName = "treatments")]
            public Treatments Treatments { get; set; }

            [XmlElement(ElementName = "chief_complain")]
            public string Chief_complain { get; set; }

            [XmlElement(ElementName = "physical_examination")]
            public string Physical_examination { get; set; }

            [XmlAttribute(AttributeName = "doctor_id")]
            public string Doctor_id { get; set; }

            [XmlAttribute(AttributeName = "subject")]
            public string Subject { get; set; }
        }

        [XmlRoot(ElementName = "continous_prescription")]
        public class Continous_prescription
        {
            [XmlAttribute(AttributeName = "start_at")]
            public string Start_at { get; set; }

            [XmlAttribute(AttributeName = "count")]
            public string Count { get; set; }

            [XmlAttribute(AttributeName = "total")]
            public string Total { get; set; }

            [XmlAttribute(AttributeName = "other_mo")]
            public string Other_mo { get; set; }
        }

        [XmlRoot(ElementName = "orders")]
        public class MedicineOrder
        {
            [XmlElement(ElementName = "item")]
            public List<Item> Item { get; set; }

            [XmlAttribute(AttributeName = "days")]
            public string Days { get; set; }

            [XmlAttribute(AttributeName = "mill")]
            public string Mill { get; set; }

            [XmlAttribute(AttributeName = "dosage_method")]
            public string Dosage_method { get; set; }
        }

        [XmlRoot(ElementName = "case")]
        public class Prescription
        {
            [XmlElement(ElementName = "profile")]
            public CustomerProfile CustomerProfile { get; set; }

            [XmlElement(ElementName = "insurance")]
            public Insurance Insurance { get; set; }

            [XmlElement(ElementName = "study")]
            public Study Study { get; set; }

            [XmlElement(ElementName = "continous_prescription")]
            public Continous_prescription Continous_prescription { get; set; }

            [XmlElement(ElementName = "orders")]
            public MedicineOrder MedicineOrder { get; set; }

            [XmlAttribute(AttributeName = "from")]
            public string From { get; set; }

            [XmlAttribute(AttributeName = "to")]
            public string To { get; set; }

            [XmlAttribute(AttributeName = "local_id")]
            public string Local_id { get; set; }

            [XmlAttribute(AttributeName = "date")]
            public string Date { get; set; }

            [XmlAttribute(AttributeName = "time")]
            public string Time { get; set; }

            [XmlAttribute(AttributeName = "app")]
            public string App { get; set; }

            [XmlAttribute(AttributeName = "request_method")]
            public string Request_method { get; set; }
        }
    }
}