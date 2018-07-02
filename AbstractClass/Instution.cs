using System.Data;
using His_Pos.Class;

namespace His_Pos.AbstractClass
{
    public class Institution : Selection
    {
        public Institution(string id,string name)
        {
            Id = id;
            Name = name;
            FullName = id + " " + name;
        }

        public Institution(DataRow dataRow)
        {
            Id = dataRow["INS_ID"].ToString();
            Name = dataRow["INS_NAME"].ToString();
            FullName = Id + " " + Name;
        }

        public Institution()
        {
        }
    }
}
