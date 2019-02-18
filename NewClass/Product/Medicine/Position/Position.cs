using System.Data;
using GalaSoft.MvvmLight;
using ZeroFormatter;

namespace His_Pos.NewClass.Product.Medicine.Position
{
    [ZeroFormattable]
    public class Position : ObservableObject
    { 
        public Position()
        {
            ID = string.Empty;
            Name = string.Empty;
        }

        public Position(DataRow r)
        {
            ID = r.Field<string>("Pos_ID");
            Name = r.Field<string>("Pos_Name");
            FullName = r.Field<string>("Pos_FullName");
        }
        [Index(0)]
        public virtual string ID { get; set; }
        [Index(1)]
        public virtual string Name { get; set; }
        [Index(2)]
        public virtual string FullName { get; set; }
    }
}
