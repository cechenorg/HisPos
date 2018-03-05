using His_Pos.AbstractClass;

namespace His_Pos.Class
{
    public class Department : Institution
    {
        public Department(string id, string name) : base(id, name)
        {
            Id = id;
            Name = name;
        }

        public string Position { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
    }
}
