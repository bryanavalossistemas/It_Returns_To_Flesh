using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    static readonly Quaternion ROT_RIGHT = Quaternion.identity, ROT_LEFT = Quaternion.Euler(0f, 180f, 0f);
    public static void InvertAxis(this Transform t)
    {
        t.rotation = t.right.x > 0f ? ROT_LEFT : ROT_RIGHT;
    }
}

public class Pool<T> where T : Component
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

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}