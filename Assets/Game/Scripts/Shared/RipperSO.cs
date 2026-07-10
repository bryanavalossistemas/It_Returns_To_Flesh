using UnityEngine;

[CreateAssetMenu(fileName = "RipperSO", menuName = "Scriptable Objects/RipperSO")]
public class RipperSO : ScriptableObject
{
    [Header("Default")]
    [Min(1)] public int initialHP = 10;
    [Min(1)] public int buffHP = 5;
    [Min(0f)] public float speed = 5f;
    [Header("Sores")]
    public Vector2 jumpForce = new(10f, 5f);
    [Header("Explosion")]
    public float explosionRadius = 5f;
    public Vector2 explosionForce = new();
    [Header("Frenzy")]
    public float frenzyDuration = 3f;
    public float frenzySpeed = 2f, visionRange = 6f, speedMultiplier = 3f;
}