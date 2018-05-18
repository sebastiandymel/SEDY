using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // SPEECH MAP
            var speechMapConfigurator = new SpeechMapConfigurator();

            var configurable = new IConfigurable[]
            {
                new AutofitStep1(),
                new AutofitStep2(),
                new AutofitStep3(),
                new AutofitStep4()
            };

            foreach (var item in configurable)
            {
                item.Accept(speechMapConfigurator);
            }
        }
    }

    /// <summary>
    /// Each AutoFit step can implement configurable to be able to accept a configurator.
    /// </summary>
    public interface IConfigurable
    {
        void Accept(IConfigurationVisitor visitor);
    }

    /// <summary>
    /// Visits each AutoFit step and configures it appropriately
    /// </summary>
    public interface IConfigurationVisitor
    {
        void Visit(AutofitStep1 step1);
        void Visit(AutofitStep2 step1);
        void Visit(AutofitStep3 step1);
        void Visit(AutofitStep4 step1);
    }

    /// <summary>
    /// Base configurator
    /// </summary>
    public class ConfigurationVisitor : IConfigurationVisitor
    {
        public virtual void Visit(AutofitStep1 step1)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(AutofitStep2 step2)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(AutofitStep3 step3)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(AutofitStep4 step4)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Configures autofit for insertion gain measurement
    /// </summary>
    public class InsertionGainConfigurator : ConfigurationVisitor { }

    /// <summary>
    /// Configures autofit for speech map measurement
    /// </summary>
    public class SpeechMapConfigurator : ConfigurationVisitor { }

    public class AutofitStep1 : IConfigurable
    {
        public void Accept(IConfigurationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class AutofitStep2 : IConfigurable
    {
        public void Accept(IConfigurationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class AutofitStep3 : IConfigurable
    {
        public void Accept(IConfigurationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class AutofitStep4 : IConfigurable
    {
        public void Accept(IConfigurationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
