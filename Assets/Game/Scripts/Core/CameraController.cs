using UnityEngine;
using Unity.Cinemachine;
using static BehaviourPlus;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private BoxCollider2D bounds2D;
    [SerializeField] private float speed = 15f, pan = 10f;
    private float minX, maxX, minY, maxY;
    [SerializeField] private float zoomSensitivity = 1f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    private bool isCameraMoving;
    private LensSettings lens;

    void Start()
    {
        lens = cam.Lens;
        lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize, minZoom, maxZoom);
        cam.Lens = lens;
    }

    void Update()
    {
        Vector3 newPos = transform.position + speed * Time.unscaledDeltaTime * (Vector3)inputManager.Move + pan * (Vector3)inputManager.Pan;
        //newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        //newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;

        float zoom = inputManager.Zoom;
        if (zoom != 0)
        {
            lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize - zoom * zoomSensitivity, minZoom, maxZoom);
            cam.Lens = lens;
        }

        if ((inputManager.Move + inputManager.Pan).magnitude > 0) SetFollow(null);
    }

    public void SetFollow(Transform t) => cam.Follow = t;

    public void UpdateConfiner(Vector3 offset, Vector3 size)
    {
        bounds2D.offset = offset;
        bounds2D.size = size;
        confiner.InvalidateBoundingShapeCache();
    }
}