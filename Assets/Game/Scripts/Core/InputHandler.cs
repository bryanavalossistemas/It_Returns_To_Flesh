using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour_UL, ILateUpdatable
{
    [SerializeField] private InputActionReference moveAction, panAction, zoomAction, pauseAction;

    void Start()
    {
        EnableValue(moveAction, MoveInput);
        EnableValue(panAction, PanInput);
        EnableValue(zoomAction, ZoomInput);
        EnableButton(pauseAction, PauseButton);

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

    #region Camera
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
    #endregion

    public bool Pause { get; private set; }
    private void PauseButton(InputAction.CallbackContext c)
    {
        Pause = c.performed;
    }

    public PhasedInput<Vector2> CameraMov { get; private set; } = new();
    public void GetCameraMov_enabled(InputAction.CallbackContext c)
    {
        CameraMov.SetPhase(c);
    }
    public void GetCameraMov(InputAction.CallbackContext c)
    {
        CameraMov.TrySetValue(c);
    }

    public enum InputPhases { None, Started, Performed, Canceled }
    public class PhasedInput
    {
        public InputPhases Phase;
        public virtual void SetPhase(InputAction.CallbackContext c)
        {
            Phase = c.phase switch
            {
                InputActionPhase.Started => InputPhases.Started,
                InputActionPhase.Performed => InputPhases.Performed,
                InputActionPhase.Canceled => InputPhases.Canceled,
                _ => InputPhases.None
            };
        }
        public bool IsStarted => Phase is InputPhases.Started;
        public bool IsTriggered => Phase is InputPhases.Started or InputPhases.Performed;
        public bool IsNone => Phase is InputPhases.None;
    }

    public class PhasedInput<T> : PhasedInput where T : struct
    {
        public T Value { get; private set; }
        public void TrySetValue(InputAction.CallbackContext c)
        {
            Value = IsTriggered ? c.ReadValue<T>() : default;
        }
    }

    public void OnLateUpdate()
    {
        Pause = false;
    }
}