﻿using CommandLineSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class POSIXStandardTests
    {
        [TestMethod]
        public void Posix_MultipleProperties()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-abc");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsTrue(result.OptionB);
            Assert.IsTrue(result.OptionC);
        }

        [TestMethod]
        public void Posix_MultiplePropertiesAndMainOption()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptionsWithMainArg>("ThisIsMainParameter","-ab","-c");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsTrue(result.OptionB);
            Assert.IsTrue(result.OptionC);
            Assert.AreEqual("ThisIsMainParameter", result.MainOption);
        }

        [TestMethod]
        public void Posix_WithParam()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_WitParams>("-a590");

            Assert.IsNotNull(result);
            Assert.AreEqual(590, result.OptionA);
        }

        [TestMethod]
        public void Posix_SingleDouble()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_SingleDouble>("-x", "44.89");

            Assert.IsNotNull(result);
            Assert.AreEqual(44.89, result.ValueX);
        }

        [TestMethod]
        public void Posix_WithParam_Alternative()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_WitParams>("-a 590");

            Assert.IsNotNull(result);
            Assert.AreEqual(590, result.OptionA);
        }

        [TestMethod]
        public void Posix_WithParam_Alternative3()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_WitParams>("-a", "590");

            Assert.IsNotNull(result);
            Assert.AreEqual(590, result.OptionA);
        }

        [TestMethod]
        public void Posix_MultipleProperties_NoMatches()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-x");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.OptionA);
            Assert.IsFalse(result.OptionB);
            Assert.IsFalse(result.OptionC);
        }

        [TestMethod]
        public void Posix_MultipleProperties2()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-ac");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsFalse(result.OptionB);
            Assert.IsTrue(result.OptionC);
        }
        
        [TestMethod]
        public void Posix_MultipleProperties3()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-a");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsFalse(result.OptionB);
            Assert.IsFalse(result.OptionC);
        }

        [TestMethod]
        public void Posix_MultipleProperties_AlternativeSyntax()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-a -b -c");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsTrue(result.OptionB);
            Assert.IsTrue(result.OptionC);
        }

        [TestMethod]
        public void Posix_MultipleProperties_AlternativeSyntax2()
        {
            var posixParser = new POSIXParser();

            var result = posixParser.Parse<TestClass_MultipleOptions>("-a");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptionA);
            Assert.IsFalse(result.OptionB);
            Assert.IsFalse(result.OptionC);
        }



        private class TestClass_MultipleOptions
        {
            [Option]
            [POSIX_Alias('a')]
            public bool OptionA { get; set; }

            [Option]
            [POSIX_Alias('b')]
            public bool OptionB { get; set; }

            [Option]
            [POSIX_Alias('c')]
            public bool OptionC { get; set; }
        }

        private class TestClass_MultipleOptionsWithMainArg
        {
            [Option]
            [POSIX_Alias('a')]
            public bool OptionA { get; set; }

            [Option]
            [POSIX_Alias('b')]
            public bool OptionB { get; set; }

            [Option]
            [POSIX_Alias('c')]
            public bool OptionC { get; set; }

            [MainOption]
            public string MainOption { get; set; }
        }

        private class TestClass_WitParams
        {
            [Option]
            [POSIX_Alias('a')]
            public int OptionA { get; set; }
        }

        private class TestClass_SingleDouble
        {
            [Option]
            [POSIX_Alias('x')]
            public double ValueX { get; set; }
        }
    }
}
