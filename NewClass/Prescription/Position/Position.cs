using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace His_Pos.NewClass.Prescription.Position
{
    public class Position : ObservableObject
    { 
        public Position()
        {
            Id = string.Empty;
            Name = string.Empty;
        }

        public Position(DataRow row)
        {
            Id = row["Pos_ID"].ToString();
            Name = row["Pos_Name"].ToString();
            FullName = row["Pos_FullName"].ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
