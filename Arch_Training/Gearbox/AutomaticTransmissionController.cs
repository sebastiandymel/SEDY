using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gearbox
{
    public class AutomaticTransmissionController
    {
        private readonly IGearCalculator gearCalculator;
        private readonly IEngineMonitoringSystem monitoring;
        private readonly IAutomaticGearBox gearbox;

        public AutomaticTransmissionController(IGearCalculator gearCalculator, IEngineMonitoringSystem monitoring, IAutomaticGearBox gearbox)
        {
            this.gearCalculator = gearCalculator;
            this.monitoring = monitoring;
            this.gearbox = gearbox;
        }

        public void Start()
        {
            if (this.monitoring.IsON() == 0)
            {
                this.gearbox.Drive();
            }
        }

        public void Stop()
        {
            if (this.monitoring.IsON() == 1)
            {
                this.gearbox.Gear = 1;
                this.gearbox.Park();
            }
        }

        /// <summary>
        /// Funkcja wołana co 10ms w przypadku jak wciśnięty jest pedał gazu
        /// </summary>
        /// <param name="percentge">Procent ile pedał gazu jest wciśnięty. Zakres 0-1</param>
        public void HandleGas(double percentge)
        {
            if (this.monitoring.IsON() == 0)
            {
                return;
            }

            this.gearbox.Gear = this.gearCalculator.CalculateGear(
                this.monitoring.GetCurrenyRPM(),
                this.gearbox.Gear);
        }

        /// <summary>
        /// Funkcja wołana co 10ms w przypadku jak wciśnięty jest hamulec
        /// </summary>
        /// <param name="percentge">Procent ile hamulece jest wciśnięty. Zakres 0-1</param>
        public void HandleBreaks(double percentage)
        {
            if (this.monitoring.IsON() == 1)
            {
                if (this.monitoring.GetCurrenyRPM() < 1000 && this.gearbox.Gear > 1)
                {
                    this.gearbox.Gear--;
                }
            }
        }

    }
}
