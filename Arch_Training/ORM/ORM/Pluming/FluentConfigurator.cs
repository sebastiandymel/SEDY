#region

using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using ORM.Inheritence._02;

#endregion

namespace ORM.Pluming
{
    /// <summary>
    ///     Configurator
    /// </summary>
    public class FluentNhConfigurator
    {
        private readonly Assembly[] _assemblies;
        private Configuration _config;

        public FluentNhConfigurator(Assembly[] assembliesToScan)
        {
            _assemblies = assembliesToScan;
        }

        public Configuration BuildConfiguration(string connectionString, bool debug = false)
        {
            var configuration = new Configuration();
            
            var builder = Fluently
                    .Configure(configuration)
                    .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                    .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString)
                        .ShowSql()
                        .FormatSql()
                        .Dialect<MsSql2012Dialect>()
                    )
                    .ExposeConfiguration(c => { c.SetProperty("hbm2ddl.keywords", "auto-quote"); })
                    .Mappings(m =>
                    {
                        var model = AutoMap.Assemblies(new AppAutomappingCfg(), _assemblies);
                        foreach (var assembly in _assemblies)
                        {
                            model
                                .UseOverridesFromAssembly(assembly)
                                .Conventions
                                .AddAssembly(assembly);
                        }
                         m.AutoMappings.Add(model);
                    })
                ;

            _config = builder
                .BuildConfiguration();

            return _config;
        }
    }
}