using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using His_Pos.Class.Declare;

namespace His_Pos.Class.Position
{
    public class Position : Selection
    {
        public Position()
        {
            Id = string.Empty;
            Name = string.Empty;
        }

        public Position(DataRow dataRow)
        {
            Id = dataRow["PositionId"].ToString().Trim();
            Name = dataRow["PositionName"].ToString().Trim();
        }
    }
}
