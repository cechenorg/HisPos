using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using His_Pos.NewClass.Prescription.Declare.DeclareFile;
using His_Pos.Service;

namespace His_Pos.NewClass.Prescription.Declare.DeclarePrescription
{
    public class DeclarePrescriptions:ObservableCollection<DeclarePrescription>
    {
        public DeclarePrescriptions()
        {

        }

        public void GetSearchPrescriptions(DateTime decStart, DateTime decEnd)
        {
            var table = DeclarePrescriptionDb.GetDeclarePrescriptionsByMonthRange(decStart, decEnd);
            foreach (DataRow r in table.Rows)
            {
                Add(new DeclarePrescription(r));
            }
        }
        public void AddPrescriptions(List<DeclarePrescription> pres)
        {
            foreach (var p in pres)
            {
                Add(p);
            }
        }

        public void SerializeFileContent()
        {
            foreach (var p in Items)
            {
                p.Patient = p.Patient.GetCustomerByCusId(p.Patient.ID);
                p.FileContent = XmlService.Deserialize<Ddata>(p.FileContentStr);
                for (int i = 1; i <= p.FileContent.Dbody.Pdata.Count; i++)
                {
                    p.FileContent.Dbody.Pdata[i-1].P10 = (i.ToString()).PadLeft(3, '0');
                }
                p.FileContent.Dbody.D31 = p.FileContent.Dbody.Pdata.Where(pd => pd.P1.Equals("3")).Sum(pd=> int.Parse(pd.P9)).ToString().PadLeft(7,'0');
                p.FileContent.Dbody.D33 = p.FileContent.Dbody.Pdata.Where(pd => pd.P1.Equals("1")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                p.FileContent.Dbody.D38 = p.FileContent.Dbody.Pdata.Where(pd => pd.P1.Equals("9")).Sum(pd => int.Parse(pd.P9)).ToString().PadLeft(8, '0');
                p.FileContent.Dhead.D16 = (int.Parse(p.FileContent.Dhead.D18) - int.Parse(p.FileContent.Dhead.D17)).ToString().PadLeft(8,'0');
            }
        }
    }
}
