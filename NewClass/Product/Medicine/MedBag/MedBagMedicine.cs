﻿using System;
using System.Globalization;
using System.Linq;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class.Product;
using Microsoft.VisualBasic;

namespace His_Pos.NewClass.Product.Medicine.MedBag
{
    public class MedBagMedicine
    {
        public MedBagMedicine(DeclareMedicine m,bool isSingle)
        {
            Id = m.Id;
            if (isSingle)
            {
                Name = Strings.StrConv(m.EngName, VbStrConv.Narrow);
                ChiName = Strings.StrConv(m.ChiName, VbStrConv.Narrow);
                Ingredient = Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = m.Days + "天";
                var usagePrint = "【" + GetPositionPrintName(m.Position) + "】" + GetUsagePrintName(m.Usage) + "用量:" + m.Dosage + "(  )"; ;
                Usage = usagePrint;
                Form = m.MedicalCategory.Form;
                Total = m.Amount.ToString();
                Note = m.Note;
            }
            else
            {
                Name = Strings.StrConv(m.Name, VbStrConv.Narrow);
                Ingredient = "成分:" + Strings.StrConv(m.Ingredient, VbStrConv.Narrow);
                SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow);
                Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow);
                MedicineDays = "共" + m.Days + "天";
                Dosage = m.Dosage.ToString(CultureInfo.InvariantCulture);
                Total = m.Days + "天" + m.Amount;
                if (m.Id.EndsWith("00") || m.Id.EndsWith("G0"))
                    Total += "顆";
                else
                {
                    Total += "個";
                }
                var usagePrint = "【" + GetPositionPrintName(m.Position) + "】" + GetUsagePrintName(m.Usage) + "每次" + m.Dosage + "(  )";
                Usage = usagePrint;
            }
        }

        public MedBagMedicine(PrescriptionOTC o, bool isSingle)
        {
            Id = o.Id;
            Name = Strings.StrConv(o.Name, VbStrConv.Narrow);
            ChiName = string.Empty;
            Usage = "【請參閱包裝使用說明】"; 
            if (isSingle)
            {
                Ingredient = string.Empty;
                SideEffect = string.Empty;
                Indication = string.Empty;
                MedicineDays = string.Empty;
                Form = string.Empty;
            }
            else
            {
                Ingredient = "成分:";
                SideEffect = "副作用:";
                Indication = "適應症:";
                MedicineDays = "共" + o.Days + "天";
                Total = o.Days + "天" + o.Amount + "個";
            }
            Ingredient = string.Empty;
            SideEffect = string.Empty;
            Indication = string.Empty;
            Dosage = string.Empty;
        }
        public MedBagMedicine(MedicineNHI m, bool isSingle)
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
                var usagePrint = "【" + GetPositionPrintName(m.PositionName) + "】" + GetUsagePrintName(m.Usage) + "用量:" + m.Dosage + "(  )"; ;
                Usage = usagePrint;
                Form = m.Form;
                Total = m.Amount.ToString();
                Note = m.Note;
            }
            else
            {
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
                var usagePrint = "【" + GetPositionPrintName(m.PositionName) + "】" + GetUsagePrintName(m.Usage) + "每次" + m.Dosage + "(  )";
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
                if(m.Days != null)
                    MedicineDays = m.Days + "天";
                else
                {
                    MedicineDays = string.Empty;
                }
                if (!string.IsNullOrEmpty(m.PositionName) && !string.IsNullOrEmpty(m.UsageName))
                {
                    var usagePrint = "【" + GetPositionPrintName(m.PositionName) + "】" + GetUsagePrintName(m.Usage) + "用量:" + m.Dosage + "(  )"; ;
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
                    var usagePrint = "【" + GetPositionPrintName(m.PositionName) + "】" + GetUsagePrintName(m.Usage) + "每次" + m.Dosage + "(  )";
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
            return ViewModelMainWindow.Positions.SingleOrDefault(p => p.Id.Replace(" ","").Equals(mPosition.Replace(" ", "")))?.Name;
        }

        private string GetUsagePrintName(Usage.Usage usage)
        {
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