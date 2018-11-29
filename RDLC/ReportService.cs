using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
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
using His_Pos.Class.Pharmacy;
using His_Pos.Class.Product;
using His_Pos.Service;
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
                ConsumeContainerWhitespace = "true",
                ReportID = "cdd7925b-803a-4208-8788-8e2ae4bd14b8"
            };
            //SetReportItem(medBagReport, selectedMedBag.MedLocations,p,medicineIndex);
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
                CanGrow = "false",
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
                    PaddingLeft = "0pt",
                    PaddingRight = "0pt",
                    PaddingTop = "0pt",
                    PaddingBottom = "0pt"
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
        public static string PrettyXml(StringWriter writer)
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
            var bytes = viewer.LocalReport.Render("PDF", deviceInfo:"");

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
                    {"PatientBirthday", DateTimeExtensions.ConvertToTaiwanCalender(p.Customer.Birthday,true)},
                    {"MedRecNum", ""},
                    {"AdjustDate", DateTimeExtensions.ConvertToTaiwanCalender(p.Treatment.AdjustDate,true)},
                    {"TreatmentDate", DateTimeExtensions.ConvertToTaiwanCalender(p.Treatment.TreatmentDate,true)},
                    {"MedicalNumber", p.Customer.IcCard.MedicalNumber},
                    {"ReleaseHospital", p.Treatment.MedicalInfo.Hospital.Name},
                    {"Division",p.Treatment.MedicalInfo.Hospital.Division.Name},
                    {"NextDrugDate", ""},
                    {"VisitBackDate", ""},
                    {"ChronicSequence", "第 " + p.ChronicSequence + " 次，共 " + p.ChronicTotal + " 次"},
                    {"MedicineId",m.Id},
                    {"EngName",m.EngName},
                    {"ChnName",m.ChiName},
                    {"Ingredient",((DeclareMedicine)m).Ingredient},
                    //{ "Form",m.},
                    { "Usage",((DeclareMedicine)m).Usage.PrintName},
                    { "Dosage",((DeclareMedicine)m).Dosage.ToString(CultureInfo.InvariantCulture)},
                    { "Total",((DeclareMedicine)m).Amount.ToString(CultureInfo.InvariantCulture)},
                    { "Days",((DeclareMedicine)m).Days},
                    //{ "Indication",},
                    //{ "SideEffect",},
                    {"Notes","＊請依照醫師指示使用，勿自行停藥!"}
                };
            return medBagDictionary;
        }

        ///////////////////////////
        private static int m_currentPageIndex;
        private static IList<Stream> m_streams;
        
        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private static Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        private static void Export(LocalReport report,MedBag selectedMedBag)
        {
            var deviceInfo = "<DeviceInfo>" +
                             "  <OutputFormat>EMF</OutputFormat>" +
                             "  <PageWidth>" + selectedMedBag.BagWidth + "cm</PageWidth>" +
                             "  <PageHeight>" + selectedMedBag.BagHeight + "cm</PageHeight>" +
                             "  <MarginTop>0cm</MarginTop>" +
                             "  <MarginLeft>0cm</MarginLeft>" +
                             "  <MarginRight>0cm</MarginRight>" +
                             "  <MarginBottom>0cm</MarginBottom>" +
                             "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        // Handler for PrintPageEvents
        private static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private static void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }
        // Create a local report for Report.rdlc, load the data,
        //    export the report to an .emf file, and print it.
        public static void Run(MedBag selectedMedBag)
        {
            LocalReport report = new LocalReport();
            report.ReportPath = @"..\..\RDLC\MedBagReport.rdlc";
            Export(report, selectedMedBag);
            Print();
        }

        public static void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }


    }
}
