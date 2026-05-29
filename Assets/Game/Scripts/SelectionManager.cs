using Mono.Cecil;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

public class SelectionManager: MonoBehaviour
{
    public enum InputState { Normal, QuickCast,TargetingLimb }
    public static InputState CurrentState = InputState.Normal;
    public static int PendingSkillID = -1;

    [SerializeField]private Camera _cam;

    private Transform _lockedTarget;
    private Vector2 _cameraOffset = new Vector3(0, 0, -10f);

    public static bool isTargetingLimb = false;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CurrentState = InputState.Normal;
            PendingSkillID = PendingSkillID -1;
            _lockedTarget = null;
            FleshRipper.SelectedRipper = null;
            UIManager.Instance.ClearUI();
            return;
        }
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D[] hits = new RaycastHit2D[0];
        if (!isPointerOverUI)
        {
            hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        }
        if (!isPointerOverUI && CurrentState == InputState.Normal && FleshRipper.SelectedRipper == null)
        {
            FleshRipper hoveredRipper = GetFirstComponent<FleshRipper>(hits);
            if (hoveredRipper != null)
                UIManager.Instance.UpdateHealth(hoveredRipper.Health, hoveredRipper.MaxHealth);
            else
                UIManager.Instance.ClearUI();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && !isPointerOverUI)
        {
            
            if (CurrentState == InputState.TargetingLimb)
            {
                FleshRipper target =
                    GetFirstComponent<FleshRipper>(hits);

                if (target != null)
                {
                    FleshLimbs limbs =
                        target.GetComponent<FleshLimbs>();

                    if (limbs != null)
                    {
                        limbs.DetonateLimb(
                            FleshLimbs.LimbType.Arms
                        );
                    }
                }

                return;
            }

            if (CurrentState == InputState.QuickCast)
            {
                FleshRipper target = GetFirstComponent<FleshRipper>(hits);
                if (target != null)
                {
                    ExecuteQuickCast(target.GetComponent<FleshCaster>());
                }
                return;
            }
            ActionCancelNode cancelNode = GetFirstComponent<ActionCancelNode>(hits);
            if (cancelNode != null)
            {
                cancelNode.TriggerCancel();
                return;
            }

            FleshRipper ripperToLock = GetFirstComponent<FleshRipper>(hits);
            if (ripperToLock != null)
            {
                FleshRipper.SelectedRipper = ripperToLock;
                _lockedTarget = ripperToLock.transform;
                ripperToLock.SelectThisUnit();
            }
        }
    }

    private void LateUpdate()
    {
        if (_lockedTarget != null)
        {
            _cam.transform.position = _lockedTarget.position + new Vector3(0, 0, -10f);
            float moveX = Keyboard.current.aKey.ReadValue() - Keyboard.current.dKey.ReadValue();
            float moveY = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
            if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveY) > 0)
            {
                _lockedTarget = null;
            }
        }
    }

    private void ExecuteQuickCast(FleshCaster caster)
    {
        if (caster == null) return;
        switch (PendingSkillID)
        {
            case 1:
                if (caster.CanCastVomit()) caster.CastVomit();
                break;
            case 2:
                if (caster.CanCastSores()) caster.CastSores();
                break;
            case 5:
                if (caster.CanCastFrenzy()) caster.CastFrenzy();
                break;
        }
    }

    private T GetFirstComponent<T>(RaycastHit2D[] hits) where T : Component
    {
        foreach (var hit in hits)
        {
            T component = hit.collider.GetComponent<T>();
            if (component != null) return component;
        }
        return null;
    }


}

