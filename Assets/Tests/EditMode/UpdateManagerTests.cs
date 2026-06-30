using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class UpdateManagerTests
{
    private GameObject managerGO;
    private UpdateManager updateManager;

    [SetUp]
    public void SetUp()
    {
        // Clear static lists via reflection
        ClearStaticLists();

        managerGO = new GameObject("UpdateManager");
        updateManager = managerGO.AddComponent<UpdateManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(managerGO);
        ClearStaticLists();
    }

    #region IUpdatable Tests

    [Test]
    public void RegisterUpdate_AddsToList()
    {
        var mock = new MockUpdatable();

        UpdateManager.RegisterUpdate(mock);

        var list = GetUpdatablesList();
        Assert.Contains(mock, (System.Collections.ICollection)list);
    }

    [Test]
    public void UnregisterUpdate_RemovesFromList()
    {
        var mock = new MockUpdatable();
        UpdateManager.RegisterUpdate(mock);

        UpdateManager.UnregisterUpdate(mock);

        var list = GetUpdatablesList();
        Assert.IsFalse(list.Contains(mock));
    }

    [Test]
    public void RegisterUpdate_MultipleTimes_AddsMultiple()
    {
        var mock1 = new MockUpdatable();
        var mock2 = new MockUpdatable();

        UpdateManager.RegisterUpdate(mock1);
        UpdateManager.RegisterUpdate(mock2);

        var list = GetUpdatablesList();
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void UnregisterUpdate_NonExistent_DoesNotThrow()
    {
        var mock = new MockUpdatable();

        Assert.DoesNotThrow(() => UpdateManager.UnregisterUpdate(mock));
    }

    #endregion

    #region IFixedUpdatable Tests

    [Test]
    public void RegisterFixedUpdate_AddsToList()
    {
        var mock = new MockFixedUpdatable();

        UpdateManager.RegisterFixedUpdate(mock);

        var list = GetFixedUpdatablesList();
        Assert.Contains(mock, (System.Collections.ICollection)list);
    }

    [Test]
    public void UnregisterFixedUpdate_RemovesFromList()
    {
        var mock = new MockFixedUpdatable();
        UpdateManager.RegisterFixedUpdate(mock);

        UpdateManager.UnregisterFixedUpdate(mock);

        var list = GetFixedUpdatablesList();
        Assert.IsFalse(list.Contains(mock));
    }

    #endregion

    #region ILateUpdatable Tests

    [Test]
    public void RegisterLateUpdate_AddsToList()
    {
        var mock = new MockLateUpdatable();

        UpdateManager.RegisterLateUpdate(mock);

        var list = GetLateUpdatablesList();
        Assert.Contains(mock, (System.Collections.ICollection)list);
    }

    [Test]
    public void UnregisterLateUpdate_RemovesFromList()
    {
        var mock = new MockLateUpdatable();
        UpdateManager.RegisterLateUpdate(mock);

        UpdateManager.UnregisterLateUpdate(mock);

        var list = GetLateUpdatablesList();
        Assert.IsFalse(list.Contains(mock));
    }

    #endregion

    #region RegisterMany / UnregisterMany Tests

    [Test]
    public void RegisterMany_WithAllInterfaces_RegistersAll()
    {
        var mock = new MockAllUpdatable();

        UpdateManager.RegisterMany(mock);

        Assert.Contains(mock, (System.Collections.ICollection)GetUpdatablesList());
        Assert.Contains(mock, (System.Collections.ICollection)GetFixedUpdatablesList());
        Assert.Contains(mock, (System.Collections.ICollection)GetLateUpdatablesList());
    }

    [Test]
    public void UnregisterMany_WithAllInterfaces_UnregistersAll()
    {
        var mock = new MockAllUpdatable();
        UpdateManager.RegisterMany(mock);

        UpdateManager.UnregisterMany(mock);

        Assert.IsFalse(GetUpdatablesList().Contains(mock));
        Assert.IsFalse(GetFixedUpdatablesList().Contains(mock));
        Assert.IsFalse(GetLateUpdatablesList().Contains(mock));
    }

    [Test]
    public void RegisterMany_WithOnlyUpdatable_OnlyRegistersUpdate()
    {
        var mock = new MockUpdatable();

        UpdateManager.RegisterMany(mock);

        Assert.Contains(mock, (System.Collections.ICollection)GetUpdatablesList());
        Assert.AreEqual(0, GetFixedUpdatablesList().Count);
        Assert.AreEqual(0, GetLateUpdatablesList().Count);
    }

    #endregion

    #region Helper Classes

    private class MockUpdatable : IUpdatable
    {
        public int UpdateCount { get; private set; }
        public void OnUpdate() => UpdateCount++;
    }

    private class MockFixedUpdatable : IFixedUpdatable
    {
        public int FixedUpdateCount { get; private set; }
        public void OnFixedUpdate() => FixedUpdateCount++;
    }

    private class MockLateUpdatable : ILateUpdatable
    {
        public int LateUpdateCount { get; private set; }
        public void OnLateUpdate() => LateUpdateCount++;
    }

    private class MockAllUpdatable : IUpdatable, IFixedUpdatable, ILateUpdatable
    {
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
        public void OnLateUpdate() { }
    }

    #endregion

    #region Reflection Helpers

    private List<IUpdatable> GetUpdatablesList()
    {
        var field = typeof(UpdateManager).GetField("updatables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (List<IUpdatable>)field.GetValue(null);
    }

    private List<IFixedUpdatable> GetFixedUpdatablesList()
    {
        var field = typeof(UpdateManager).GetField("fixedUpdatables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (List<IFixedUpdatable>)field.GetValue(null);
    }

    private List<ILateUpdatable> GetLateUpdatablesList()
    {
        var field = typeof(UpdateManager).GetField("lateUpdatables", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (List<ILateUpdatable>)field.GetValue(null);
    }

    private void ClearStaticLists()
    {
        GetUpdatablesList()?.Clear();
        GetFixedUpdatablesList()?.Clear();
        GetLateUpdatablesList()?.Clear();
    }

    #endregion
}
