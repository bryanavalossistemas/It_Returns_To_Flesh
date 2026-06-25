using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static BehaviourPlus;

public class GameManager : MonoBehaviour
{
    public const int CivilianLayer = 9, ExplodableLayer = 11, InstakillLayer = 8;
    public const float RayLength = 0.05f;
    [Header("Respawn")]
    [SerializeField] private GameObject ripperPrefab;
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
    public SelectionTarget selectionTarget;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        bool inGameplay = scene.buildIndex > 0;
        gameObject.SetActive(inGameplay);
        Time.timeScale = 1f;
        if (!inGameplay) return;

        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
    }

    public void RegisterRipper() => nRippers++;
    
    public void DeadRipper()
    {
        nRippers--;
        if (nRippers <= 0) SpawnRipper(spawnPoint);
    }
<<<<<<< Updated upstream

    private void SpawnRipper(Transform transform)
    {
        Instantiate(ripperPrefab, transform.position, Quaternion.identity);
    }
    public void UpdateCheckPoint(Transform newCheckPoint)
    {
        spawnPoint = newCheckPoint;
        Debug.Log("¡Checkpoint actualizado!");
    }

    void Update()
    {
        if (inputManager.Pause) Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
        //PauseMenu.instance.TogglePauseMenu();
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
        if (transform == null)
        {
            Debug.LogError("SpawnPoint no asignado en GameManager");
            return;
        }
        if (ripperPrefab == null)
        {
            Debug.LogError("RipperPrefab no asignado en GameManager");
            return;
        }
        Instantiate(ripperPrefab, transform.position, Quaternion.identity);
    }
>>>>>>> Stashed changes

    public void TriggerSkill(int pos)
    {
        Action[] skills = { SkillVomit, SkillSores, SkillExplode, SkillCephalic, SkillFrenzy };
        skills[pos].Invoke();
        FleshRipper.SelectedRipper = null;
        uiManager.ClearUI();
    }

    private void SkillVomit()
    {
        selectionTarget = SelectionTarget.Ripper;
    }

    private void SkillSores()
    {
        selectionTarget = SelectionTarget.Ripper;
    }

    private void SkillExplode()
    {
        selectionTarget = SelectionTarget.Limb;
    }

    private void SkillCephalic()
    {

    }

    private void SkillFrenzy()
    {
        selectionTarget = SelectionTarget.Ripper;
    }
}