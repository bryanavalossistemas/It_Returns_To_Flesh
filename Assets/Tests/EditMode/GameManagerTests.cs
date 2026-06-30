using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameManagerTests
{
    private GameManager gameManager;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("GameManager");
        gameManager = gameObject.AddComponent<GameManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void ModifyHP_PositiveValue_IncreasesHP()
    {
        // HP starts at 0 by default (not scene-loaded), set via reflection for testing
        SetHP(5);
        SetMaxHP(10);

        gameManager.ModifyHP(3);

        Assert.AreEqual(8, gameManager.HP);
    }

    [Test]
    public void ModifyHP_ExceedsMax_ClampsToMax()
    {
        SetHP(8);
        SetMaxHP(10);

        gameManager.ModifyHP(5);

        Assert.AreEqual(10, gameManager.HP);
    }

    [Test]
    public void ModifyHP_NegativeValue_DecreasesHP()
    {
        SetHP(5);
        SetMaxHP(10);

        gameManager.ModifyHP(-2);

        Assert.AreEqual(3, gameManager.HP);
    }

    [Test]
    public void ModifyHP_ToZero_TriggersRestart()
    {
        SetHP(1);
        SetMaxHP(10);

        // ModifyHP calls RestartLevel when HP <= 0, which calls core.ReloadScene()
        // Since core is null in test, we just verify HP goes to 0 or below
        // The actual restart would throw, so we catch it
        try
        {
            gameManager.ModifyHP(-1);
        }
        catch (System.NullReferenceException)
        {
            // Expected: RestartLevel calls core.ReloadScene() but core is null in tests
        }

        Assert.LessOrEqual(gameManager.HP, 0);
    }

    [Test]
    public void ModifyHP_BelowZero_TriggersRestart()
    {
        SetHP(2);
        SetMaxHP(10);

        try
        {
            gameManager.ModifyHP(-5);
        }
        catch (System.NullReferenceException)
        {
            // Expected: RestartLevel calls core.ReloadScene() but core is null in tests
        }

        Assert.AreEqual(-3, gameManager.HP);
    }

    [Test]
    public void RegisterRipper_IncreasesCount()
    {
        // Register multiple rippers
        gameManager.RegisterRipper();
        gameManager.RegisterRipper();
        gameManager.RegisterRipper();

        // RipperDead decrements; after 3 registers and 2 deaths, should not restart
        gameManager.RipperDead();
        gameManager.RipperDead();

        // Third death should trigger restart (nRippers <= 0)
        try
        {
            gameManager.RipperDead();
        }
        catch (System.NullReferenceException)
        {
            // Expected: RestartLevel calls core.ReloadScene() but core is null
        }

        // If we got here without crashing on the first two deaths, registration worked
        Assert.Pass("RegisterRipper correctly tracked ripper count");
    }

    [Test]
    public void RipperDead_WhenLastRipper_TriggersRestart()
    {
        gameManager.RegisterRipper();

        bool threwOnRestart = false;
        try
        {
            gameManager.RipperDead();
        }
        catch (System.NullReferenceException)
        {
            threwOnRestart = true;
        }

        // RestartLevel was called (attempted core.ReloadScene)
        Assert.IsTrue(threwOnRestart, "Expected RestartLevel to be called when last ripper dies");
    }

    [Test]
    public void Constants_AreCorrect()
    {
        Assert.AreEqual(9, GameManager.CivilianLayer);
        Assert.AreEqual(11, GameManager.ExplodableLayer);
        Assert.AreEqual(8, GameManager.InstakillLayer);
        Assert.AreEqual(0.5f, GameManager.RayLength);
    }

    [Test]
    public void SelectionTarget_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)GameManager.SelectionTarget.None);
        Assert.AreEqual(1, (int)GameManager.SelectionTarget.Ripper);
        Assert.AreEqual(2, (int)GameManager.SelectionTarget.Limb);
    }

    // Helper methods to set private/readonly properties via reflection
    private void SetHP(int value)
    {
        var prop = typeof(GameManager).GetProperty("HP");
        prop.SetValue(gameManager, value);
    }

    private void SetMaxHP(int value)
    {
        var prop = typeof(GameManager).GetProperty("MaxHP");
        prop.SetValue(gameManager, value);
    }
}
