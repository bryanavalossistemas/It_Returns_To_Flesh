using UnityEngine;
using EditorAttributes;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SceneDropdown] public int[] phases;
}