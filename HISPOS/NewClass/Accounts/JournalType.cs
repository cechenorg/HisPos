using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Accounts
{
    public class JournalType
    {
        public int JournalTypeID { get; set; }
        public string JournalTypeName { get; set; }
        public string JournalTypeFullName { get; set; }
        public JournalType(int typeID, string typeName, string typeFullName)
        {
            JournalTypeID = typeID;
            JournalTypeName = typeName;
            JournalTypeFullName = typeFullName;
        }
    }
}
