using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BehaviourPlus;

public interface IHoverable
{
    void EnterHover();
    void ExitHover();
}
public interface ISelectable
{
    void Select();
    void Deselect();
}

public class SelectionManager : MonoBehaviour, IUpdatable
{
    public IHoverable Hovered { get; private set; }
    public ISelectable Selected { get; private set; }
    private LayerMask selectableMask = ~0;
    private PointerEventData pointerData;

    void Start() => pointerData = new(EventSystem.current);

    public void OnUpdate()
    {
        TryHover(inputManager.PointerPos);
        if (inputManager.PointerClick) TrySelect(inputManager.PointerPos);
    }

    public void TryHover(Vector2 screenPos)
    {
        IHoverable hoverable = GetHoverable();
        if (hoverable == null) ExitHover();
        else EnterHover(hoverable);

        IHoverable GetHoverable()
        {
            if (IsPointerOverUI(screenPos)) return null;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos, selectableMask);
            if (hit != null && hit.TryGetComponent(out IHoverable hoverable)) return hoverable;
            return null;
        }
    }

    public void EnterHover(IHoverable hoverable)
    {
        if (ReferenceEquals(Hovered, hoverable)) return;
        Hovered?.ExitHover();
        Hovered = hoverable;
        Hovered.EnterHover();
    }

    public void ExitHover()
    {
        Hovered?.ExitHover();
        Hovered = null;
    }

    public void TrySelect(Vector2 screenPos)
    {
        if (IsPointerOverUI(screenPos)) return;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, selectableMask);
        if (hit != null && hit.TryGetComponent(out ISelectable selectable)) Select(selectable);
    }

    public void Select(ISelectable selectable)
    {
        if (!ReferenceEquals(Selected, selectable))
        {
            Selected?.Deselect();
            Selected = selectable;
        }
        Selected.Select();
    }

    public void Deselect()
    {
        Selected?.Deselect();
        Selected = null;
    }

    private bool IsPointerOverUI(Vector2 screenPos)
    {
        pointerData.position = screenPos;
        List<RaycastResult> uiResults = new();
        EventSystem.current.RaycastAll(pointerData, uiResults);
        return uiResults.Count > 0;
    }
}