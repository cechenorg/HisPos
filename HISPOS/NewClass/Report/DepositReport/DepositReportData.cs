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

        public int NormalDeposit
        {
            get { return DepositReportDataCollection.Where(_ => _.AdjustCase.IsNormal() && _.IsCooperative == false).Sum(s => s.Deposit); }
        }

        public int ChronicDeposit
        {
            get { return DepositReportDataCollection.Where(_ => _.AdjustCase.IsChronic() && _.IsCooperative == false).Sum(s => s.Deposit); }
        }

        public int PrescribeDeposit
        {
            get { return DepositReportDataCollection.Where(_ => _.AdjustCase.CheckIsPrescribe() && _.IsCooperative == false).Sum(s => s.Deposit); }
        }

        public int CooperativeDeposit
        {
            get { return DepositReportDataCollection.Where(_ => _.IsCooperative).Sum(s => s.Deposit); }
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
            Deposit = (int)Convert.ToDouble(r["CashFlow_Value"].ToString());
            IsCooperative = Convert.ToInt32(r["Is_Cooperative"].ToString()) == 1;  
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

        private int _deposit;

        public int Deposit
        {
            get { return _deposit; }
            set { Set(() => Deposit, ref _deposit, value); }
        }

        private bool _isCooperative;

        public bool IsCooperative
        {
            get { return _isCooperative; }
            set { Set(() => IsCooperative, ref _isCooperative, value); }
        }
        
    }
}
