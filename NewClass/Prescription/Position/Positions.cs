using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Position
{
    public class Positions:Collection<Position>
    {
        public Positions()
        {

        }

        private void Init()
        {
            var table = PositionDb.GetData();
            foreach (DataRow row in table.Rows)
            {
                Add(new Position(row));
            }
        }
    }
}
