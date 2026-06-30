using UnityEngine;
using static BehaviourPlus;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isActivated) return;
        if (col.TryGetRipper(out _))
        {
            isActivated = true;
            gameManager.UpdateCheckPoint(transform);
        }
    }
}