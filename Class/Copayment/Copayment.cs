namespace His_Pos.Class.Copayment
{
    public class Copayment : Selection
    {
        public Copayment(string id,string name)
        {
            Id = id;
            Name = name;
        }

        public double Point { get; set; }
    }
}
