using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class IcCard
    {
        public string ICNumber { get; set; }//卡片號碼
        public Customer Customer { get; set; } //姓名.身分證字號.出生日期.性別
        public IcMarks IcMarks { get; set; } = new IcMarks();//卡片註銷註記.保險對象身分註記.新生兒出生日期.新生兒胞胎註記
        public string SendDate { get; set; }//發卡日期
        public string ValidityPeriod { get; set; }//卡片有效期限
        public int AvailableTimes { get; set; }//就醫可用次數
        public int LastMedicalNumber { get; set; }//最近一次就醫序號
    }
}
