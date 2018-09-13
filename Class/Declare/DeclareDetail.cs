using His_Pos.Class.Product;
using System;
using System.Data;
using System.Xml;

namespace His_Pos.Class.Declare
{
    public class DeclareDetail : ICloneable
    {
        public DeclareDetail(string medicalOrder,string medicalId, double percent, double price, int sequence, string start, string end)
        {
            MedicalOrder = medicalOrder;
            MedicalId = medicalId;
            Dosage = 0;
            Total = 00001.0;
            Price = price;
            Percent = percent;
            CountPoint();
            Sequence = sequence;
            SetDate(start, end);
        }

        public DeclareDetail(DeclareMedicine medicine, AdjustCase.AdjustCase adjustCase, int sequence)
        {
            if (!medicine.PaySelf || adjustCase.Id == "3")//p1
                MedicalOrder = "1";
            else
                MedicalOrder = "4";
            SetMedicine(medicine);
            SetMedicate(medicine);
            Sequence = sequence;
            CountPoint();
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
            Point = xml.SelectSingleNode("p9") == null ? 0 : Convert.ToDouble(xml.SelectSingleNode("p9").InnerText);
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
        }
       
        public string MedicalOrder { get; set; }//p1
        public string MedicalId { get; set; }//p2
        public double Dosage { get; set; }//p3
        public string Usage { get; set; }//p4
        public string Position { get; set; }//p5
        public double Percent { get; set; }//p6
        public double Total { get; set; }//p7
        public double Price { get; set; }//p8
        public double Point { get; set; }//p9
        public int Sequence { get; set; }//p10
        public int Days { get; set; }//p11
        public string StartDate { get; set; }//p12
        public string EndDate { get; set; }//p13
        public string MedicalPersonnelIcNumber { get; set; }//p14
        public string Form { get; set; }
        public string Name { get; set; }

        private void SetMedicate(DeclareMedicine medicine)
        {
            Dosage = double.Parse(medicine.MedicalCategory.Dosage);//p3
            Usage = medicine.Usage.Id;//p4
            Position = medicine.Position;//p5
            Days = int.Parse(medicine.Days);//p11
        }

        private void SetMedicine(DeclareMedicine medicine)
        {
            MedicalId = medicine.Id;//p2
            Total = medicine.Amount;//p7
            Price = double.Parse(medicine.HcPrice);//p8
        }

        private void SetDate(string start, string end)
        {
            StartDate = start;//p12
            EndDate = end;//p13
        }

        private void SetMedicalPersonnelIcNumber(string icNum)
        {
            MedicalPersonnelIcNumber = icNum;
        }

        private void CountPoint()//p9
        {
            if (MedicalOrder == "1")
                Point = Price * Total;
            else
            {
                Point = Price * Total * Percent;
            }
        }
        public object Clone()
        {
            return new DeclareDetail(this);
        }
    }
}