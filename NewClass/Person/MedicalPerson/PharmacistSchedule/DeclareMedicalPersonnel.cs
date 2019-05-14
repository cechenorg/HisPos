﻿using System.Data;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class DeclareMedicalPersonnel:MedicalPersonnel
    {
        public DeclareMedicalPersonnel(MedicalPersonnel selectedMedicalPersonnel) : base()
        {
            ID = selectedMedicalPersonnel.ID;
            Name = selectedMedicalPersonnel.Name;
            IDNumber = selectedMedicalPersonnel.IDNumber;
            PrescriptionCount = null;
            StartDate = selectedMedicalPersonnel.StartDate;
            LeaveDate = selectedMedicalPersonnel.LeaveDate;
        }

        public DeclareMedicalPersonnel(DataRow r) : base(r)
        {
            PrescriptionCount = r.Field<int?>("Prescription_Count");
        }
        private int? prescriptionCount;
        public int? PrescriptionCount
        {
            get => prescriptionCount;
            set
            {
                Set(() => PrescriptionCount, ref prescriptionCount, value);
            }
        }
    }
}