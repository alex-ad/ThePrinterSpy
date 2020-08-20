using System.Data.SqlClient;

namespace ThePrinterSpyService.Core
{
    public class DataBaseConnection
    {
        public readonly RegistryConfig Cfg;

        public DataBaseConnection()
        {
            Cfg = new RegistryConfig();
        }

        public SqlConnection Get()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.Password = Cfg.DbPassword;
            sb.DataSource = Cfg.DbServer;
            sb.UserID = Cfg.DbUser;
            sb.InitialCatalog = Cfg.DbName;

            return new SqlConnection(sb.ConnectionString);
        }
    }
}
