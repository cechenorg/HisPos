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
                    MedicalOrder = medicalOrder;//p1
                    MedicalId = medicalId;//p2
                    Percent = percent;//p6 支付成數
                    Total = total;//p7 總量
                    Price = price;//p8 單價
                    CountPoint();
                    Sequence = sequence;//p10
                    SetDate(start, end);
                    break;
                case "dayPay":
                    MedicalOrder = medicalOrder;//p1
                    MedicalId = medicalId;//p2
                    Dosage = 0001.00;//p3
                    Percent = percent;//p6 支付成數
                    Total = total;//p7 總量
                    Price = price;//p8 單價
                    CountPoint();//p9 點數
                    Sequence = sequence;//p10 循序號
                    Days = int.Parse(Total.ToString());//天數
                    SetDate(start, end);//起屹日期
                    break;
            }
        }

        public DeclareDetail(DeclareMedicine medicine, int sequence)
        {
            const string medicationDetail = "1"; //p1 醫令類別 用藥明細
            const string notPriced = "4"; //p1 醫令類別 不得另計價之藥品
            MedicalOrder = !medicine.PaySelf ? medicationDetail : notPriced;
            PaySelf = medicine.PaySelf;
            SetMedicine(medicine);
            Sequence = sequence;
            CountPoint();
        }

        public DeclareDetail(PrescriptionOTC otc)
        {
            MedicalOrder = "0";
            PaySelf = true;
            MedicalId = otc.Id;//p2 藥品代號 
            Total = otc.Amount;//p7 總量
            Price = otc.Price;//p8 單價
            Dosage = otc.Dosage;//p3
            Usage = otc.Usage?.Id;//p4
            Position = string.IsNullOrEmpty(otc.Position)? otc.Position:string.Empty;//p5
            Days = string.IsNullOrEmpty(otc.Days) ? 0 : int.Parse(otc.Days); //p11
            Point = 0;
        }

        public DeclareDetail()
        {
        }

        public DeclareDetail(DataRow row) {
            MedicalOrder = row["HISDECDET_ID"].ToString();
            MedicalId = row["HISMED_ID"].ToString();
            Name = row["PRO_NAME"].ToString();
            Dosage = row["HISDECDET_AMOUNT"].ToString() == "" ? 0 : Convert.ToDouble(row["HISDECDET_AMOUNT"].ToString());
            Usage = row["HISWAY_ID"].ToString();
            Days = row["HISDECDET_DRUGDAY"].ToString() == "" ? 0 : Convert.ToInt32(row["HISDECDET_DRUGDAY"].ToString());
        }

        public DeclareDetail(XmlNode xml) {
            MedicalOrder = xml.SelectSingleNode("p1") == null ? null : xml.SelectSingleNode("p1").InnerText;
            MedicalId = xml.SelectSingleNode("p2") == null ? null : xml.SelectSingleNode("p2").InnerText;
            Dosage = xml.SelectSingleNode("p3") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p3").InnerText);
            Usage = xml.SelectSingleNode("p4") == null ? null : xml.SelectSingleNode("p4").InnerText;
            Position = xml.SelectSingleNode("p5") == null ? null : xml.SelectSingleNode("p5").InnerText;
            Percent = xml.SelectSingleNode("p6") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p6").InnerText);
            Total = xml.SelectSingleNode("p7") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p7").InnerText);
            Price = xml.SelectSingleNode("p8") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p8").InnerText);
            Point = xml.SelectSingleNode("p9") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p9").InnerText);
            Sequence = xml.SelectSingleNode("p10") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p10").InnerText);
            Days = xml.SelectSingleNode("p11") == null ? 0 : Convert.ToInt32(xml.SelectSingleNode("p11").InnerText);
            StartDate = xml.SelectSingleNode("p12") == null ? null : xml.SelectSingleNode("p12").InnerText;
            EndDate = xml.SelectSingleNode("p13") == null ? null : xml.SelectSingleNode("p13").InnerText;
            Form = xml.SelectSingleNode("p14") == null ? null : xml.SelectSingleNode("p14").InnerText;
        }
        public DeclareDetail(DeclareDetail declareDetail) {
            MedicalOrder = declareDetail.MedicalOrder;
            MedicalId = declareDetail.MedicalId;
            Dosage = declareDetail.Dosage;
            Usage = declareDetail.Usage;
            Position = declareDetail.Position;
            Percent = declareDetail.Percent;
            Total = declareDetail.Total;
            Price = declareDetail.Price;
            Point = declareDetail.Point;
            Sequence = declareDetail.Sequence;
            Days = declareDetail.Days;
            StartDate = declareDetail.StartDate;
            EndDate = declareDetail.EndDate;
            Form = declareDetail.Form;
            PaySelf = PaySelf;
        }
       
        public string MedicalOrder { get; set; }//p1
        public string MedicalId { get; set; }//p2
        public double Dosage { get; set; }//p3
        public string Usage { get; set; }//p4
        public string Position { get; set; }//p5
        public double Percent { get; set; }//p6
        public double Total { get; set; }//p7
        public double Price { get; set; }//p8
        public int Point { get; set; }//p9
        public int Sequence { get; set; }//p10
        public int Days { get; set; }//p11
        public string StartDate { get; set; }//p12
        public string EndDate { get; set; }//p13
        public string MedicalPersonnelIcNumber { get; set; }//p14
        public string Form { get; set; }
        public string Name { get; set; }
        public bool PaySelf { get; set; }

        private void SetMedicine(DeclareMedicine medicine, bool paySelf = false)
        {
            MedicalId = medicine.Id;//p2 藥品代號 
            Total = medicine.Amount;//p7 總量
            Price = paySelf ? medicine.Price : medicine.HcPrice;//p8 單價
            Dosage = medicine.MedicalCategory.Dosage;//p3
            Usage = medicine.Usage.Id;//p4
            Position = medicine.Position;//p5
            Days = string.IsNullOrEmpty(medicine.Days) ? 0 : int.Parse(medicine.Days); //p11
        }

        private void SetDate(string start, string end)
        {
            StartDate = start;//p12
            EndDate = end;//p13
        }

        private void CountPoint()//p9
        {
            if (PaySelf)
                Point = 0;
            else
            {
                //醫令類別1（用藥明細）：以單價＊總量
                if (MedicalOrder == "1" || (MedicalOrder=="4" && MedicalId.Length == 10))
                    Point = Convert.ToInt32(Math.Round(Price * Total, MidpointRounding.AwayFromZero));
                /*
                 *醫令類別4（不得另計價之藥品、檢驗（查）、診療項目或材料）：
                 *醫令類別4且醫令代碼長度10碼者，以單價＊總量。
                 *醫令類別4且醫令代碼不為10碼者，以單價＊總量＊支付成數。
                 */
                else
                {
                    var addition = Percent * 0.01;
                    Point = Convert.ToInt32(Math.Round(Price * Total * addition, MidpointRounding.AwayFromZero));
                }
            }
        }
        public object Clone()
        {
            return new DeclareDetail(this);
        }
    }
}