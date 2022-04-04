using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;

namespace His_Pos.NewClass.Report.DepositReport
{
    public class DepositReportDataList : ObservableObject
    {
        public List<DepositReportData> DepositReportDataCollection { get; set; }

        public int NormalCount
        {
            get { return DepositReportDataCollection.Count(_ => _.AdjustCase.IsNormal()); }
        }

        public int ChronicCount
        {
            get { return DepositReportDataCollection.Count(_ => _.AdjustCase.IsChronic()); }
        }

        public int PrescribeCount
        {
            get { return DepositReportDataCollection.Count(_ => _.AdjustCase.CheckIsPrescribe()); }
        }

        public DepositReportDataList(DataTable table)
        {
            DepositReportDataCollection = new List<DepositReportData>();

            foreach (DataRow row in table.Rows)
            {
                DepositReportDataCollection.Add(new DepositReportData(row));
            }
        }

       
    }


    public class DepositReportData : ObservableObject
    {

        public DepositReportData(DataRow r)
        {
           
            PremasID = r.Field<int>("PreMas_ID");
             
            AdjustCase = new AdjustCase();
            AdjustCase.ID = r.Field<string>("PreMas_AdjustCaseID");
            Deposit = Convert.ToDouble(r["CashFlow_Value"].ToString());
        }

        private int _premasID;

        public int PremasID
        {
            get { return _premasID; }
            set { Set(() => PremasID, ref _premasID, value); }
        }

        private AdjustCase _adjustCase;

        public AdjustCase AdjustCase
        {
            get { return _adjustCase; }
            set { Set(() => AdjustCase, ref _adjustCase, value); }
        }

        private double _deposit;

        public double Deposit
        {
            get { return _deposit; }
            set { Set(() => Deposit, ref _deposit, value); }
        }
    }
}
