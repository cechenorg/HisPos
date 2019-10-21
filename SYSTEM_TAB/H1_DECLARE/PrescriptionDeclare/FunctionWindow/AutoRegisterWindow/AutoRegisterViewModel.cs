using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Prescription;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.AutoRegisterWindow
{
    public class AutoRegisterViewModel : ViewModelBase
    {
        public string CurrentPrescriptionInfo { get; }
        public AutoRegisterViewModel(Prescription current, Prescriptions registerList)
        {
            CurrentPrescriptionInfo =
                $"姓名:{current.Patient.Name} 就醫日:{DateTimeExtensions.ConvertToTaiwanCalendarChineseFormat(current.AdjustDate, true)}\n 院所:{current.Institution.Name} 科別:{current.Division.Name} 次數:{current.ChronicTotal}-{current.ChronicSeq}";
        }
    }
}
