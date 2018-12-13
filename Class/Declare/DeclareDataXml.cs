using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace His_Pos.Class.Declare
{
    [XmlRoot(ElementName = "ddata")]
    public class Ddata :INotifyPropertyChanged
    {
        [XmlElement(ElementName = "decId")]
        public string DecId { get; set; }
        private Dhead _dhead;
        [XmlElement(ElementName = "dhead")]
        public Dhead Dhead
        {
            get => _dhead;
            set
            {
                _dhead = value;
                OnPropertyChanged(nameof(Dhead));
            }
        }

        private Dbody _dbody { get; set; }

        [XmlElement(ElementName = "dbody")]
        public Dbody Dbody { get => _dbody;
            set
            {
                _dbody = value;
                OnPropertyChanged(nameof(Dbody));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [XmlRoot(ElementName = "dhead")]
    public class Dhead : INotifyPropertyChanged
    {
        private string _d1;
        [XmlElement(ElementName = "d1")]
        public string D1 { get=>_d1;
            set
            {
                _d1 = value;
                OnPropertyChanged(nameof(D1));
            }
        }

        private string _d2;
        [XmlElement(ElementName = "d2")]
        public string D2
        {
            get => _d2;
            set
            {
                _d2 = value;
                OnPropertyChanged(nameof(D2));
            }
        }

        private string _d3;
        [XmlElement(ElementName = "d3")]
        public string D3
        {
            get => _d3;
            set
            {
                _d3 = value;
                OnPropertyChanged(nameof(D3));
            }
        }
        private string _d4;
        [XmlElement(ElementName = "d4")]
        public string D4
        {
            get => _d4;
            set
            {
                _d4 = value;
                OnPropertyChanged(nameof(D4));
            }
        }
        private string _d5;
        [XmlElement(ElementName = "d5")]
        public string D5
        {
            get => _d5;
            set
            {
                _d5 = value;
                OnPropertyChanged(nameof(D5));
            }
        }
        private string _d6;
        [XmlElement(ElementName = "d6")]
        public string D6
        {
            get => _d6;
            set
            {
                _d6 = value;
                OnPropertyChanged(nameof(D6));
            }
        }
        private string _d7;
        [XmlElement(ElementName = "d7")]
        public string D7
        {
            get => _d7;
            set
            {
                _d7 = value;
                OnPropertyChanged(nameof(D7));
            }
        }
        private string _d8;
        [XmlElement(ElementName = "d8")]
        public string D8
        {
            get => _d8;
            set
            {
                _d8 = value;
                OnPropertyChanged(nameof(D8));
            }
        }
        private string _d9;
        [XmlElement(ElementName = "d9")]
        public string D9
        {
            get => _d9;
            set
            {
                _d9 = value;
                OnPropertyChanged(nameof(D9));
            }
        }
        private string _d13;
        [XmlElement(ElementName = "d13")]
        public string D13
        {
            get => _d13;
            set
            {
                _d13 = value;
                OnPropertyChanged(nameof(D13));
            }
        }
        private string _d14;
        [XmlElement(ElementName = "d14")]
        public string D14
        {
            get => _d14;
            set
            {
                _d14 = value;
                OnPropertyChanged(nameof(D14));
            }
        }
        private string _d15;
        [XmlElement(ElementName = "d15")]
        public string D15
        {
            get => _d15;
            set
            {
                _d15 = value;
                OnPropertyChanged(nameof(D15));
            }
        }
        private string _d16;
        [XmlElement(ElementName = "d16")]
        public string D16
        {
            get => _d16;
            set
            {
                _d16 = value;
                OnPropertyChanged(nameof(D16));
            }
        }
        private string _d17;
        [XmlElement(ElementName = "d17")]
        public string D17
        {
            get => _d17;
            set
            {
                _d17 = value;
                OnPropertyChanged(nameof(D17));
            }
        }
        private string _d18;
        [XmlElement(ElementName = "d18")]
        public string D18
        {
            get => _d18;
            set
            {
                _d18 = value;
                OnPropertyChanged(nameof(D18));
            }
        }
        private string _d20;
        [XmlElement(ElementName = "d20")]
        public string D20
        {
            get => _d20;
            set
            {
                _d20 = value;
                OnPropertyChanged(nameof(D20));
            }
        }
        private string _d21;
        [XmlElement(ElementName = "d21")]
        public string D21
        {
            get => _d21;
            set
            {
                _d21 = value;
                OnPropertyChanged(nameof(D21));
            }
        }
        private string _d22;
        [XmlElement(ElementName = "d22")]
        public string D22
        {
            get => _d22;
            set
            {
                _d22 = value;
                OnPropertyChanged(nameof(D22));
            }
        }
        private string _d23;
        [XmlElement(ElementName = "d23")]
        public string D23
        {
            get => _d23;
            set
            {
                _d23 = value;
                OnPropertyChanged(nameof(D23));
            }
        }
        private string _d24;
        [XmlElement(ElementName = "d24")]
        public string D24
        {
            get => _d24;
            set
            {
                _d24 = value;
                OnPropertyChanged(nameof(D24));
            }
        }
        private string _d25;
        [XmlElement(ElementName = "d25")]
        public string D25
        {
            get => _d25;
            set
            {
                _d25 = value;
                OnPropertyChanged(nameof(D25));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [XmlRoot(ElementName = "dbody")]
    public class Dbody:INotifyPropertyChanged
    {
        private string _d26;
        [XmlElement(ElementName = "d26")]
        public string D26
        {
            get => _d26;
            set
            {
                _d26 = value;
                OnPropertyChanged(nameof(D26));
            }
        }
        private string _d30;
        [XmlElement(ElementName = "d30")]
        public string D30
        {
            get => _d30;
            set
            {
                _d30 = value;
                OnPropertyChanged(nameof(D30));
            }
        }
        private string _d31;
        [XmlElement(ElementName = "d31")]
        public string D31
        {
            get => _d31;
            set
            {
                _d31 = value;
                OnPropertyChanged(nameof(D31));
            }
        }
        private string _d32;
        [XmlElement(ElementName = "d32")]
        public string D32
        {
            get => _d32;
            set
            {
                _d32 = value;
                OnPropertyChanged(nameof(D32));
            }
        }
        private string _d33;
        [XmlElement(ElementName = "d33")]
        public string D33
        {
            get => _d33;
            set
            {
                _d33 = value;
                OnPropertyChanged(nameof(D33));
            }
        }
        private string _d35;
        [XmlElement(ElementName = "d35")]
        public string D35
        {
            get => _d35;
            set
            {
                _d35 = value;
                OnPropertyChanged(nameof(D35));
            }
        }
        private string _d36;
        [XmlElement(ElementName = "d36")]
        public string D36
        {
            get => _d36;
            set
            {
                _d36 = value;
                OnPropertyChanged(nameof(D36));
            }
        }
        private string _d37;
        [XmlElement(ElementName = "d37")]
        public string D37
        {
            get => _d37;
            set
            {
                _d37 = value;
                OnPropertyChanged(nameof(D37));
            }
        }
        private string _d38;
        [XmlElement(ElementName = "d38")]
        public string D38
        {
            get => _d38;
            set
            {
                _d38 = value;
                OnPropertyChanged(nameof(D38));
            }
        }
        private string _d43;
        [XmlElement(ElementName = "d43")]
        public string D43
        {
            get => _d43;
            set
            {
                _d43 = value;
                OnPropertyChanged(nameof(D43));
            }
        }
        private string _d44;
        [XmlElement(ElementName = "d44")]
        public string D44
        {
            get => _d44;
            set
            {
                _d44 = value;
                OnPropertyChanged(nameof(D44));
            }
        }
        [XmlElement(ElementName = "pdata")]
        public List<Pdata> Pdata { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [XmlRoot(ElementName = "pdata")]
    public class Pdata:INotifyPropertyChanged
    {
        private string _p1;
        [XmlElement(ElementName = "p1")]
        public string P1
        {
            get => _p1;
            set
            {
                _p1 = value;
                OnPropertyChanged(nameof(P1));
            }
        }
        private string _p2;
        [XmlElement(ElementName = "p2")]
        public string P2
        {
            get => _p2;
            set
            {
                _p2 = value;
                OnPropertyChanged(nameof(P2));
            }
        }
        private string _p3;
        [XmlElement(ElementName = "p3")]
        public string P3
        {
            get => _p3;
            set
            {
                _p3 = value;
                OnPropertyChanged(nameof(P3));
            }
        }
        private string _p4;
        [XmlElement(ElementName = "p4")]
        public string P4
        {
            get => _p4;
            set
            {
                _p4 = value;
                OnPropertyChanged(nameof(P4));
            }
        }
        private string _p5;
        [XmlElement(ElementName = "p5")]
        public string P5
        {
            get => _p5;
            set
            {
                _p5 = value;
                OnPropertyChanged(nameof(P5));
            }
        }
        private string _p6;
        [XmlElement(ElementName = "p6")]
        public string P6
        {
            get => _p6;
            set
            {
                _p6 = value;
                OnPropertyChanged(nameof(P6));
            }
        }
        private string _p7;
        [XmlElement(ElementName = "p7")]
        public string P7
        {
            get => _p7;
            set
            {
                _p7 = value;
                OnPropertyChanged(nameof(P7));
            }
        }
        private string _p8;
        [XmlElement(ElementName = "p8")]
        public string P8
        {
            get => _p8;
            set
            {
                _p8 = value;
                OnPropertyChanged(nameof(P8));
            }
        }
        private string _p9;
        [XmlElement(ElementName = "p9")]
        public string P9
        {
            get => _p9;
            set
            {
                _p9 = value;
                OnPropertyChanged(nameof(P9));
            }
        }
        private string _p10;
        [XmlElement(ElementName = "p10")]
        public string P10
        {
            get => _p10;
            set
            {
                _p10 = value;
                OnPropertyChanged(nameof(P10));
            }
        }
        private string _p11;
        [XmlElement(ElementName = "p11")]
        public string P11
        {
            get => _p11;
            set
            {
                _p11 = value;
                OnPropertyChanged(nameof(P11));
            }
        }
        private string _p12;
        [XmlElement(ElementName = "p12")]
        public string P12
        {
            get => _p12;
            set
            {
                _p12 = value;
                OnPropertyChanged(nameof(P12));
            }
        }
        private string _p13;
        [XmlElement(ElementName = "p13")]
        public string P13
        {
            get => _p13;
            set
            {
                _p13 = value;
                OnPropertyChanged(nameof(P13));
            }
        }
        private string _p15;
        [XmlElement(ElementName = "p15")]
        public string P15
        {
            get => _p15;
            set
            {
                _p15 = value;
                OnPropertyChanged(nameof(P15));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}