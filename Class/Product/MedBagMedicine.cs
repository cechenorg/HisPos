using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace His_Pos.Class.Product
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
                string usagePrint = "【" + GetPositionPrintName(m.Position) + "】" + m.Usage.PrintName + "用量:" + m.Dosage + "(  )"; ;
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
                string usagePrint = "【" + GetPositionPrintName(m.Position) + "】" + m.Usage.PrintName + "每次" + m.Dosage + "(  )";
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
            return MainWindow.Positions.SingleOrDefault(p => p.Id.Replace(" ","").Equals(mPosition.Replace(" ", "")))?.Name;
        }
    }
}
