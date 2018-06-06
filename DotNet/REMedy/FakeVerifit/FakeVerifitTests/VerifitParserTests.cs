using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeVerifit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakeVerifitTests
{
    [TestClass()]
    public class VerifitParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParseSetClient_ThrowsException_When_BadInput()
        {
            VerifitParser parser = new VerifitParser();

            var input =
                "foobar";
            parser.ParseSetClientCommand(input);
        }

        [TestMethod]
        public void ParseSetClient_Get_Mode_Rem()
        {
            VerifitParser parser = new VerifitParser();

            var input =
                "setClient speechmap rem -target dsladult -languagetype nontonal  -transducer headphone      -age 96  -lefthl { _ _ _ _ _ _ _ _ _ _ _ _ } -leftbc { _ _ _ _ _ _ _ _ _ _ _ _ } -leftucl { _ _ _ _ _ _ _ _ _ _ _ _ }  -righthl { _ _ _ _ _ _ _ _ _ _ _ _ } -rightbc { _ _ _ _ _ _ _ _ _ _ _ _ } -rightucl { _ _ _ _ _ _ _ _ _ _ _ _ }";
            var data = parser.ParseSetClientCommand(input);

            Assert.AreEqual("rem", data.Mode);
        }


        [TestMethod]
        public void ParseSetClient_Get_Mode_Srem()
        {
            VerifitParser parser = new VerifitParser();

            var input =
                "setClient speechmap srem -target dsladult -languagetype nontonal  -transducer headphone      -age 96  -lefthl { _ _ _ _ _ _ _ _ _ _ _ _ } -leftbc { _ _ _ _ _ _ _ _ _ _ _ _ } -leftucl { _ _ _ _ _ _ _ _ _ _ _ _ }  -righthl { _ _ _ _ _ _ _ _ _ _ _ _ } -rightbc { _ _ _ _ _ _ _ _ _ _ _ _ } -rightucl { _ _ _ _ _ _ _ _ _ _ _ _ }";
            var data = parser.ParseSetClientCommand(input);

            Assert.AreEqual("srem", data.Mode);
        }


        [TestMethod]
        public void ParseSetClient_Get_Parameters()
        {
            VerifitParser parser = new VerifitParser();

            var input =
                "setClient speechmap srem -target dsladult -languagetype nontonal  -transducer headphone      -age 96  -lefthl { _ _ _ _ _ _ _ _ _ _ _ _ } -leftbc { _ _ _ _ _ _ _ _ _ _ _ _ } -leftucl { _ _ _ _ _ _ _ _ _ _ _ _ }  -righthl { _ _ _ _ _ _ _ _ _ _ _ _ } -rightbc { _ _ _ _ _ _ _ _ _ _ _ _ } -rightucl { _ _ _ _ _ _ _ _ _ _ _ _ }";
            var data = parser.ParseSetClientCommand(input);

            TestParameter(data, "-target", "dsladult");
            TestParameter(data, "-languagetype", "nontonal");
            TestParameter(data, "-transducer", "headphone");
            TestParameter(data, "-age", "96");
            TestParameter(data, "-lefthl", "{ _ _ _ _ _ _ _ _ _ _ _ _ }");
        }

        [TestMethod]
        public void ParseSetClient_Get_Parmeters_With_Values()
        {
            VerifitParser parser = new VerifitParser();
            var input =
                "setClient speechmap rem -target dsladult -languagetype nontonal  -transducer headphone      -age 207  -lefthl { _ 60.0 60.0 60.0 60.0 65.0 70.0 65.0 _ 70.0 _ _ } -leftbc { 80.0 75.0 75.0 _ 75.0 75.0 75.0 75.0 _ 80.0 _ _ } -leftucl { _ 115.0 _ _ _ 115.0 _ 110.0 _ 120.0 _ _ }  -righthl { _ 60.0 60.0 60.0 60.0 65.0 70.0 65.0 _ 70.0 _ _ } -rightbc { 80.0 75.0 75.0 _ 75.0 75.0 75.0 75.0 _ 80.0 _ _ } -rightucl { _ 115.0 _ _ _ 115.0 _ 110.0 _ 120.0 _ _ }";
            var data = parser.ParseSetClientCommand(input);

            TestParameter(data, "-age", "207");
            TestParameter(data, "-lefthl", "{ _ 60.0 60.0 60.0 60.0 65.0 70.0 65.0 _ 70.0 _ _ }");
            TestParameter(data, "-leftbc", "{ 80.0 75.0 75.0 _ 75.0 75.0 75.0 75.0 _ 80.0 _ _ }");
            TestParameter(data, "-leftucl", "{ _ 115.0 _ _ _ 115.0 _ 110.0 _ 120.0 _ _ }");
            TestParameter(data, "-righthl", "{ _ 60.0 60.0 60.0 60.0 65.0 70.0 65.0 _ 70.0 _ _ }");
            TestParameter(data, "-rightbc", "{ 80.0 75.0 75.0 _ 75.0 75.0 75.0 75.0 _ 80.0 _ _ }");
            TestParameter(data, "-rightucl", "{ _ 115.0 _ _ _ 115.0 _ 110.0 _ 120.0 _ _ }");
        }

        [TestMethod]
        public void ParseSetClient_Get_Parmeters_With_Same_Key_Twice()
        {
            VerifitParser parser = new VerifitParser();
            var input =
                "setClient speechmap rem -target dsladult -target dslchild";
            var data = parser.ParseSetClientCommand(input);

            TestParameter(data, "-target", "dslchild");
        }

        private void TestParameter(SetClientData data, string key, string value)
        {
            Assert.IsTrue(data.Parameters.ContainsKey(key));
            Assert.AreEqual(value, data.Parameters[key]);
        }

        [TestMethod]
        public void ParseSetTest_GetAllValues()
        {
            VerifitParser parser = new VerifitParser();
            const string input = "setTest speechmap srem right 2 speech-wbcarrots avg65";
            var data = parser.ParseSetTestCommand(input);

            Assert.AreEqual("srem", data.Mode);
            Assert.AreEqual(Side.Right, data.Side);
            Assert.AreEqual("2", data.SlotNumber);
            Assert.AreEqual("speech-wbcarrots", data.StimulusName);
            Assert.AreEqual("avg65", data.Level);
        }

        [TestMethod]
        public void ParseIsCalibrated_TestBox_Right()
        {
            VerifitParser parser = new VerifitParser();
            const string input = "calibrated srem right";
            var data = parser.ParseIsCalibratedCommand(input);

            Assert.AreEqual("srem", data.Mode);
            Assert.AreEqual(Side.Right, data.Side);
        }

        [TestMethod]
        public void ParseIsCalibrated_OnEar_Left()
        {
            VerifitParser parser = new VerifitParser();
            const string input = "calibrated rem left";
            var data = parser.ParseIsCalibratedCommand(input);

            Assert.AreEqual("rem", data.Mode);
            Assert.AreEqual(Side.Left, data.Side);
        }
    }
}
