﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using GalaSoft.MvvmLight;
using His_Pos.Class;
using His_Pos.Class.Employee;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Product.Medicine.MedBag;
using His_Pos.Properties;
using His_Pos.Service;
using Microsoft.Reporting.WinForms;
using Employee = His_Pos.NewClass.Person.Employee.Employee;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;

namespace His_Pos.NewClass.PrescriptionRefactoring.Service
{
    public abstract class PrescriptionService:ObservableObject
    {
        #region AbstractFunctions
        public abstract bool CheckPrescription();
        public abstract bool NormalAdjust();
        public abstract bool ErrorAdjust();
        public abstract bool DepositAdjust();
        public abstract bool Register();
        #endregion
        public PrescriptionService()
        {

        }

        protected PrescriptionService(Prescription p)
        {
            current = p;
        }

        protected Prescription current { get; set; }
        #region Functions
        public static PrescriptionService CreateService(Prescription p)
        {
            var ps = PrescriptionServiceProvider.CreateService(p.Type);
            ps.current = p;
            return ps;
        }
        public bool StartNormalAdjust()
        {
            return NormalAdjust();
        }
        public bool StartErrorAdjust()
        {
            return ErrorAdjust();
        }
        public bool StartDepositAdjust()
        {
            return DepositAdjust();
        }
        public bool StartRegister()
        {
            return Register();
        }

        public bool SetPharmacist(Employee selectedPharmacist,int prescriptionCount)
        {
            if (selectedPharmacist is null || string.IsNullOrEmpty(selectedPharmacist.IDNumber))
            {
                MessageWindow.ShowMessage(Resources.尚未選擇藥師, MessageType.ERROR);
                return false;
            }
            if (prescriptionCount >= 80)
            {
                var confirm = new ConfirmWindow(Resources.調劑張數提醒 + prescriptionCount + "張，是否繼續調劑?", "調劑張數提醒", true);
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                var result = (bool)confirm.DialogResult;
                if (!result) return false;
            }
            current.Pharmacist = selectedPharmacist;
            return true;
        }

        protected void CheckAnonymousPatient()
        {
            if (!current.IsPrescribe) return;
            if (!current.Patient.CheckData())
            {
                var confirm = new ConfirmWindow("尚未選擇客戶或資料不全，是否以匿名取代?", "");
                Debug.Assert(confirm.DialogResult != null, "confirm.DialogResult != null");
                if ((bool)confirm.DialogResult)
                    current.Patient = Customer.GetCustomerByCusId(0);
                current.SetPrescribeAdjustCase();
            }
            current.SetPrescribeAdjustCase();
        }

        protected bool CheckValidCustomer()
        {
            if (current.Patient.CheckData()) return false;
            MessageWindow.ShowMessage("尚未選擇客戶", MessageType.ERROR);
            return true;
        }

        protected bool CheckMedicines()
        {
            var errorMsg = current.Medicines.Check();
            if (string.IsNullOrEmpty(errorMsg)) return true;
            MessageWindow.ShowMessage(errorMsg, MessageType.WARNING);
            return false;
        }

        public bool CheckMedicalNumber()
        {
            if (string.IsNullOrEmpty(current.TempMedicalNumber))
            {
                var medicalNumberEmptyConfirm = new ConfirmWindow("就醫序號尚未填寫，確認繼續?(\"否\"返回填寫，\"是\"繼續調劑)?", "卡序確認");
                Debug.Assert(medicalNumberEmptyConfirm.DialogResult != null, "medicalNumberEmptyConfirm.DialogResult != null");
                return (bool)medicalNumberEmptyConfirm.DialogResult;
            }

            if (current.TempMedicalNumber.Length != 4)
            {
                MessageWindow.ShowMessage("就醫序號長度錯誤，應為4碼", MessageType.ERROR);
                return false;
            }
            return true;
        }

