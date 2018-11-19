using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace His_Pos.Class.Product
{
    public class MedBagMedicine
    {
        public MedBagMedicine(DeclareMedicine m)
        {
            Id = m.Id;
            Name = Strings.StrConv(m.Name, VbStrConv.Narrow, 0);
            Ingredient = "成分:"+Strings.StrConv(m.Ingredient, VbStrConv.Narrow, 0);
            SideEffect = "副作用:" + Strings.StrConv(m.SideEffect, VbStrConv.Narrow, 0);
            Indication = "適應症:" + Strings.StrConv(m.Indication, VbStrConv.Narrow, 0);
            MedicineDays = "共"+m.Days+"天";
            Dosage = m.Dosage;
            Total = m.Days + "天" + m.Amount;
            if (m.Id.EndsWith("00") || m.Id.EndsWith("G0"))
                Total += "顆";
            string usagePrint = "【" + GetPositionPrintName(m.Position) + "】" + m.Usage.PrintName;
            Usage = usagePrint;
        }

        public MedBagMedicine(PrescriptionOTC o)
        {
            Id = o.Id;
            Name = o.Name;
            Ingredient = string.Empty;
            SideEffect = string.Empty;
            Indication = string.Empty;
            MedicineDays = o.Days;
            Dosage = o.Dosage;
            string usagePrint = "【" + GetPositionPrintName(o.Position) + "】" + o.Usage.PrintName;
            Usage = usagePrint;
        }
        public string MedNo { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ingredient { get; set; }
        public string SideEffect { get; set; }
        public string Indication { get; set; }
        public string MedicineDays { get; set; }
        public string Total { get; set; }
        public string Dosage { get; set; }
        public string Usage { get; set; }
        private string GetPositionPrintName(string mPosition)
        {
            return MainWindow.Positions.SingleOrDefault(p => p.Id.Replace(" ","").Equals(mPosition.Replace(" ", "")))?.Name;
        }
    }
}
