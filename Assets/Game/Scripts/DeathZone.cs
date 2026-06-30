using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetRipper(out var ripper)) ripper.RipperDead();
    }
}