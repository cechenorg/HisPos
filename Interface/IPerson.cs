using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    public interface IPerson
    {
        string Id { get; set; }
        string Name { get; set; }
        string IcNumber { get; set; }
    }
}
