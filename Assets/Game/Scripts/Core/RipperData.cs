using UnityEngine;

[CreateAssetMenu(fileName = "Ripper", menuName = "Scriptable Objects/Ripper/Species")]
public class RipperData : ScriptableObject
{
    [Min(0f)] public float maxSpeed = 5f;
    public int maxHp;
}