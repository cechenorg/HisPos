using System;
using System.Data;

namespace His_Pos.Class {
    public class Chronic {
        public Chronic() {
            hospital = new Hospital();
            division = new Division.Division();
        }
        public Chronic(DataRow row) {
            DecMasId = row["HISDECMAS_ID"].ToString();
            ContinueNum = row["HISDECMAS_CONTINUOUSNUM"].ToString();
            ContinueTotal = row["HISDECMAS_CONTINUOUSTOTAL"].ToString();
            TreatDate = Convert.ToDateTime(row["HISDECMAS_TREATDATE"].ToString()).ToString("yyyy/MM/dd");
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
