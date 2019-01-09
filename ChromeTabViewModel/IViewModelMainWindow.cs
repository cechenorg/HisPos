namespace His_Pos.ChromeTabViewModel
{
    public interface IViewModelMainWindow
    {
        bool CanMoveTabs { get; set; }
        bool ShowAddButton { get; set; }

        string CardReaderStatus { get; set; }
        string SamDcStatus { get; set; }
        string HpcCardStatus { get; set; }
    }
}
