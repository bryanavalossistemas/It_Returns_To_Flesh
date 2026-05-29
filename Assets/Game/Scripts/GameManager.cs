using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
    [SerializeField] private Transform spawnPoint;
    int totalRippers;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterRipper() => totalRippers++;

    public void DeadRipper()
    {
        totalRippers--;
        if (totalRippers <= 0) SpawnNewRipper();
    }

    private void SpawnNewRipper()
    {
        Instantiate(
            ripperPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Debug.Log("NEW RIPPER SPAWNED");
    }
}