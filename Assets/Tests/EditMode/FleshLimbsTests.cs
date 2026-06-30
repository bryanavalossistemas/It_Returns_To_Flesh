using NUnit.Framework;

[TestFixture]
public class FleshLimbsTests
{
    [Test]
    public void LimbType_Head_HasValue0()
    {
        Assert.AreEqual(0, (int)FleshLimbs.LimbType.Head);
    }

    [Test]
    public void LimbType_Arms_HasValue1()
    {
        Assert.AreEqual(1, (int)FleshLimbs.LimbType.Arms);
    }

    [Test]
    public void LimbType_Legs_HasValue2()
    {
        Assert.AreEqual(2, (int)FleshLimbs.LimbType.Legs);
    }

    [Test]
    public void LimbType_HasExactly3Values()
    {
        var values = System.Enum.GetValues(typeof(FleshLimbs.LimbType));

        Assert.AreEqual(3, values.Length);
    }
}
