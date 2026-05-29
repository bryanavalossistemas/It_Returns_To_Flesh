using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX = 100f;
    [SerializeField] private float minY = -50f; 
    [SerializeField] private float maxY = 50f;

    [Header("Zoom")]
    [SerializeField] private float zoomSensitivity = 1f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;

    private InputAction _moveAction;
    private Camera _cam;

    void Start()
    {
        _moveAction = InputSystem.actions["Player/Move"];
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 newPos = transform.position + new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.unscaledDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;

        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        if (scrollDelta != 0)
        {
            float zoomAmount = (scrollDelta / 120f) * zoomSensitivity;
            float targetZoom = _cam.orthographicSize - zoomAmount;
            _cam.orthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }
}
