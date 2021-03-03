namespace His_Pos.Interface
{
    internal interface IStockTakingRecord
    {
        string EmpName { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
    }
}