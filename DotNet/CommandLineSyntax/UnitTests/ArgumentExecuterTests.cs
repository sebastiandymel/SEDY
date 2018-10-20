using CommandLineSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ArgumentExecuterTests
    {
        [TestMethod]
        public void ArgumentExecuter_CanExecuteParameterlessMethods()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_ParameterlessMethods>("MyMethod1", "--someOtherMethod");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Method1Executed);
            Assert.IsTrue(result.Method2Executed);
        
            var targetInstance = new TestClass_ParameterlessMethods();
            executer.Execute(targetInstance, "MyMethod1", "--someOtherMethod");
            Assert.IsTrue(targetInstance.Method1Executed);
            Assert.IsTrue(targetInstance.Method2Executed);
        }

        [TestMethod]
        public void ArgumentExecuter_CanExecuteParameterlessMethods2()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_ParameterlessMethods>("--someOtherMethod");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Method1Executed);
            Assert.IsTrue(result.Method2Executed);

            var targetInstance = new TestClass_ParameterlessMethods();
            executer.Execute(targetInstance, "--someOtherMethod");
            Assert.IsFalse(targetInstance.Method1Executed);
            Assert.IsTrue(targetInstance.Method2Executed);
        }

        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodsWithInput()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_MethodsWithParameters>("--v:5");
            Assert.IsNotNull(result);
            Assert.AreEqual(50, result.Value);
        }

        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodsWithInputAndConverter()
        {
            var executer = new ArgumentExecutor();
            executer.RegisterCustomConverter<MyCustomType>(x => 
            {
                if (x == "magicValue")
                {
                    return new MyCustomType { Id = "Works!" };
                }
                return null;
            });
            var result = executer.Execute<TestClass_MethodsWithParameterAndCustomConversionNeeded>("--v=magicValue");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("Works!", result.Value.Id);
        }

        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodWithInputOutput()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_InputOutput>("5", "99.9");

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.SomeInput);
            Assert.AreEqual(99.9, result.SomeOutput);
        }
        
        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodWithInput()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_Input>("99.9");

            Assert.IsNotNull(result);
            Assert.AreEqual(99.9, result.SomeInput);
        }


        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodWithInputAndOther()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_InputWithAdditionalOptions>("99.9", "-h");

            Assert.IsNotNull(result);
            Assert.AreEqual(99.9, result.SomeInput);
            Assert.IsTrue(result.HelpExecuted);
        }

        [TestMethod]
        public void ArgumentExecuter_CanExecuteMethodWithOutputAndOther()
        {
            var executer = new ArgumentExecutor();
            var result = executer.Execute<TestClass_OutputWithAdditionalOptions>("-h", "55");

            Assert.IsNotNull(result);
            Assert.AreEqual(55, result.SomeOutput);
            Assert.IsTrue(result.HelpExecuted);

            var targetInstance = new TestClass_OutputWithAdditionalOptions();
            executer.Execute(targetInstance, "-h", "55");
            Assert.AreEqual(55, targetInstance.SomeOutput);
            Assert.IsTrue(targetInstance.HelpExecuted);
        }

        #region Private test classes

        private class TestClass_ParameterlessMethods
        {
            public bool Method1Executed { get; set; }

            [Option]
            public void MyMethod1()
            {
                Method1Executed = true;
            }

            public bool Method2Executed { get; set; }

            [Option]
            [OptionAlias("--someOtherMethod")]
            public void MyMethod2()
            {
                Method2Executed = true;
            }
        }

        private class TestClass_MethodsWithParameters
        {
            public int Value { get; set; }

            [Option]
            [OptionAlias("--v")]
            public void ChangeValue(int input)
            {
                Value = input * 10;
            }
        }

        private class TestClass_MethodsWithParameterAndCustomConversionNeeded
        {
            public MyCustomType Value { get; set; }

            [Option]
            [OptionAlias("--v", "=")]
            public void ChangeValue(MyCustomType input)
            {
                Value = input;
            }
        }

        private class MyCustomType
        {
             public string Id { get; set; }
        }

        private class TestClass_InputOutput
        {    
            public int SomeInput { get; private set; }
            public double SomeOutput { get; private set; }

            public void MyMethod1([MainInputAttribute] int myInput, [MainOutputAttribute] double myOutput)
            {
                SomeInput = myInput;
                SomeOutput = myOutput;
            }
        }

        private class TestClass_Input
        {
            public double SomeInput { get; private set; }

            public void MyMethod1([MainInputAttribute] double myInput)
            {
                SomeInput = myInput;
            }
        }


        private class TestClass_InputWithAdditionalOptions
        {
            public double SomeInput { get; private set; }
            public bool HelpExecuted { get; private set; }

            public void MyMethod1([MainInputAttribute] double myInput)
            {
                SomeInput = myInput;
            }

            [Option]
            [OptionAlias("-h")]
            public void Help()
            {
                HelpExecuted = true;
            }
        }

        private class TestClass_OutputWithAdditionalOptions
        {
            public double SomeOutput { get; private set; }
            public bool HelpExecuted { get; private set; }

            public void MyMethod1([MainOutputAttribute] double myInput)
            {
                SomeOutput = myInput;
            }

            [Option]
            [OptionAlias("-h")]
            public void Help()
            {
                HelpExecuted = true;
            }
        }

        #endregion Private test classes
    }
}
