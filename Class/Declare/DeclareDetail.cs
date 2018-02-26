using His_Pos.Class.Product;

namespace His_Pos.Class.Declare
{
    public class DeclareDetail
    {
        public DeclareDetail(string medicalId, double percent,double price,int sequence,string start,string end)
        {
            MedicalOrder = "9";
            MedicalId = medicalId;
            Dosage = 0;
            Total = 00001.0;
            Price = price;
            Percent = percent;
            CountPoint();
            Sequence = sequence;
            SetDate(start,end);
        }

        public DeclareDetail(Medicine medicine,string adjustCase,int sequence)
        {
            if(!medicine.PaySelf || adjustCase == "3")//p1
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
        public string MedicalPersonnelId { get; set; }//p14
        public string Form { get; set; }

        private void SetMedicate(Medicine medicine)
        {
            Dosage = double.Parse(medicine.MedicalCategory.Dosage);//p3
            Usage = medicine.MedicalCategory.Usage;//p4
            Position = medicine.MedicalCategory.Position;//p5
            Days = medicine.MedicalCategory.Days;//p11
        }
        private void SetMedicine(Medicine medicine)
        {
            MedicalId = medicine.Id;//p2
            Total = medicine.Total;//p7
            Price = medicine.HcPrice;//p8
        }
        private void SetDate(string start,string end)
        {
            StartDate = start;//p12
            EndDate = end;//p13
        }
        private void SetMedicalPersonnelId(string Id)
        {
            MedicalPersonnelId = Id;
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
      
    }   
}
