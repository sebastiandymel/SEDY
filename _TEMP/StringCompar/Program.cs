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
            };

            foreach (var item in items)
            {
                Console.WriteLine($"{Compare(item)} \t = {item}");
            }

            Console.Read();
        }

        private static bool Compare(string input)
        {
            var regExp = @".Format\(.*\)";
            var matches = Regex.Match(input, regExp);
            return matches.Success;
        }
    }
}
