namespace His_Pos.Interface
{
    interface IStockTakingRecord
    {
        string EmpName { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
    }
}
