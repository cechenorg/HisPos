using His_Pos.Class.Product;
using System;
using System.Data;
using System.Xml;

namespace His_Pos.Class.Declare
{
    public class DeclareDetail : ICloneable
    {
        public DeclareDetail(string medicalOrder,string medicalId, double percent,double total, double price, int sequence, string start, string end,string type)
        {
            switch (type)
            {
                case "service":
                    P1MedicalOrder = medicalOrder;
                    P2MedicalId = medicalId;
                    P6Percent = percent;
                    P7Total = total;
                    P8Price = price;
                    CountPoint();
                    P10Sequence = sequence;
                    SetDate(start, end);
                    break;
                case "dayPay":
                    P1MedicalOrder = medicalOrder;
                    P2MedicalId = medicalId;
                    P3Dosage = 0001.00;
                    P6Percent = percent;
                    P7Total = total;
                    P8Price = price;
                    CountPoint();
                    P10Sequence = sequence;
                    P11Days = int.Parse(P7Total.ToString());
                    SetDate(start, end);
                    break;
            }
        }

        public DeclareDetail(DeclareMedicine medicine, int sequence)
        {
            const string medicationDetail = "1";
            const string notPriced = "4";
            P1MedicalOrder = !medicine.PaySelf ? medicationDetail : notPriced;
            PaySelf = medicine.PaySelf;
            SetMedicine(medicine);
            P10Sequence = sequence;
            CountPoint();
        }

        public DeclareDetail(PrescriptionOTC otc)
        {
            P1MedicalOrder = "0";
            PaySelf = true;
            P2MedicalId = otc.Id;
            P7Total = otc.Amount;
            P8Price = otc.Price;
            P3Dosage = otc.Dosage;
            P4Usage = otc.Usage?.Name;
            P5Position = string.IsNullOrEmpty(otc.Position)? otc.Position:string.Empty;
            P11Days = string.IsNullOrEmpty(otc.Days) ? 0 : int.Parse(otc.Days);
            P9Point = 0;
        }

        public DeclareDetail()
        {
        }

        public DeclareDetail(DataRow row) {
            P1MedicalOrder = row["HISDECDET_ID"].ToString();
            P2MedicalId = row["HISMED_ID"].ToString();
            Name = row["PRO_NAME"].ToString();
            P3Dosage = row["HISDECDET_AMOUNT"].ToString() == "" ? 0 : Convert.ToDouble(row["HISDECDET_AMOUNT"].ToString());
            P4Usage = row["HISWAY_ID"].ToString();
            P11Days = row["HISDECDET_DRUGDAY"].ToString() == "" ? 0 : Convert.ToInt32(row["HISDECDET_DRUGDAY"].ToString());
        }

