using UnityEngine;

public class GuillotineBlade : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetRipper(out var ripper)) ripper.RipperDead();
    }
}