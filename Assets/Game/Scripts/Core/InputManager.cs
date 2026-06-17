using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour_UL, ILateUpdatable
{
    [SerializeField] private InputActionReference moveAction, panAction, zoomAction, pauseAction;
    [SerializeField] private InputActionReference _1Action, _2Action, _3Action, _4Action, _5Action, deselectAction;
    [SerializeField] private InputActionReference pointerPosAction, pointerClickAction;

    void Start()
    {
        EnableValue(moveAction, MoveInput);
        EnableValue(panAction, PanInput);
        EnableValue(zoomAction, ZoomInput);
        EnableButton(pauseAction, PauseButton);
        EnableButton(_1Action, _1Button);
        EnableButton(_2Action, _2Button);
        EnableButton(_3Action, _3Button);
        EnableButton(_4Action, _4Button);
        EnableButton(_5Action, _5Button);
        EnableButton(deselectAction, DeselectButton);
        EnableValue(pointerPosAction, PointerPosInput);
        EnableButton(pointerClickAction, PointerClickButton);

        static void EnableValue(InputActionReference input, Action<InputAction.CallbackContext> callback)
        {
            InputAction action = input.action;
            action.performed += callback;
            action.canceled += callback;
            action.Enable();
        }
        static void EnableButton(InputActionReference input, Action<InputAction.CallbackContext> callback)
        {
            InputAction action = input.action;
            action.performed += callback;
            action.Enable();
        }
    }

    /*void OnDisable()
    {
        DisableAction(moveAction, MoveInput);
        DisableAction(zoomAction, ZoomInput);
        DisableAction(pauseAction, PauseButton);

        static void DisableAction(InputActionReference input, Action<InputAction.CallbackContext> performed)
        {
            InputAction action = input.action;
            action.performed -= performed;
            action.Disable();
        }
    }*/

    public Vector2 Move { get; private set; }
    private void MoveInput(InputAction.CallbackContext c)
    {
        Move = c.ReadValue<Vector2>();
    }

    public Vector2 Pan { get; private set; }
    private void PanInput(InputAction.CallbackContext c)
    {
        Pan = c.ReadValue<Vector2>();
    }

    public float Zoom { get; private set; }
    private void ZoomInput(InputAction.CallbackContext c)
    {
        Zoom = c.ReadValue<float>();
    }

    public bool Pause { get; private set; }
    private void PauseButton(InputAction.CallbackContext _) => Pause = true;

    public bool _1 { get; private set; }
    private void _1Button(InputAction.CallbackContext _) => _1 = true;
    public bool _2 { get; private set; }
    private void _2Button(InputAction.CallbackContext _) => _2 = true;
    public bool _3 { get; private set; }
    private void _3Button(InputAction.CallbackContext _) => _3 = true;
    public bool _4 { get; private set; }
    private void _4Button(InputAction.CallbackContext _) => _4 = true;
    public bool _5 { get; private set; }
    private void _5Button(InputAction.CallbackContext _) => _5 = true;
    public bool Deselect { get; private set; }
    private void DeselectButton(InputAction.CallbackContext _) => _5 = true;

    public Vector2 PointerPos { get; private set; }
    private void PointerPosInput(InputAction.CallbackContext c)
    {
        PointerPos = c.ReadValue<Vector2>();
    }

    public bool PointerClick { get; private set; }
    private void PointerClickButton(InputAction.CallbackContext _) => PointerClick = true;

    public void OnLateUpdate()
    {
        Pause = _1 = _2 = _3 = _4 = _5 = Deselect = PointerClick = false;
    }
}