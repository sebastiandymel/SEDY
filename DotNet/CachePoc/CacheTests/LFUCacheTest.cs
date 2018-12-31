using CachePoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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

        [TestMethod]
        public void LFU_CanGetOrAdd()
        {
            var cache = new LFUCache<int>(2);
            var factoryUsed = false;
            var result = cache.GetOrAdd("MyKey1", () =>
            {
                factoryUsed = true;
                return 5;
            });

            Assert.IsTrue(factoryUsed);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void LFU_CanGetOrAdd2()
        {
            var cache = new LFUCache<int>(2);
            var factoryUsed = false;
            cache.GetOrAdd("MyKey1", () =>
            {
                factoryUsed = true;
                return 5;
            });
            var result = cache.GetOrAdd("MyKey1", () =>
            {
                Assert.Fail("Should not use factory");
                return -1;
            });

            Assert.IsTrue(factoryUsed);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void LFU_CanGetOrAdd3()
        {
            var cache = new LFUCache<int>(2);
            Assert.IsTrue(cache.TryAdd("MyKey1", 100));
            var result = cache.GetOrAdd("MyKey1", () =>
            {
                Assert.Fail("Should not use factory");
                return -1;
            });

            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void SmokeTest1()
        {
            var cache = new LFUCache<int>(10);
            var succcessCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (cache.TryAdd($"Key{i}", i))
                {
                    succcessCount++;
                }
            }

            Assert.AreEqual(10, cache.Count);
            Assert.AreEqual(10, succcessCount);
        }

        [TestMethod]
        public void SmokeTest2()
        {
            var cache = new LFUCache<int>(10);
            var succcessCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (cache.TryAdd($"Key", i))
                {
                    succcessCount++;
                }
            }

            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(1000, succcessCount);
        }

        [TestMethod]
        public void SmokeTest4()
        {
            var cache = new LFUCache<int>(100);
            var succcessCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                if (cache.TryAdd($"Key{i}", i))
                {
                    succcessCount++;
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                if (cache.TryAdd($"Key{i}", i))
                {
                    succcessCount++;
                }
            }

            Assert.AreEqual(100, cache.Count);
            Assert.IsFalse(cache.TryGet("Key999", out var val));
            Assert.IsTrue(cache.TryGet("Key99", out var val2));
            Assert.IsFalse(cache.TryGet("Key100", out var val3));
        }

        [TestMethod]
        public void PerformanceTest_Size1_TryAdd()
        {
            PerformanceTest_TryAdd(
                cacheSize: 1,
                population: 100,
                expectedTimeMs: 10,
                rounds: 1000);

            PerformanceTest_TryAdd(
                cacheSize: 1,
                population: 1000,
                expectedTimeMs: 100,
                rounds: 100);
        }

        [TestMethod]
        public void PerformanceTest_Size100_TryAdd()
        {
            PerformanceTest_TryAdd(
               cacheSize: 100,
               population: 100,
               expectedTimeMs: 10,
               rounds: 1000);

            PerformanceTest_TryAdd(
                cacheSize: 100,
                population: 1000,
                expectedTimeMs: 100,
                rounds: 100);
        }

        [TestMethod]
        public void PerformanceTest_Size1000_TryAdd()
        {
            PerformanceTest_TryAdd(
               cacheSize: 1000,
               population: 100,
               expectedTimeMs: 10,
               rounds: 1000);

            PerformanceTest_TryAdd(
                cacheSize: 1000,
                population: 1000,
                expectedTimeMs: 100,
                rounds: 100);
        }

        private void PerformanceTest_TryAdd(int cacheSize, int population, int expectedTimeMs, int rounds = 100)
        {
            var totalTime = 0.0;

            for (int j = 0; j < rounds; j++)
            {
                var cache = new LFUCache<int>(cacheSize);
                var succcessCount = 0;
                var stopWatch = new Stopwatch();

                stopWatch.Start();
                for (int i = 0; i < population; i++)
                {
                    if (cache.TryAdd($"Key{i}", i))
                    {
                        succcessCount++;
                    }
                }
                stopWatch.Stop();
                totalTime += stopWatch.ElapsedMilliseconds;
            }

            double time = totalTime / rounds;

            Debug.Write($"Average time = {time} [ms]");
            Assert.IsTrue(time < expectedTimeMs);
        }
    }
}
