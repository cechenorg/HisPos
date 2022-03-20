using His_Pos.NewClass;

namespace His_Pos.NewClass.Report.CashFlow
{
    public class CashFlowAccount
    {
        public CashFlowAccount(CashFlowType type, string accountName, int ID)
        {
            Type = type;
            AccountName = accountName;
            AccountID = ID;
        }

        public CashFlowType Type { get; set; }
        public string AccountName { get; set; }
        public int AccountID { get; set; }
    }
}