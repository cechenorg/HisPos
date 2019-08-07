namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public struct MedicineInventoryStruct
    {
        public int ID { get; set; }
        public double Amount { get; set; }

        public MedicineInventoryStruct(int id, double amount)
        {
            ID = id;
            Amount = amount;
        }
    }
}