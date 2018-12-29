using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcMusicStore.Models;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using MvcMusicStore.ROP;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private const string PromoCode = "FREE";

        private readonly MusicStoreEntities _storeContext = new MusicStoreEntities();

        // GET: /Checkout/
        public ActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            if (ModelState.IsValid)
            {
                order.Username = User.Identity.Name;
                order.OrderDate = DateTime.Now;

                _storeContext.Orders.Add(order);

                await ShoppingCart.GetCart(_storeContext, HttpContext.User.Identity.Name).CreateOrder(order);

                await _storeContext.SaveChangesAsync();

                return RedirectToAction("Complete", new {id = order.OrderId});
            }

            return View(order);
        }

        private Result<Order, bool> UpdateOrder(Order order)
        {

        }


        // GET: /Checkout/Complete
        public async Task<ActionResult> Complete(int id)
        {
            return GetOrder(id).Map(ToDetails).Map(ToView).Success;
        }

        private ActionResult ToView(CheckoutDetailsViewModel vm)
        {
            return View(vm);
        }

        private Result<Order, string> GetOrder(int id)
        {
            var o = _storeContext
                .Orders
                .Where(a => a.OrderId == id)
                .Include(a => a.OrderDetails)
                .Include(a => a.OrderDetails.Select(b => b.Album))
                .FirstOrDefault();
            if (o == null)
            {
                return Result<Order, string>.Failed("Failed failed failed");
            }
            return Result<Order, string>.Succeded(o);
        }

        private CheckoutDetailsViewModel ToDetails(Order o)
        {
            var vm = new CheckoutDetailsViewModel
            {
                Username = o.Username,
                OrderDetails = o.OrderDetails.Select(a => new CheckoutOrderDetailVM
                {
                    AlbumName = a.Album.Title,
                    Quantity = a.Quantity,
                    UnitPrice = a.UnitPrice
                }).ToList(),
                OrderId = o.OrderId,
                Address = o.Address,
                City = o.City,
                Country = o.Country,
                Email = o.Email,
                FirstName = o.FirstName,
                LastName = o.LastName,
                OrderDate = o.OrderDate,
                Phone = o.Phone,
                PostalCode = o.PostalCode,
                State = o.State,
                Total = o.Total
            };
            return vm;
        }
    }
}