using UnityEngine;
using static BehaviourPlus;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isActivated) return;

        FleshRipper ripper = col.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            isActivated = true;
             gameManager.UpdateCheckPoint(transform);
        }
    }
}