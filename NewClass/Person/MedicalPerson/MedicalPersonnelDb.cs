using System.Data;

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
