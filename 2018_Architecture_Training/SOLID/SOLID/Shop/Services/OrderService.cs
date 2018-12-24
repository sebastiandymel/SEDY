using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using SOLID.Shop.Model;
using SOLID.Shop.Utility;

namespace SOLID.Shop.Services
{
    public class OrderService
    {
        private readonly PaymentServiceFactory factory;
        private readonly IEmailNotificationService emailNotificationService;
        private readonly IInventory inventory;

        public OrderService(
            PaymentServiceFactory factory,
            IEmailNotificationService emailNotificationService, 
            IInventory inventory)
        {
            this.factory = factory;
            this.emailNotificationService = emailNotificationService;
            this.inventory = inventory;
        }

        public void Checkout(Order order, PaymentDetails paymentDetails, bool notifyCustomer)
        {
            ChargeCustomer(paymentDetails, order);
            ReserveItemInInventory(order);
            if (notifyCustomer)
            {
                NotifyCustomerAboutSuccessfullOrder(order);
            }
        }

        private void NotifyCustomerAboutSuccessfullOrder(Order order)
        {
            this.emailNotificationService.NotifyCustomer(
                order.CustomerEmail, 
                new OrderEmailMessage(order));
        }

        private void ReserveItemInInventory(Order order)
        {
            foreach (var orderItem in order.Items)
            {
                this.inventory.Reserve(orderItem.Item.EAN, orderItem.Quantity);
            }
        }

        private void ChargeCustomer(PaymentDetails paymentDetails, Order order)
        {
            var paymentService = this.factory.Create(paymentDetails.PaymentMethod);
            paymentService.Charge(paymentDetails, order);
        }
    }

    public interface IInventory
    {
        void Reserve(string item, int quantity);
    }

    public class DefaultInventory : IInventory
    {
        private InventorySystem inventorySystem;

        public DefaultInventory()
        {
            this.inventorySystem = new InventorySystem();
        }
        public void Reserve(string item, int quantity)
        {
            inventorySystem.Reserve(item, quantity);
        }
    }

    public interface IPaymentService
    {
        PaymentMethod Method { get; }
        void Charge(PaymentDetails details, Order order);
    }

    public class PaymentServiceFactory
    {
        private readonly Dictionary<PaymentMethod, IPaymentService> services;

        public PaymentServiceFactory(IPaymentService[] newServices)
        {
            if (newServices == null || newServices.Length == 0)
            {
                throw new ArgumentException("No services to add!");
            }
            if (newServices.GroupBy(x => x.Method).Any(i => i.Count() > 1))
            {
                throw new ArgumentException("Multiple payment services for the same method");
            }
            this.services = newServices.ToDictionary(x => x.Method, y => y);
        }

        public IPaymentService Create(PaymentMethod method)
        {
            if (this.services.ContainsKey(method))
            {
                return this.services[method];
            }
            throw new KeyNotFoundException("SADSSDAS");
        }
    }

    public class BTCPaymentService : IPaymentService
    {
        public PaymentMethod Method => PaymentMethod.BTC;
        public void Charge(PaymentDetails details, Order order)
        {
            throw new NotImplementedException();
        }
    }

    public class CreditCartPaymentService : IPaymentService
    {
        public PaymentMethod Method => PaymentMethod.CreditCard;
        public void Charge(PaymentDetails paymentDetails, Order order)
        {
            using (var paymentGateway = new PaymentGateway())
            {
                try
                {
                    paymentGateway.Credentials = "account credentials";
                    paymentGateway.CardNumber = paymentDetails.CreditCardNumber;
                    paymentGateway.ExpiresMonth = paymentDetails.ExpiresMonth;
                    paymentGateway.ExpiresYear = paymentDetails.ExpiresYear;
                    paymentGateway.NameOnCard = paymentDetails.CardholderName;
                    paymentGateway.AmountToCharge = order.Items.Sum(a => a.Item.Price * a.Quantity);

                    paymentGateway.Charge();
                }
                catch (AvsMismatchException ex)
                {
                    throw new OrderException("The card gateway rejected the card based on the address provided.", ex);
                }
                catch (Exception ex)
                {
                    throw new OrderException("There was a problem with your card.", ex);
                }
            }
        }
    }

    public interface IEmailNotificationService
    {
        void NotifyCustomer(string customerEmail, IMessage messageData);
    }

    public class EmailEmailNotificationService : IEmailNotificationService
    {
        public void NotifyCustomer(string customerEmail, IMessage messageData)
        {
            if (!string.IsNullOrEmpty(customerEmail))
                using (var message = new MailMessage("orders@somewhere.com", customerEmail))
                using (var client = new SmtpClient("localhost"))
                {
                    message.Subject = messageData.Subject;
                    message.Body = message.Body;

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Problem sending notification email", ex);
                    }
                }
        }
    }

    public interface IMessage
    {
        string Subject { get; }
        string Body { get; }
    }

    public class OrderEmailMessage : IMessage
    {
        private readonly Order order;

        public OrderEmailMessage(Order order)
        {
            this.order = order;
            Subject= "Your order placed on " + DateTime.Now;
            Body = "Your order details: \n " + order;
        }

        public string Subject { get; }
        public string Body { get; }
    }
}