using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Usage
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
