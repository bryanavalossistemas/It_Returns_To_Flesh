#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class TransformExtensions
{
    static readonly Quaternion ROT_RIGHT = Quaternion.identity, ROT_LEFT = Quaternion.Euler(0f, 180f, 0f);
    public static void InvertAxis(this Transform t)
    {
        t.rotation = t.right.x > 0f ? ROT_LEFT : ROT_RIGHT;
    }

#if UNITY_EDITOR
    public static void DestroyChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(t.GetChild(i).gameObject);
        }
    }

    public static void AggregateChildren<T,U>(this Transform t, T[] array, Object prefab) where U : MonoBehaviour, Init<T>
    {
        for (int i = 0; i < array.Length; i++)
        {
            U b = (U)PrefabUtility.InstantiatePrefab(prefab, t);
            Undo.RegisterCreatedObjectUndo(b.gameObject, $"Create {typeof(U).Name}");
            b.Init(array[i]);
        }
    }
#endif
}

public struct LoopArray<T>
{
    private readonly T[] array;
    private int start, index;

    public LoopArray(T[] array, int start = 0)
    {
        this.array = array;
        this.start = start;
        index = start;
    }

    public readonly T Current() => array[index];

    public T Next()
    {
        index++;
        if (index == array.Length) index = 0;
        return Current();
    }

    public T Reset()
    {
        index = start;
        return Current();
    }
}

public interface Init<T>
{
    public void Init(T value);
}