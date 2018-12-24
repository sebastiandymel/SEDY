using System;
using NHibernate;
using ORM.Inheritence._01;
using ORM.Inheritence._02;
using ORM.Inheritence._03;

namespace ORM.Inheritence
{
    public class InheriteneDemo
    {
        public static void Run(ISession session)
        {
            Generate01Inserts(session);
            Generate02Inserts(session);
            Generate03Inserts(session);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("===============01===============");
            var auditedResult = session.QueryOver<Audited>()
                .List();

            Console.WriteLine("===============02===============");
            var carResult = session.QueryOver<Car>()
                .List();

            Console.WriteLine("===============03===============");
            var employeeResult = session.QueryOver<Employee>()
                .List();
        }

        private static void Generate01Inserts(ISession session)
        {
            var product = new Product()
            {
                Code = "123",
                CreateDate = DateTime.Now,
                CreateUser = "testowy",
                Name = "product123",
            };
            var user = new User()
            {
                CreateDate = DateTime.Now,
                CreateUser = "testowy",
                Name = "product123",
            };
            session.Save(product);
            session.Save(user);
        }

        private static void Generate02Inserts(ISession session)
        {
            var newCar = new NewCar()
            {
                AvailableFrom = DateTime.Now,
                ConstructionDate = DateTime.Now,
                Manufacturer = "BMW",
                StickerPrice = 130000,
                VIN = "123456789",
                WasRefreshed = true
            };
            var usedCar = new UsedCar()
            {
                Manufacturer = "BMW",
                VIN = "23456789",
                ConstructionDate = DateTime.Now,
                StickerPrice = 130000,
                IsFirstOwner = true,
                LastOwnerName = "Jan",
                LastOwnerSurname = "Kowalski"
            };
            session.Save(newCar);
            session.Save(usedCar);
        }

        private static void Generate03Inserts(ISession session)
        {
            var scientist = new Scientist()
            {
                Name = "John",
                Surname = "Smith",
                ClearanceLevel = "All",
                Division = "Military",
                Specialization = "Mathematics"
                
            };
            var security = new Security()
            {
                Name = "John",
                Surname = "Smith",
                IsArmed = false,
                SecurityAgencyName = "Internal"
            };
            session.Save(scientist);
            session.Save(security);
        }
    }
}
