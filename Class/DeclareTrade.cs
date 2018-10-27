namespace His_Pos.Class
{
    public class DeclareTrade
    {
        public DeclareTrade(string vCusId,string vEmpId,string vPaySelf,string vDeposit,string vReceiveMoney,string vCopayMent,string vPayMoney,string vChange, string vPayWay) {
            CusId = vCusId;
            EmpId = vEmpId;
            PaySelf = vPaySelf;
            Deposit = vDeposit;
            ReceiveMoney = vReceiveMoney;
            CopayMent = vCopayMent;
            PayWay = vPayWay;
            PayMoney = vPayMoney;
            Change = vChange;
        }
        
       public string DecMasId { get; set; }
        public string CusId { get; set; }
        public string EmpId { get; set; }
        public string PaySelf { get; set; } //自費
        public string Deposit { get; set; }
        public string ReceiveMoney { get; set; }
        public string CopayMent { get; set; }
        public string PayMoney { get; set; } //已付金額
        public string Change { get; set; } //找零
        public string PayWay { get; set; }
    }
}