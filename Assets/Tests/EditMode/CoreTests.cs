using NUnit.Framework;

[TestFixture]
public class CoreTests
{
    [Test]
    public void MenusIndex_IsZero()
    {
        Assert.AreEqual(0, Core.MenusIndex);
    }
}
