using System.Data;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.PaymentCategory
{
    public class PaymentCategory : Selection
    {
        public PaymentCategory()
        {
        }

        public PaymentCategory(XmlNode xml)
        {
            Id = xml.SelectSingleNode("d5") == null ? null : xml.SelectSingleNode("d5").InnerText;
        }

        public PaymentCategory(DataRow dataRow)
        {
            Id = dataRow["HISPAYCAT_ID"].ToString();
            Name = dataRow["HISPAYCAT_NAME"].ToString();
            FullName = dataRow["HISPAYCAT_FULLNAME"].ToString();
        }

        public PaymentCategory(DeclareFileDdata d)
        {
            Id = !string.IsNullOrEmpty(d.Dbody.D5) ? d.Dbody.D5 : string.Empty;
        }

        public PaymentCategory ShallowCopy()
        {
            return (PaymentCategory)MemberwiseClone();
        }
    }
}
