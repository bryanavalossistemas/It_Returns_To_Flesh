using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class GameManager : MonoBehaviour
{
    public const int CivilianLayer = 7, ExplodableLayer = 8, InstakillLayer = 9, CheckpointLayer = 11;
    public const float RayLength = 0.5f;
    [SerializeField] public LevelSO currentLevelData;
    [SerializeField] private CameraController cameraController;
    public Material normalMat, ripperSelectedMat;
    [HideInInspector] public int selectedSkill = -1;
    public LayerMask groundLayer, pushableLayer;
    private Transform spawnPoint;
    private int nRippers;
    public Material normalMat, ripperSelectedMat;
    public int selectedSkill = -1;
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
    private Transform followedRipper;
    [SerializeField] private LevelController levelController;
    public LayerMask whatCanBePushed;
    private bool isRestarting;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool inGameplay = scene.buildIndex > 0;
        gameObject.SetActive(inGameplay);
        Time.timeScale = 1f;
        if (!inGameplay) return;

        nRippers = 0;
        MaxHP = ripperSO.initialHP;
        HP = MaxHP;
        isRestarting = false;
        uiManager.UpdateHealth(HP, MaxHP);
    }

    public void RegisterRipper() => nRippers++;
    
    public void DeadRipper()
    {
        nRippers--;
        if (nRippers <= 0) SpawnRipper(spawnPoint);
    }
    /// <summary>
    /// Pide que mueran todos los Rippers vivos (con su animaciÃ³n de muerte individual).
    /// GameManager (ensamblado Core) no conoce el tipo FleshRipper (ensamblado Gameplay),
    /// asÃ­ que solo dispara este evento; RipperPool (en Gameplay) escucha y hace el trabajo.
    /// El reinicio real del nivel ocurre solo cuando el Ãºltimo Ripper termina su animaciÃ³n
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
            //cameraController targer = null
            if (FleshRipper.SelectedRipper != null)
            {
                FleshRipper.SelectedRipper.SetSelectedVisual(false);
                FleshRipper.SelectedRipper = null;
            }
            uiManager.ClearUI();
        }
    }
=======
    private void SpawnRipper(Transform transform)
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
    }

    public void TriggerSkill(int pos)
    {
        selectionTarget = SelectionTarget.Ripper;
        OnAA2();
        uiManager.ClearUI();
    }

    public void ModifyHP(int n)
    {
        HP += n;
        if (HP > MaxHP) HP = MaxHP;
        if (HP <= 0) KillAllRippers();
        uiManager.UpdateHealth(HP, MaxHP);
    }

    private void SkillSores()
    {
        selectionTarget = SelectionTarget.Ripper;
    }

    private void SkillExplode()
    {
        selectionTarget = SelectionTarget.Limb;
    }
    public void NextPhase() => core.NextScene();

    public void TriggerCameraShake(float intensity)
    {
        if (cameraController != null)
        {
            cameraController.ShakeCamera(intensity);
        }
    }
}
