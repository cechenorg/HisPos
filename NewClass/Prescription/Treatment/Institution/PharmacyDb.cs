using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.Institution {
    
    public static class PharmacyDb {
        public static DataTable GetCurrentPharmacy() { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CurrentPharmacy]"); ;
        }

        
    }
}
