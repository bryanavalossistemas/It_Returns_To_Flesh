using UnityEngine;
using EditorAttributes;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [SceneDropdown] public int[] phases;
    [Header("Configuración de Habilidades")]
    [Tooltip("Índices: 0=Vomit, 1=Sores, 2=Explode, 3=Cephalic, 4=Frenzy")]
    public bool[] unlockedSkills = new bool[5] { true, true, true, true, true };
}