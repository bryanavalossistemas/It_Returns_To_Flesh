using UnityEngine;

[CreateAssetMenu(fileName = "RipperSO", menuName = "Scriptable Objects/RipperSO")]
public class RipperSO : ScriptableObject
{
    //HP
    [Min(1)] public int initialHP = 10, buffHP = 5;
    [Min(0f)] public float speed = 5f;
    //Sores
    public Vector2 jumpForce = new(10f, 5f);
    //Frenzy
    public float frenzyDuration = 3f, frenzySpeed = 2f, visionRange = 6f, speedMultiplier = 3f;
}