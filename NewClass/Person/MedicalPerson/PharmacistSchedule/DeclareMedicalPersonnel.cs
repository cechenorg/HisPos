using GalaSoft.MvvmLight;
using His_Pos.Service;
using System;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class DeclareMedicalPersonnel : ObservableObject
    {
        public DeclareMedicalPersonnel(Employee.Employee selectedMedicalPersonnel)
        {
            if (selectedMedicalPersonnel is null) return;
            ID = selectedMedicalPersonnel.ID;
            Name = selectedMedicalPersonnel.Name;
            IDNumber = selectedMedicalPersonnel.IDNumber;
            PrescriptionCount = null;
            StartDate = selectedMedicalPersonnel.StartDate;
            LeaveDate = selectedMedicalPersonnel.LeaveDate;
        }

        public DeclareMedicalPersonnel(DataRow r)
        {
            ID = r.Field<int>("Emp_ID");
            Name = r.Field<string>("Emp_Name");
            IDNumber = r.Field<string>("Emp_IDNumber");
            StartDate = r.Field<DateTime?>("Emp_StartDate");
            LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
            IsEnable = r.Field<bool>("Emp_IsEnable");
            IsLocal = r.Field<bool>("Emp_IsLocal");
            if (NewFunction.CheckDataRowContainsColumn(r, "PrescriptionCount"))
                PrescriptionCount = r.Field<int?>("Prescription_Count");
        }

        private int id;

        public int ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
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

        private string name;

        public string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }

        private string idNumber;

        public string IDNumber
        {
            get => idNumber;
            set
            {
                Set(() => IDNumber, ref idNumber, value);
            }
        }

        private DateTime? startDate;//到職日

        public virtual DateTime? StartDate
        {
            get => startDate;
            set
            {
                Set(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? leaveDate;//離職日

        public virtual DateTime? LeaveDate
        {
            get => leaveDate;
            set
            {
                Set(() => LeaveDate, ref leaveDate, value);
            }
        }

        private bool isEnable;//備註

        [IgnoreFormat]
        public virtual bool IsEnable
        {
            get => isEnable;
            set
            {
                Set(() => IsEnable, ref isEnable, value);
            }
        }

        private bool isLocal;//是否為本店新增

        [IgnoreFormat]
        public virtual bool IsLocal
        {
            get => isLocal;
            set
            {
                Set(() => IsLocal, ref isLocal, value);
            }
        }
    }
}