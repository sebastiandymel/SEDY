using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ORM.Pluming
{
    public static class ReflectionUtils
    {
        public static bool Has<TAttribute>(this MemberInfo @this, bool inherit = true)
            where TAttribute : class
        {
            return @this.GetCustomAttributes(typeof(TAttribute), inherit).Any();
        }
        public static IEnumerable<TAttribute> Get<TAttribute>(this MemberInfo @this)
            where TAttribute : class
        {
            return @this.GetCustomAttributes(typeof(TAttribute), true).Select(a => (TAttribute)a);
        }
    }
}