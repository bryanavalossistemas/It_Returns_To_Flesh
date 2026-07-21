using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam, miniCamera;
    [SerializeField] private float speed = 15f, pan = 10f;
    private float minX, maxX, minY, maxY;
    private Vector2 worldMin, worldMax;
    [SerializeField] private float zoomSensitivity = 1f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    private bool isCameraMoving;
    private LensSettings lens;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    void Start()
    {
        lens = cam.Lens;
        ClampZoom();
    }

    public void ControllerUpdate()
    {
        float newZoom = inputManager.Zoom;
        if (newZoom != 0)
        {
            ClampZoom(newZoom * zoomSensitivity);
            CalculateEdges();
        }

        Vector2 moveV = speed * Time.unscaledDeltaTime * inputManager.Move + pan * inputManager.Pan;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + moveV.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y + moveV.y, minY, maxY);
        transform.position = pos;

        if (moveV != Vector2.zero) SetFollow(null);
    }

    private void ClampZoom(float newZoom = 0f)
    {
        lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize - newZoom, minZoom, maxZoom);
        cam.Lens = lens;
    }

    public void SetFollow(Transform t) => cam.Follow = t;
    public void SetMiniCamera(Transform t) => miniCamera.Follow = t;

    public void UpdateBounds(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        worldMin = tilemap.CellToWorld(bounds.min);
        worldMax = tilemap.CellToWorld(bounds.max);
        CalculateEdges();
    }

    private void CalculateEdges()
    {
        float halfHeight = lens.OrthographicSize, halfWidth = halfHeight * lens.Aspect;
        minX = worldMin.x + halfWidth;
        maxX = worldMax.x - halfWidth;
        minY = worldMin.y + halfHeight;
        maxY = worldMax.y - halfHeight;
        ScreenTooBig(ref minX, ref maxX, worldMin.x, worldMax.x);
        ScreenTooBig(ref minY, ref maxY, worldMin.y, worldMax.y);

        static void ScreenTooBig(ref float min, ref float max, float worldMin, float worldMax)
        {
            if (min > max)
                min = max = (worldMin + worldMax) * 0.5f;
        }
    }

    public void ShakeCamera(float intensity = 1f)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(intensity);
        }
    }
}