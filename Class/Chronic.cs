using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class {
    public class Chronic {
        public Chronic(DataRow row) {
            DecMasId = row["HISDECMAS_ID"].ToString();
            ContinueNum = row["HISDECMAS_CONTINUOUSNUM"].ToString();
            ContinueTotal = row["HISDECMAS_CONTINUOUSTOTAL"].ToString();
            TreatDate = row["HISDECMAS_TREATDATE"].ToString();
            hospital = new Hospital(row,DataSource.InitHospitalData);
            division = new Division.Division(row);
        } 


        public string DecMasId { get; set; }
        public string ContinueNum { get; set; }
        public string ContinueTotal { get; set; }
        public string TreatDate { get; set; }
        public string InsId { get; set; }
        public Hospital hospital { get; set; }
        public Division.Division division { get; set; }
    }
}
