using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class.Product
{
    public class MedBagMedicine
    {
        public MedBagMedicine(DeclareMedicine m)
        {
            Id = m.Id;
            Name = m.Name;
            Ingredient = m.Ingredient;
            SideEffect = m.SideEffect;
            Indication = m.Indication;
            Position = m.Position;
            MedicineDays = m.Days;
            Dosage = m.Dosage;
            Usage = m.Usage.Name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ingredient { get; set; }
        public string SideEffect { get; set; }
        public string Indication { get; set; }
        public string Position { get; set; }
        public string MedicineDays { get; set; }
        public string Total { get; set; }
        public string Dosage { get; set; }
        public string Usage { get; set; }
    }
}
