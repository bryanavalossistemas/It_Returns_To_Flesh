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
}