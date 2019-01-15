using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass;
using His_Pos.NewClass.Usage;

namespace His_Pos.NewClass.Prescription.Usage
{
    public class Usages:Collection<Usage>
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
                Add(new Usage(row));
            }
        }
    }
}
