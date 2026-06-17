using UnityEngine;

public class ActionCancelNode : MonoBehaviour, ISelectable
{
    public FleshCaster cancelCaster;

    public void Deselect()
    {
        
    }

    public void Select()
    {
        cancelCaster.CancelVomit();
        gameObject.SetActive(false);
    }
}
