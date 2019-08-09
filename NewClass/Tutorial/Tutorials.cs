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
        public Tutorials(DataRow r) {
        }
    }
}
