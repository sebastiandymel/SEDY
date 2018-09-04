using NUnit.Framework;
using SOLID.LSP;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace RectangleTest
{
    public class RectableTests
    {
        [Test]
        public void TestWithRectangle()
        {
            var sut = new Rectangle(3,7);

            Assert.AreEqual(21, sut.Area);
        }

        [Test]
        public void TestWithSquare()
        {
            Rectangle sut = new Square(5);

            Assert.AreEqual(25, sut.Area); //This test will fail. Area equals 49.
        }
    }
}
