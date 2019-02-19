using System.Data;
using His_Pos.Class;

namespace His_Pos.NewClass.Person.Customer.CustomerHistory
{
    public class PrescriptionHistory:CustomerHistoryBase
    {
        public PrescriptionHistory(){}
        public PrescriptionHistory(DataRow r):base(r,HistoryType.Prescription)
        {

        }
    }
}
