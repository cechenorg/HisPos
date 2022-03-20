using GalaSoft.MvvmLight.CommandWpf;

namespace His_Pos.ChromeTabViewModel
{
    public interface IViewModelMainWindow
    {
        string CardReaderStatus { get; set; }
        string SamDcStatus { get; set; }
        string HpcCardStatus { get; set; }
        bool IsBusy { get; set; }
        string BusyContent { get; set; }
        RelayCommand InitialData { get; set; }
    }
}