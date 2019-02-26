using System.Data;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class PrescribeHistory : CustomerHistoryBase
    {
        public PrescribeHistory(DataRow r):base(r,HistoryType.Prescribe)
        {

        }
    }
}
