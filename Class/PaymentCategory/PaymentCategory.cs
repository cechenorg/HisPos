using System.Data;

namespace His_Pos.Class.PaymentCategory
{
    public class PaymentCategory : Selection
    {
        public PaymentCategory()
        {
        }

        public PaymentCategory(DataRow dataRow)
        {
            Id = dataRow["HISPAYCAT_ID"].ToString();
            Name = dataRow["HISPAYCAT_NAME"].ToString();
            FullName = dataRow["HISPAYCAT_FULLNAME"].ToString();
        }
    }
}
