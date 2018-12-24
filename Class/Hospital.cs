using System;
using System.Data;
using System.Linq;
using System.Xml;
using His_Pos.Class.Declare;
using His_Pos.Class.Person;
using His_Pos.Service;

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
            Common = false;
        }

        public Hospital(DataRow dataRow,DataSource source)
        {
            Id = dataRow["INS_ID"].ToString();
            Name = dataRow["INS_NAME"].ToString();
            FullName = dataRow["INS_FULLNAME"].ToString();
            if (dataRow.Table.Columns.Contains("INS_IS_COMMON"))
                Common = Convert.ToBoolean(dataRow["INS_IS_COMMON"]);
            else
            {
                Common = MainWindow.Hospitals.SingleOrDefault(h => h.Id.Equals(Id)).Common;
            }
            switch (source) {
                case DataSource.InitHospitalData:
                    Doctor = new MedicalPersonnel();
                    Division = new Division.Division();
                    break;
                case DataSource.GetHospitalData:
                    Doctor = new MedicalPersonnel(dataRow,true);
                    Division = MainWindow.Divisions.SingleOrDefault(d=>d.Id.Equals(dataRow["HISDIV_ID"]));
                    break;
            }
        }
        public Hospital(XmlNode xml) {
            Doctor = new MedicalPersonnel();
            Id = xml.SelectSingleNode("d21") == null ? null : xml.SelectSingleNode("d21").InnerText;
            Doctor.IcNumber = xml.SelectSingleNode("d24") == null ? null : xml.SelectSingleNode("d24").InnerText;
            Division = new Division.Division(xml);
        }
        public Hospital(XmlDocument xml)
        {
            Doctor = new MedicalPersonnel();
            Id = xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/study").Attributes["doctor_id"].Value;
            Name = MainWindow.Hospitals.SingleOrDefault(hos => hos.Id == Id).Name;
            FullName = MainWindow.Hospitals.SingleOrDefault(hos => hos.Id == Id).FullName;
            Doctor.IcNumber = Id;
            Division = MainWindow.Divisions.SingleOrDefault(div => div.Id == xml.SelectSingleNode("DeclareXml/DeclareXmlDocument/case/study").Attributes["subject"].Value);
        }
        public Hospital(DeclareFileDdata d)
        {
            Doctor = new MedicalPersonnel();
            Doctor.IcNumber = !string.IsNullOrEmpty(d.Dhead.D24) ? d.Dhead.D24 : string.Empty;
        }

        private MedicalPersonnel _doctor;

        public MedicalPersonnel Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                NotifyPropertyChanged(nameof(Doctor));
            }
        }

        private Division.Division _division;

        public Division.Division Division
        {
            get => _division;
            set
            {
                _division = value;
                NotifyPropertyChanged(nameof(Division));
            }
        }

        private bool _common;

        public bool Common
        {
            get => _common;
            set
            {
                _common = value;
                NotifyPropertyChanged(nameof(Common));
            }
        }
    }
}