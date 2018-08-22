using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using His_Pos.Class;
using His_Pos.Class.MedBag;
using His_Pos.Class.MedBagLocation;
using His_Pos.Class.Product;
using Microsoft.Reporting.WinForms;

namespace His_Pos.RDLC
{
    public static class ReportService
    {
        public const string ReportPath = @"..\..\RDLC\MedBagReport.rdlc";
        public static Report CreatReport(MedBag selectedMedBag,Prescription p, int medicineIndex)
        {
            var medBagReport = new Report
            {
                Xmlns = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition",
                Rd = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner",
                Body = new Body
                {
                    ReportItems = new ReportItems(),
                    Height = selectedMedBag.BagHeight + "cm",
                    Style = new Style()
                },
                Page = new Page
                {
                    PageHeight = selectedMedBag.BagHeight.ToString(CultureInfo.InvariantCulture) + "cm",
                    PageWidth = selectedMedBag.BagWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                    Style = string.Empty,
                    LeftMargin = "0cm",
                    RightMargin = "0cm",
                    TopMargin = "0cm",
                    BottomMargin = "0cm",
                    ColumnSpacing = "0cm"
                },
                Width = selectedMedBag.BagWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                AutoRefresh = "0",
                ReportUnitType = "cm",
                ReportID = "cdd7925b-803a-4208-8788-8e2ae4bd14b8"
            };
            SetReportItem(medBagReport, selectedMedBag.MedLocations,p,medicineIndex);
            return medBagReport;
        }

        private static void SetReportItem(Report medBagReport, ObservableCollection<MedBagLocation> locations, Prescription p,int medicineIndex)
        {
            foreach (var m in locations)
                if (m.Name != "MedicineList")
                {
                    var locationDictionary = CreateDictionary(p,MainWindow.CurrentPharmacy, medicineIndex);
                    var valuePair = locationDictionary.SingleOrDefault(x => x.Key.Equals(m.Name));
                    medBagReport.Body.ReportItems.Textbox.Add(CreatTextBoxField(m,valuePair.Value));
                }
        }
        private static Textbox CreatTextBoxField(MedBagLocation m, string fieldContent)
        {
            return new Textbox
            {
                Name = m.Name,
                DefaultName = m.Name,
                CanGrow = "true",
                KeepTogether = "true",
                Top = m.PathY.ToString(CultureInfo.InvariantCulture) + "cm",
                Left = m.PathX.ToString(CultureInfo.InvariantCulture) + "cm",
                Height = m.RealHeight.ToString(CultureInfo.InvariantCulture) + "cm",
                Width = m.RealWidth.ToString(CultureInfo.InvariantCulture) + "cm",
                Paragraphs = new Paragraphs
                {
                    Paragraph = new Paragraph
                    {
                        Style = new Style
                        {
                            TextAlign = "Left"
                        },
                        TextRuns = new TextRuns
                        {
                            TextRun = new TextRun
                            {
                                Value = fieldContent,
                                Style = string.Empty
                            }
                        }
                    }
                },
                Style = new Style
                {
                    Border = new Border { Style = "None" },
                    PaddingLeft = "2pt",
                    PaddingRight = "2pt",
                    PaddingTop = "2pt",
                    PaddingBottom = "2pt"
                }
            };
        }
        public static string SerializeObject<T>(Report report)
        {
            var xmlSerializer = new XmlSerializer(report.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, report);
                return PrettyXml(textWriter);
            }
        }
        private static string PrettyXml(StringWriter writer)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(writer.ToString());

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }
        public static void CreatePdf(MedBag selectedMedBag,int medicineIndex)
        {
            var deviceInfo = "<DeviceInfo>" +
                             "  <OutputFormat>PDF</OutputFormat>" +
                             "  <PageWidth>" + selectedMedBag.BagWidth + "cm</PageWidth>" +
                             "  <PageHeight>" + selectedMedBag.BagHeight + "cm</PageHeight>" +
                             "  <MarginTop>0cm</MarginTop>" +
                             "  <MarginLeft>0cm</MarginLeft>" +
                             "  <MarginRight>0cm</MarginRight>" +
                             "  <MarginBottom>0cm</MarginBottom>" +
                             "</DeviceInfo>";
            deviceInfo = string.Format(deviceInfo, selectedMedBag.BagWidth, selectedMedBag.BagHeight);
            var viewer = new ReportViewer { ProcessingMode = ProcessingMode.Local };
            viewer.LocalReport.ReportPath = @"..\..\RDLC\MedBagReport.rdlc";
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo, out _, out _, out _,
                out _, out _);

            using (var fs = new FileStream("output"+ medicineIndex +".pdf", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private static Dictionary<string,string> CreateDictionary(Prescription p,Pharmacy currentPharmacy,int medicineIndex)
        {
            var m = p.Medicines[medicineIndex];
            var medBagDictionary =
                new Dictionary<string, string>
                {
                    {"Pharmacy", currentPharmacy.Name},
                    {"PharmacyId", currentPharmacy.Id},
                    {"PharmacyAddr", currentPharmacy.Address},
                    {"PharmacyTel", currentPharmacy.Tel},
                    {"MedicalPerson", MainWindow.CurrentUser.Name},
                    {"PatientName", p.Customer.Name},
                    {"PatientId", p.Customer.IcNumber},
                    {"PatientTel", p.Customer.ContactInfo.Tel},
                    {"PatientGender", p.Customer.Gender ? "男" : "女"},
                    {"PatientBirthday", p.Customer.Birthday},
                    {"MedRecNum", ""},
                    {"AdjustDate", p.Treatment.AdjustDateStr},
                    {"TreatmentDate", p.Treatment.TreatDateStr},
                    {"MedicalNumber", p.Customer.IcCard.MedicalNumber},
                    {"ReleaseHospital", p.Treatment.MedicalInfo.Hospital.Name},
                    {"Division",p.Treatment.MedicalInfo.Hospital.Division.Name},
                    {"NextDrugDate", ""},
                    {"VisitBackDate", ""},
                    {"ChronicSequence", "第 " + p.ChronicSequence + " 次，共 " + p.ChronicTotal + " 次"},
                    {"MedicineId",m.Id},
                    { "EngName",m.EngName},
                    { "ChnName",m.ChiName},
                    { "Ingredient",m.Ingredient},
                    //{ "Form",m.},
                    { "Usage",m.Usage.PrintName},
                    { "Dosage",m.Dosage},
                    { "Total",m.Amount.ToString()},
                    { "Days",m.Days},
                    //{ "Indication",},
                    //{ "SideEffect",},
                    {"Notes","＊請依照醫師指示使用，勿自行停藥!"}
                };
            return medBagDictionary;
        }
    }
}
