using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
    public class DeclareDetail
    {
        public DeclareDetail(Medicine medicine)
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
        public string Days { get; set; }//p11
        public string StartDate { get; set; }//p12
        public string EndDate { get; set; }//p13
        public string MedicalPersonnelId { get; set; }//p14
    }
}
