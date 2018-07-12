using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Copayment
{
    public class CopaymentDb
    {
        public ObservableCollection<Copayment> Copayments { get; } = new ObservableCollection<Copayment>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetCopaymentsData]", dbConnection);
            foreach (DataRow copayment in divisionTable.Rows)
            {
                var c = new Copayment(copayment);
                Copayments.Add(c);
            }
        }

        /*
         *回傳對應部分負擔之id + name string
         */

        public string GetCopayment(string tag)
        {
            string result = string.Empty;
            GetData();
            foreach (var copayment in Copayments)
            {
                if (copayment.Id == tag)
                {
                    result = copayment.FullName;
                }
            }
            return result;
        }
    }
}