namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public struct MedicineInventoryStruct
    {
        public int ID { get; set; }
        public double Inventory { get; set; }

        public MedicineInventoryStruct(int id, double inventory)
        {
            ID = id;
            Inventory = inventory;
        }
    }
}