using UnityEngine;
using System.Collections.Generic;

public interface IUpdatable { void OnUpdate(); }
public interface IFixedUpdatable { void OnFixedUpdate(); }
public interface ILateUpdatable { void OnLateUpdate(); }

public class MonoBehaviour_UU : MonoBehaviour
{
    void OnEnable() => UpdateManager.RegisterUpdate(this as IUpdatable);
    void OnDisable() => UpdateManager.UnregisterUpdate(this as IUpdatable);
}
public class MonoBehaviour_UF : MonoBehaviour
{
    void OnEnable() => UpdateManager.RegisterFixedUpdate(this as IFixedUpdatable);
    void OnDisable() => UpdateManager.UnregisterFixedUpdate(this as IFixedUpdatable);
}
public class MonoBehaviour_UL : MonoBehaviour
{
    void OnEnable() => UpdateManager.RegisterLateUpdate(this as ILateUpdatable);
    void OnDisable() => UpdateManager.UnregisterLateUpdate(this as ILateUpdatable);
}
public class MonoBehaviour_UM : MonoBehaviour
{
    void OnEnable() => UpdateManager.RegisterMany(this);
    void OnDisable() => UpdateManager.UnregisterMany(this);
}

public class UpdateManager : MonoBehaviour
{
    #region IUpdatale
    private static readonly List<IUpdatable> updatables = new();

    public static void RegisterUpdate(IUpdatable script) => updatables.Add(script);
    public static void UnregisterUpdate(IUpdatable script) => updatables.Remove(script);

    void Update()
    {
        for (int i = 0; i < updatables.Count; i++) updatables[i].OnUpdate();
    }
    #endregion
    #region IFixedUpatable
    private static readonly List<IFixedUpdatable> fixedUpdatables = new();

    public static void RegisterFixedUpdate(IFixedUpdatable script) => fixedUpdatables.Add(script);
    public static void UnregisterFixedUpdate(IFixedUpdatable script) => fixedUpdatables.Remove(script);

    void FixedUpdate()
    {
        for (int i = 0; i < fixedUpdatables.Count; i++) fixedUpdatables[i].OnFixedUpdate();
    }
    #endregion
    #region ILateUpdatable
    private static readonly List<ILateUpdatable> lateUpdatables = new();

    public static void RegisterLateUpdate(ILateUpdatable script) => lateUpdatables.Add(script);
    public static void UnregisterLateUpdate(ILateUpdatable script) => lateUpdatables.Remove(script);

    void LateUpdate()
    {
        for (int i = 0; i < lateUpdatables.Count; i++) lateUpdatables[i].OnLateUpdate();
    }
    #endregion
    #region All
    public static void RegisterMany(object obj)
    {
        if (obj is IUpdatable u) RegisterUpdate(u);
        if (obj is IFixedUpdatable f) RegisterFixedUpdate(f);
        if (obj is ILateUpdatable l) RegisterLateUpdate(l);
    }
    public static void UnregisterMany(object obj)
    {
        if (obj is IUpdatable u) UnregisterUpdate(u);
        if (obj is IFixedUpdatable f) UnregisterFixedUpdate(f);
        if (obj is ILateUpdatable l) UnregisterLateUpdate(l);
    }
    #endregion
}