using UnityEngine;
using EditorAttributes;

[CreateAssetMenu(fileName = "PhaseSO", menuName = "Scriptable Objects/PhaseSO")]
public class PhaseSO : ScriptableObject
{
    [SceneDropdown] public int sceneIndex;
    [Header("Configuración de Habilidades"), Tooltip("Índices: 0=Vomit, 1=Sores, 2=Explode, 3=Cephalic, 4=Frenzy")]
    public bool[] unlockedSkills = new bool[5] { true, true, true, true, true };
}