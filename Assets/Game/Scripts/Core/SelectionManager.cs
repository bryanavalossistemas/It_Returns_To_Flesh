using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        Hovered = null;
        Selected = null;
        timer = 0f;
    }

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
    public void EnterHover(IHoverable hoverable) => HandlePointerActivation(ref Hovered, hoverable, h => h.EnterHover(), h => h.ExitHover());
    public void ExitHover() => HandlePointerDeactivation(ref Hovered, h => h.ExitHover());

    public void TrySelect(Vector2 screenPos) => HandlePointer<ISelectable>(screenPos, Select, null);
    public void Select(ISelectable selectable) => HandlePointerActivation(ref Selected, selectable, s => s.Select(), s => s.Deselect(), true);
    public void Deselect() => HandlePointerDeactivation(ref Selected, s => s.Deselect());

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
    private void HandlePointerActivation<T>(ref T current, T next, Action<T> onEnter, Action<T> onExit, bool allowRepeated = false) where T : class
    {
        if (ReferenceEquals(current, next))
        {
            if (!allowRepeated) return;
        }
        else
        {
            if (current != null) onExit(current);
            current = next;
        }
        onEnter(current);
    }
    private void HandlePointerDeactivation<T>(ref T current, Action<T> onExit) where T : class
    {
        if (current != null) onExit(current);
        current = null;
    }
}