using UnityEngine;
using Cysharp.Threading.Tasks;
using static BehaviourPlus;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelSO[] levels;
    private PhaseSO[] phases;
    private int phaseIndex;

    public LevelSO[] GetLevels() => levels;

    public void StartLevel(int n)
    {
        phases = levels[n].phases;
        phaseIndex = 0;
        core.LoadScene(GetPhaseSO().sceneIndex);
    }

    private PhaseSO GetPhaseSO()
    {
        PhaseSO p = phases[phaseIndex];
        gameManager.phaseSO = p;
        uiManager.UpdateSkillsUI();
        return p;
    }

    public void PhaseCompleted()
    {
        (int sceneIndex, TransitionMode tMode) = NextSceneIndex();
        core.LoadSceneAsync(sceneIndex, tMode, UniTask.Delay(1000));
    }

    private (int, TransitionMode) NextSceneIndex()
    {
        phaseIndex++;
        if (phaseIndex < phases.Length) return (GetPhaseSO().sceneIndex, TransitionMode.SlideRight);
        else return ((int)SceneTypes.LevelSelector, TransitionMode.None);
    }
}