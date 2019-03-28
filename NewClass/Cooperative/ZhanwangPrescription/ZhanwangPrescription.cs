using His_Pos.NewClass.CooperativeInstitution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Cooperative.ZhanwangPrescription
{
    public class ZhanwangPrescription : CooperativePrescription
    {
        public ZhanwangPrescription(string path) {
            FilePath = path;
            GetFile();
        }
        public string FilePath { get; set; }
        public CooperativePrescription cooperativePrescription { get; set; }
        private void GetFile() { }
    }
}
