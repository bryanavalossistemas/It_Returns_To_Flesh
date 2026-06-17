using UnityEngine;
using static BehaviourPlus;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Usamos GetComponentInParent por si el collider que choca está en un objeto hijo
        FleshRipper ripper = col.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            ripper.Die();
        }
        else
        {
            Debug.Log("Algo tocó la DeathZone, pero no es un FleshRipper: " + col.name);
        }
    }
}