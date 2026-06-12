using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isActivated) return;

        FleshRipper ripper = col.GetComponentInParent<FleshRipper>();

        if (ripper != null)
        {
            isActivated = true;
             GameManager.Instance.UpdateCheckPoint(this.transform);
        }
    }
}