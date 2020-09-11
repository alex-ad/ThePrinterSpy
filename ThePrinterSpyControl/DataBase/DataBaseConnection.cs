using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace ThePrinterSpyControl.DataBase
{
    public class DataBaseConnection
    {
        public readonly RegistryConfig Cfg;

        public DataBaseConnection()
        {
            Cfg = new RegistryConfig();
        }

        public EntityConnection Get()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder
            {
                Password = Cfg.DbPassword,
                DataSource = Cfg.DbServer,
                UserID = Cfg.DbUser,
                InitialCatalog = Cfg.DbName
            };

            string sqlConn = sb.ToString();
            string providerName = "System.Data.SqlClient";

            EntityConnectionStringBuilder eb = new EntityConnectionStringBuilder();
            eb.Provider = providerName;
            eb.ProviderConnectionString = sqlConn;
            eb.Metadata = @"res://*/DataBase.PrintSpyDb.csdl|res://*/DataBase.PrintSpyDb.ssdl|res://*/DataBase.PrintSpyDb.msl";

            return new EntityConnection(eb.ToString());
        }
    }
}
