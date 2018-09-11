using System;
using System.Text.RegularExpressions;

namespace StringCompar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var items = new [] 
            {
                "String.AlaMaKota",
                "String.Something.Format(x)",
                "String.Something.Format(x,y)",
                "String.Something.Format(x,y)A",
                "String.Something.Format(x,y",
                "String.Something.Format{x,y}A",
                "String.Something{0}.Format(x,y",
                "{0.00}",
                "{}}",
            };

            foreach (var item in items)
            {
                Console.WriteLine($"{Compare(item)} \t = {item}");
            }

            Console.WriteLine("============================");

            foreach (var item in items)
            {
                Console.WriteLine($"{Compare2(item)} \t = {item}");
            }

            Console.Read();
        }

        private static bool Compare2(string input)
        {
            var regExp = @"\{.*\}";
            var matches = Regex.Match(input, regExp);
            return matches.Success;
        }

        private static bool Compare(string input)
        {
            var regExp = @"^.*.Format\(.*\)$";
            var matches = Regex.Match(input, regExp);
            return matches.Success;
        }
    }
}
