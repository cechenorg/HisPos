using System.Data;

namespace His_Pos.Class.TreatmentCase
{
    public class TreatmentCase : Selection
    {
        public TreatmentCase(DataRow dataRow)
        {
            Id = dataRow["HISMEDCAS_ID"].ToString();
            Name = dataRow["HISMEDCAS_NAME"].ToString();
            FullName = dataRow["FULLNAME"].ToString();
        }

        public TreatmentCase()
        {
            
        }
    }
}
