namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public static class PrescriptionServiceProvider
    {
        public static PrescriptionService CreateService(PrescriptionType type)
        {
            switch (type)
            {
                case PrescriptionType.Cooperative:
                    return new OrthopedicsPrescriptionService();
                default:
                    return new NormalPrescriptionService();
            }
        }

    }
}
