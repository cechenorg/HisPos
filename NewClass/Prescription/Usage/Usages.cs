using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.Usage;

namespace His_Pos.NewClass.Prescription.Usage
{
    public class Usages:Collection<NewClass.Usage.Usage>
    {
        public Usages()
        {
            Init();
        }
        private void Init()
        {
            var table = UsageDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new NewClass.Usage.Usage(row));
            }
        }
    }
}
