using UnityEngine;
using System.Collections.Generic;

public static class TransformExtensions
{
    static readonly Quaternion ROT_RIGHT = Quaternion.identity, ROT_LEFT = Quaternion.Euler(0f, 180f, 0f);
    public static void InvertAxis(this Transform t)
    {
        t.rotation = t.right.x > 0f ? ROT_LEFT : ROT_RIGHT;
    }
}

public static class ColliderExtensions
{
    public static bool TryGetRipper(this Collider2D col, out FleshRipper ripper)
    {
        ripper = col.GetComponent<FleshRipper>() ?? col.GetComponentInParent<FleshRipper>();
        return ripper != null;
    }
}

public static class HealthHelper
{
    public static bool ModifyClamp(ref int current, int delta, int max)
    {
        current = Mathf.Clamp(current + delta, 0, max);
        return current <= 0;
    }
}

public interface IPool
{
    void PoolStart();
    void PoolEnd();
}

public class Pool<T> where T : Component, IPool
{
    private readonly T prefab;
    private readonly Queue<T> pool = new();

    public Pool(T prefab, int initialSize)
    {
        this.prefab = prefab;
        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get(Vector3 pos = default)
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
        }
        else obj = GameObject.Instantiate(prefab);
        obj.transform.position = pos;
        obj.PoolStart();
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.PoolEnd();
        pool.Enqueue(obj);
    }
}