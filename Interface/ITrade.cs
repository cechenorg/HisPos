namespace His_Pos.Interface
{
    public interface ITrade
    {
        double Cost { get; set; }
        double TotalPrice { get; set; }
        double Amount { get; set; }
        double Price { get; set; }

        string CountStatus { get; set; }
        string FocusColumn { get; set; }

        void CalculateData(string inputSource);
    }
}