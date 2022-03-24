using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Medicine.Base;
using Microsoft.VisualBasic;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace His_Pos.NewClass.Medicine.MedBag
{
    public class MedBagMedicine
    {
        public MedBagMedicine(MedicineNHI m, bool isSingle)
        {
            Id = m.ID;
            if (isSingle)
            {
                if (m.ControlLevel > 0)
                    Id += "[管]";
                Name = Strings.StrConv(m.EnglishName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChineseName, VbStrConv.Narrow);
                Ingredient = Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = m.Days + "天";
                Total = m.Amount.ToString();
                var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim() + "每次" + m.Dosage;
                if (m.ID.EndsWith("100") || m.ID.EndsWith("1G0"))
                {
                    Total += "粒";
                    usagePrint += "粒";
                }
                else
                {
                    usagePrint += "(  )";
                }
                Usage = usagePrint;
                Form = m.Form;
                Note = m.Note;
            }
            else
            {
                if (m.ControlLevel > 0)
                    Name += "[管]";
                Name += Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = "共" + m.Days + "天";
                Dosage = (Convert.ToDouble(m.Dosage)).ToString(CultureInfo.InvariantCulture);
                Total = m.Days + "天" + m.Amount;
                var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim() + "每次" + m.Dosage;
                if (m.ID.EndsWith("100") || m.ID.EndsWith("1G0"))
                {
                    Total += "粒";
                    usagePrint += "粒";
                }
                else
                {
                    usagePrint += "(  )";
                }
                Usage = usagePrint;
            }
        }

        public MedBagMedicine(MedicineOTC m, bool isSingle)
        {
            Id = m.ID;
            if (isSingle)
            {
                Name = Strings.StrConv(m.EnglishName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChineseName, VbStrConv.Narrow);
                Ingredient = string.Empty;
                SideEffect = string.Empty;
                Indication = string.Empty;
                if (m.Days != null)
                    MedicineDays = m.Days + "天";
                else
                {
                    MedicineDays = string.Empty;
                }
                if (!string.IsNullOrEmpty(m.PositionID) && !string.IsNullOrEmpty(m.UsageName))
                {
                    var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim() + "用量:" + m.Dosage + "(  )"; ;
                    Usage = usagePrint;
                }
                else
                {
                    Usage = string.Empty;
                }
                Form = string.Empty;
                Total = m.Amount.ToString();
                Note = string.Empty;
            }
            else
            {
                Name = Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow);
                if (m.Days != null)
                    MedicineDays = "共" + m.Days + "天";
                else
                {
                    MedicineDays = string.Empty;
                }
                Dosage = m.Dosage != null ? (Convert.ToDouble(m.Dosage)).ToString(CultureInfo.InvariantCulture) : string.Empty;
                if (m.Days != null)
                    Total = m.Days + "天" + m.Amount;
                else
                {
                    Total = m.Amount.ToString();
                }
                if (m.ID.EndsWith("100") || m.ID.EndsWith("1G0"))
                    Total += "粒";
                if (!string.IsNullOrEmpty(m.PositionID) && !string.IsNullOrEmpty(m.UsageName))
                {
                    var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim();
                    Usage = usagePrint;
                }
                else
                {
                    Usage = string.Empty;
                }
                Form = string.Empty;
                Total = m.Amount.ToString();
                Note = string.Empty;
            }
        }

        public MedBagMedicine(MedicineSpecialMaterial m, bool isSingle)
        {
            Id = m.ID;
            if (isSingle)
            {
                Name = Strings.StrConv(m.EnglishName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChineseName, VbStrConv.Narrow);
                Ingredient = Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = m.Days + "天";
                Usage = string.Empty;
                Form = string.Empty;
                Total = m.Amount.ToString();
                var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim() + "每次" + m.Dosage;
                if (m.ID.EndsWith("100") || m.ID.EndsWith("1G0"))
                {
                    Total += "粒";
                    usagePrint += "粒";
                }
                else
                {
                    usagePrint += "(  )";
                }
                Usage = usagePrint;
                Note = string.Empty;
            }
            else
            {
                Name = Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = "共" + m.Days + "天";
                Dosage = string.Empty;
                if (m.Days is null)
                    Total = m.Amount.ToString();
                else
                {
                    Total = m.Days + "天" + m.Amount;
                }
                Usage = string.Empty;
                var usagePrint = GetPositionPrintName(m.PositionID) + GetUsagePrintName(m.Usage).Trim() + "每次" + m.Dosage;
                if (m.ID.EndsWith("100") || m.ID.EndsWith("1G0"))
                {
                    Total += "粒";
                    usagePrint += "粒";
                }
                else
                {
                    usagePrint += "(  )";
                }
            }
        }

        public string MedNo { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string ChiName { get; set; }
        public string Ingredient { get; set; }
        public string SideEffect { get; set; }
        public string Indication { get; set; }
        public string MedicineDays { get; set; }
        public string Total { get; set; }
        public string Dosage { get; set; }
        public string Usage { get; set; }
        public string Form { get; set; }
        public string Note { get; set; }
        public int Order { get; set; }

        private string GetPositionPrintName(string mPosition)
        {
            var positionName = ViewModelMainWindow.GetPosition(mPosition).Name;
            if (string.IsNullOrEmpty(positionName))
                return string.Empty;
            return "【" + positionName.Replace("  ", string.Empty) + "】";
        }

        private string GetUsagePrintName(Usage.Usage usage)
        {
            usage = ViewModelMainWindow.GetUsage(usage.Name);
            if (usage.PrintName is null || string.IsNullOrEmpty(usage.PrintName)) return string.Empty;
            if (!usage.PrintName.Contains("(0)")) return usage.PrintName;
            var match = Regex.Matches(usage.Name, @"\d+");
            var replaceMatch = Regex.Matches(usage.PrintName, @"\(\d+\)");
            var print = string.Empty;
            var tempPrint = usage.PrintName;
            var index = 0;
            foreach (Match m in replaceMatch)
            {
                print = tempPrint.Replace(m.Value, match[index].Value);
                tempPrint = print;
                index++;
            }
            return print;
        }
    }
}