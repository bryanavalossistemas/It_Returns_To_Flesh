using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class GameManager : MonoBehaviour_UU, IUpdatable
{
    public const int CivilianLayer = 9, ExplodableLayer = 11, InstakillLayer = 8;
    public const float RayLength = 0.5f;
    [SerializeField] private CameraController cameraController;
    public Material normalMat, ripperSelectedMat;
    [HideInInspector] public int selectedSkill = -1;
    public LayerMask groundLayer, pushableLayer;
    public enum SelectionTarget
    {
        None,
        Ripper,
        Limb
    }
    [HideInInspector] public SelectionTarget selectionTarget;
    private int nRippers;
    private Transform spawnPoint;
    public event Action OnAA, OnAA2;
    public event Action<Vector3> OnSpawnRipper;
    public int MaxHP {  get; private set; }
    public int HP { get; private set; }
    [SerializeField] private RipperSO ripperSO;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool inGameplay = scene.buildIndex > Core.MenusIndex;
        gameObject.SetActive(inGameplay);
        Time.timeScale = 1f;
        if (!inGameplay) return;

        MaxHP = ripperSO.initialHP;
        HP = MaxHP;
    }

    public void SetLevelData(Transform spawnPoint, Tilemap tilemap)
    {
        this.spawnPoint = spawnPoint;
        Bounds bounds = tilemap.localBounds;
        bounds.center += tilemap.transform.position;
        cameraController.UpdateConfiner(bounds.center, bounds.size);
    }
    public void RestartLevel() => core.ReloadScene();

    public void OnUpdate()
    {
        if (inputManager.Pause)
        {
            bool isPaused = Time.timeScale == 0f;
            Time.timeScale = isPaused? 1f : 0f;
            //PauseMenu.instance.TogglePauseMenu();
        }
        bool[] _numbers = { inputManager._1, inputManager._2, inputManager._3, inputManager._4, inputManager._5 };
        for (int i = 0; i < _numbers.Length; i++)
        {
            if (_numbers[i]) uiManager.SelectButton(i);
        }
        if (inputManager.Deselect)
        {
            selectedSkill = -1;
            //cameraController target = null
            OnAA();
            uiManager.ClearUI();
        }
    }

    public void UpdateCheckPoint(Transform checkPoint) => spawnPoint = checkPoint;

    public void RegisterRipper() => nRippers++;
    public void RipperDead()
    {
        nRippers--;
        if (nRippers <= 0) RestartLevel();
    }
    public void SpawnRipper(Transform transform) => OnSpawnRipper(transform.position);

    public void TriggerSkill(int pos)
    {
        Action[] skills = { SkillVomit, SkillSores, SkillExplode, SkillCephalic, SkillFrenzy };
        skills[pos].Invoke();
        OnAA2();
        uiManager.ClearUI();
    }

    private void SkillVomit() => selectionTarget = SelectionTarget.Ripper;
    private void SkillSores() => selectionTarget = SelectionTarget.Ripper;
    private void SkillExplode() => selectionTarget = SelectionTarget.Limb;
    private void SkillCephalic() => selectionTarget = SelectionTarget.Ripper;
    private void SkillFrenzy() => selectionTarget = SelectionTarget.Ripper;

    public void ModifyHP(int n)
    {
        int hp = HP;
        if (HealthHelper.ModifyClamp(ref hp, n, MaxHP)) RestartLevel();
        HP = hp;
    }
}