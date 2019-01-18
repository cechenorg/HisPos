using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Person.MedicalPerson
{
    public static class MedicalPersonnelDb
    {
        public static DataTable GetData()
        { 
           return MainWindow.ServerConnection.ExecuteProc("[Get].[MedicalPersonnels]");
        }
    }
}
