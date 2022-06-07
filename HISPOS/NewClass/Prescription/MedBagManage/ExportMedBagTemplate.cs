using His_Pos.Service.ExportService;

namespace His_Pos.NewClass.Prescription.MedBagManage
{
    public class ExportMedBagTemplate : ExportExcelTemplate
    {
        public override string GetSheetName()
        {
            return (Source as MedBagPrescriptionStructs).Type.Equals("Res") ? "預約" : "登錄";
        }

        protected override void CreateExcelSettings()
        {
            MedBagPrescriptionStructs data = Source as MedBagPrescriptionStructs;

            Settings.Add(new ExportClassExcelSetting(data, 1, 1, new[] { "Status", "CustomerName", "Institution", "Division", "AdjustDate", "StockValue" })
                                                    .SetHeaders(new[] { "備藥狀態", "姓名", "院所", "科別", "調劑日", "預估現值" }));
        }
    }
}