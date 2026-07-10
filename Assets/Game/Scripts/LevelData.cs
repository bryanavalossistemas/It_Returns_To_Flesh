using UnityEngine;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class LevelData : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Tilemap tilemap;

    void Start() => gameManager.SetLevelData(spawnPoint, tilemap);
}