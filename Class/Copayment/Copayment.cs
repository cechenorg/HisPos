namespace His_Pos.Class.Copayment
{
    public class Copayment : Selection
    {
        public Copayment()
        {
        }

        public Copayment(string id,string name)
        {
            Id = id;
            Name = name;
        }
        public int Point { get; set; }
    }
}
