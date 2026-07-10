using UnityEngine;

public class ActionCancelNode : MonoBehaviour, ISelectable
{
    [SerializeField] private FleshRipper body;

    public void Select()
    {
        body.CancelVomit();
        gameObject.SetActive(false);
    }
    public void Deselect()
    {

    }
}