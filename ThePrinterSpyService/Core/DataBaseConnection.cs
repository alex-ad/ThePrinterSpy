using System.Data.SqlClient;

namespace ThePrinterSpyService.Core
{
    public class DataBaseConnection
    {
        private readonly RegistryConfig _cfg;

        public DataBaseConnection()
        {
            _cfg = new RegistryConfig();
        }

        public SqlConnection Get()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder
            {
                Password = _cfg.DbPassword,
                DataSource = _cfg.DbServer,
                UserID = _cfg.DbUser,
                InitialCatalog = _cfg.DbName
            };

            return new SqlConnection(sb.ConnectionString);
        }
    }
}
