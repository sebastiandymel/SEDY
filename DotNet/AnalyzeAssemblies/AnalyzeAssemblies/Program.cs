using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeAssemblies
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowMenu();
            }

            var input = ConvertToInput(args);
            var config = ConvertToConfig(input);
            var allAssemblies = GetAssembliesPaths(config.TargetDir);
            var results = GetAnalizeResults(allAssemblies);

            Console.Read();
        }

        private static Analize[] GetAnalizeResults(string[] allAssemblies)
        {
            var results = new List<Analize>();

            foreach (var item in allAssemblies)
            {
                try
                {
                    var analize = new Analize();
                    analize.AssemblyPath = item;

                    var asm = Assembly.LoadFile(item);

                    var types = asm.GetTypes();
                    var typeResult = new List<AnalizedType>();
                    foreach (var type in types)
                    {
                        var typeDetails = new AnalizedType();
                        typeDetails.Name = type.FullName;

                        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
                        if (constructor != null)
                        {
                            typeDetails.ConstructorParametersCount = constructor.GetParameters().Length;
                        }

                        typeResult.Add(typeDetails);
                    }

                    analize.Result = typeResult.ToArray();
                }
                catch  { }
            }

            return results.ToArray();
        }

        private static string[] GetAssembliesPaths(string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                return Directory.GetFiles(targetDir, "*.dll", SearchOption.AllDirectories);
            }
            return new string[0];
        }

        private static Config ConvertToConfig(Dictionary<string, string> input)
        {
            var result = new Config();

            result.Pattern = "*.dll";

            if (input.ContainsKey("-t"))
            {
                result.TargetDir = input["-t"];
            }

            if (input.ContainsKey("-o"))
            {
                result.TargetDir = input["-o"];
            }

            if (input.ContainsKey("-p"))
            {
                result.Pattern = input["-p"];
            }

            return result;
        }

        private static void ShowMenu()
        {
            Console.WriteLine("Analyze assemblies");
            Console.WriteLine("Options:");
            Console.WriteLine(@"-t target_path. For example: C:\SomeDirectory");
            Console.WriteLine(@"-o output_path. For example: output.txt");
            Console.WriteLine(@"-p pattern to search files. For example *.dll");
        }

        private static Dictionary<string,string> ConvertToInput(string[] args)
        {
            var result = new Dictionary<string,string>();
            for (int i = 0; i < args.Length; i+=2)
            {
                var prop = args[i];
                var val = args[i + 1];

                result[prop] = val;
            }

            return result;
        }

        private class Config
        {
            public string TargetDir { get; set; }
            public string ResultFileName { get; set; }
            public string Pattern { get; set; }
        }

        private class Analize
        {
            public string AssemblyPath { get; set; }
            public AnalizedType[] Result { get; set; }
        }

        public class AnalizedType
        {
            public string Name { get; set; }
            public int ConstructorParametersCount { get; set; }
        }
    }
}
