using NHibernate;

namespace ORM.Versioning
{
    public class VersionedDemo
    {
        public static void Run(ISessionFactory sessionFactory)
        {
            FillDatabase(sessionFactory);

            //symulujemy osobne requesty do aplikacji. Tu pokazaliśmy formularz
            var v1 = GetVersioned(sessionFactory);
            //symulujemy osobne requesty do aplikacji. Tu pokazaliśmy formularz
            var v2 = GetVersioned(sessionFactory);

            //symulujemy osobne requesty do aplikacji. Tu zapisujemy formularz

            using (var session = sessionFactory.OpenSession())
            {
                using (var tran = session.BeginTransaction())
                {
                    v2.Name = "2";
                    session.Update(v2);
                    tran.Commit();
                }
            }

            //symulujemy osobne requesty do aplikacji. Tu zapisujemy formularz
            using (var session = sessionFactory.OpenSession())
            {
                using (var tran = session.BeginTransaction())
                {
                    v1.Name = "3";
                    session.Update(v1);
                    tran.Commit();
                }
            }
        }

        private static VersionedEntity GetVersioned(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session
                    .QueryOver<VersionedEntity>()
                    .SingleOrDefault();
            }
        }

        private static void FillDatabase(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var versioned = new VersionedEntity()
                {
                    Name = "1"
                };
                session.Save(versioned);
            }
        }
    }
}