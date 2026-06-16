using Unity.Cinemachine;
using UnityEngine;
using static BehaviourPlus;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
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
        Vector3 newPos = transform.position + speed * Time.unscaledDeltaTime * (Vector3)inputHandler.Move + pan * (Vector3)inputHandler.Pan;
        //newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        //newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;

        float zoom = inputHandler.Zoom;
        if (zoom != 0)
        {
            lens.OrthographicSize = Mathf.Clamp(lens.OrthographicSize - zoom * zoomSensitivity, minZoom, maxZoom);
            cam.Lens = lens;
        }     
    }
}
