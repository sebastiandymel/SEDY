using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CommandLineSyntax
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ProgramInfo.Info);
                return;
            }

            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);
            optionParser.Parse(args);
            var executor = new OptionExecutor();
            var output = executor.Execute(optionParser.ParsedOptions, optionParser.MainArgument);

            Console.WriteLine(output);
            Console.WriteLine("Hello World!");
            Console.WriteLine(optionParser.MainArgument);

            Console.ReadLine();
        }
    }

    public static class ProgramInfo
    {
        public static string Info => $"{Assembly.GetExecutingAssembly().GetName().Name} <directory path> [--days:<number of days>] [--version] [--help]";
    }


    public class MainCommand : OptionBase
    {
        public override string LongKey => null;
        public override string ShortKey => null;
        public override string Description => null;

        public override object Execute(object input)
        {
            return input;

        }

        public override string UsageNote => "<directory info>";
        public override bool TryBuild(string argument)
        {
            if (Path.IsPathFullyQualified(argument))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// CLI Option
    /// https://softwareengineering.stackexchange.com/questions/307467/what-are-good-habits-for-designing-command-line-arguments
    /// </summary>
    public interface IOption : IPipelineCommand
    {
        /// <summary>
        /// Long version of the option switch. For example --version, --help
        /// </summary>
        string LongKey { get; }
        /// <summary>
        /// Shor version of the option switch. For example -v, -h
        /// </summary>
        string ShortKey { get; }
        /// <summary>
        /// Display note of this option
        /// </summary>
        string Description { get; }
        /// <summary>
        /// How to use this option, with example
        /// </summary>
        string UsageNote { get; }
        /// <summary>
        /// Does it match given argument? If so, build up this option
        /// </summary>
        /// <param name="argument"></param>
        bool TryBuild(string argument);
    }

    public abstract class OptionBase : IOption
    {
        public abstract string LongKey { get; }
        public abstract string ShortKey { get; }
        public abstract string Description { get; }
        public virtual bool IsExclusive => false;
        public virtual string UsageNote
        {
            get
            {
                return string.Format(UsageNoteFormat, LongKey, ShortKey, Description);
            }
        }
        public abstract object Execute(object input);

        public virtual bool TryBuild(string argument)
        {
            return argument == ShortKey || argument == LongKey;
        }

        public const string UsageNoteFormat = "{0,-10} {1, -7} {2}";
    }

    public interface IPipelineCommand
    {
        /// <summary>
        /// Exclusive option means that it should be used as the only one switch in arguments.
        /// For example --help. It doesnt make any sense to have --help and other options inside arguments.
        /// </summary>
        bool IsExclusive { get; }
        /// <summary>
        /// Executes option
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        object Execute(object input);
    }

    public class HelpOption : OptionBase
    {
        private KnownOptions knownOptions;

        public HelpOption(KnownOptions knownOptions)
        {
            this.knownOptions = knownOptions;
        }

        public override string LongKey => "--help";
        public override string ShortKey => "-h";
        public override string Description => "Displays help for this program";
        public override bool IsExclusive => true;
        public override object Execute(object input)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(ProgramInfo.Info);
            //sb.AppendLine(string.Format(OptionBase.UsageNoteFormat, "[Long]", "[Short]", "[Description]"));
            foreach (var item in this.knownOptions.All.OrderBy(o => o.LongKey))
            {
                sb.AppendLine(item.UsageNote);
            }
            Console.Write(sb.ToString());
            return input;
        }
    }

    public class VersionOption : OptionBase
    {
        public override string LongKey => "--version";

        public override string ShortKey => "-v";

        public override string Description => "Displays version info";

        public override object Execute(object input)
        {
            Console.WriteLine($"Progrma version: {VersionInfo.Version}");

            return input;
        }
    }

    /// <summary>
    /// Defines all the known options to execute
    /// </summary>
    public class KnownOptions
    {
        /// <summary>
        /// All the known options for the CLI, in order of execution
        /// </summary>
        public virtual IOption[] All => new IOption[]
        {
            new HelpOption(this),
            new VersionOption(),
            new DaysSinceOption(),
            new MainCommand(),
        };

        public virtual IPipelineCommand[] Pipeline => new IOption[]
        {
            new HelpOption(this),
            new VersionOption(),
            new DaysSinceOption(),
            new MainCommand(),
        };
    }

/// <summary>
/// Responsible for parsing command line arguments into object model options.
/// </summary>
public class OptionParser
{
    public OptionParser(KnownOptions knownOptions)
    {
        KnownOptions = knownOptions;
    }

    private KnownOptions KnownOptions { get; }

    public virtual void Parse(string[] arguments)
    {
        var results = new List<IOption>();
        var invalidOptions = new List<string>();

        foreach (var argument in arguments)
        {
            foreach (var item in this.KnownOptions.All)
            {
                if (item.TryBuild(argument))
                {
                    results.Add(item);
                }
            }
        }
        MainArgument = arguments.SingleOrDefault(a => !a.StartsWith("-"));
        ParsedOptions = results.ToArray();
        InavlidOptions = invalidOptions.ToArray();
    }

    public string MainArgument { get; private set; }
    public IPipelineCommand[] ParsedOptions { get; private set; } = Array.Empty<IOption>();
    public string[] InavlidOptions { get; private set; } = Array.Empty<string>();
}

public class OptionExecutor
{
    public object Execute(IPipelineCommand[] all, object mainArgument)
    {
        /* Cli commands are arranged in pipeline:
                     +-----------+        +-----------+      +-----------+        +-----------+
         (Input) --->|CliCommand1+------->|CliCommand2+----->|CliCommand3+------->|CliCommand4|----> (Output)
                     +-----------+        +-----------+      +-----------+        +-----------+
         * 
         */
        object input = mainArgument;
        var anyExclusive = all.FirstOrDefault(x => x.IsExclusive);
        if (anyExclusive != null)
        {
            anyExclusive.Execute(input);
            return null;
        }
        foreach (var option in all)
        {
            input = option.Execute(input);
        }

        return input;
    }
}

public static class VersionInfo
{
    public static Version Version => new Version(1, 0, 0);
}

public class DaysSinceOption : OptionBase
{
    private int daysSince = 0;

    public override string LongKey => "--days";
    public override string ShortKey => "-d";
    public override string Description => "Number of days in history lookup. Example --days:100";
    public override string UsageNote => string.Format(OptionBase.UsageNoteFormat, $"{LongKey}:n", $"{ShortKey}:n", Description);
    public override object Execute(object input)
    {
        return input;
    }
    public override bool TryBuild(string argument)
    {
       
        var split = argument.Split(":");
        if (split.Length <= 1)
        {
            return false;
        }
        var match = base.TryBuild(split[0]) && int.TryParse(split[1], out this.daysSince);
        return match;
    }
    public int DaysSince => this.daysSince;
}
}
