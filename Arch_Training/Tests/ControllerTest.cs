using System;
using Gearbox;
using NUnit.Framework;

namespace Tests
{
    public class ControllerTest
    {
        [Test]
        public void NewGear_IncreasGear()
        {
            //var gear = AutomaticTransmissionController.CalculateGear(2500, 3);
            
            //Assert.AreEqual(4, gear);
        }

        //[Test]
        //public void HandleGas_IncreasGear()
        //{
        //    var gearbox = new AutomaticGearBoxStub
        //    {
        //        Gear = 1
        //    };
        //    var monitoring = new EngineMonitoringStub(5000);
        //    var sut = new AutomaticTransmissionController(monitoring, gearbox);

        //    sut.HandleGas(50);

        //    Assert.AreEqual(2, gearbox.Gear);
        //}

        //[Test]
        //public void HandleGas_5thGearIsMax()
        //{
        //    var gearbox = new AutomaticGearBoxStub
        //    {
        //        Gear = 5
        //    };
        //    var monitoring = new EngineMonitoringStub(5000);
        //    var sut = new AutomaticTransmissionController(monitoring, gearbox);

        //    sut.HandleGas(50);

        //    Assert.AreEqual(5, gearbox.Gear);
        //}

        private class AutomaticGearBoxStub: IAutomaticGearBox
        {
            public int Gear { get; set; }
            public int GetMaxGears()
            {
                throw new NotImplementedException();
            }

            public void Drive()
            {
                throw new NotImplementedException();
            }

            public void Park()
            {
                throw new NotImplementedException();
            }

            public void Neutral()
            {
                throw new NotImplementedException();
            }

            public void Reverse()
            {
                throw new NotImplementedException();
            }
        }

        private class EngineMonitoringStub : IEngineMonitoringSystem
        {
            private int rpm;

            public EngineMonitoringStub(int rpm)
            {
                
            }
            public int GetCurrenyRPM()
            {
                return this.rpm;
            }

            public int IsON()
            {
                return 1;
            }
        }
    }
}
