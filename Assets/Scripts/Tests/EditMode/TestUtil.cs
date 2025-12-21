using NUnit.Framework;

namespace Tests.EditMode
{
    /**
     * Problem: Validate utility currency formatting helpers.
     * Goal: Ensure ConvertToTextAndReduceCurrency returns expected strings.
     * Approach: Use NUnit assertions on known inputs.
     * Time: O(1) per assertion.
     * Space: O(1).
     */
    public class TestUtil
    {
        [Test]
        public void TestAddElements()
        {
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(1000), "1000");
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(10000), "10K");
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(1234), "1234");
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(12340), "12K");
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(150000), "150K");
            Assert.AreEqual(Util.Util.ConvertToTextAndReduceCurrency(152345), "152K");
        }
    }
}
