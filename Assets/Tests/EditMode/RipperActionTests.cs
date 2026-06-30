using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RipperActionTests
{
    #region VomitRA Tests

    [Test]
    public void VomitRA_DefaultDuration_Is5()
    {
        var action = new VomitRA();

        Assert.AreEqual(5f, action.duration);
    }

    [Test]
    public void VomitRA_DefaultCancellable_IsTrue()
    {
        var action = new VomitRA();

        Assert.IsTrue(action.cancellable);
    }

    [Test]
    public void VomitRA_IsRipperAction()
    {
        var action = new VomitRA();

        Assert.IsInstanceOf<RipperAction>(action);
    }

    #endregion

    #region SoresRA Tests

    [Test]
    public void SoresRA_DefaultJumpForce_IsZero()
    {
        var action = new SoresRA();

        Assert.AreEqual(Vector2.zero, action.jumpForce);
    }

    [Test]
    public void SoresRA_IsRipperAction()
    {
        var action = new SoresRA();

        Assert.IsInstanceOf<RipperAction>(action);
    }

    [Test]
    public void SoresRA_JumpForce_CanBeSet()
    {
        var action = new SoresRA { jumpForce = new Vector2(10f, 5f) };

        Assert.AreEqual(new Vector2(10f, 5f), action.jumpForce);
    }

    #endregion

    #region ExplodeRA Tests

    [Test]
    public void ExplodeRA_DefaultExplosionRadius_IsCorrect()
    {
        var action = new ExplodeRA();

        Assert.AreEqual(9.73f, action.explosionRadius, 0.001f);
    }

    [Test]
    public void ExplodeRA_DefaultExplosionForce_IsCorrect()
    {
        var action = new ExplodeRA();

        Assert.AreEqual(new Vector2(43.7f, 15.4f), action.explosionForce);
    }

    [Test]
    public void ExplodeRA_IsRipperAction()
    {
        var action = new ExplodeRA();

        Assert.IsInstanceOf<RipperAction>(action);
    }

    #endregion

    #region FrenzyRA Tests

    [Test]
    public void FrenzyRA_DefaultSpeedMultiplier_Is2()
    {
        var action = new FrenzyRA();

        Assert.AreEqual(2f, action.speedMultiplier);
    }

    [Test]
    public void FrenzyRA_DefaultHungryMultiplier_Is3()
    {
        var action = new FrenzyRA();

        Assert.AreEqual(3f, action.hungryMultiplier);
    }

    [Test]
    public void FrenzyRA_DefaultVisionRange_Is6()
    {
        var action = new FrenzyRA();

        Assert.AreEqual(6f, action.visionRange);
    }

    [Test]
    public void FrenzyRA_DefaultDuration_Is5()
    {
        var action = new FrenzyRA();

        Assert.AreEqual(5f, action.duration);
    }

    [Test]
    public void FrenzyRA_IsRipperAction()
    {
        var action = new FrenzyRA();

        Assert.IsInstanceOf<RipperAction>(action);
    }

    #endregion

    #region RipperAction Inheritance Tests

    [Test]
    public void RipperAction_IsAbstract()
    {
        Assert.IsTrue(typeof(RipperAction).IsAbstract);
    }

    [Test]
    public void AllActionTypes_InheritFromRipperAction()
    {
        Assert.IsTrue(typeof(RipperAction).IsAssignableFrom(typeof(VomitRA)));
        Assert.IsTrue(typeof(RipperAction).IsAssignableFrom(typeof(SoresRA)));
        Assert.IsTrue(typeof(RipperAction).IsAssignableFrom(typeof(ExplodeRA)));
        Assert.IsTrue(typeof(RipperAction).IsAssignableFrom(typeof(FrenzyRA)));
    }

    #endregion
}
