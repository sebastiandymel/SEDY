using System.Configuration;
using NHibernate;
using Configuration = NHibernate.Cfg.Configuration;

namespace MvcMusicStore.NH
{
    public class NhibernateSessionFactory
    {
        private static Configuration _config;

        public ISession OpenSession()
        {
            if (_config == null)
                _config = Configure();
            return _config.BuildSessionFactory().OpenSession();
        }

        private Configuration Configure()
        {
            var ret = new FluentNhConfigurator(
                new[]
                {
                    typeof(NhibernateSessionFactory).Assembly
                }).BuildConfiguration(ConfigurationManager.ConnectionStrings["MusicStoreEntities"].ConnectionString);
            return ret;
        }
    }
}