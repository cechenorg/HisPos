namespace His_Pos.Database
{
    public interface DatabaseConnection
    {
        void OpenConnection();
        void CloseConnection();
        void LogError(string procName, string parameters, string error);
    }
}