        public static IEnumerable<ReportParameter> CreateSingleMedBagParameter(MedBagMedicine m,Prescription p)
        {
            var treatmentDate = DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                   treatmentDate.Split('/')[2] + "日";
            var cusGender = p.Patient.CheckGender();
            string patientTel;
            if (!string.IsNullOrEmpty(p.Patient.CellPhone))
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.CellPhone : p.Patient.CellPhone + "(註)";
            else
            {
                if (!string.IsNullOrEmpty(p.Patient.Tel))
                    patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.Tel : p.Patient.Tel + "(註)";
                else
                    patientTel = p.Patient.ContactNote;
            }
            return new List<ReportParameter>
                    {
                        new ReportParameter("PharmacyName_Id",
                            VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                        new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                        new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                        new ReportParameter("MedicalPerson",VM.CurrentPharmacy.GetPharmacist().Name),
                        new ReportParameter("PatientName", p.Patient.Name),
                        new ReportParameter("PatientGender_Birthday",(cusGender) + "/" + DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)),
                        new ReportParameter("TreatmentDate", treatmentDateChi),
                        new ReportParameter("RecId", " "), //病歷號
                        new ReportParameter("Division",p.Division is null ?string.Empty:p.Division.Name),
                        new ReportParameter("Hospital", p.Institution.Name),
                        new ReportParameter("PaySelf", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ServicePoint", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                        new ReportParameter("TotalPoint", p.PrescriptionPoint.TotalPoint.ToString()),
                        new ReportParameter("CopaymentPoint", p.PrescriptionPoint.CopaymentPoint.ToString()),
                        new ReportParameter("HcPoint", p.PrescriptionPoint.ApplyPoint.ToString()),
                        new ReportParameter("MedicinePoint", p.PrescriptionPoint.MedicinePoint.ToString(CultureInfo.InvariantCulture)),
                        new ReportParameter("MedicineId", m.Id),
                        new ReportParameter("MedicineName", m.Name),
                        new ReportParameter("MedicineChineseName", m.ChiName),
                        new ReportParameter("Ingredient", m.Ingredient),
                        new ReportParameter("Indication", m.Indication),
                        new ReportParameter("SideEffect", m.SideEffect),
                        new ReportParameter("Note", m.Note),
                        new ReportParameter("Usage", m.Usage),
                        new ReportParameter("MedicineDay", m.MedicineDays),
                        new ReportParameter("Amount", m.Total),
                        new ReportParameter("Form", m.Form),
                        new ReportParameter("PatientTel", patientTel)
                    };
        }
        public static IEnumerable<ReportParameter> CreateMultiMedBagParameter(Prescription p)
        {
            var treatmentDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var treatmentDateChi = string.Empty;
            if (!string.IsNullOrEmpty(treatmentDate))
                treatmentDateChi = treatmentDate.Split('/')[0] + "年" + treatmentDate.Split('/')[1] + "月" +
                                      treatmentDate.Split('/')[2] + "日";
            var cusGender = p.Patient.CheckGender();
            string patientTel;
            if (!string.IsNullOrEmpty(p.Patient.CellPhone))
                patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.CellPhone : p.Patient.CellPhone + "(註)";
            else
            {
                if (!string.IsNullOrEmpty(p.Patient.Tel))
                    patientTel = string.IsNullOrEmpty(p.Patient.ContactNote) ? p.Patient.Tel : p.Patient.Tel + "(註)";
                else
                    patientTel = p.Patient.ContactNote;
            }
            return new List<ReportParameter>
            {
                new ReportParameter("PharmacyName_Id",
                    VM.CurrentPharmacy.Name + "(" + VM.CurrentPharmacy.ID + ")"),
                new ReportParameter("PharmacyAddress", VM.CurrentPharmacy.Address),
                new ReportParameter("PharmacyTel", VM.CurrentPharmacy.Tel),
                new ReportParameter("MedicalPerson", VM.CurrentPharmacy.GetPharmacist().Name),
                new ReportParameter("PatientName", p.Patient.Name),
                new ReportParameter("PatientGender_Birthday",cusGender + "/" +DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true)),
                new ReportParameter("TreatmentDate", treatmentDateChi),
                new ReportParameter("Hospital", p.Institution.Name),
                new ReportParameter("PaySelf", p.PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ServicePoint", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalPoint", p.PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentPoint",p.PrescriptionPoint.CopaymentPoint.ToString()),
                new ReportParameter("HcPoint", p.PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("MedicinePoint", p.PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("Division", p.Division is null ?string.Empty:p.Division.Name),
                new ReportParameter("PatientTel", patientTel)
            };
        }
        #endregion

        public static IEnumerable<ReportParameter> CreateDepositSheetParameters(Prescription p)
        {
            var adjustDate =
                DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var dateString = DateTimeExtensions.ConvertDateStringSplitToChinese(adjustDate);
            var printTime = adjustDate + "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ")";
            return new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", p.Patient.Name),
                new ReportParameter("AdjustDate", dateString),
                new ReportParameter("Deposit", p.PrescriptionPoint.Deposit.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(p.PrescriptionPoint.Deposit)),
                new ReportParameter("PrintTime", printTime)
            };
        }

        public static IEnumerable<ReportParameter> CreateReceiptParameters(Prescription p)
        {
            var adjustDate = DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, true);
            var cusGender = p.Patient.CheckGender();
            var copaymentPoint = p.PrescriptionPoint.CopaymentPoint;
            var actualReceive = p.PrescriptionPoint.ActualReceive;
            var birth = DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, true);
            if (p.PrescriptionStatus.IsVIP)
            {
                copaymentPoint = 0;
                actualReceive = p.PrescriptionPoint.ActualReceive - p.PrescriptionPoint.CopaymentPoint;
            }
            string patientName;
            if (string.IsNullOrEmpty(p.Patient.Name) || p.Patient.Name.Equals("匿名"))
                patientName = " ";
            else
                patientName = p.Patient.Name;
            if(string.IsNullOrEmpty(birth))
                birth = "  /  /  ";
            if (p.AdjustCase.ID.Equals("0"))
            {
                return new List<ReportParameter>
                    {
                        new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                        new ReportParameter("PatientName", patientName),
                        new ReportParameter("Gender", cusGender),
                        new ReportParameter("Birthday",birth),
                        new ReportParameter("AdjustDate", adjustDate),
                        new ReportParameter("Hospital", p.Institution.Name),
                        new ReportParameter("Doctor", " "), //病歷號
                        new ReportParameter("MedicalNumber"," "),
                        new ReportParameter("MedicineCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("MedicalServiceCost", (p.PrescriptionPoint.AmountsPay - p.PrescriptionPoint.AmountSelfPay).ToString()),
                        new ReportParameter("TotalMedicalCost","0"),
                        new ReportParameter("CopaymentCost", "0"),
                        new ReportParameter("HcPay", "0"),
                        new ReportParameter("SelfCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                        new ReportParameter("ActualReceive", p.PrescriptionPoint.ActualReceive.ToString()),
                        new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(p.PrescriptionPoint.ActualReceive))
                    };
            }
            return new List<ReportParameter>
            {
                new ReportParameter("Pharmacy", VM.CurrentPharmacy.Name),
                new ReportParameter("PatientName", patientName),
                new ReportParameter("Gender", cusGender),
                new ReportParameter("Birthday",birth),
                new ReportParameter("AdjustDate", adjustDate),
                new ReportParameter("Hospital", string.IsNullOrEmpty(p.Institution.Name)?" ":p.Institution.Name),
                new ReportParameter("Doctor", " "), //病歷號
                new ReportParameter("MedicalNumber", string.IsNullOrEmpty(p.TempMedicalNumber)?" ":p.TempMedicalNumber),
                new ReportParameter("MedicineCost", p.PrescriptionPoint.MedicinePoint.ToString()),
                new ReportParameter("MedicalServiceCost", p.PrescriptionPoint.MedicalServicePoint.ToString()),
                new ReportParameter("TotalMedicalCost",p.PrescriptionPoint.TotalPoint.ToString()),
                new ReportParameter("CopaymentCost", copaymentPoint.ToString()),
                new ReportParameter("HcPay", p.PrescriptionPoint.ApplyPoint.ToString()),
                new ReportParameter("SelfCost", p.PrescriptionPoint.AmountSelfPay.ToString()),
                new ReportParameter("ActualReceive", actualReceive.ToString()),
                new ReportParameter("ActualReceiveChinese", NewFunction.ConvertToAsiaMoneyFormat(actualReceive))
            };
        }
    }
}
