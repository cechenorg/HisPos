using His_Pos.Class.Person;
using System.Data;

namespace His_Pos.Class
{
    public class IcCard
    {
        public IcCard()
        {
            IcMarks = new IcMarks();
        }
        public IcCard(DataRow row) {
         
        }
        public string IcNumber { get; set; }//卡片號碼
        public IcMarks IcMarks { get; set; } = new IcMarks();//卡片註銷註記.保險對象身分註記.新生兒出生日期.新生兒胞胎註記
        public string SendDate { get; set; }//發卡日期
        public string ValidityPeriod { get; set; }//卡片有效期限
        public int AvailableTimes { get; set; }//就醫可用次數
        public IcCardPay IcCardPay { get; set; } //門診醫療費用 住院醫療費用
        public IcCardPrediction IcCardPrediction { get; set; } //預防保健項目
        public Pregnant Pregnant { get; set; } //孕婦檢查項目
        public Vaccination Vaccination { get; set; } //預防接種項目
        public string MedicalNumber { get; set; }//D7就醫序號
    }
}