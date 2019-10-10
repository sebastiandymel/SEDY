using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketContracts;
using Client.Messages;
using MassTransit;

namespace Client
{
    internal class BasketService
    {
        private readonly object _consoleLock = new object();
        private readonly IBusControl _busControl;
        private Random _r = new Random();
        private IDictionary<string,int> _basket = new ConcurrentDictionary<string,int>();

        public BasketService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        /// <summary>
        /// Infrastructure for interacting with the console.
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            var userName = new SiRandomNameGeneratorNETStandard.PersonNameGenerator().GetRandomNameAndLastName();
            WriteLineInColor($"Your user name is:{userName}",ConsoleColor.Yellow);
            WriteInstructions();

            while (true)
            {
                var option = Console.ReadLine();
                var itemName = System.Text.NamesGenerator.RandomName();
                var quantity = _r.Next(10);
                switch (option)
                {
                    case "0":
                        WriteLineInColor($"Adding item:'{itemName}', quantity:{quantity}",ConsoleColor.DarkGreen);
                        if (_basket.ContainsKey(itemName))
                            _basket[itemName] += quantity;
                        else
                            _basket.Add(itemName, quantity);
                        await OnItemAdded(userName, itemName, quantity);
                        break;
                    case "1":
                        if (_basket.Keys.Count == 0)
                        {
                            Console.WriteLine("Nothing to remove.");
                            break;
                        }
                        itemName = _basket.Keys.ToArray()[_r.Next(0, _basket.Keys.Count - 1)];
                        quantity = _r.Next(0, _basket[itemName]);
                        WriteLineInColor($"Adding item:'{itemName}', quantity:{quantity}",ConsoleColor.Blue);
                        await OnItemRemoved(userName, itemName, quantity);
                        break;
                    case "2":
                        await OnBasketSubmited(userName);
                        break;
                    default:
                        WriteInstructions();
                        break;
                }
            }
        }

        private async Task OnBasketSubmited(string userName)
        {
            await _busControl.Publish<IBasketSubmited>(new BasketSubmited(userName));
        }

        private async Task OnItemRemoved(string userName, string itemName, int quantity)
        {
            await _busControl.Publish<IItemRemoved>(new ItemRemoved(userName, itemName, quantity));
        }

        private async Task OnItemAdded(string userName, string itemName, int quantity)
        {
            await _busControl.Publish<IItemAdded>(new ItemAdded(userName, itemName, quantity));
        }

        public void WriteInstructions()
        {
            WriteLineInColor("Options:",ConsoleColor.Green);
            WriteLineInColor("0 - Add item", ConsoleColor.Green);
            WriteLineInColor("1 - Remove item", ConsoleColor.Green);
            WriteLineInColor("2 - Submit basket", ConsoleColor.Green);
        }

        public void WriteLineInColor(string msg, ConsoleColor color)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}