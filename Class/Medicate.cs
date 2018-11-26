using System.Data;

namespace His_Pos.Class
{
    public class Medicate
    {
        public Medicate()
        {
            Dosage = 0;
            Form = "";
        }

        public Medicate(DataRow dataRow)
        {
            Unit = dataRow["HISMED_UNIT"].ToString();
            Form = dataRow["HISMED_FORM"].ToString();
        }

        public string Unit { get; set; }
        public double Dosage { get; set; }
        public string Form { get; set; }
    }
}