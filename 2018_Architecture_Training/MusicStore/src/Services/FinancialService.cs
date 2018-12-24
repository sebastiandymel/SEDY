using System;

namespace MvcMusicStore.Services
{
    public class FinancialService
    {
        private decimal _lastExchangeRate = 1;

        public decimal ToDolar(decimal amount, string currenyCode) {
            var risc = CalculateRisc();
            return amount * risc;
        }

        internal static decimal CalculateRisc() {
            var risc = 1.0m;
            if (DateTime.Now.Hour > 17 || DateTime.Now.Hour < 7) {
                risc += 0.3m;
            }
            risc += ((decimal)DateTime.Now.TimeOfDay.Minutes) / 180;//wymiania jest tylko o pełnych godzinach. Nie wiemy co się dzieje pomiędzy, więc podbijamy ryzyko
            return risc;
        }
    }
}