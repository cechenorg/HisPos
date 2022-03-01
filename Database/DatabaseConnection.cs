namespace His_Pos.Database
{
    public interface DatabaseConnection
    {
        void OpenConnection();

        void CloseConnection();
    }
}