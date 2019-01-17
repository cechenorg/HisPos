using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.Medicine.Position
{
    public class Positions:Collection<Position>
    {
        public Positions()
        {
            Init();
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
