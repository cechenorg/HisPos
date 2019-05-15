using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public abstract class PrescriptionService:ObservableObject
    {
        #region AbstractFunctions
        public abstract bool NormalAdjust();
        public abstract bool ErrorAdjust();
        public abstract bool DepositAdjust();
        public abstract bool Register();
        #endregion
        public PrescriptionService()
        {

        }

        protected PrescriptionService(Prescription p)
        {
            current = p;
        }

        protected Prescription current { get; set; }
        #region Functions
        public PrescriptionService CreateService(Prescription p)
        {
            var ps = PrescriptionServiceProvider.CreateService(p.Type);
            ps.current = p;
            return ps;
        }
        public bool StartNormalAdjust()
        {
            return NormalAdjust();
        }
        public bool StartErrorAdjust()
        {
            return ErrorAdjust();
        }
        public bool StartDepositAdjust()
        {
            return DepositAdjust();
        }
        public bool StartRegister()
        {
            return Register();
        }
        #endregion
    }
}
