﻿using System.Data;
using His_Pos.AbstractClass;
using His_Pos.Class.Person;

namespace His_Pos.Class
{
    public class Hospital : Selection
    {
        public Hospital()
        {
            Id = "";
            Name = "";
            Doctor = new MedicalPersonnel();
            Division = new Division.Division();
        }

        public Hospital(DataRow dataRow,DataSource source)
        {
            Id = dataRow["INS_ID"].ToString();
            Name = dataRow["INS_NAME"].ToString();
            FullName = dataRow["INS_FULLNAME"].ToString();
            switch (source) {
                case DataSource.InitHospitalData:
                    Doctor = new MedicalPersonnel();
                    Division = new Division.Division();
                    break;
                case DataSource.GetHospitalData:
                    Doctor = new MedicalPersonnel(dataRow);
                    Division = new Division.Division(dataRow);
                    break;
            }
        }
    
        public MedicalPersonnel Doctor { get; set; }
        public Division.Division Division { get; set; }
    }
}