using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Tutorial
{
    public class Tutorials : ObservableCollection<Tutorial>
    {
        public Tutorials(DataTable table) {
            foreach (DataRow r in table.Rows) {
                Add(new Tutorial(r));
            }
        }
        public static Tutorials GetData() {
            var table = TutorialDb.GetData();
            return new Tutorials(table);
        }
    }
}
