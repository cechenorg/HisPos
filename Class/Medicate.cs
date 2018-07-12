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

        public Medicate(string dosage, Usage usage, int days, string position)
        {
            Dosage = dosage;
            Usage = usage;
            Days = days;
            Position = position;
        }

        public string Dosage { get; set; }
        public Usage Usage { get; set; }
        public int Days { get; set; }
        public string Position { get; set; }
        public string Form { get; set; }
    }
}