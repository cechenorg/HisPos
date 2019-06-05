using System;
using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.Service;

namespace His_Pos.NewClass.Person.MedicalPerson.PharmacistSchedule
{
    public class DeclareMedicalPersonnel : ObservableObject
    {
        public DeclareMedicalPersonnel(Employee.Employee selectedMedicalPersonnel)
        {
            if(selectedMedicalPersonnel is null) return;
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
            StartDate = r.Field<DateTime>("Emp_StartDate");
            LeaveDate = r.Field<DateTime?>("Emp_LeaveDate");
            IsEnable = r.Field<bool>("Emp_IsEnable");
            IsLocal = r.Field<bool>("Emp_IsLocal");
            if(NewFunction.CheckDataRowContainsColumn(r,"Prescription_Count"))
                PrescriptionCount = r.Field<int?>("Prescription_Count");
        }
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }//姓名
        public virtual string IDNumber { get; set; }//身分證字號
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
        public virtual bool IsEnable
        {
            get => isEnable;
            set
            {
                Set(() => IsEnable, ref isEnable, value);
            }
        }
        private bool isLocal;//是否為本店新增
        public virtual bool IsLocal
        {
            get => isLocal;
            set
            {
                Set(() => IsLocal, ref isLocal, value);
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
    }
}
