using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int CivilianLayer = 9, ExplodableLayer = 11, InstakillLayer = 8;
    public const float RayLength = 0.05f;
    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
    public LayerMask groundLayer, pushableLayer;
    private Transform spawnPoint;
    private int nRippers;
    public Material normalMat, ripperSelectedMat;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool inGameplay = scene.buildIndex > 0;
        gameObject.SetActive(inGameplay);
        Time.timeScale = 1f;
        if (!inGameplay) return;

        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
    }

    public void RegisterRipper() => nRippers++;
    public void DeadRipper()
    {
        nRippers--;
        if (nRippers <= 0) SpawnRipper(spawnPoint);
    }

    private void SpawnRipper(Transform transform)
    {
        Instantiate(ripperPrefab, transform.position, Quaternion.identity);
    }
    public void UpdateCheckPoint(Transform newCheckPoint)
    {
    spawnPoint = newCheckPoint;
    Debug.Log("¡Checkpoint actualizado!");
    }
}