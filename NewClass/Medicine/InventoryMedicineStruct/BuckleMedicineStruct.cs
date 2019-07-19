namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public struct BuckleMedicineStruct
    {
        public int ID { get; set; }
        public double BuckleAmount { get; set; }

        public BuckleMedicineStruct(int id, double buckleAmount)
        {
            ID = id;
            BuckleAmount = buckleAmount;
        }
    }
}
