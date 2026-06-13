using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float minX = -10.996f;
    [SerializeField] private float maxX = 185.95f;
    [SerializeField] private float minY = -7.991f; 
    [SerializeField] private float maxY = 12.98f;

    [Header("Zoom")]
    [SerializeField] private float zoomSensitivity = 1f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    private InputAction _moveAction;
    [SerializeField] private CinemachineCamera _cam;
    private LensSettings lens;

    void Start()
    {
        _moveAction = InputSystem.actions["Player/Move"];
        lens = _cam.Lens;
        lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize, minZoom, maxZoom);
        _cam.Lens = lens;
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
        Vector3 newPos = transform.position + speed * Time.unscaledDeltaTime * (Vector3)moveInput;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;

        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        if (scrollDelta != 0)
        {
            float zoomAmount = (scrollDelta / 120f) * zoomSensitivity;
            float targetZoom = lens.OrthographicSize - zoomAmount;
            lens.OrthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            _cam.Lens = lens;
        }
    }
}
