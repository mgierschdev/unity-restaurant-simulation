using NUnit.Framework;

public class TestUtil
{
    [Test]
    public void TestAddElements()
    {
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(1000), "1000");
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(10000), "10 K");
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(1234), "1234");
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(12340), "12 K");
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(150000), "150 K");
        Assert.AreEqual(Util.convertToTextAndReduceCurrency(152345), "152 K");
    }
}
