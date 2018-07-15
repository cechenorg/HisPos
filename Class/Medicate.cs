using System.Data;

namespace His_Pos.Class
{
    public class Medicate
    {
        public Medicate()
        {
        }

        public Medicate(DataRow dataRow)
        {
            Dosage = dataRow["HISMED_UNIT"].ToString();
            Form = dataRow["HISMED_FORM"].ToString();
        }

        public string Dosage { get; set; }
        public string Form { get; set; }
    }
}