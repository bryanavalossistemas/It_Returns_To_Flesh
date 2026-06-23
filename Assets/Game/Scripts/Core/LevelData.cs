using UnityEngine;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class LevelData : MonoBehaviour
{
    public Tilemap tilemap;
    public Transform spawnPoint;

    void Start() => gameManager.SetLevelData(this);
}