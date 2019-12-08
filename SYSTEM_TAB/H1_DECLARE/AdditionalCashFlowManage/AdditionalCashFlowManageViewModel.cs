using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Prescription;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using His_Pos.Class;
using MaskedTextBox = Xceed.Wpf.Toolkit.MaskedTextBox;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.AdditionalCashFlowManage
{
    public class AdditionalCashFlowManageViewModel : TabBase {
        public override TabBase getTab() {
            return this;
        }
        public Collection<string> CashFlowType => new Collection<string> {"雜支"};
        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                Set(() => EndDate, ref endDate, value);
            }
        }
        private bool payCheck = false;
        public bool PayCheck
        {
            get => payCheck;
            set { Set(() => PayCheck, ref payCheck, value); }
        }
        private bool gainCheck = true;
        public bool GainCheck
        {
            get => gainCheck;
            set { Set(() => GainCheck, ref gainCheck, value); }
        }
        private string typeName;
        public string TypeName
        {
            get => typeName;
            set { Set(() => TypeName, ref typeName, value); }
        }
        private string cashflowName;
        public string CashflowName {
            get => cashflowName;
            set { Set(() => CashflowName,ref cashflowName,value); }
        }
        private double cashflowValue;
        public double CashflowValue
        {
            get => cashflowValue;
            set { Set(() => CashflowValue, ref cashflowValue, value); }
        }

        #region Commands
        public RelayCommand SubmitCommand { get; set; }
        public RelayCommand<MaskedTextBox> DateMouseDoubleClick { get; set; }
        public RelayCommand Search { get; set; }

        #endregion

        public AdditionalCashFlowManageViewModel()
        {
            InitCommand();
        }

        private void InitCommand()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
            DateMouseDoubleClick = new RelayCommand<MaskedTextBox>(DateMouseDoubleClickAction);
            Search = new RelayCommand(SearchAction);
            
        }

        private void SubmitAction() {
            if (GainCheck)
                PrescriptionDb.ProcessCashFlow("額外收入", CashflowName, 0, CashflowValue);
            else
                PrescriptionDb.ProcessCashFlow("額外收入", CashflowName, 0, CashflowValue * -1);

            MessageWindow.ShowMessage("新增成功!",Class.MessageType.SUCCESS);
        }

        private void DateMouseDoubleClickAction(MaskedTextBox sender)
        {
            switch (sender.Name)
            {
                case "StartDate":
                    StartDate = DateTime.Today;
                    break;
                case "EndDate":
                    EndDate = DateTime.Today;
                    break;
            }
        }

        private void SearchAction()
        {
            if (StartDate is null)
            {
                MessageWindow.ShowMessage("請填寫起始日期",MessageType.ERROR);
                return;
            }
            if (EndDate is null)
            {
                MessageWindow.ShowMessage("請填寫結束日期",MessageType.ERROR);
                return;
            }

        }
    }
}
