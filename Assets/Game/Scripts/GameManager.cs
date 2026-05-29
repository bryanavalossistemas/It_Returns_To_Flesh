using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckRemainingRippers()
    {
        StartCoroutine(CheckAfterFrame());
    }

    private IEnumerator CheckAfterFrame()
    {
        yield return null;

        FleshRipper[] allRippers =
            FindObjectsByType<FleshRipper>(
                FindObjectsSortMode.None);

        if (allRippers.Length == 0)
        {
            SpawnNewRipper();
        }
    }

    private void SpawnNewRipper()
    {
        Debug.Log("SPAWNING NEW RIPPER");

        Instantiate(
            ripperPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}