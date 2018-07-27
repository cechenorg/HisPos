using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Interface
{
    public interface IPrincipal
    {

        string Id { get; set; }
        string Name { get; set; }
        string NickName { get; set; }
        string Telphone { get; set; }
        string Fax { get; set; }
        string Email { get; set; }
        string Line { get; set; }
        bool IsEnable { get; set; }
    }
}
