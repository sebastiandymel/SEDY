using CommandLineSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class AttributeParserTests
    {
        [TestMethod]
        public void AttributeParser_EmptyArgs_NoProperties()
        {
            var parser = new AttributeParser();
            
            var result = parser.Parse<Empty>();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AttributeParser_MainOptionString()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_MainOption_String>("SomeMainArgument");

            Assert.IsNotNull(result);
            Assert.AreEqual("SomeMainArgument", result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_MainOptionInt()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_MainOption_Int>("51");

            Assert.IsNotNull(result);
            Assert.AreEqual(51, result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_MainOptionWithOtherProperties()
        {
            var parser = new AttributeParser();
            // For example 
            // C:\> myProgram 51 -o SomeOtherStuff
            var result = parser.Parse<TestClass_MainOptionWithOtherProps>("51", "-o", "SomeOtherStuff");

            Assert.IsNotNull(result);
            Assert.AreEqual(51, result.Arg1);
            Assert.AreEqual("SomeOtherStuff", result.SomeValue);
        }

        [TestMethod]
        public void AttributeParser_SingleStringProperty()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_String>("Arg1:MyValue");

            Assert.IsNotNull(result);
            Assert.AreEqual("MyValue", result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleStringPropertySpecialAlias()
        {
            var parser = new AttributeParser();
            // "/p:SomeAlias"
            var result = parser.Parse<TestClass_IntWithSpecialAlias>("/p:SomeAlias:123123");

            Assert.IsNotNull(result);
            Assert.AreEqual(123123, result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleBoolProperty()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_Bool>("Arg1");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleBoolPropertyAlliased()
        {
            var parser = new AttributeParser();

            Assert.IsTrue(parser.Parse<TestClass_BoolAlliased>("--somebool").Arg1);
            Assert.IsTrue(parser.Parse<TestClass_BoolAlliased>("--somebool1").Arg1);
            Assert.IsTrue(parser.Parse<TestClass_BoolAlliased>("XYZ").Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleBoolPropertyAlliased_WithExplicitValues()
        {
            var parser = new AttributeParser();

            Assert.IsTrue(parser.Parse<TestClass_BoolAlliased>("--somebool:true").Arg1);
            Assert.IsTrue(parser.Parse<TestClass_BoolAlliased>("--somebool1:true").Arg1);
            Assert.IsFalse(parser.Parse<TestClass_BoolAlliased>("XYZ:false").Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleStringPropertyAlliased()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_StringAlliased>("--str-value-xxx=MyValue");

            Assert.IsNotNull(result);
            Assert.AreEqual("MyValue", result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleStringPropertyWithCustomSplitter()
        {
            var parser = new AttributeParser();
            Assert.AreEqual("MyValue", parser.Parse<TestClass_StringPropertyWithCustomSplitter>("-val##MyValue").Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleStringPropertyAlliased_DoesNotRespectPropName()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_StringAlliased>("Arg1:MyValue");

            Assert.IsNotNull(result);
            Assert.AreNotEqual("MyValue", result.Arg1);
        }

        [TestMethod]
        public void AttributeParser_SingleRequiredStringProperty()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_StringRequired>("Arg1:MyValue");

            Assert.IsNotNull(result);
            Assert.AreEqual("MyValue", result.Arg1);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingOptionException))]
        public void AttributeParser_MissingRequiredProperty_ProduceException()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_StringRequired>();

        }

        [TestMethod]
        public void AttributeParser_RequiredIntProperty()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_IntRequired>("--someInt:560");

            Assert.AreEqual(560, result.IntArg);

        }

        [TestMethod]
        public void AttributeParser_RequiredDoubleProperty()
        {
            var parser = new AttributeParser();                       

            Assert.AreEqual(560.99, parser.Parse<TestClass_DoubleRequired>("--somedouble:560.99").DoubleProp);
            Assert.AreEqual(-5, parser.Parse<TestClass_DoubleRequired>("--somedouble:-5").DoubleProp);
            Assert.AreEqual(0, parser.Parse<TestClass_DoubleRequired>("--somedouble:0").DoubleProp);
        }

        [TestMethod]
        public void AttributeParser_RequiredCustomTypeProperty()
        {
            var parser = new AttributeParser();

            parser.RegisterCustomConverter<CustomType>(i =>
            {
                if (i == "mystringvalue")
                {
                    return new CustomType { SomeValue = 100 };
                }
                return null;
            });

            var result = parser.Parse<TestClass_CustomPropertyType>("--someprop:mystringvalue");

            Assert.AreEqual(result.MyProp.SomeValue, 100);

        }

        [TestMethod]
        public void AttributeParser_OutputPath()
        {
            var parser = new AttributeParser();

            parser.RegisterCustomConverter<FileInfo>(i =>
            {
                if (i == @"c:\SomePath\SubPath")
                {
                    return new FileInfo(i);
                }
                return null;
            });

            var result = parser.Parse<TestClass_Path>(@"-o", @"c:\SomePath\SubPath");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.OutputPath);
        }

        [TestMethod]
        [ExpectedException(typeof(OptionFormatException))]
        public void AttributeParser_RequiredCustomTypeProperty_WithoutCustomConverterThrowsException()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_CustomPropertyType>("--someprop:mystringvalue");

            Assert.AreEqual(result.MyProp.SomeValue, 100);

        }

        [TestMethod]
        [ExpectedException(typeof(OptionFormatException))]
        public void AttributeParser_RequiredIntProperty_WrongFormatException()
        {
            var parser = new AttributeParser();
            parser.Parse<TestClass_IntRequired>("--someInt:56xx0");
        }

        [TestMethod]
        public void AttributeParser_MultipleStringProperties()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_MultipleStrings>("Arg1:MyValue", "Arg3:Weird", "Arg2:SomeOtherValue");

            Assert.IsNotNull(result);
            Assert.AreEqual("MyValue", result.Arg1);
            Assert.AreEqual("SomeOtherValue", result.Arg2);
            Assert.AreEqual("Weird", result.Arg3);
        }

        [TestMethod]
        public void AttributeParser_InputOutput()
        {
            var parser = new AttributeParser();
            var result = parser.Parse<TestClass_InputOutput>("50", "200");

            Assert.AreEqual(50, result.Input);
            Assert.AreEqual(200, result.Output);
        }

        [TestMethod]
        public void AttributeParser_MultipleStringProperties_ButNoArgumentsMatching()
        {
            var parser = new AttributeParser();

            var result = parser.Parse<TestClass_MultipleStrings>("Arg5:MyValue", "Arg7:Weird", "Arg9:SomeOtherValue");

            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.Arg1));
            Assert.IsTrue(string.IsNullOrEmpty(result.Arg2));
            Assert.IsTrue(string.IsNullOrEmpty(result.Arg3));
        }

        #region Private test classes
        private class Empty
        {

        }

        private class TestClass_String
        {
            [Option]
            public string Arg1 { get; set; }
        }
        private class TestClass_IntWithSpecialAlias
        {
            [Option]
            [OptionAlias("/p:SomeAlias")]
            public int Arg1 { get; set; }
        }
        private class TestClass_MainOption_String
        {
            [MainInputAttribute]
            public string Arg1 { get; set; }
        }
        private class TestClass_MainOption_Int
        {
            [MainInputAttribute]
            public int Arg1 { get; set; }
        }
        private class TestClass_MainOptionWithOtherProps
        {
            [MainInputAttribute]
            public int Arg1 { get; set; }

            [Option]
            [OptionAlias("-o", " ")]
            public string SomeValue { get; set; }
        }
        private class TestClass_StringPropertyWithCustomSplitter
        {
            [Option]
            [OptionAlias("-val", "##")]
            public string Arg1 { get; set; }
        }
        private class TestClass_Bool
        {
            [Option]
            public bool Arg1 { get; set; }
        }
        private class TestClass_BoolAlliased
        {
            [Option]
            [OptionAlias("--somebool")]
            [OptionAlias("--somebool2")]
            [OptionAlias("XYZ")]
            public bool Arg1 { get; set; }
        }
        private class TestClass_StringAlliased
        {
            [Option]
            [OptionAlias("--str-value-xxx", "=")]
            public string Arg1 { get; set; }
        }
        private class TestClass_StringRequired
        {
            [Option(isRequired: true)]
            public string Arg1 { get; set; }
        }
        private class TestClass_MultipleStrings
        {
            [Option]
            public string Arg1 { get; set; }

            [Option]
            public string Arg2 { get; set; }
            
            [Option]
            public string Arg3 { get; set; }
        }

        private class TestClass_IntRequired
        {
            [Option(isRequired: true)]
            [OptionAlias("--someInt")]
            public int IntArg { get; set; }
        }

        private class TestClass_Path
        {
            [Option(isRequired: true)]
            [OptionAlias("-o", " ")]
            public FileInfo OutputPath { get; set; }
        }

        private class TestClass_DoubleRequired
        {
            [Option(isRequired: true)]
            [OptionAlias("--somedouble")]
            public double DoubleProp { get; set; }
        }

        private class TestClass_CustomPropertyType
        {
            [Option(isRequired: true)]
            [OptionAlias("--someprop")]
            public CustomType MyProp { get; set; }
        }

        private class TestClass_InputOutput
        {
            [MainInputAttribute]
            public int Input { get; set; }

            [MainOutputAttribute]
            public int Output { get; set; }
        }

        private class CustomType
        {
            public int SomeValue { get; set; }
        }       

        #endregion Private test classes
    }
}
