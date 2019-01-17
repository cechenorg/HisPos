using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.Medicine.Usage
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
