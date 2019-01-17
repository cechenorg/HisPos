using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person
{
    public class Genders : ObservableCollection<Gender>
    {
        public Genders() {
            Add(new Gender("男"));
            Add(new Gender("女"));
        }
    }
}
