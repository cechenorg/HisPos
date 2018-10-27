using System.Collections.Generic;

namespace His_Pos.Class.Declare
{
    public class DeclareFileDdata:Ddata
    {
        public DeclareFileDdata()
        {
            Dhead = new Dhead();
            Dbody = new Dbody {Pdata = new List<Pdata>()};
            CanDeclare = true;
            HasError = false;
        }
        public DeclareFileDdata(Ddata d)
        {
            DecId = d.DecId;
            Dhead = d.Dhead;
            Dbody = d.Dbody;
            Dbody.Pdata = d.Dbody.Pdata;
            CanDeclare = true;
            HasError = false;
        }

        private bool _canDeclare;

        public bool CanDeclare
        {
            get => _canDeclare;
            set
            {
                _canDeclare = value;
                OnPropertyChanged(nameof(CanDeclare));
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }
    }
}
