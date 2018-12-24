using System.Collections.Generic;

namespace MvcMusicStore.Domain.Entities
{
    public class User
    {
        public IList<Cart> Cart { get; set; }
    }
}