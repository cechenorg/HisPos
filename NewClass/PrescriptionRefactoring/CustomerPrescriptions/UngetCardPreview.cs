using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.PrescriptionRefactoring.CustomerPrescriptions
{
    public class UngetCardPreview : CusPrePreviewBase
    {
        public UngetCardPreview(DataRow r):base(r)
        {

        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override Prescription CreatePrescription()
        {
            throw new NotImplementedException();
        }

        public void MakeUp()
        {

        }
    }
}
