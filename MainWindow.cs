using MenuUserControl;
using System.Collections.Generic;

namespace His_Pos
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public string[] HisFeatures = { Properties.Resources.hisPrescription };
        public static string[] HisFeatures1Item = { Properties.Resources.hisPrescriptionDeclare ,Properties.Resources.hisPrescriptionInquire,Properties.Resources.hisPrescriptionRevise};
        public string[] HisFeaturesIcon = { @"..\Images\PrescriptionIcon.png"};
        public List<UserControl1> HisFeaturesList;
        public List<string[]> HisFeaturesItemsList;
        private readonly string[] _posFeatures = {  };
        private readonly string[] _posFeatures1Item =
        {
            
        };
        private readonly string[] _posFeaturesIcon = { };
        private List<UserControl1> _posFeaturesList;
        private List<string[]> _posFeaturesItemsList;
        public bool HisHided = false;
        
    }
}
