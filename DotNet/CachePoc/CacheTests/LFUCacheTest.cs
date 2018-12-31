using CachePoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheTests
{
    [TestClass]
    public class LFUCacheTest
    {
        [TestMethod]
        public void LFU_CanAdd()
        {
            var cache = new LFUCache<object>(2);

            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.AreEqual(2, cache.Count);
        }

        [TestMethod]
        public void LFU_CanAdd_PromoteToCache()
        {
            var cache = new LFUCache<object>(2);

            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.IsFalse(cache.TryAdd("SomeKey3", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
                        
            Assert.AreEqual(2, cache.Count);
        }

        [TestMethod]
        public void LFU_CanGet()
        {
            var cache = new LFUCache<object>(2);

            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.IsTrue(cache.TryGet("SomeKey", out var val1));
            Assert.IsTrue(cache.TryGet("SomeKey2", out var val2));
            Assert.IsFalse(cache.TryGet("SomeOtherKey", out var val3));
        }

        [TestMethod]
        public void LFU_CanAdd_NoMoreThanSize()
        {
            var cache = new LFUCache<object>(2);

            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.IsFalse(cache.TryAdd("SomeKey3", new object()));
            Assert.IsFalse(cache.TryAdd("SomeKey4", new object()));
            Assert.IsFalse(cache.TryGet("SomeKey4", out var val));
            Assert.AreEqual(2, cache.Count);
        }

        [TestMethod]
        public void LFU_WhenPromoted_OtherKeysRemovedFromCache()
        {
            var cache = new LFUCache<object>(2);

            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey", new object()));
            Assert.IsTrue(cache.TryAdd("SomeKey2", new object()));
            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGet("SomeKey", out var val1));
            Assert.IsTrue(cache.TryGet("SomeKey2", out var val2));

            Assert.IsFalse(cache.TryAdd("SomeOther", new object()));
            Assert.IsFalse(cache.TryAdd("SomeOther", new object()));
            Assert.IsTrue(cache.TryAdd("SomeOther", new object()));

            Assert.AreEqual(2, cache.Count);
        }
    }
}
