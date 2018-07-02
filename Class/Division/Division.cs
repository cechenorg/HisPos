namespace His_Pos.Class.Division
{
    public class Division : Selection
    {
        public Division()
        {
        }

        public Division(string id,string name)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
        }
    }
}
