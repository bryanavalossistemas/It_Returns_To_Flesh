using UnityEngine;
using static BehaviourPlus;

public class RipperPool : MonoBehaviour
{
    [SerializeField] private FleshRipper ripperPrefab;
    //private Pool<FleshRipper> poolRipper;

    void Start()
    {
        gameManager.OnSpawnRipper += SpawnRipper;
        gameManager.OnRequestKillAllRippers += KillAllRippers;
        //poolRipper = new(ripperPrefab, 3);
        //poolRipper.Return(f);
    }

    void OnDestroy()
    {
        gameManager.OnSpawnRipper -= SpawnRipper;
        gameManager.OnRequestKillAllRippers -= KillAllRippers;
    }

    private void SpawnRipper(Vector3 pos) => Instantiate(ripperPrefab, pos, Quaternion.identity);
    //private void SpawnRipper(Vector3 pos) => poolRipper.Get(pos);

    private void KillAllRippers()
    {
        FleshRipper[] rippers = FindObjectsByType<FleshRipper>(FindObjectsSortMode.None);
        if (rippers.Length == 0)
        {
            gameManager.RestartLevel();
            return;
        }
        foreach (FleshRipper ripper in rippers) ripper.RipperDead();
    }
}