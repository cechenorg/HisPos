using System.Data;
using System.Xml;

namespace His_Pos.Class.PaymentCategory
{
    public class PaymentCategory : Selection
    {
        public PaymentCategory()
        {
        }
        public PaymentCategory(XmlNode xml) {
            Id = xml.SelectSingleNode("d5") == null ? null : xml.SelectSingleNode("d5").InnerText;
        }
        public PaymentCategory(DataRow dataRow)
        {
            Id = dataRow["HISPAYCAT_ID"].ToString();
            Name = dataRow["HISPAYCAT_NAME"].ToString();
            FullName = dataRow["HISPAYCAT_FULLNAME"].ToString();
        }
    }
}
