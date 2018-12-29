using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MvcMusicStore.Models;
using MvcMusicStore.Models.Entities;

namespace MvcMusicStore.Services
{
    public class OrderService
    {
        public Task<List<OrderDetail>> GetNotOrderedCDs(MusicStoreEntities _storeContext)
        {
            return _storeContext
                .OrderDetails
                .Where(a => a.ShipmentId == null && a.Order.Status == OrderStatus.New)
                .ToListAsync();
        }
    }
}