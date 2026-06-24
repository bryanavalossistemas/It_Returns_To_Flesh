using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
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

public class SelectionManager : MonoBehaviour_UU, IUpdatable
{
    private IHoverable Hovered;
    private ISelectable Selected;
    private PointerEventData pointerData;
    private LayerMask selectableMask = ~0;
    private float timer;
    private Vector2 lastPos;

    void Start() => pointerData = new(EventSystem.current);

    public void OnUpdate()
    {
        Vector2 pos = inputManager.PointerPos;
        timer += Time.deltaTime;
        if ((pos - lastPos).sqrMagnitude > 0.01f || timer >= 0.1f)
        {
            timer = 0f;
            lastPos = pos;
            TryHover(pos);
        }
        if (inputManager.PointerClick) TrySelect(pos);
    }

    public void TryHover(Vector2 screenPos) => HandlePointer<IHoverable>(screenPos, EnterHover, ExitHover);
    public void EnterHover(IHoverable hoverable) => HandlePointerActivation(ref Hovered, hoverable);
    public void ExitHover() => HandlePointerDeactivation(ref Hovered);

    public void TrySelect(Vector2 screenPos) => HandlePointer<ISelectable>(screenPos, Select, null);
    public void Select(ISelectable selectable) => HandlePointerActivation(ref Selected, selectable, true);
    public void Deselect() => HandlePointerDeactivation(ref Selected);

    private void HandlePointer<T>(Vector2 screenPos, Action<T> onHit, Action onMiss) where T : class
    {
        if (PointerResult(out T result)) onHit(result);
        else onMiss?.Invoke();

        bool PointerResult(out T result)
        {
            result = default;
            if (IsPointerOverUI(screenPos)) return false;
            Vector2 worldPos = core.MainCamera.ScreenToWorldPoint(screenPos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos, selectableMask);
            if (hit != null && hit.TryGetComponent(out T comp))
            {
                result = comp;
                return true;
            }
            return false;
        }

        bool IsPointerOverUI(Vector2 screenPos)
        {
            pointerData.position = screenPos;
            List<RaycastResult> uiResults = new();
            EventSystem.current.RaycastAll(pointerData, uiResults);
            return uiResults.Count > 0;
        }
    }
    private void HandlePointerActivation<T>(ref T current, T next, bool allowRepeated = false) where T : class
    {
        if (ReferenceEquals(current, next))
        {
            if (!allowRepeated) return;
        }
        else
        {
            if (current != null) ExitDeselect(current);
            current = next;
        }
        EnterSelect(current);
    }
    private void HandlePointerDeactivation<T>(ref T current) where T : class
    {
        ExitDeselect(current);
        current = null;
    }
    private void EnterSelect<T>(T current)
    {
        switch (current)
        {
            case IHoverable h:
                h.EnterHover();
                break;
            case ISelectable s:
                s.Select();
                break;
        }
    }
    private void ExitDeselect<T>(T current)
    {
        switch (current)
        {
            case IHoverable h:
                h.ExitHover();
                break;
            case ISelectable s:
                s.Deselect();
                break;
        }
    }
}