using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CommandLineSyntax
{
    // Microsoft CLI syntax:
    // https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-R2-and-2012/cc771080(v=ws.11)

    class Program
    {
        static void Main(string[] args)
        {
            var parser = new AttributeParser();
            var configuration = parser.Parse<ProgramConfoguration>(args);

            if (configuration.ShowHelp)
            {
                Console.WriteLine("HEEELP");
            }

            Console.ReadLine();
        }
    }

    /// <summary>
    /// Parses arguments intro strongly typed object.
    /// Each property can be represented as separate option.
    /// 
    /// </summary>
    /// <example>     
    /// public class ProgramConfoguration
    ///    {
    ///    [Option]
    ///    [OptionAlias("--days")]
    ///    [OptionAlias("-d")]
    ///    public int DaysSince { get; set; }
    ///
    ///    [Option]
    ///    [OptionAlias("--help")]
    ///    [OptionAlias("-h")]
    ///    public bool ShowHelp { get; set; }
    ///     }
    /// </example>
    public class AttributeParser : IAttributeParser
    {
        private Dictionary<Type, Func<string, object>> customConverters = new Dictionary<Type, Func<string, object>>();

        public T Parse<T>(params string[] arguments) where T : new()
        {
            var result = new T();
            var argumments = arguments.ToList();
            var allProps = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in allProps)
            {
                var mainArg = property.GetCustomAttribute<MainInputAttributeAttribute>();
                if (mainArg != null)
                {
                    property.SetValue(result, ConvertToPropertyType(arguments[0], property.PropertyType));
                    continue;
                }
                var optionAttribute = property.GetCustomAttribute<OptionAttribute>(true);
                if (optionAttribute != null)
                {
                    var alliases = ExtractAlliases(property);                    
                    if (TryMatch(arguments, alliases, out var alias, out var argument))
                    {
                        if (ShouldTakeNextArg(alias, argument, property, arguments))
                        {
                            var i = Array.IndexOf<string>(arguments, argument);
                            property.SetValue(result, ConvertToPropertyType(arguments[i + 1], property.PropertyType));
                        }

                        var split = Split(alias, argument, property.PropertyType);
                        if (split.Length == 2)
                        {
                            property.SetValue(result, ConvertToPropertyType(split[1], property.PropertyType));
                        }
                        else if (property.PropertyType.IsAssignableFrom(typeof(bool)))
                        {
                            property.SetValue(result, true);
                        }
                    }
                    else if (optionAttribute.IsRequired)
                    {
                        throw new MissingOptionException("Missing parameter", "!");
                    }
                }

                var outputArg = property.GetCustomAttribute<MainOutputAttributeAttribute>();
                if (outputArg != null)
                {
                    property.SetValue(result, ConvertToPropertyType(arguments.Last(), property.PropertyType));
                    continue;
                }
            }

            return result;
        }

        protected virtual bool ShouldTakeNextArg(Alias alias, string argument, PropertyInfo property, string[] arguments)
        {
            return alias.Splitter == " ";
        }

        protected virtual string[] Split(Alias alias, string argument, Type propertyType)
        {
            return argument.Replace(alias.Name, string.Empty).Split(alias.Splitter);
        }

        protected virtual bool TryMatch(string[] arguments, Alias[] alliases, out Alias foundAlias, out string foundArgument)
        {
            foundArgument = null;
            foundAlias = default(Alias);

            foreach (var arg in arguments)
            {
                foreach (var item in alliases)
                {
                    if (arg.StartsWith(item.Name))
                    {
                        foundArgument = arg;
                        foundAlias = item;
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual Alias[] ExtractAlliases(MemberInfo property)
        {
            var alliases = new List<Alias>();
            var alliasAttributes = property.GetCustomAttributes<OptionAliasAttribute>();
            if (alliasAttributes != null && alliasAttributes.Any())
            {
                alliases.AddRange(alliasAttributes.Select(a => new Alias { Name = a.Alias, Splitter = a.Splitter }));
            }
            else
            {
                alliases.Add(new Alias { Name = property.Name, Splitter = ":" });
            }
            return alliases.ToArray();
        }

        public void RegisterCustomConverter<T>(Func<string, T> converter) where T:class
        {
            this.customConverters.Add(typeof(T), converter);
        }

        protected object ConvertToPropertyType(string input, Type expectedType)
        {
            try
            {
                if (this.customConverters.ContainsKey(expectedType))
                {
                    var customConverter = customConverters[expectedType];
                    return customConverter(input);
                }

                var converted = Convert.ChangeType(input, expectedType, CultureInfo.InvariantCulture);
                return converted;
            }
            catch (Exception ex)
            {
                throw new OptionFormatException("Invalid option format", ex);
            }
        }

        protected struct Alias
        {
            public string Name { get; set; }
            public string Splitter { get; set; }
        }        
    }

    public class ArgumentExecutor: AttributeParser
    {
        public T Execute<T>(params string[] arguments) where T : new()
        {
            var result = new T();
            var argumments = arguments.ToList();
            var allMethods = result.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

            var pipeline = new List<Action>();

            foreach (var method in allMethods)
            {
                if (method.GetParameters().Any(p => 
                p.GetCustomAttribute<MainInputAttributeAttribute>() != null ||
                p.GetCustomAttribute<MainOutputAttributeAttribute>() != null))
                {
                    var methodParams = method.GetParameters();
                    if (methodParams.Length == 2)
                    {
                        var methodArgs = new object[2];
                        if (methodParams[0].GetCustomAttribute<MainInputAttributeAttribute>() != null)
                        {
                            methodArgs[0] = ConvertToPropertyType(arguments[0], method.GetParameters()[0].ParameterType);
                        }
                        if (methodParams[1].GetCustomAttribute<MainOutputAttributeAttribute>() != null)
                        {
                            methodArgs[1] = ConvertToPropertyType(arguments.Last(), method.GetParameters()[1].ParameterType);
                        }
                        pipeline.Add(() => method.Invoke(result, methodArgs));
                    }
                    if (methodParams.Length == 1)
                    {
                        var methodArgs = new object[1];
                        if (methodParams[0].GetCustomAttribute<MainInputAttributeAttribute>() != null)
                        {
                            methodArgs[0] = ConvertToPropertyType(arguments[0], method.GetParameters()[0].ParameterType);
                        }
                        if (methodParams[0].GetCustomAttribute<MainOutputAttributeAttribute>() != null)
                        {
                            methodArgs[0] = ConvertToPropertyType(arguments.Last(), method.GetParameters()[0].ParameterType);
                        }
                        pipeline.Add(() => method.Invoke(result, methodArgs));
                    }
                    continue;
                }

                var optionAttribute = method.GetCustomAttribute<OptionAttribute>(true);
                if (optionAttribute != null)
                {
                    var alliases = ExtractAlliases(method);
                    if (TryMatch(arguments, alliases, out var alias, out var argument))
                    {
                        if (alias.Splitter == " ")
                        {
                            var i = Array.IndexOf<string>(arguments, argument);
                            var input = ConvertToPropertyType(arguments[i + 1], method.GetParameters()[0].ParameterType);
                            Action toExecute = () => method.Invoke(result, new[] { input });
                            pipeline.Add(toExecute);
                            continue;
                        }

                        var split = argument.Split(alias.Splitter);
                        if (split.Length == 2)
                        {
                            var input = ConvertToPropertyType(split[1], method.GetParameters()[0].ParameterType);
                            Action toExecute = () => method.Invoke(result, new[] { input });
                            pipeline.Add(toExecute);
                        }
                        else if (method.GetParameters().Length == 0)
                        {
                            method.Invoke(result, new object[0]);
                        }
                    }
                    else if (optionAttribute.IsRequired)
                    {
                        throw new MissingOptionException("Missing parameter", "!");
                    }
                }

                foreach (var item in pipeline)
                {
                    item();
                }
            }

            return result;
        }

    }

    public class POSIXParser: AttributeParser
    {
        // POSIX (Portable Operating System Interface for UNIX)
        //- Arguments are options if they begin with a hyphen delimiter(‘-’).
        //- Multiple options may follow a hyphen delimiter in a single token if the options do not take arguments.Thus, ‘-abc’ is equivalent to ‘-a -b -c’.
        //- Option names are single alphanumeric characters(as for isalnum; see Classification of Characters).
        //- Certain options require an argument.For example, the ‘-o’ command of the ld command requires an argument—an output file name.
        //- An option and its argument may or may not appear as separate tokens. (In other words, the whitespace separating them is optional.) Thus, ‘-o foo’ and ‘-ofoo’ are equivalent.
        //- Options typically precede other non-option arguments.


        protected override bool TryMatch(string[] arguments, Alias[] alliases, out Alias foundAlias, out string foundArgument)
        {
            foundArgument = null;
            foundAlias = default(Alias);

            foreach (var arg in arguments)
            {
                if (arg.StartsWith("-"))
                {
                    for (int i = 1; i < arg.Length; i++)
                    {
                        foreach (var item in alliases)
                        {
                            if (arg[i] == item.Name[0])
                            {
                                foundArgument = arg;
                                foundAlias = item;
                                return true;
                            }
                        }
                    }
                    
                }
                
            }

            return false;
        }

        protected override string[] Split(Alias alias, string argument, Type expectedType)
        {
           var newargument = argument.Replace("-", String.Empty);
            if (expectedType != typeof(bool) && alias.Splitter == String.Empty && newargument.Length > 1)
            {
                var result = new string[2];
                result[0] = alias.Name;
                result[1] = newargument.Substring(1, newargument.Length - 1);
                return result;
            }
            else
            {
                return base.Split(alias, argument, expectedType);
            }
        }

        protected override bool ShouldTakeNextArg(Alias alias, string argument, PropertyInfo property, string[] arguments)
        {
            var i = Array.IndexOf<string>(arguments, argument);
            if (arguments.Length > (i + 1) && property.PropertyType != typeof(bool))
            {
                return true;
            }
            else
            {
                return base.ShouldTakeNextArg(alias, argument, property, arguments);
            }            
        }
    }
    
    public class POSIX_Alias : OptionAliasAttribute
    {
        public POSIX_Alias(char alias) : base(new string(new[] { alias }), String.Empty)
        {           
        }        
    }

    public class OptionFormatException : Exception
    {
        public OptionFormatException(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
    public class MissingOptionException : ArgumentException
    {
        public MissingOptionException(string msg, string alias) : base(msg, alias)
        {

        }
    }

    public class ProgramConfoguration
    {
        [Option]
        [OptionAlias("--days")]
        [OptionAlias("-d")]
        public int DaysSince { get; set; }

        [Option]
        [OptionAlias("--help")]
        [OptionAlias("-h")]
        public bool ShowHelp { get; set; }

        [Option]
        [OptionAlias("--version")]
        [OptionAlias("-v")]
        public bool ShowVersion { get; set; }

        [MainInputAttribute]
        public string ThisIsMainArgument { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(bool isRequired = false)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MainInputAttributeAttribute: OptionAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MainOutputAttributeAttribute : OptionAttribute
    {
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class OptionAliasAttribute : Attribute
    {
        public OptionAliasAttribute(string alias) : this(alias, ":")
        {            
        }

        public OptionAliasAttribute(string alias, string splitter)
        {
            Alias = alias;
            Splitter = splitter;
        }

        public string Alias { get; }
        public string Splitter { get; }
    }

    


}
