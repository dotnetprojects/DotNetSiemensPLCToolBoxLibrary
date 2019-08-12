using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DotNetSimaticDatabaseProtokollerLibrary.Connections
{
    public class DatabaseConnection:IDisposable
    {
        private IDbConnection connection = null;
        private DatabaseConfig databaseConfig = null;
        public DatabaseConnection(DatabaseConfig dbConf)
        {
            databaseConfig = dbConf;
        }

        public void Connect()
        {
            switch (databaseConfig.DatabaseDriver)
            {
                case DatabaseConfig.Databasedriver.SQLlite:
                    connection = new SQLiteConnection(databaseConfig.ConnectionString);
                    break;
                case DatabaseConfig.Databasedriver.MSSQL:
                    connection = new SqlConnection(databaseConfig.ConnectionString);
                    break;
                case DatabaseConfig.Databasedriver.MySQL:
                    connection = new MySqlConnection(databaseConfig.ConnectionString);
                    break;
                case DatabaseConfig.Databasedriver.Firebird:
                    connection = new FbConnection(databaseConfig.ConnectionString);
                    break;
                case DatabaseConfig.Databasedriver.PostgreSQL:
                    connection = new NpgsqlConnection(databaseConfig.ConnectionString);
                    break;
            }

            connection.Open();
        }

        public IDataReader ReadData()
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = databaseConfig.Abfrage;
            return cmd.ExecuteReader();
        }

        public bool ReadTrigger(string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            var wrt = cmd.ExecuteScalar();
            cmd.Dispose();
            return Convert.ToBoolean(wrt);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
