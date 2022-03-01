using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Medicine.Position;
using His_Pos.NewClass.Medicine.Usage;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Prescription.Treatment.AdjustCase;
using His_Pos.NewClass.Prescription.Treatment.Copayment;
using His_Pos.NewClass.Prescription.Treatment.Division;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using His_Pos.NewClass.Prescription.Treatment.PaymentCategory;
using His_Pos.NewClass.Prescription.Treatment.PrescriptionCase;
using His_Pos.NewClass.Prescription.Treatment.SpecialTreat;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ZeroFormatter;
using DiseaseCode = His_Pos.NewClass.Prescription.Treatment.DiseaseCode.DiseaseCode;
using DiseaseCodeDb = His_Pos.NewClass.Prescription.Treatment.DiseaseCode.DiseaseCodeDb;
using PaymentCategory = His_Pos.NewClass.Prescription.Treatment.PaymentCategory.PaymentCategory;

namespace His_Pos.NewClass.OfflineDataSet
{
    [ZeroFormattable]
    public class OfflineDataSet
    {
        [Index(0)]
        public virtual IList<Institution> Institutions { get; set; }

        [Index(1)]
        public virtual IList<Division> Divisions { get; set; }

        [Index(2)]
        public virtual IList<Employee> MedicalPersonnels { get; set; }

        [Index(3)]
        public virtual IList<DiseaseCode> DiseaseCodes { get; set; }

        [Index(4)]
        public virtual IList<AdjustCase> AdjustCases { get; set; }

        [Index(5)]
        public virtual IList<PrescriptionCase> PrescriptionCases { get; set; }

        [Index(6)]
        public virtual IList<Copayment> Copayments { get; set; }

        [Index(7)]
        public virtual IList<PaymentCategory> PaymentCategories { get; set; }

        [Index(8)]
        public virtual IList<SpecialTreat> SpecialTreats { get; set; }

        [Index(9)]
        public virtual IList<Usage> Usages { get; set; }

        [Index(10)]
        public virtual IList<Position> Positions { get; set; }

        [Index(11)]
        public virtual IList<Product> Products { get; set; }

        [Index(12)]
        public virtual int ReaderCom { get; set; }

        [Index(13)]
        public virtual bool NewInstitution { get; set; }

        public OfflineDataSet()
        {
        }

        public OfflineDataSet(Institutions institutions, Divisions divisions, Employees medicalPersonnels, AdjustCases adjustCases, PrescriptionCases prescriptionCases, Copayments copayments, PaymentCategories paymentCategories, SpecialTreats specialTreats, Usages usages, Positions positions)
        {
            Institutions = new List<Institution>();
            Institutions = institutions.ToList();
            Divisions = new List<Division>();
            Divisions = divisions.ToList();
            MedicalPersonnels = new List<Employee>();
            MedicalPersonnels = medicalPersonnels.ToList();
            DiseaseCodes = new List<DiseaseCode>();
            var diseaseTable = DiseaseCodeDb.GetDiseaseCodes();
            foreach (DataRow r in diseaseTable.Rows)
            {
                DiseaseCodes.Add(new DiseaseCode(r));
            }
            var diseaseICD9Table = DiseaseCodeDb.GetICD9DiseaseCodes();
            foreach (DataRow r in diseaseICD9Table.Rows)
            {
                var disease = new DiseaseCode(r);
                disease.ICD9_ID = r.Field<string>("DisCodeMap_ICD9_ID");
                DiseaseCodes.Add(disease);
            }
            AdjustCases = new List<AdjustCase>();
            AdjustCases = adjustCases.ToList();
            PrescriptionCases = new List<PrescriptionCase>();
            PrescriptionCases = prescriptionCases.ToList();
            Copayments = new List<Copayment>();
            Copayments = copayments.ToList();
            PaymentCategories = new List<PaymentCategory>();
            PaymentCategories = paymentCategories.ToList();
            SpecialTreats = new List<SpecialTreat>();
            SpecialTreats = specialTreats.ToList();
            Usages = new List<Usage>();
            Usages = usages.ToList();
            Positions = new List<Position>();
            Positions = positions.ToList();
            Products = new List<Product>();
            //var productStructs = new ProductStructs(ProductDB.GetProductStructsBySearchString(""));
            //foreach (var p in productStructs)
            //{
            //    Products.Add(new Product(p));
            //}
            ReaderCom = ViewModelMainWindow.CurrentPharmacy.ReaderCom;
            NewInstitution = ViewModelMainWindow.CurrentPharmacy.NewInstitution;
        }
    }
}