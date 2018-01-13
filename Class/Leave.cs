namespace His_Pos.Class
{
    public struct Leave
	{
        public  int SickLeave { get; set; }
        public int FuneralLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int AnnualLeave { get; set; }
        public int AdjustLeave { get; set; }
        public int MarriageLeave { get; set; }

        public Leave(int sick, int funeral, int maternity, int annual, int adjust, int marriage)
        {
            SickLeave = sick;
            FuneralLeave = funeral;
            MaternityLeave = maternity;
            AnnualLeave = annual;
            AdjustLeave = adjust;
            MarriageLeave = marriage;
        }
    }
}
