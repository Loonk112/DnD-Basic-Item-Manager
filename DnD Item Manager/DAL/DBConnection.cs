using MySql.Data.MySqlClient;

namespace DnD_Item_Manager.DAL
{
    internal class DBConnection
    {
        private MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();

        private static DBConnection instance = null;
        // Połączenie do bazy danych
        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBConnection();
                return instance;
            }
        }
        public MySqlConnection Connection => new MySqlConnection(stringBuilder.ToString());

        // Wypełnia 'stringBuilder' na podstawie 'Setting' gdzie są zawarte dane dostępowe
        private DBConnection()
        {
            stringBuilder.UserID = Settings.Default.userID;
            stringBuilder.Password = Settings.Default.password;
            stringBuilder.Database = Settings.Default.database;
            stringBuilder.Port = Settings.Default.port;
            stringBuilder.Server = Settings.Default.server;

        }
    }
}
