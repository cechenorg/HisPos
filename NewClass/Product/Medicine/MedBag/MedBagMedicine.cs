using System;
using System.Globalization;
using System.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.Product;
using Microsoft.VisualBasic;

namespace His_Pos.NewClass.Product.Medicine.MedBag
{
    public class MedBagMedicine
    {
        public MedBagMedicine(MedicineNHI m, bool isSingle,int? medNo = null)
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
                var usagePrint = GetPositionPrintName(m.PositionName) + GetUsagePrintName(m.Usage).Trim() + "用量:" + m.Dosage + "(  )";
                Usage = usagePrint;
                Form = m.Form;
                Total = m.Amount.ToString();
                Note = m.Note;
            }
            else
            {
                MedNo = ((int)medNo).ToString();
                Name = Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = "共" + m.Days + "天";
                Dosage = (Convert.ToDouble(m.Dosage)).ToString(CultureInfo.InvariantCulture);
                Total = m.Days + "天" + m.Amount;
                if (m.ID.EndsWith("00") || m.ID.EndsWith("G0"))
                    Total += "顆";
                else
                {
                    Total += "個";
                }
                var usagePrint =  GetPositionPrintName(m.PositionName) + GetUsagePrintName(m.Usage).Trim() + "每次" + m.Dosage + "( )";
                Usage = usagePrint;
            }
        }
        public MedBagMedicine(MedicineOTC m, bool isSingle, int? medNo = null)
        {
            Id = m.ID;
            if (isSingle)
            {
                Name = Strings.StrConv(m.EnglishName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChineseName, VbStrConv.Narrow);
                Ingredient = string.Empty;
                SideEffect = string.Empty;
                Indication = string.Empty;
                if(m.Days != null)
                    MedicineDays = m.Days + "天";
                else
                {
                    MedicineDays = string.Empty;
                }
                if (!string.IsNullOrEmpty(m.PositionName) && !string.IsNullOrEmpty(m.UsageName))
                {
                    var usagePrint = GetPositionPrintName(m.PositionName) + GetUsagePrintName(m.Usage).Trim() + "用量:" + m.Dosage + "(  )"; ;
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
                MedNo = ((int)medNo).ToString();
                Name = Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + string.Empty;
                SideEffect = "副作用:" + string.Empty;
                Indication = "適應症:" + string.Empty;
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
                
                if (m.ID.EndsWith("00") || m.ID.EndsWith("G0"))
                    Total += "顆";
                else
                {
                    Total += "個";
                }
                if (!string.IsNullOrEmpty(m.PositionName) && !string.IsNullOrEmpty(m.UsageName))
                {
                    var usagePrint = GetPositionPrintName(m.PositionName) + GetUsagePrintName(m.Usage).Trim();
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
        public MedBagMedicine(MedicineSpecialMaterial m, bool isSingle, int? medNo = null)
        {
            Id = m.ID;
            if (isSingle)
            {
                Name = Strings.StrConv(m.EnglishName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChineseName, VbStrConv.Narrow);
                Ingredient = string.Empty;
                SideEffect = string.Empty;
                Indication = string.Empty;
                if(m.Days is null)
                    MedicineDays = m.Days + "天";
                else
                {
                    MedicineDays = string.Empty;
                }
                Usage = string.Empty;
                Form = string.Empty;
                Total = m.Amount.ToString();
                Note = string.Empty;
            }
            else
            {
                MedNo = ((int)medNo).ToString();
                Name = Strings.StrConv(m.FullName, VbStrConv.Narrow);
                Ingredient = "成分:" + string.Empty;
                SideEffect = "副作用:" + string.Empty;
                Indication = "適應症:" + string.Empty;
                if (m.Days is null)
                    MedicineDays = string.Empty;
                else
                {
                    MedicineDays = "共" + m.Days + "天";
                }
                Dosage = string.Empty;
                if (m.Days is null)
                    Total = m.Amount.ToString();
                else
                {
                    Total = m.Days + "天" + m.Amount;
                }
                Usage = string.Empty;
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
        private string GetPositionPrintName(string mPosition)
        {
            var positionName = ViewModelMainWindow.Positions
                .SingleOrDefault(p => p.ID.Replace(" ", "").Equals(mPosition.ToUpper().Replace(" ", "")))?.Name;
            if (string.IsNullOrEmpty(positionName))
                return string.Empty;
            return "【"+positionName.Replace("  ", string.Empty) + "】";
        }

        private string GetUsagePrintName(Usage.Usage usage)
        {
            usage = ViewModelMainWindow.GetUsage(usage.Name);
            if (usage.PrintName is null) return string.Empty;
            if (!usage.PrintName.Contains("(0)")) return usage.PrintName;
            if (ViewModelMainWindow.Usages.SingleOrDefault(u => u.Reg.IsMatch(usage.Name)) != null)
            {
                var match = ViewModelMainWindow.Usages.SingleOrDefault(u => u.Reg.IsMatch(usage.Name))?.Reg.Match(usage.Name);
                var print = string.Empty;
                var tempPrint = usage.PrintName;
                var currentIndex = 0;
                for (var i = 1; i < match.Groups.Count; i++)
                {
                    var rightParenthesisIndex = tempPrint.IndexOf(")");
                    var replace = "(" + (i - 1) + ")";
                    print += usage.PrintName.Substring(currentIndex, rightParenthesisIndex + 1).Replace(replace, match.Groups[i].Value);
                    currentIndex+= rightParenthesisIndex+1;
                    tempPrint = usage.PrintName.Substring(currentIndex, (usage.PrintName.Length- currentIndex));
                }
                return print + tempPrint;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
