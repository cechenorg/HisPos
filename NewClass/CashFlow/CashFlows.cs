using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.CashFlow {
    public class CashFlows : ObservableCollection<CashFlow> {
        public CashFlows() { }

        public void GetCashFlowByDate(DateTime sDate) {
            Clear();
            DataTable table = CashFlowDb.GetCashFlowByDate(sDate);
            CashFlow cashFlow = new CashFlow();
            foreach (DataRow r in table.Rows) {
                cashFlow.Date = r.Field<DateTime>("CashFlow_Time");
                switch (r.Field<string>("CashFlow_Name")) {
                    case "部分負擔":
                        cashFlow.CopaymentValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "合作部分負擔":
                        cashFlow.ClinicCopaymentValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "自費":
                        cashFlow.PaySelfValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "合作自費":
                        cashFlow.ClinicPaySelfValue = r.Field<decimal>("CashFlow_Value");
                        break; 
                    case "骨科毛利":
                        cashFlow.ClinicProfitValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "排除骨科總點數(慢箋 + 其他)":
                        cashFlow.PrescriptionwithoutCooperativeValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "自費調劑耗用":
                        cashFlow.PayselfMedUseValue = r.Field<decimal>("CashFlow_Value");
                        break;
                    case "慢箋與一般箋調劑耗用":
                        cashFlow.MedicineUseValue = r.Field<decimal>("CashFlow_Value");
                        break; 
                    case "押金":
                        cashFlow.DepositValue = r.Field<decimal>("CashFlow_Value");
                        break; 
                }
            }
            Add(cashFlow);
        }
    }
}
