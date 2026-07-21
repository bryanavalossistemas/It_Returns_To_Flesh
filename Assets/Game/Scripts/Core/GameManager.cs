using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class GameManager : MonoBehaviour_UU, IUpdatable
{
    public const int CivilianLayer = 7, ExplodableLayer = 8, InstakillLayer = 9, CheckpointLayer = 11;
    public const float RayLength = 0.5f;
    [HideInInspector] public PhaseSO phaseSO;
    [SerializeField] private CameraController cameraController;
    public Material normalMat, ripperSelectedMat;
    [HideInInspector] public int selectedSkill = -1;
    public LayerMask groundLayer, pushableLayer;
    private Transform spawnPoint;
    private int nRippers;
    public enum SelectionTarget
    {
        None,
        Ripper,
        Limb
    }
    [HideInInspector] public SelectionTarget selectionTarget;
    public event Action OnAA, OnAA2;
    public event Action<Vector3> OnSpawnRipper;
    public int MaxHP {  get; private set; }
    public int HP { get; private set; }
    [SerializeField] private RipperSO ripperSO;
    private Transform followedRipper;
    [SerializeField] private LevelController levelController;
    public LayerMask whatCanBePushed;
    private bool isRestarting;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool inGameplay = scene.buildIndex > Core.MenusIndex;
        gameObject.SetActive(inGameplay);
        Time.timeScale = 1f;
        if (!inGameplay) return;

        //LoadLevelData(scene.buildIndex - Core.MenusIndex - 1);
        uiManager.UpdateSkillsUI();
        nRippers = 0;
        MaxHP = ripperSO.initialHP;
        HP = MaxHP;
        isRestarting = false;
        uiManager.UpdateHealth(HP, MaxHP);
    }

    public void SetLevelData(Transform spawnPoint, Tilemap tilemap)
    {
        this.spawnPoint = spawnPoint;
        cameraController.UpdateBounds(tilemap);
    }
    public void RestartLevel() => core.ReloadScene();

    /// <summary>
    /// Pide que mueran todos los Rippers vivos (con su animacion de muerte individual).
    /// GameManager (ensamblado Core) no conoce el tipo FleshRipper (ensamblado Gameplay),
    /// asÃ­ que solo dispara este evento; RipperPool (en Gameplay) escucha y hace el trabajo.
    /// El reinicio real del nivel ocurre solo cuando el Ãºltimo Ripper termina su animacion
    /// y llama a RipperDead() de este GameManager (mismo flujo que cuando mueren en combate).
    /// Ãšsalo para HP <= 0 y para el botÃ³n de Reiniciar.
    /// </summary>
    public event Action OnRequestKillAllRippers;

    public void KillAllRippers()
    {
        Debug.Log("KillAllRippers llamado");
        if (isRestarting) return;
        isRestarting = true;

        Debug.Log("Suscriptores de OnRequestKillAllRippers: " + (OnRequestKillAllRippers?.GetInvocationList().Length ?? 0));

        if (OnRequestKillAllRippers != null) OnRequestKillAllRippers();
        else RestartLevel(); // Nadie escuchando -> reinicia directo
    }

    public void OnUpdate()
    {
        cameraController.ControllerUpdate();
        if (inputManager.Pause)
        {
            bool isPaused = Time.timeScale == 0f;
            Time.timeScale = isPaused? 1f : 0f;
            //PauseMenu.instance.TogglePauseMenu();
        }
        bool[] _numbers = { inputManager._1, inputManager._2, inputManager._3, inputManager._4, inputManager._5 };
        for (int i = 0; i < _numbers.Length; i++)
        {
            if (_numbers[i] && phaseSO != null && phaseSO.unlockedSkills[i])
            {
                uiManager.SelectButton(i);
            }
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

    public void ConvertCivilian(Transform civilian)
    {
        Destroy(civilian.gameObject);
        SpawnRipper(civilian);
        MaxHP += ripperSO.buffHP;
        HP += ripperSO.buffHP;
        uiManager.UpdateHealth(HP, MaxHP);
        audioManager.PlaySfx(SFXEnum.Eating, civilian.position);
    }

    public void TriggerSkill(int pos)
    {
        //Action[] skills = { SkillVomit, SkillSores, SkillExplode, SkillCephalic, SkillFrenzy };
        //skills[pos].Invoke();
        selectionTarget = SelectionTarget.Ripper;
        OnAA2();
        uiManager.ClearUI();
    }

    /*private void SkillVomit() => selectionTarget = SelectionTarget.Ripper;
    private void SkillSores() => selectionTarget = SelectionTarget.Ripper;
    private void SkillExplode() => selectionTarget = SelectionTarget.Limb;
    private void SkillCephalic() => selectionTarget = SelectionTarget.Ripper;
    private void SkillFrenzy() => selectionTarget = SelectionTarget.Ripper;*/

    public void ModifyHP(int n)
    {
        HP += n;
        if (HP > MaxHP) HP = MaxHP;
        if (HP <= 0) KillAllRippers();
        uiManager.UpdateHealth(HP, MaxHP);
    }

    public void FollowRipper(Transform t)
    {
        if (followedRipper == t) return;
        followedRipper = t;
        cameraController.SetMiniCamera(followedRipper);
    }

    public LevelSO[] GetLevels() => levelController.GetLevels();
    public void StartLevel(int n) => levelController.StartLevel(n);
    public void PhaseCompleted() => levelController.PhaseCompleted();
    public void TriggerCameraShake(float intensity)
    {
        if (cameraController != null)
        {
            cameraController.ShakeCamera(intensity);
        }
    }
}