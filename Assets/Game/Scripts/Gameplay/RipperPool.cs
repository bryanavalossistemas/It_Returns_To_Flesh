using UnityEngine;
using static BehaviourPlus;

public class RipperPool : MonoBehaviour
{
    [SerializeField] private FleshRipper ripperPrefab;
    //private Pool<FleshRipper> poolRipper;

    void Start()
    {
        gameManager.OnSpawnRipper += SpawnRipper;
        //poolRipper = new(ripperPrefab, 3);
        //poolRipper.Return(f);
    }
    void OnDestroy() => gameManager.OnSpawnRipper -= SpawnRipper;

    private void SpawnRipper(Vector3 pos) => Instantiate(ripperPrefab, pos, Quaternion.identity);
    //private void SpawnRipper(Vector3 pos) => poolRipper.Get(pos);
}