        public DeclareDetail(XmlNode xml) {
            P1MedicalOrder = xml.SelectSingleNode("p1") == null ? null : xml.SelectSingleNode("p1").InnerText;
            P2MedicalId = xml.SelectSingleNode("p2") == null ? null : xml.SelectSingleNode("p2").InnerText;
            P3Dosage = xml.SelectSingleNode("p3") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p3").InnerText);
            P4Usage = xml.SelectSingleNode("p4") == null ? null : xml.SelectSingleNode("p4").InnerText;
            P5Position = xml.SelectSingleNode("p5") == null ? null : xml.SelectSingleNode("p5").InnerText;
            P6Percent = xml.SelectSingleNode("p6") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p6").InnerText);
            P7Total = xml.SelectSingleNode("p7") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p7").InnerText);
            P8Price = xml.SelectSingleNode("p8") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p8").InnerText);
            P9Point = xml.SelectSingleNode("p9") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p9").InnerText);
            P10Sequence = xml.SelectSingleNode("p10") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p10").InnerText);
            P11Days = xml.SelectSingleNode("p11") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p11").InnerText);
            P12StartDate = xml.SelectSingleNode("p12") == null ? null : xml.SelectSingleNode("p12").InnerText;
            P13EndDate = xml.SelectSingleNode("p13") == null ? null : xml.SelectSingleNode("p13").InnerText;
            Form = xml.SelectSingleNode("p14") == null ? null : xml.SelectSingleNode("p14").InnerText;
        }
        public DeclareDetail(DeclareDetail declareDetail) {
            P1MedicalOrder = declareDetail.P1MedicalOrder;
            P2MedicalId = declareDetail.P2MedicalId;
            P3Dosage = declareDetail.P3Dosage;
            P4Usage = declareDetail.P4Usage;
            P5Position = declareDetail.P5Position;
            P6Percent = declareDetail.P6Percent;
            P7Total = declareDetail.P7Total;
            P8Price = declareDetail.P8Price;
            P9Point = declareDetail.P9Point;
            P10Sequence = declareDetail.P10Sequence;
            P11Days = declareDetail.P11Days;
            P12StartDate = declareDetail.P12StartDate;
            P13EndDate = declareDetail.P13EndDate;
            Form = declareDetail.Form;
            PaySelf = PaySelf;
        }
        /*
         * P1 醫令類別 :
         * 1:用藥明細
         * 2:診療明細
         * 3:特殊材料
         * 4:不得另計價之藥品、檢驗（查）、診療項目或材料
         * 9:藥事服務
         */
        public string P1MedicalOrder { get; set; }
        //P2 藥品（項目）代號
        public string P2MedicalId { get; set; }
        /*
         * P3 藥品用量:
         * 整數4位，小數2位
         * P1(醫令類別)為1或4且醫令代碼為藥價基準代碼之案件，本欄為必填欄位
         */
        public double P3Dosage { get; set; }
        /*
         * P4 (藥品)使用頻率:
         * P1(醫令類別)為1或4且醫令代碼為藥價基準代碼之案件，本欄為必填欄位
         */
        public string P4Usage { get; set; }
        /*
         * P5 給藥途徑/作用部位:
         * P1(醫令類別)為1或4且醫令代碼為藥價基準代碼之案件，本欄為必填欄位
         */
        public string P5Position { get; set; }
        //P6 支付成數 
        public double P6Percent { get; set; }
        /*
         * P7 總量:
         * 五位整數，一位小數
         */
        public double P7Total { get; set; }
        /*
         * P8 單價:
         * 七位整數，二位小數
         */
        public double P8Price { get; set; }
        /*
         * P9 點數:
         * 八位整數，小數點後四捨五入，總量乘單價，並加成計算至整數（點）為止。
         * P1 == 1 : P9 = P8 * P7 (單價＊總量)
         * P1 == 2.3.9 : P9 = P8 * P7 * P6(單價＊總量＊支付成數)
         * P1 == 4 :
         * P1.Length == 10 => P9 = P8 * P7 (單價＊總量)
         * P1.Length < 10 => P9 = P8 * P7 * P6(單價＊總量＊支付成數)
         */
        public int P9Point { get; set; }
        //P10 醫令序
        public int P10Sequence { get; set; }
        //P11 藥品給藥日份
        public int P11Days { get; set; }
        //P12 執行時間-起
        public string P12StartDate { get; set; }
        //P13 執行時間-迄
        public string P13EndDate { get; set; }
        public string Form { get; set; }
        public string Name { get; set; }
        public bool PaySelf { get; set; }

        private void SetMedicine(DeclareMedicine medicine, bool paySelf = false)
        {
            P2MedicalId = medicine.Id;//p2 藥品代號 
            P7Total = medicine.Amount;//p7 總量
            P8Price = paySelf ? medicine.Price : medicine.HcPrice;//p8 單價
            P3Dosage = medicine.MedicalCategory.Dosage;//p3
            P4Usage = medicine.Usage.Name;//p4
            P5Position = medicine.Position;//p5
            P11Days = string.IsNullOrEmpty(medicine.Days) ? 0 : int.Parse(medicine.Days); //p11
        }

        private void SetDate(string start, string end)
        {
            P12StartDate = start;//p12
            P13EndDate = end;//p13
        }

        private void CountPoint()//p9
        {
            if (PaySelf)
                P9Point = 0;
            else
            {
                //醫令類別1（用藥明細）：以單價＊總量
                if (P1MedicalOrder == "1" || (P1MedicalOrder=="4" && P2MedicalId.Length == 10))
                    P9Point = Convert.ToInt32(Math.Round(P8Price * P7Total, MidpointRounding.AwayFromZero));
                /*
                 *醫令類別4（不得另計價之藥品、檢驗（查）、診療項目或材料）：
                 *醫令類別4且醫令代碼長度10碼者，以單價＊總量。
                 *醫令類別4且醫令代碼不為10碼者，以單價＊總量＊支付成數。
                 */
                else
                {
                    var addition = P6Percent * 0.01;
                    P9Point = Convert.ToInt32(Math.Round(P8Price * P7Total * addition, MidpointRounding.AwayFromZero));
                }
            }
        }
        public object Clone()
        {
            return new DeclareDetail(this);
        }
    }
}