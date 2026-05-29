using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool _isRespawning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        FleshRipper[] allRippers =
            FindObjectsByType<FleshRipper>(
                FindObjectsSortMode.None);

        if (allRippers.Length == 0 && !_isRespawning)
        {
            _isRespawning = true;
            SpawnNewRipper();
        }
    }

    private void SpawnNewRipper()
    {
        Instantiate(
            ripperPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Debug.Log("NEW RIPPER SPAWNED");

        _isRespawning = false;
    }
}