using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace RandomResourceGenerator
{
    class Program
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        static void Main(string[] args)
        {
            GenerateColorResources(10_000);
            Console.ReadLine();
        }

        private static void GenerateColorResources(int count)
        {
            var fileName = "Colors.xaml";

            var resourceDictionary = new ResourceDictionary();

            for (int i = 0; i < count; i++)
            {
                resourceDictionary.Add(
                    $"ThisIsSomeKey{i}",
                    Color.FromArgb(255, (byte)rnd.Next(0,255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255))
                    );
            }

            var output = XamlWriter.Save(resourceDictionary);
            File.WriteAllText(fileName, output);

            Console.WriteLine("Colors.xaml generated");
        }
    }
}
