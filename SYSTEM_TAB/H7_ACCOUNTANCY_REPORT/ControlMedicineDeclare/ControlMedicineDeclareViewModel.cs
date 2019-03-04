using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.Medicine.ControlMedicineDeclare;
using System;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare {
    public class ControlMedicineDeclareViewModel : TabBase {
        public override TabBase getTab() {
            return this;
        }

        #region Var
        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                Set(() => SDateTime, ref sDateTime, value);
            }
        }
        private DateTime eDateTime = DateTime.Now;
        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                Set(() => EDateTime, ref eDateTime, value);
            }
        }
        private ControlMedicineDeclares controlMedicineDeclares = new ControlMedicineDeclares();
        public ControlMedicineDeclares ControlMedicineDeclares
        {
            get { return controlMedicineDeclares; }
            set
            {
                Set(() => ControlMedicineDeclares, ref controlMedicineDeclares, value);
            }
        }
        #endregion
        public ControlMedicineDeclareViewModel() {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
        }
    }
}
