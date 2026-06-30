using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RipperCommandsTests
{
    private RipperCommands commands;

    [SetUp]
    public void SetUp()
    {
        commands = ScriptableObject.CreateInstance<RipperCommands>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(commands);
    }

    [Test]
    public void DefaultHpNeeded_Is2()
    {
        Assert.AreEqual(2, commands.hpNeeded);
    }

    [Test]
    public void DefaultAction_IsNull()
    {
        Assert.IsNull(commands.action);
    }

    [Test]
    public void Action_CanBeAssigned_VomitRA()
    {
        commands.action = new VomitRA();

        Assert.IsNotNull(commands.action);
        Assert.IsInstanceOf<VomitRA>(commands.action);
    }

    [Test]
    public void Action_CanBeAssigned_ExplodeRA()
    {
        commands.action = new ExplodeRA();

        Assert.IsNotNull(commands.action);
        Assert.IsInstanceOf<ExplodeRA>(commands.action);
    }

    [Test]
    public void Action_CanBeAssigned_FrenzyRA()
    {
        commands.action = new FrenzyRA();

        Assert.IsNotNull(commands.action);
        Assert.IsInstanceOf<FrenzyRA>(commands.action);
    }

    [Test]
    public void Action_CanBeChanged()
    {
        commands.action = new VomitRA();
        commands.action = new FrenzyRA();

        Assert.IsInstanceOf<FrenzyRA>(commands.action);
    }

    [Test]
    public void HpNeeded_CanBeModified()
    {
        commands.hpNeeded = 5;

        Assert.AreEqual(5, commands.hpNeeded);
    }
}
