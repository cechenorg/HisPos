using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashReport
{
    public class CashReport: ObservableObject
    {
        public CashReport()
        {
             
        }
        public CashReport(DataRow r) {
            TypeId = r.Field<string>("TypeId");
            TypeName = r.Field<string>("TypeName");
            CopayMentPrice = r.Field<int>("CopayMentPrice");
            PaySelfPrice = r.Field<int>("PaySelfPrice");
            AllPaySelfPrice = r.Field<int>("AllPaySelfPrice");
            DepositPrice = r.Field<int>("DepositPrice");
            OtherPrice = r.Field<int>("OtherPrice");
            TotalPrice = r.Field<int>("TotalPrice");
        }
        public string typeId;
        public string TypeId
        {
            get => typeId;
            set
            {
                Set(() => TypeId, ref typeId, value);
            }
        }
        public string typeName;
        public string TypeName
        {
            get => typeName;
            set
            {
                Set(() => TypeName, ref typeName, value);
            }
        }
        private int copayMentPrice;
        public int CopayMentPrice
        {
            get => copayMentPrice;
            set
            {
                Set(() => CopayMentPrice, ref copayMentPrice, value);
            }
        }
        private int paySelfPrice;
        public int PaySelfPrice
        {
            get => paySelfPrice;
            set
            {
                Set(() => PaySelfPrice, ref paySelfPrice, value);
            }
        }
        private int allPaySelfPrice;
        public int AllPaySelfPrice
        {
            get => allPaySelfPrice;
            set
            {
                Set(() => AllPaySelfPrice, ref allPaySelfPrice, value);
            }
        }
        private int depositPrice;
        public int DepositPrice
        {
            get => depositPrice;
            set
            {
                Set(() => DepositPrice, ref depositPrice, value);
            }
        }
        private int otherPrice;
        public int OtherPrice
        {
            get => otherPrice;
            set
            {
                Set(() => OtherPrice, ref otherPrice, value);
            }
        }
        private int totalPrice;
        public int TotalPrice
        {
            get => totalPrice;
            set
            {
                Set(() => TotalPrice, ref totalPrice, value);
            }
        }
        private double total;
        public double Total
        {
            get => total;
            set
            {
                Set(() => Total, ref total, value);
            }
        }

        private double totalOTC;
        public double TotalOTC
        {
            get => totalOTC;
            set
            {
                Set(() => TotalOTC, ref totalOTC, value);
            }
        }
        private double totalMedProfit;
        public double TotalMedProfit
        {
            get => totalMedProfit;
            set
            {
                Set(() => TotalMedProfit, ref totalMedProfit, value);
            }
        }
        private double totalMedUse;
        public double TotalMedUse
        {
            get => totalMedUse;
            set
            {
                Set(() => TotalMedUse, ref totalMedUse, value);
            }
        }
        private double totalMedCash;
        public double TotalMedCash
        {
            get => totalMedCash;
            set
            {
                Set(() => TotalMedCash, ref totalMedCash, value);
            }
        }
        private double totalMed;
        public double TotalMed
        {
            get => totalMed;
            set
            {
                Set(() => TotalMed, ref totalMed, value);
            }
        }
        private double totalMedChange;
        public double TotalMedChange
        {
            get => totalMedChange;
            set
            {
                Set(() => TotalMedChange, ref totalMedChange, value);
            }
        }
        private double totalOTCChange;
        public double TotalOTCChange
        {
            get => totalOTCChange;
            set
            {
                Set(() => TotalOTCChange, ref totalOTCChange, value);
            }
        }
    }
}
