using JetBrains.Annotations;
using UnityEngine;

public class ActionCancelNode : MonoBehaviour
{
    public FleshCaster cancelCaster;

    public void TriggerCancel()
    {
        cancelCaster.CancelVomit();
        gameObject.SetActive(false);
    }
}
