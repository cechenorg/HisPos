using His_Pos.Class;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Prescription.Declare.DeclarePreview;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace His_Pos.NewClass.Prescription.Declare.DeclareFile
{
    public enum PDataType
    {
        Service = 0,//藥事服務費
        SimpleForm = 1,//日劑藥費
    }

    [XmlRoot(ElementName = "tdata")]
    public class Tdata
    {
        public Tdata()
        {
        }

        public Tdata(DeclarePreviewOfMonth selectedFile, string pharmacyID, DateTime startDate, DateTime endDate)
        {
            T1 = "30";
            T2 = pharmacyID;
            T3 = (selectedFile.DeclareDate.Year - 1911).ToString().PadLeft(3, '0') +
                 selectedFile.DeclareDate.Month.ToString().PadLeft(2, '0');
            T4 = "2";
            T5 = "1";
            T6 = DateTimeExtensions.ConvertToTaiwanCalender(DateTime.Today);
            var normalPres = selectedFile.DeclarePres.Where(p => p.IsDeclare &&
                                                                          (p.AdjustCase.ID.Equals("1") || p.AdjustCase.ID.Equals("3") || p.AdjustCase.ID.Equals("4")
                                                                           || p.AdjustCase.ID.Equals("5") || p.AdjustCase.ID.Equals("D"))).ToList();
            var chronicPres = selectedFile.DeclarePres.Where(p => p.IsDeclare && p.AdjustCase.ID.Equals("2")).ToList();
            var normalCount = normalPres.Count;
            var chronicCount = chronicPres.Count;
            var normalApplyPoints = normalPres.Sum(p => int.Parse(p.FileContent.Dhead.D16));
            var chronicApplyPoints = chronicPres.Sum(p => int.Parse(p.FileContent.Dhead.D16));
            T7 = normalCount.ToString().PadLeft(6, '0');
            T8 = normalApplyPoints.ToString().PadLeft(10, '0');
            T9 = chronicCount.ToString().PadLeft(6, '0');
            T10 = chronicApplyPoints.ToString().PadLeft(10, '0');
            T11 = (normalCount + chronicCount).ToString().PadLeft(8, '0');
            T12 = (normalApplyPoints + chronicApplyPoints).ToString().PadLeft(10, '0');
            T13 = DateTimeExtensions.ConvertToTaiwanCalender(startDate);
            T14 = DateTimeExtensions.ConvertToTaiwanCalender(endDate);
        }

        [XmlElement(ElementName = "t1")]
        public string T1 { get; set; }

        [XmlElement(ElementName = "t2")]
        public string T2 { get; set; }

        [XmlElement(ElementName = "t3")]
        public string T3 { get; set; }

        [XmlElement(ElementName = "t4")]
        public string T4 { get; set; }

        [XmlElement(ElementName = "t5")]
        public string T5 { get; set; }

        [XmlElement(ElementName = "t6")]
        public string T6 { get; set; }

        [XmlElement(ElementName = "t7")]
        public string T7 { get; set; }

        [XmlElement(ElementName = "t8")]
        public string T8 { get; set; }

        [XmlElement(ElementName = "t9")]
        public string T9 { get; set; }

        [XmlElement(ElementName = "t10")]
        public string T10 { get; set; }

        [XmlElement(ElementName = "t11")]
        public string T11 { get; set; }

        [XmlElement(ElementName = "t12")]
        public string T12 { get; set; }

        [XmlElement(ElementName = "t13")]
        public string T13 { get; set; }

        [XmlElement(ElementName = "t14")]
        public string T14 { get; set; }
    }

    [XmlRoot(ElementName = "pharmacy")]
    public class DeclareFile
    {
        public DeclareFile()
        {
        }

        public DeclareFile(DeclarePreviewOfMonth selectedFile, string pharmacyID, DateTime startDate, DateTime endDate)
        {
            Tdata = new Tdata(selectedFile, pharmacyID, startDate, endDate);
            var tempList = new List<Ddata>();
            Ddata = new List<Ddata>();
            var dd = new List<Ddata>();
            foreach (var p in selectedFile.DeclarePres.Where(p => p.IsDeclare))
            {
                foreach (var pdata in p.FileContent.Dbody.Pdata)
                {
                    if (pdata.P1.Equals("3"))
                        if (pdata.P2.Length > 12) 
                        {
                            pdata.P2 = pdata.P2.Substring(0, 12);
                        }
                }
                tempList.Add(p.FileContent);
            }

            foreach (var g in tempList.OrderBy(d => int.Parse(d.Dhead.D1)).GroupBy(d => d.Dhead.D1).Select(group => group.ToList()).ToList())
            {
                dd.AddRange(g.OrderBy(d => d.Dhead.D2));
            }

            for (var i = 1; i <= 4; i++)
            {
                foreach (var d in dd.Where(d => d.Dhead.D1.Equals(i.ToString())))
                {
                    Ddata.Add(d);
                }
            }
        }

        [XmlElement(ElementName = "tdata")]
        public Tdata Tdata { get; set; }

        [XmlElement(ElementName = "ddata")]
        public List<Ddata> Ddata { get; set; }
    }

    [XmlRoot(ElementName = "ddata")]
    public class Ddata
    {
        public Ddata()
        {
        }

        public Ddata(Prescription p, List<Pdata> details)
        {
            Dhead = new Dhead(p);
            Dbody = new Dbody(p, details);
            var totalPoint = int.Parse(Dbody.D31) + int.Parse(Dbody.D32) + int.Parse(Dbody.D33);
            if (Dbody.D38 != null)
                totalPoint += int.Parse(Dbody.D38);
            Dhead.D18 = $"{totalPoint:00000000}";
            Dhead.D16 = $"{int.Parse(Dhead.D18) - int.Parse(Dhead.D17):00000000}";
        }

        [XmlElement(ElementName = "dhead")]
        public Dhead Dhead { get; set; }

        [XmlElement(ElementName = "dbody")]
        public Dbody Dbody { get; set; }
    }

    [XmlRoot(ElementName = "dhead")]
    public class Dhead
    {
        public Dhead()
        {
        }

        public Dhead(Prescription p)
        {
            var point = p.PrescriptionPoint;
            D1 = p.AdjustCase.ID;
            D2 = string.Empty;
            D3 = p.Patient.IDNumber;
            D4 = string.Empty;
            if (!D1.Equals("2") && !D1.Equals("D"))
                D5 = p.PaymentCategory?.ID;
            D6 = DateTimeExtensions.NullableDateToTWCalender(p.Patient.Birthday, false);
            D7 = p.MedicalNumber;
            D8 = p.MainDisease.ID;
            D9 = p.SubDisease?.ID;
            D13 = p.Division?.ID;
            D14 = p.TreatDate is null ? string.Empty : DateTimeExtensions.ConvertToTaiwanCalender((DateTime)p.TreatDate);
            D15 = p.Copayment.Id;
            D17 = $"{point.CopaymentPoint:0000}";
            D20 = p.Patient.Name;
            D21 = p.Institution.ID;
            D22 = p.PrescriptionCase?.ID;
            D23 = DateTimeExtensions.NullableDateToTWCalender(p.AdjustDate, false);
            if (!p.AdjustCase.CheckIsQuitSmoking() && !p.AdjustCase.CheckIsHomeCare())
                D24 = D21;
            else
                D24 = string.Empty;
            D25 = p.Pharmacist.IDNumber;
        }

        [XmlElement(ElementName = "d1")]
        public string D1 { get; set; }

        [XmlElement(ElementName = "d2")]
        public string D2 { get; set; }

        [XmlElement(ElementName = "d3")]
        public string D3 { get; set; }

        [XmlElement(ElementName = "d4")]
        public string D4 { get; set; }

        [XmlElement(ElementName = "d5")]
        public string D5 { get; set; }

        [XmlElement(ElementName = "d6")]
        public string D6 { get; set; }

        [XmlElement(ElementName = "d7")]
        public string D7 { get; set; }

        [XmlElement(ElementName = "d8")]
        public string D8 { get; set; }

        [XmlElement(ElementName = "d9")]
        public string D9 { get; set; }

        [XmlElement(ElementName = "d13")]
        public string D13 { get; set; }

        [XmlElement(ElementName = "d14")]
        public string D14 { get; set; }

        [XmlElement(ElementName = "d15")]
        public string D15 { get; set; }

        [XmlElement(ElementName = "d16")]
        public string D16 { get; set; }

        [XmlElement(ElementName = "d17")]
        public string D17 { get; set; }

        [XmlElement(ElementName = "d18")]
        public string D18 { get; set; }

        [XmlElement(ElementName = "d20")]
        public string D20 { get; set; }

        [XmlElement(ElementName = "d21")]
        public string D21 { get; set; }

        [XmlElement(ElementName = "d22")]
        public string D22 { get; set; }

        [XmlElement(ElementName = "d23")]
        public string D23 { get; set; }

        [XmlElement(ElementName = "d24")]
        public string D24 { get; set; }

        [XmlElement(ElementName = "d25")]
        public string D25 { get; set; }
    }

    [XmlRoot(ElementName = "dbody")]
    public class Dbody
    {
        public Dbody()
        {
        }

        public Dbody(Prescription p, List<Pdata> details)
        {
            var point = p.PrescriptionPoint;
            D26 = p.SpecialTreat?.ID;
            D30 = p.AdjustCase.ID.Equals("D") ? "00" : p.MedicineDays.ToString().PadLeft(2, '0');
            D31 = $"{details.Where(d => d.P1.Equals("3")).Sum(d => int.Parse(d.P9)):0000000}";
            D32 = "00000000";
            D33 = details.Where(d => d.P1.Equals("1")).Sum(d => int.Parse(d.P9)).ToString().PadLeft(8, '0');
            D35 = p.ChronicSeq is null ? string.Empty : p.ChronicSeq.ToString();
            D36 = p.ChronicTotal is null ? string.Empty : p.ChronicTotal.ToString();
            var medicalService = details.SingleOrDefault(pd => pd.P1.Equals("9"));
            if (medicalService != null)
            {
                D37 = p.MedicalServiceCode;
                D38 = medicalService.P9.PadLeft(8, '0');
            }
            D43 = p.OriginalMedicalNumber;
            if (p.Copayment != null && p.Copayment.Id.Equals("903"))
                D44 = p.Card.NewBornBirthday;
            Pdata = new List<Pdata>();
            foreach (var d in details)
            {
                var pdata = new Pdata();
                pdata = d.DeepCloneViaJson();
                if (pdata.P1.Equals("3") && pdata.P2.Length > 12)
                    pdata.P2 = pdata.P2.Substring(0, 12);
                Pdata.Add(pdata);
            }
        }

        [XmlElement(ElementName = "d26")]
        public string D26 { get; set; }

        [XmlElement(ElementName = "d30")]
        public string D30 { get; set; }

        [XmlElement(ElementName = "d31")]
        public string D31 { get; set; }

        [XmlElement(ElementName = "d32")]
        public string D32 { get; set; }

        [XmlElement(ElementName = "d33")]
        public string D33 { get; set; }

        [XmlElement(ElementName = "d35")]
        public string D35 { get; set; }

        [XmlElement(ElementName = "d36")]
        public string D36 { get; set; }

        [XmlElement(ElementName = "d37")]
        public string D37 { get; set; }

        [XmlElement(ElementName = "d38")]
        public string D38 { get; set; }

        [XmlElement(ElementName = "d43")]
        public string D43 { get; set; }

        [XmlElement(ElementName = "d44")]
        public string D44 { get; set; }

        [XmlElement(ElementName = "pdata")]
        public List<Pdata> Pdata { get; set; }
    }

    [XmlRoot(ElementName = "pdata")]
    public class Pdata
    {
        public Pdata()
        {
        }

        public Pdata(Medicine.Base.Medicine m, string serial, DateTime adjustDate)
        {
            if (m is MedicineNHI && !m.PaySelf)
            {
                P1 = "1";
                P2 = m.ID;
                P7 = $"{m.Amount:00000.0}";
                P8 = $"{m.NHIPrice:0000000.00}";
                P9 =
                    $"{Math.Round(Convert.ToDouble((m.NHIPrice * m.Amount).ToString()), 0, MidpointRounding.AwayFromZero):0000000}";
                P3 = $"{m.Dosage:0000.00}";
                P4 = m.UsageName;
                P5 = m.PositionID;
                P10 = serial;
                P11 = $"{m.Days:00}";
                P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate);
                P13 = P12;
                PaySelf = false;
                IsBuckle = m.IsBuckle;
                Order = m.Order;
                SendAmount = m.SendAmount;
                AdjustNoBuckle = m.AdjustNoBuckle;
                IsClosed = m.IsClosed;
            }
            else if (m is MedicineSpecialMaterial && !m.PaySelf)
            {
                P1 = "3";
                P2 = m.ID;
                P3 = $"{m.Dosage:0000.00}";
                P4 = m.UsageName;
                P5 = m.PositionID;
                P7 = $"{m.Amount:00000.0}";
                P8 = $"{m.NHIPrice:0000000.00}";
                P9 =
                    $"{Math.Round(Convert.ToDouble((m.NHIPrice * m.Amount * 1.05).ToString()), 0, MidpointRounding.AwayFromZero):0000000}";
                P6 = "105";
                P10 = serial;
                P11 = $"{m.Days:00}";
                P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate);
                P13 = P12;
                PaySelf = false;
                IsBuckle = m.IsBuckle;
                Order = m.Order;
                SendAmount = m.SendAmount;
                AdjustNoBuckle = m.AdjustNoBuckle;
                IsClosed = m.IsClosed;
            }
            else if (m is MedicineVirtual)
            {
                P1 = "G";
                P2 = m.ID;
                P7 = $"{0.00:00000.0}";
                P8 = $"{0.00:0000000.00}";
                P9 = $"{0.00:0000000}";
                P10 = serial;
                P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate);
                P13 = P12;
                PaySelf = false;
                IsBuckle = false;
                BuckleAmount = 0;
                Order = m.Order;
                SendAmount = m.SendAmount;
                AdjustNoBuckle = true;
                IsClosed = true;
            }
            else
            {
                P1 = "0";
                P2 = m.ID;
                P7 = m.Amount.ToString();
                var dosage = m.Dosage is null ? string.Empty : m.Dosage.ToString();
                P3 = dosage;
                P4 = m.UsageName;
                P5 = m.PositionID;
                P8 = string.Empty;
                P9 =
                    $"{Math.Round(Convert.ToDouble(m.TotalPrice.ToString()), 0, MidpointRounding.AwayFromZero):0000000}";
                P10 = string.Empty;
                var days = m.Days is null ? string.Empty : $"{m.Days:00}";
                P11 = days;
                P12 = string.Empty;
                P13 = P12;
                PaySelf = m.PaySelf;
                IsBuckle = m.IsBuckle;
                Order = m.Order;
                SendAmount = m.SendAmount;
                AdjustNoBuckle = m.AdjustNoBuckle;
                IsClosed = m.IsClosed;
            }
            PaySelfValue = m.Price;
            BuckleAmount = m.BuckleAmount;
        }

        public Pdata(PDataType type, string code, int percentage, int amount, DateTime adjustDate)
        {
            switch (type)
            {
                case PDataType.Service:
                    P1 = "9";
                    P2 = code;
                    P3 = string.Empty;
                    P4 = string.Empty;
                    P5 = string.Empty;
                    P6 = percentage.ToString();
                    P7 = $"{amount:00000.0}";
                    switch (code)
                    {
                        case "05202B":
                        case "05223B":
                            P8 = $"{(int)ServicePoint.CODE_05223B:0000000.00}";
                            P9 = $"{(int)ServicePoint.CODE_05223B * percentage * 0.01:00000000}";
                            break;

                        case "05210B":
                            P8 = $"{(int)ServicePoint.CODE_05210B:0000000.00}";
                            P9 = $"{(int)ServicePoint.CODE_05210B * percentage * 0.01:00000000}";
                            break;

                        case "05206B":
                            P8 = $"{(int)ServicePoint.CODE_05206B:0000000.00}";
                            P9 = $"{(int)ServicePoint.CODE_05206B * percentage * 0.01:00000000}";
                            break;
                    }

                    P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate);
                    P13 = P12;
                    PaySelf = false;
                    IsBuckle = true;
                    AdjustNoBuckle = true;
                    IsClosed = true;
                    break;

                case PDataType.SimpleForm:
                    P1 = "1";
                    switch (code)
                    {
                        case "22":
                            P2 = "MA1";
                            break;

                        case "31":
                            P2 = "MA2";
                            break;

                        case "37":
                            P2 = "MA3";
                            break;

                        case "41":
                            P2 = "MA4";
                            break;
                    }

                    P3 = $"{1.0:0000.00}";
                    P4 = string.Empty;
                    P5 = string.Empty;
                    P6 = percentage.ToString();
                    P7 = $"{amount:00000.0}";
                    P8 = $"{int.Parse(code):00000.0}";
                    P9 = $"{int.Parse(code) * amount:00000000}";
                    P11 = amount.ToString().PadLeft(2, '0');
                    P12 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate);
                    P13 = DateTimeExtensions.ConvertToTaiwanCalenderWithTimeZero(adjustDate.AddDays(amount - 1));
                    PaySelf = false;
                    IsBuckle = true;
                    AdjustNoBuckle = true;
                    IsClosed = true;
                    break;
            }
        }

        [XmlElement(ElementName = "p1")] public string P1 { get; set; }
        [XmlElement(ElementName = "p2")] public string P2 { get; set; }
        [XmlElement(ElementName = "p3")] public string P3 { get; set; }
        [XmlElement(ElementName = "p4")] public string P4 { get; set; }
        [XmlElement(ElementName = "p5")] public string P5 { get; set; }
        [XmlElement(ElementName = "p6")] public string P6 { get; set; }
        [XmlElement(ElementName = "p7")] public string P7 { get; set; }
        [XmlElement(ElementName = "p8")] public string P8 { get; set; }
        [XmlElement(ElementName = "p9")] public string P9 { get; set; }
        private string _p10;
        [XmlElement(ElementName = "p10")] public string P10 { get; set; }
        [XmlElement(ElementName = "p11")] public string P11 { get; set; }
        [XmlElement(ElementName = "p12")] public string P12 { get; set; }
        [XmlElement(ElementName = "p13")] public string P13 { get; set; }
        [XmlElement(ElementName = "p15")] public string P15 { get; set; }
        [XmlIgnore] public bool PaySelf { get; set; }
        [XmlIgnore] public bool IsBuckle { get; set; }
        [XmlIgnore] public double? BuckleAmount { get; set; }
        [XmlIgnore] public double? PaySelfValue { get; set; }
        [XmlIgnore] public int Order { get; set; }
        [XmlIgnore] public double SendAmount { get; set; }
        [XmlIgnore] public bool AdjustNoBuckle { get; set; }
        [XmlIgnore] public bool IsClosed { get; set; }
    }
}