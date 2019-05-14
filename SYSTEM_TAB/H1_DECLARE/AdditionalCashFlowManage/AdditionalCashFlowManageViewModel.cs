using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage
{
    public class AdditionalCashFlowManageViewModel : TabBase {
        public override TabBase getTab() {
            return this;
        }
        private bool payCheck = false;
        public bool PayCheck
        {
            get { return payCheck; }
            set { Set(() => PayCheck, ref payCheck, value); }
        }
        private bool gainCheck = true;
        public bool GainCheck
        {
            get { return gainCheck; }
            set { Set(() => GainCheck, ref gainCheck, value); }
        }
        private string typeName;
        public string TypeName
        {
            get { return typeName; }
            set { Set(() => TypeName, ref typeName, value); }
        }
        private string cashflowName;
        public string CashflowName {
            get { return cashflowName; }
            set { Set(() => CashflowName,ref cashflowName,value); }
        }
        private double cashflowValue;
        public double CashflowValue
        {
            get { return cashflowValue; }
            set { Set(() => CashflowValue, ref cashflowValue, value); }
        }
        public RelayCommand SubmitCommand { get; set; }

        public AdditionalCashFlowManageViewModel() {
            SubmitCommand = new RelayCommand(SubmitAction);
        }
        private void SubmitAction() {
            if (GainCheck)
                PrescriptionDb.ProcessCashFlow("額外收入", CashflowName, 0, CashflowValue);
            else
                PrescriptionDb.ProcessCashFlow("額外收入", CashflowName, 0, CashflowValue * -1);

            MessageWindow.ShowMessage("新增成功!",Class.MessageType.SUCCESS);
        }
    }
}
