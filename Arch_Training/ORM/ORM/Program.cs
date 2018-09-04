using System;
using System.Configuration;
using ORM.Inheritence;
using ORM.Pluming;
using ORM.Versioning;

namespace ORM
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            var config =
                new FluentNhConfigurator(new[] {typeof(Program).Assembly}).BuildConfiguration(connectionString, true);
            config.Deploy(connectionString);

            var sessionFactory = config.BuildSessionFactory();
            var sesson = sessionFactory.OpenSession();

            InheriteneDemo.Run(sesson);

            //VersionedDemo.Run(sessionFactory);

            Console.ReadLine();
        }
    }
}