using GalaSoft.MvvmLight;
using System.Data;
using ZeroFormatter;

namespace His_Pos.NewClass.Prescription.Treatment.Division
{
    [ZeroFormattable]
    public class Division : ObservableObject
    {
        public Division()
        {
        }

        public Division(DataRow r)
        {
            ID = r.Field<string>("Div_ID");
            Name = r.Field<string>("Div_Name");
            FullName = r.Field<string>("Div_FullName");
        }

        private string id;

        [Index(0)]
        public virtual string ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }

        private string name;

        [Index(1)]
        public virtual string Name
        {
            get => name;
            set
            {
                Set(() => Name, ref name, value);
            }
        }

        private string fullName;

        [Index(2)]
        public virtual string FullName
        {
            get => fullName;
            set
            {
                Set(() => FullName, ref fullName, value);
            }
        }

        public bool CheckIDNotEmpty()
        {
            return !string.IsNullOrEmpty(ID);
        }

        public bool IsDentistry()
        {
            return ID.Equals("40");
        }

        public object Clone()
        {
            var copy = new Division();
            copy.ID = this.ID;
            copy.Name = (string)this.Name.Clone();
            copy.FullName = this.ID;
            return copy;
        }
    }
}