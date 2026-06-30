using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RipperSOTests
{
    private RipperSO ripperSO;

    [SetUp]
    public void SetUp()
    {
        ripperSO = ScriptableObject.CreateInstance<RipperSO>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(ripperSO);
    }

    [Test]
    public void DefaultInitialHP_Is10()
    {
        Assert.AreEqual(10, ripperSO.initialHP);
    }

    [Test]
    public void DefaultBuffHP_Is5()
    {
        Assert.AreEqual(5, ripperSO.buffHP);
    }

    [Test]
    public void DefaultSpeed_Is5()
    {
        Assert.AreEqual(5f, ripperSO.speed);
    }

    [Test]
    public void DefaultJumpForce_IsCorrect()
    {
        Assert.AreEqual(new Vector2(10f, 5f), ripperSO.jumpForce);
    }

    [Test]
    public void DefaultFrenzyDuration_Is3()
    {
        Assert.AreEqual(3f, ripperSO.frenzyDuration);
    }

    [Test]
    public void DefaultFrenzySpeed_Is2()
    {
        Assert.AreEqual(2f, ripperSO.frenzySpeed);
    }

    [Test]
    public void DefaultVisionRange_Is6()
    {
        Assert.AreEqual(6f, ripperSO.visionRange);
    }

    [Test]
    public void DefaultSpeedMultiplier_Is3()
    {
        Assert.AreEqual(3f, ripperSO.speedMultiplier);
    }

    [Test]
    public void InitialHP_CanBeModified()
    {
        ripperSO.initialHP = 20;

        Assert.AreEqual(20, ripperSO.initialHP);
    }

    [Test]
    public void Speed_CanBeModified()
    {
        ripperSO.speed = 10f;

        Assert.AreEqual(10f, ripperSO.speed);
    }

    [Test]
    public void FrenzyDuration_CanBeModified()
    {
        ripperSO.frenzyDuration = 7f;

        Assert.AreEqual(7f, ripperSO.frenzyDuration);
    }
}
