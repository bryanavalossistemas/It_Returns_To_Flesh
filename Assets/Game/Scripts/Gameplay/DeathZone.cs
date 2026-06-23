using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        FleshRipper ripper = col.GetComponentInParent<FleshRipper>();

        if (ripper != null) ripper.Die();
    }
}