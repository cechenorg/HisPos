using System.Data;

namespace His_Pos.Class
{
    public class Medicate
    {
        public Medicate()
        {
            Dosage = "";
            Form = "";
        }

        public Medicate(DataRow dataRow)
        {
            Unit = dataRow["HISMED_UNIT"].ToString();
            Form = dataRow["HISMED_FORM"].ToString();
        }

        public string Unit { get; set; }
        public string Dosage { get; set; }
        public string Form { get; set; }
    }
}