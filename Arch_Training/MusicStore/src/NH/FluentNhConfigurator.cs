#region

using System.Globalization;
using System.IO;
using System.Reflection;
using Castle.Services.Logging.NLogIntegration;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

#endregion

namespace MvcMusicStore.NH
{
    /// <summary>
    ///   Configurator
    /// </summary>
    public class FluentNhConfigurator
    {
        public const int HiLoSize = 1000;
        public const int BatchSize = HiLoSize/10;
        private readonly Assembly[] _assemblies;
        private NHibernate.Cfg.Configuration _config;

        public FluentNhConfigurator(Assembly[] assembliesToScan)
        {
            _assemblies = assembliesToScan;
        }

        public NHibernate.Cfg.Configuration BuildConfiguration(string connectionString, bool debug=false)
        {
            var configuration = new NHibernate.Cfg.Configuration();
            configuration.SetProperty(Environment.ShowSql, "false");
            //configure NH to use ISet from .NET instead of from Iesi.Collections
            //configuration.SetProperty(Environment.CollectionTypeFactoryClass, typeof (Net4CollectionTypeFactory).AssemblyQualifiedName);

            var builder = Fluently
                .Configure(configuration)
                .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString)
                    //.ShowSql()
                    //.FormatSql()
                    .Dialect<MsSql2008Dialect>()
                )
                .ExposeConfiguration(c =>
                {
                    c.SetProperty(Environment.BatchSize, BatchSize.ToString(CultureInfo.InvariantCulture));
                    c.SetProperty("hbm2ddl.keywords", "auto-quote");

                    if (debug)
                    {
                        c.SetProperty(Environment.ShowSql, "true");
                        c.SetProperty(Environment.FormatSql, "true");
                        c.SetProperty("LogSqlInConsole", "true");
                    }
                    else
                    {
                        c.SetProperty(Environment.ShowSql, "false");
                        c.SetProperty(Environment.FormatSql, "false");
                        c.SetProperty("LogSqlInConsole", "false");
                    }
                    c.SetProperty("nhibernate-logger", typeof (NLogFactory).AssemblyQualifiedName);
                })
                .Mappings(m =>
                {
                    var model = AutoMap.Assemblies(new AppAutomappingCfg(),_assemblies);
                    m.AutoMappings.Add(model);
                })
                ;
            if (debug)
            {
                builder = builder.ExposeConfiguration(cfg =>
                {
                    const string schemaPath = @"c:\temp\nh_mappings_export\schema.sql";
                    using (var file = File.Open(schemaPath, FileMode.CreateNew))
                    using (var writer = new StreamWriter(file))
                    {
                        var se = new SchemaExport(cfg);
                        se.Create(writer, false);
                    }
                });
            }
            _config = builder
                .BuildConfiguration();

            return _config;
        }
    }
}