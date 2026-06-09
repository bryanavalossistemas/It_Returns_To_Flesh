using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
    private Transform spawnPoint;
    int totalRippers;

    void Awake()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
    }

    public void RegisterRipper() => totalRippers++;

    public void DeadRipper()
    {
        totalRippers--;
        if (totalRippers <= 0) SpawnNewRipper();
    }

    private void SpawnNewRipper()
    {
        Instantiate(ripperPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("NEW RIPPER SPAWNED");
    }
}