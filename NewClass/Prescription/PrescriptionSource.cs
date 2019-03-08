namespace His_Pos.NewClass.Prescription
{
    public enum PrescriptionSource
    {
        Normal = 0,//一般箋 新慢箋 自費調劑
        Cooperative = 1,//合作診所處方
        ChronicReserve = 2,//慢箋預約
        Register = 3,//登錄處方
    }

    public enum PrescriptionDeclareStatus
    {
        Adjust = 0,//調劑
        Register = 1,//登錄
        Prescribe = 2,//自費
        NoCard = 3//自費
    }
}
