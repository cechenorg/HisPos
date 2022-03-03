using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.Usage
{
    public class Usages : Collection<Usage>
    {
        public Usages()
        {
            Init();
        }

        public Usages(IList<Usage> list)
        {
            foreach (var u in list)
                Add(u);
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