using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ORM.Pluming
{
    /// <summary>
    ///   Configurator
    /// </summary>
    public static class FluentNhConfiguratorExtender
    {
        public static NHibernate.Cfg.Configuration Deploy(this NHibernate.Cfg.Configuration config, string ConnectionString)
        {
            var schemas = config.ClassMappings.Select(a => a.Table.Schema).Distinct().Where(a => !string.IsNullOrEmpty(a)).ToArray();
            PreDatabaseCreate(ConnectionString, schemas);
            new NHibernate.Tool.hbm2ddl.SchemaExport(config).Execute(true, true, false);
            return config;
        }

        private static void PreDatabaseCreate(string ConnectionString, IEnumerable<string> schemas)
        {
            Contract.Requires(schemas != null);
            Contract.Requires(string.IsNullOrEmpty(ConnectionString));
            var sqls = DropDatabseSQL(ConnectionString)
                .Union(CreateDatabseSQL(ConnectionString));
            var ConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master"
            };
            ExecuteScripts(ConnectionStringBuilder.ToString(), sqls.ToArray());
            ExecuteScripts(ConnectionString, CreateSchemasSQL(ConnectionString, schemas).ToArray());
        }
        private static void ExecuteScripts(string connection, IEnumerable<string> commands)
        {
            var sql = string.Join("\n", commands);

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        private static IEnumerable<string> DropDatabseSQL(string ConnectionString)
        {
            var databaseName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
            return new string[]
                       {
                           "use master",
                           string.Format("IF EXISTS(SELECT name FROM master..sysdatabases WHERE name = N'{0}')",databaseName),
                           "Begin",
                           string.Format("alter database {0} set single_user with rollback immediate",databaseName),
                           string.Format("Drop database {0}", databaseName),
                           "End"
                       };
        }
        private static IEnumerable<string> CreateDatabseSQL(string ConnectionString)
        {

            var databaseName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
            return new[]
                       {
                           string.Format("CREATE DATABASE {0} ", databaseName)
                               //,"Go"
                               //"ON ( NAME = {0}_dat,FILENAME = '{1}.mdf',SIZE = 20MB,FILEGROWTH = 5MB )"
                               //+ "LOG ON ( NAME = '{0}_log', FILENAME = '{1}.ldf',SIZE = 10MB,FILEGROWTH = 5MB ) ",
                               //databaseName, Path.Combine(_databasesFolder, databaseName))
                       };
        }
        public static IEnumerable<string> CreateSchemasSQL(string ConnectionString, IEnumerable<string> schemas)
        {
            Contract.Requires(schemas != null);
            Contract.Requires(string.IsNullOrEmpty(ConnectionString));
            schemas = schemas.Where(a => a != "dbo");//schemat dbo jest domyslny, więc nie można go tworzyć
            var databaseName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
            return new[] { "use " + databaseName }
                .Union(schemas.Select(a => string.Format("EXECUTE( 'CREATE SCHEMA {0}')", a)));
        }
    }
}