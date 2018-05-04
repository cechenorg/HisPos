using System.Data;

namespace His_Pos.Class.Person
{
    public class Person
    {
        public Person()
        {

        }
        public Person(DataRow dataRow)
        {
            Id = dataRow["EMP_ID"].ToString();
            Name = dataRow["EMP_NAME"].ToString();
            IcNumber = dataRow["EMP_IDNUM"].ToString();
            Birthday = dataRow["EMP_BIRTH"].ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string IcNumber { get; set; }
        public string Birthday { get; set; }
    }
}
