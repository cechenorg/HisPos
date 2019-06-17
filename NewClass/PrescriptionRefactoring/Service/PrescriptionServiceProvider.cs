namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public class PrescriptionServiceProvider
    {
        public PrescriptionServiceProvider()
        {

        }

        public static PrescriptionService CreateService(PrescriptionType type)
        {
            switch (type)
            {
                case PrescriptionType.Cooperative:
                    return new CooperativePrescriptionService();
                case PrescriptionType.Orthopedics:
                    return new OrthopedicsPrescriptionService();
                default:
                    return new NormalPrescriptionService();
            }
        }

    }
}
