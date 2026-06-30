using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BehaviourPlusTests
{
    private GameObject coreGO;
    private Core coreComponent;
    private GameManager gmComponent;
    private InputManager imComponent;
    private AudioManager amComponent;
    private UIManager umComponent;
    private PrefsManager pmComponent;

    [SetUp]
    public void SetUp()
    {
        // Reset the static state via reflection before each test
        ResetBehaviourPlus();

        coreGO = new GameObject("Core");
        coreComponent = coreGO.AddComponent<Core>();
        gmComponent = coreGO.AddComponent<GameManager>();
        imComponent = coreGO.AddComponent<InputManager>();
        amComponent = coreGO.AddComponent<AudioManager>();
        umComponent = coreGO.AddComponent<UIManager>();
        pmComponent = coreGO.AddComponent<PrefsManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(coreGO);
        ResetBehaviourPlus();
    }

    [Test]
    public void Init_FirstCall_ReturnsTrue()
    {
        bool result = BehaviourPlus.Init(coreComponent, gmComponent, imComponent, amComponent, umComponent, pmComponent);

        Assert.IsTrue(result);
    }

    [Test]
    public void Init_FirstCall_SetsAllReferences()
    {
        BehaviourPlus.Init(coreComponent, gmComponent, imComponent, amComponent, umComponent, pmComponent);

        Assert.AreEqual(coreComponent, BehaviourPlus.core);
        Assert.AreEqual(gmComponent, BehaviourPlus.gameManager);
        Assert.AreEqual(imComponent, BehaviourPlus.inputManager);
        Assert.AreEqual(amComponent, BehaviourPlus.audioManager);
        Assert.AreEqual(umComponent, BehaviourPlus.uiManager);
        Assert.AreEqual(pmComponent, BehaviourPlus.prefsManager);
    }

    [Test]
    public void Init_SecondCall_ReturnsFalse()
    {
        BehaviourPlus.Init(coreComponent, gmComponent, imComponent, amComponent, umComponent, pmComponent);

        // Create different instances
        var go2 = new GameObject("Core2");
        var core2 = go2.AddComponent<Core>();
        var gm2 = go2.AddComponent<GameManager>();
        var im2 = go2.AddComponent<InputManager>();
        var am2 = go2.AddComponent<AudioManager>();
        var um2 = go2.AddComponent<UIManager>();
        var pm2 = go2.AddComponent<PrefsManager>();

        bool result = BehaviourPlus.Init(core2, gm2, im2, am2, um2, pm2);

        Assert.IsFalse(result);
        // Original references should remain
        Assert.AreEqual(coreComponent, BehaviourPlus.core);

        Object.DestroyImmediate(go2);
    }

    [Test]
    public void Init_SecondCall_DoesNotOverwriteReferences()
    {
        BehaviourPlus.Init(coreComponent, gmComponent, imComponent, amComponent, umComponent, pmComponent);

        var go2 = new GameObject("Core2");
        var core2 = go2.AddComponent<Core>();
        var gm2 = go2.AddComponent<GameManager>();
        var im2 = go2.AddComponent<InputManager>();
        var am2 = go2.AddComponent<AudioManager>();
        var um2 = go2.AddComponent<UIManager>();
        var pm2 = go2.AddComponent<PrefsManager>();

        BehaviourPlus.Init(core2, gm2, im2, am2, um2, pm2);

        Assert.AreEqual(coreComponent, BehaviourPlus.core);
        Assert.AreEqual(gmComponent, BehaviourPlus.gameManager);
        Assert.AreEqual(imComponent, BehaviourPlus.inputManager);
        Assert.AreEqual(amComponent, BehaviourPlus.audioManager);
        Assert.AreEqual(umComponent, BehaviourPlus.uiManager);
        Assert.AreEqual(pmComponent, BehaviourPlus.prefsManager);

        Object.DestroyImmediate(go2);
    }

    private void ResetBehaviourPlus()
    {
        // Use reflection to reset the static properties
        var type = typeof(BehaviourPlus);
        var coreField = type.GetProperty("core");
        var gmField = type.GetProperty("gameManager");
        var imField = type.GetProperty("inputManager");
        var amField = type.GetProperty("audioManager");
        var umField = type.GetProperty("uiManager");
        var pmField = type.GetProperty("prefsManager");

        coreField?.SetValue(null, null);
        gmField?.SetValue(null, null);
        imField?.SetValue(null, null);
        amField?.SetValue(null, null);
        umField?.SetValue(null, null);
        pmField?.SetValue(null, null);
    }
}
