using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Medicine.Position
{
    public class Positions : Collection<Position>
    {
        public Positions()
        {
            Init();
        }

        public Positions(IList<Position> list)
        {
            foreach (var p in list)
                Add(p);
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