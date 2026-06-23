using UnityEngine;

public class ActionCancelNode : MonoBehaviour, ISelectable
{
    [SerializeField] private FleshCaster cancelCaster;

    public void Select()
    {
        cancelCaster.CancelVomit();
        gameObject.SetActive(false);
    }
    public void Deselect()
    {

    }
}