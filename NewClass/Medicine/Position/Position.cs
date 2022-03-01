using GalaSoft.MvvmLight;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Medicine.Position
{
    [ZeroFormattable]
    public class Position : ObservableObject
    {
        public Position()
        {
        }

        public Position(DataRow r)
        {
            ID = r.Field<string>("Pos_ID").TrimEnd();
            Name = r.Field<string>("Pos_Name").TrimEnd();
            FullName = r.Field<string>("Pos_FullName").TrimEnd();
        }

        [Index(0)]
        public virtual string ID { get; set; } = string.Empty;

        [Index(1)]
        public virtual string Name { get; set; } = string.Empty;

        [Index(2)]
        public virtual string FullName { get; set; } = string.Empty;
    }
}