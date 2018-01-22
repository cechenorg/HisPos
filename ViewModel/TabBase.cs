using GalaSoft.MvvmLight;

namespace His_Pos.ViewModel
{
    public abstract class TabBase : ViewModelBase
    {
        private int _tabNumber;
        public int TabNumber
        {
            get { return _tabNumber; }
            set
            {
                if (_tabNumber != value)
                {
                    Set(() => TabNumber, ref _tabNumber, value);
                }
            }
        }

        private string _tabName;
        public string TabName
        {
            get { return _tabName; }
            set
            {
                if (_tabName != value)
                {
                    Set(() => TabName, ref _tabName, value);
                }
            }
        }

        public abstract TabBase getTab();

    }
}
