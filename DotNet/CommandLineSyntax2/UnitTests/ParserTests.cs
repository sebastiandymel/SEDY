using CommandLineSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Parser_Defaults()
        {
            // ARRANGE
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(0, optionParser.ParsedOptions.Length);
        }

        [TestMethod]
        public void Parser_VersionOption_Short()
        {
            // ARRANGE
            var arguments = new[]
            {
                "-v"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is VersionOption);
        }

        [TestMethod]
        public void Parser_DaysSince_Short()
        {
            // ARRANGE
            var arguments = new[]
            {
                "-d:1000"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is DaysSinceOption);
            Assert.AreEqual(1000, ((DaysSinceOption)optionParser.ParsedOptions[0]).DaysSince);
        }

        [TestMethod]
        public void Parser_DaysSince_Long()
        {
            // ARRANGE
            var arguments = new[]
            {
                "--days:456"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is DaysSinceOption);
            Assert.AreEqual(456, ((DaysSinceOption)optionParser.ParsedOptions[0]).DaysSince);
        }


        [TestMethod]
        public void Parser_HelpOption_Short()
        {
            // ARRANGE
            var arguments = new[]
            {
                "-h"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is HelpOption);
        }

        [TestMethod]
        public void Parser_VersionOption_Long()
        {
            // ARRANGE
            var arguments = new[]
            {
                "--version"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is VersionOption);
        }

        [TestMethod]
        public void Parser_HelpOption_Long()
        {
            // ARRANGE
            var arguments = new[]
            {
                "--help"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.IsNull(optionParser.MainArgument);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(1, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is HelpOption);
        }

        [TestMethod]
        public void Parser_HelpAndVersion()
        {
            // ARRANGE
            var arguments = new[]
            {
                "--help",
                "-v"
        };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.AreEqual(2, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[0] is HelpOption);
            Assert.IsTrue(optionParser.ParsedOptions[1] is VersionOption);
        }

        [TestMethod]
        public void Parser_VersionAndHelp()
        {
            // ARRANGE
            var arguments = new[]
            {
               "-v",
               "--help"
        };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.AreEqual(2, optionParser.ParsedOptions.Length);
            Assert.IsTrue(optionParser.ParsedOptions[1] is HelpOption);
            Assert.IsTrue(optionParser.ParsedOptions[0] is VersionOption);
        }

        [TestMethod]
        public void Parser_JustMainArgument()
        {
            // ARRANGE
            var arguments = new[]
            {
                "someargumentNotAnOption"
            };
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.IsNotNull(optionParser.ParsedOptions);
            Assert.IsNotNull(optionParser.InavlidOptions);
            Assert.AreEqual(0, optionParser.InavlidOptions.Length);
            Assert.AreEqual(0, optionParser.ParsedOptions.Length);
            Assert.IsNotNull(optionParser.MainArgument);
            Assert.AreEqual("someargumentNotAnOption", optionParser.MainArgument);
        }

        [TestMethod]
        public void Parser_SmokeTest()
        {
            // ARRANGE
            var arguments = new[]
            {
                "--xxx",
                "--abc",
                "--xyz:500"
            };
            var option1 = new TestOption 
            {
                LongKey = "--xxx"
            };
            var option2 = new TestOption
            {
                LongKey = "--abc"
            };
            var option3 = new TestOptionWithParam
            {
                LongKey = "--xyz"
            };
            var knownOptions = new TestKnownOptions(option1, option2, option3);
            var optionParser = new OptionParser(knownOptions);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.AreEqual(3, optionParser.ParsedOptions.Length);
            Assert.AreEqual("500", option3.ParamValue);
        }
        
        [TestMethod]
        public void Parser_SmokeTest_LongKey()
        {
            // ARRANGE
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);
            var arguments = GetRequiredLongParams(knownOptions.All);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.AreEqual(knownOptions.All.Length, optionParser.ParsedOptions.Length);
        }

        [TestMethod]
        public void Parser_SmokeTest_ShortKey()
        {
            // ARRANGE
            var knownOptions = new KnownOptions();
            var optionParser = new OptionParser(knownOptions);
            var arguments = GetRequiredShortParams(knownOptions.All);

            // ACT
            optionParser.Parse(arguments);

            // ASSERT
            Assert.AreEqual(knownOptions.All.Length, optionParser.ParsedOptions.Length);
        }

        #region Helpers

        private string[] GetRequiredLongParams(IOption[] options)
        {
            var result = new List<string>();
            result.Add(@"C:\XXX");
            foreach (var item in options)
            {
                if (item is DaysSinceOption withParam)
                {
                    result.Add($"{item.LongKey}:999");
                }
                else if (item.LongKey != null)
                {
                    result.Add(item.LongKey);
                }
            }

            return result.ToArray();
        }

        private string[] GetRequiredShortParams(IOption[] options)
        {
            var result = new List<string>();
            result.Add(@"C:\XXX");
            foreach (var item in options)
            {
                if (item is DaysSinceOption withParam)
                {
                    result.Add($"{item.ShortKey}:999");
                }
                else if (item.ShortKey != null)
                {
                    result.Add(item.ShortKey);
                }
            }

            return result.ToArray();
        }

        #endregion Helpers
    }

    [TestClass]
    public class OptionExecutorTest
    {
        [TestMethod]
        public void Executor_ExecutAllOptions()
        {
            var executor = new OptionExecutor();
            var option1 = new TestOption();
            var option2 = new TestOption();
            var input = new[] { option1, option2 };

            executor.Execute(input, "someInput");

            Assert.IsTrue(input.All(x => x.IsExecuted));            
        }

        [TestMethod]
        public void Executor_ExecutsSingleExclusiveOption()
        {
            var executor = new OptionExecutor();
            var option1 = new TestOption();
            var option2 = new TestOption { IsExclusive = true };
            var option3 = new TestOption();
            var input = new[] { option1, option2, option3 };

            executor.Execute(input, "someInput");

            Assert.IsTrue(input.Count(x => x.IsExecuted) == 1);
        }

        [TestMethod]
        public void IntegrationTest_1()
        {
            var inputScenarios = new[]
            {
                new string[0],
                new string[] { "--help" },
                new string[] { "-h" },
                new string[] { "--version" },
                new string[] { "-v" },
                new string[] { "--help", "-v" },
            };

            foreach (var scenario in inputScenarios)
            {
                var knownOptions = new KnownOptions();
                var optionParser = new OptionParser(knownOptions);
                optionParser.Parse(scenario);
                var executor = new OptionExecutor();
                executor.Execute(optionParser.ParsedOptions, optionParser.MainArgument);
            }            
        }       
    }

    internal class TestOption : IOption
    {
        public string LongKey { get; set; }
        public string ShortKey { get; set; }
        public string Description { get; set; }
        public string UsageNote { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsExecuted { get; private set; }
        public object Execute(object input)
        {
            IsExecuted = true;
            return input;
        }
        public virtual bool TryBuild(string argument)
        {
            return argument == LongKey || argument == ShortKey;
        }
    }

    internal class TestOptionWithParam : TestOption
    {
        public string ParamValue { get; private set; }

        public override bool TryBuild(string argument)
        {
            var split = argument.Split(":");
            if (split.Length <= 1)
            {
                return false;
            }
            ParamValue = split[1];
            return base.TryBuild(split[0]);
        }
    }

    internal class TestKnownOptions : KnownOptions
    {
        private readonly IOption[] testOptions;

        public TestKnownOptions(params IOption[] testOptions)
        {
            this.testOptions = testOptions;
        }
        public override IOption[] All => this.testOptions;
    }
}
