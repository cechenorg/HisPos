using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product.Medicine.Position
{
    public class Position : ObservableObject
    { 
        public Position()
        {
            Id = string.Empty;
            Name = string.Empty;
        }

        public Position(DataRow r)
        {
            Id = r.Field<string>("Pos_ID");
            Name = r.Field<string>("Pos_Name");
            FullName = r.Field<string>("Pos_FullName");
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
